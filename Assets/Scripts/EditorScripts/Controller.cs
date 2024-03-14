using System.Collections;
using System.Collections.Generic;
using System.Text;
using Defective.JSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

// Ended up as somewhat of a god class, sorry. Some things shouldn't be here,
// but I tried commenting as much as made sense
public class Controller : MonoBehaviour
{

    public API_Links apiLinks;

    public GameObject cam;
    public Vector3 playerStartPosition;
    public string username;
    public string token;

    public bool inPlayer = false;
    public bool inCatalog = false;
    public bool colliderModeWalls;
    public bool colliderModeFloors;
    public bool inTopViewCamera = false;
    public bool draggingSelectedItem = false;
    public bool draggedItemSnapped = false;
    public bool inInputBox = false;

    public GameObject cameraEditMode;
    public GameObject player;

    public List<GameObject> dragables;
    // public List<GameObject> colliders;

    public UI ui;

    public GameObject heldItem;
    public GameObject selectedItem;

    private Vector3 previousCameraPosition;
    private Quaternion previousCameraRotation;

    public GameObject sceneItemList;

    public List<GameObject> typeButtons;

    public JsonSaveScene saveSceneComponent;

    private float camNearClipPlaneTopView = 5f;
    private float camNearClipPlaneNormalView = 0.3f;

    private Vector3 cameraDragOrigin;
    private Vector3 cameraDragDifference;

    public Vector3 camCenterPointGround;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);

    public bool cursorOverSideMenu = false;

    private Vector2 mousePositionPrevious;
    private bool rotatingCameraSleep;
    // private float currentCameraRotationDegrees;
    private float currentCameraRotationDegreesX;
    private float currentCameraRotationDegreesY;

    public Shader shaderAlwaysVisible;

    public float currentClippingHeight;
    public float currentWorkingFloorLevel;
    public GameObject currentWorkingFloorObject;

    public int currentSkyboxIndex;

    public List<GameObject> listOfPortals;

    public GameObject playerDarkScreenFade;

    public GameObject canvasMain;

    public bool inGuideMode;
    public Vector3 lastPlayerPositionInGuide;
    public int nextIndexToReach;

    public bool inSetStartPosition;

    public GameObject videoPlayer;
    public GameObject audioPlayer;
    public GameObject htmlDisplayObject;

    public bool blockScrollCameraMovement;

    public List<GameObject> buildings;
    private bool changingBuildings;

    public bool changingClippingHeight;

    void Start()
    {
        apiLinks = this.gameObject.GetComponent<API_Links>();

        dragables = new List<GameObject>();
        // colliders = new List<GameObject>();
        listOfPortals = new List<GameObject>();

        colliderModeWalls = false;
        colliderModeFloors = false;

        selectedItem = null;

        mousePositionPrevious = Input.mousePosition;
        rotatingCameraSleep = false;
        // currentCameraRotationDegrees = 0;
        currentCameraRotationDegreesX = 30;
        currentCameraRotationDegreesY = -180;

        HandleChangeClippingHeight();
        HandleChangeFloorHeight();

        currentSkyboxIndex = 0;

        inGuideMode = false;
        nextIndexToReach = 0;

        inSetStartPosition = false;

        playerStartPosition = new Vector3(0, 1, 0);

        // GetUsernameAndTokenLocal();
        blockScrollCameraMovement = false;

        changingClippingHeight = false;

        username = "user";
        token = "fe82818f-90a0-484a-bea4-f5e24b81cdb3";
    }

    // Used only for camera dragging with Middle Click
    private void LateUpdate()
    {
        // On first click take camera origin
        if (!inPlayer && !inCatalog && Input.GetMouseButtonDown(2))
        {
            cameraDragOrigin = Input.mousePosition;
            Cursor.visible = false;
            return;
        }

        // If not in player or in catalog and middle mouse button is pressed
        if (!inPlayer && !inCatalog && Input.GetMouseButton(2))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - cameraDragOrigin);
            Vector3 move = new Vector3(pos.x * 20f, 0, pos.y * 20f);

            // If past a certain angle looking up/down, consider "up" to be the horizontal axis
            Vector3 forwardHorizontal = cam.transform.forward;
            float camAngleX = cam.transform.rotation.eulerAngles.x;
            if (inTopViewCamera || camAngleX > 60 || camAngleX < -60)
            {
                forwardHorizontal = cam.transform.up;
            }

            // Never move vertical
            forwardHorizontal.y = 0f;

            // Apply translation and update cameraOrigin for next cycle
            cam.transform.position -= cam.transform.right * move.x;
            cam.transform.position -= forwardHorizontal * move.z;
            cameraDragOrigin = Input.mousePosition;
        }

        // Quite certain this can be removed, won't remove it just because too lazy to test what would break
        // Sorry
        if (!inPlayer && !inCatalog && Input.GetMouseButtonUp(2))
        {
            Cursor.visible = true;
            ui.UpdateCameraGroundCenterPoint();
        }

    }

    void Update()
    {
        // On right mouse down, set initial mouse position for rotation
        if (!inPlayer && !inCatalog && !inTopViewCamera && Input.GetMouseButtonDown(1))
        {
            mousePositionPrevious = Input.mousePosition;
        }

        // On right mouse up, make cursor visible again
        if (!inPlayer && !inCatalog && !inTopViewCamera && Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }

        // Camera rotation with Right Click
        if (!inPlayer && !inCatalog && !inTopViewCamera && Input.GetMouseButton(1))
        {
            CameraRotation();
        }

        // Camera movement
        if (!inPlayer && !inCatalog)
        {
            CameraMovement();
        }

        // If in preview mode and item is being held, maintain rotation of said item
        if (inPlayer && heldItem != null)
            heldItem.transform.eulerAngles = new Vector3(0, 180, 0);

        // If in catalog and ESCAPE or Q is pressed, close catalogs
        if (inCatalog && !inInputBox && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            inCatalog = false;
            ui.CloseCatalog();
            ui.CloseLightingCatalog();
            ui.ClosePortalCatalog();
            ui.CloseGuideMenu();
            ui.CloseWidgetCatalog();

            // If start position mode, disable
            if (inSetStartPosition)
                ui.SetStartPositionToggle();
            return;
        }

        // If in preview mode and C is pressed, open Catalog
        if (!inCatalog && !inInputBox && Input.GetKeyDown(KeyCode.C))
        {
            inCatalog = true;
            ui.GameObject().GetComponent<Catalog>().OpenCatalog();
        }

        // If in preview mode and B is pressed, open Lighting Catalog
        if (!inCatalog && !inInputBox && Input.GetKeyDown(KeyCode.B))
        {
            inCatalog = true;
            ui.OpenLightingCatalog();
        }

        // If in preview mode and G is pressed, open Guide Path menu
        if (!inCatalog && !inInputBox && !inPlayer && Input.GetKeyDown(KeyCode.G))
        {
            inCatalog = true;
            ui.OpenGuideMenu();
        }

        // If in preview mode and ESCAPE/Q is pressed
        if (inPlayer && !inInputBox && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            // If item is being held, drop it 
            if (heldItem != null)
            {
                heldItem.transform.SetParent(null);
                heldItem = null;
            }

            // If item is selected, unselect it
            if (selectedItem != null)
                selectedItem.GetComponent<Item>().SetSelected(false);

            // Change camera mode to edit
            SetCameraMode(false);
        }

        // If in edit mode and Collider mode, and no item is selected, and ESCAPE/Q is pressed, leave collider mode
        if (!inPlayer && (colliderModeWalls || colliderModeFloors) && selectedItem == null && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            colliderModeWalls = false;
            colliderModeFloors = false;
            ui.lineDraw = false;
            ui.colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            ui.colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
            ui.lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
            ui.lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

            ui.lineObjectInstance.SetActive(false);
        }

        // If in edit mode and ESCAPE/Q is pressed, desselect item, if any selected
        if (!inPlayer && !inInputBox && selectedItem != null && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            selectedItem.GetComponent<Item>().SetSelected(false);
        }

        // If in edit mode and an item is selected, activate relevant UI elements
        // This is quite inefficient, should add an additional check to make it stop looping after state was changed
        if (!inPlayer && !inCatalog)
        {
            ui.selectedItemUIElements.SetActive(selectedItem != null);
        }

        // If in guide mode, and WASD, Space or LCrtl is pressed, exit guide mode
        if (inGuideMode &&
                (
                    Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.D) ||
                    Input.GetKeyDown(KeyCode.Space) ||
                    Input.GetKeyDown(KeyCode.LeftShift)
                )
            )
        {
            inGuideMode = false;
        }

        // Hide/Show buildings keybind combination ("secret")
        if (!changingBuildings && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.K))
        {
            StartCoroutine(ChangeBuildingThenWait());
        }

        // Camera zoom in/out, if cursor not over a blocking UI element
        if (!blockScrollCameraMovement)
            CameraZoom();

        if (changingClippingHeight && Input.GetKeyUp(KeyCode.Mouse0))
            changingClippingHeight = false;
    }


    private IEnumerator ChangeBuildingThenWait()
    {
        changingBuildings = true;

        bool currentState = buildings[0].activeSelf;
        foreach (GameObject building in buildings)
            building.SetActive(!currentState);

        yield return new WaitForSeconds(.5f);
        changingBuildings = false;
    }

    // Used to deal with camera rotation in aerial mode
    public void CameraRotation()
    {
        if (!rotatingCameraSleep)
        {
            // Hide mouse cursor
            Cursor.visible = false;

            // Get mouse position
            Vector2 currentPosition = Input.mousePosition;

            // Mouse wraping, without this would be limited to screen size
            // !!! !!! !!! DOESN'T WORK IN WEBGL !!! !!! !!!
            // if (currentPosition.x < 100 || currentPosition.x > 600 || currentPosition.y < 100 || currentPosition.y > 600)
            // {
            //     SetCursorPos(400, 400);
            //     mousePositionPrevious = Input.mousePosition;
            //     currentPosition = Input.mousePosition;
            // }

            // Calculate new deltas and update previous position
            float deltaPositionX = currentPosition.x - mousePositionPrevious.x;
            float deltaPositionY = currentPosition.y - mousePositionPrevious.y;

            mousePositionPrevious = currentPosition;

            // Incase a wrap happened, update is too slow and would bug camera, force return and update correctly on next frame
            // if (Mathf.Abs(deltaPositionX) > 50 || Mathf.Abs(deltaPositionY) > 50)
            //     return;

            // Calculate new rotation values for camera, with vertical rotation clamped to 180 range (90 up/down)
            currentCameraRotationDegreesX -= deltaPositionY / 8;
            currentCameraRotationDegreesY += deltaPositionX / 8;
            currentCameraRotationDegreesX = Mathf.Clamp(currentCameraRotationDegreesX, -90, 90);

            // Apply new rotation to camera based on mouse movements
            cam.transform.eulerAngles = new Vector3(currentCameraRotationDegreesX, currentCameraRotationDegreesY, 0);

            rotatingCameraSleep = true;
            StartCoroutine(SleepThenChangeRotateCameraBool());
        }
    }

    // Allows camera transform to apply properly, small interval is added before changing the bool value
    private IEnumerator SleepThenChangeRotateCameraBool()
    {
        yield return new WaitForSeconds(.01f * Time.deltaTime);
        rotatingCameraSleep = false;
    }

    // Camera zoom with scroll wheel and +/- keys
    public void CameraZoom()
    {
        Vector3 transformVec = cam.transform.forward;

        // Allows for different scale values on Unity and WebGL. Was used initially, can be removed now
#if UNITY_WEBGL && !UNITY_EDITOR

        transformVec *= 5f;

#else

        transformVec *= 5f;

#endif

        // Zoom with keyboard buttons + and -
        if (!inPlayer && !inCatalog && Input.GetKey(KeyCode.Equals))
        {
            cam.transform.Translate(transformVec * Time.deltaTime, Space.World);
        }
        else if (!inPlayer && !inCatalog && Input.GetKey(KeyCode.Minus))
        {
            cam.transform.Translate(-transformVec * Time.deltaTime, Space.World);
        }

        // Zoom with mouse scroll wheel
        if (!inPlayer && !inCatalog && Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            cam.transform.Translate(transformVec * 50 * Time.deltaTime, Space.World);
        }
        else if (!inPlayer && !inCatalog && Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            cam.transform.Translate(-transformVec * 50 * Time.deltaTime, Space.World);
        }
    }

    // Camera movement in aerial mode
    public void CameraMovement()
    {
        Vector3 transformVec = new Vector3(0, 0, 0);
        float multiplier = 0f;

        // Allows for different scale values on Unity and WebGL. Was used initially, can be removed now
#if UNITY_WEBGL && !UNITY_EDITOR

        multiplier = 10f;

#else

        multiplier = 10f;

#endif

        // WASD and ARROW keys
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            float extraMultiplier = 1f;
            if (!inTopViewCamera)
            {
                // Required these weird checks to change the transform vec in certain scenarios
                // Without this, when looking more than 45 degrees up/down, the movement would be inverted
                if (Mathf.Abs(cam.transform.eulerAngles.x) >= 45 && Mathf.Abs(cam.transform.eulerAngles.x) <= 90)
                {
                    extraMultiplier = (90 + Mathf.Abs(cam.transform.eulerAngles.x)) / 90;
                    transformVec = cam.transform.up;
                }
                else if (Mathf.Abs(cam.transform.eulerAngles.x) < 45 || Mathf.Abs(cam.transform.eulerAngles.x) > 315)
                {
                    extraMultiplier = (315 + Mathf.Abs(cam.transform.eulerAngles.x)) / 315;
                    transformVec = cam.transform.forward;
                }
                else
                {
                    transformVec = -cam.transform.up;
                }
            }
            else
            {
                transformVec = cam.transform.up;
            }

            transformVec.y = 0;
            cam.transform.position += transformVec * multiplier * extraMultiplier * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            cam.transform.position -= cam.transform.right * multiplier * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            float extraMultiplier = 1f;
            if (!inTopViewCamera)
            {
                // Required these weird checks to change the transform vec in certain scenarios
                // Without this, when looking more than 45 degrees up/down, the movement would be inverted
                if (Mathf.Abs(cam.transform.eulerAngles.x) >= 45 && Mathf.Abs(cam.transform.eulerAngles.x) <= 90)
                {
                    extraMultiplier = (90 + Mathf.Abs(cam.transform.eulerAngles.x)) / 90;
                    transformVec = cam.transform.up;
                }
                else if (Mathf.Abs(cam.transform.eulerAngles.x) < 45 || Mathf.Abs(cam.transform.eulerAngles.x) > 315)
                {
                    extraMultiplier = (315 + Mathf.Abs(cam.transform.eulerAngles.x)) / 315;
                    transformVec = cam.transform.forward;
                }
                else
                {
                    transformVec = -cam.transform.up;
                }
            }
            else
            {
                transformVec = cam.transform.up;
            }

            transformVec.y = 0;
            cam.transform.position -= transformVec * multiplier * extraMultiplier * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            cam.transform.position += cam.transform.right * multiplier * Time.deltaTime;
        }

        // Space and LeftShift (vertical movement)
        if (Input.GetKey(KeyCode.Space) && !inTopViewCamera)
        {
            cam.transform.position += new Vector3(0, 1, 0) * multiplier * 2 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && !inTopViewCamera)
        {
            cam.transform.position -= new Vector3(0, 1, 0) * multiplier * 2 * Time.deltaTime;
        }
    }

    // Used to change between Ground Exploration mode and Aerial mode
    public void SetCameraMode(bool newMode)
    {
        // If in top view, exit first. This will update Top View button in side menu, along with camera clipping plane
        if (inTopViewCamera)
            SetCameraTopView();

        // Update various modes and buttons
        inPlayer = newMode;
        colliderModeWalls = false;
        colliderModeFloors = false;
        ui.colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
        ui.colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();

        // If start position mode, disable
        if (inSetStartPosition)
            ui.SetStartPositionToggle();

        // Update visibility of guide point arrows
        SetVisibilityDragablesGuidePoints(!newMode);

        // Save camera position in the previous mode
        if (inPlayer)
        {
            previousCameraPosition = cam.transform.position;
            previousCameraRotation = cam.transform.rotation;
            cam.transform.parent = player.transform;
            cam.transform.localPosition = new Vector3(0, 0.5f, 0);
            cam.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            cam.transform.parent = null;
            cam.transform.position = previousCameraPosition;
            cam.transform.rotation = previousCameraRotation;

            if (videoPlayer != null)
            {
                videoPlayer.GetComponent<VideoPlayerControls>().Reset();
            }

            if (audioPlayer != null)
            {
                audioPlayer.GetComponent<AudioPlayerControls>().Reset();
            }
        }

        // If going into player mode and an item was selected, place player next to item
        if (selectedItem != null)
        {
            if (newMode)
            {
                player.gameObject.transform.position = selectedItem.transform.position + selectedItem.transform.forward * 5;
                player.transform.LookAt(selectedItem.transform);
            }
            selectedItem.GetComponent<Item>().SetSelected(false);
            ui.selectedItemUIElements.SetActive(false);
        }
        else
        {
            player.gameObject.transform.position = playerStartPosition;
        }

        // Hide/Show various UI elements
        ui.sideMenu.SetActive(!inPlayer);
        ui.sideMenuItemListing.SetActive(!inPlayer);
        ui.guideMenuButton.SetActive(!inPlayer);
        ui.saveMuseumButton.SetActive(!inPlayer);
        ui.gKeyIcon.SetActive(!inPlayer);
        ui.qKeyIcon.SetActive(inPlayer);
        ui.exitIcon.SetActive(inPlayer);

        // Enable/Disable player item in the world
        player.SetActive(newMode);

        // Update dragable sphere visibilities (arrows)
        SetVisibilityDragables(!newMode);

        // Update cursor
        Cursor.visible = !newMode;
        if (newMode)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    // Used to change to Top Down view camera
    public void SetCameraTopView()
    {
        // If exiting top view, return to previous location and make UI elements show up
        if (inTopViewCamera)
        {
            // Restore previous camera position and rotation
            cam.gameObject.transform.position = previousCameraPosition;
            cam.gameObject.transform.rotation = previousCameraRotation;

            // Restore near clipping plane
            cam.GetComponent<Camera>().nearClipPlane = camNearClipPlaneNormalView;

            // Update Top View button state
            ui.topViewCameraButton.GetComponent<ButtonCameraTopView>().ResetToBaseColor();
        }
        // If entering top view, put camera in top position and remove UI element for camera rotation
        else
        {
            // Store previous position and rotation
            previousCameraPosition = cam.gameObject.transform.position;
            previousCameraRotation = cam.gameObject.transform.rotation;

            // Set new camera transform for top view
            cam.gameObject.transform.position = new Vector3(0, 20, 0);
            cam.gameObject.transform.eulerAngles = new Vector3(90, -180, 0);

            // Increase near clipping plane
            cam.GetComponent<Camera>().nearClipPlane = camNearClipPlaneTopView;
        }

        inTopViewCamera = !inTopViewCamera;
    }

    // Used to add an item to the list of world media items
    public void AddToScene(GameObject newObj)
    {
        dragables.Add(newObj);
    }

    // Don't remember what this is used for, oops.
    private void SetVisibilityDragables(bool visibility)
    {
        foreach (GameObject dragable in dragables)
        {
            if (dragable == null)
                GameObject.Destroy(dragable);
            else
            {
                foreach (Transform child in dragable.transform)
                {
                    child.gameObject.SetActive(visibility);
                }
            }
        }
    }

    private void SetVisibilityDragablesGuidePoints(bool visibility)
    {
        GameObject[] listOfObjs = GameObject.FindGameObjectsWithTag("GuidePointSphere");
        foreach (GameObject sphereObj in listOfObjs)
            sphereObj.GetComponent<Dragable>().EnableDisableSideArrows(visibility);
    }

    // Remove selected item from world. Also removes from list of items, and guide menu
    public void RemoveSelectedFromWorld()
    {
        if (selectedItem != null)
        {
            GameObject tempObj = selectedItem;

            // Remove item from guide points if necessary
            ui.mainCanvas.GetComponent<GuideMenu>().RemoveItemFromPathBasedOnParent(selectedItem);

            // Remove from list of items for guide building
            if (selectedItem.GetComponent<Item>().itemListingForGuide != null)
                selectedItem.GetComponent<Item>().itemListingForGuide.GetComponent<ButtonSceneItemListingGuide>().RemoveItemFromList();

            // Remove listing from list of SceneItems
            int childCount = sceneItemList.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject objInListing = sceneItemList.transform.GetChild(i).gameObject.GetComponent<SceneItemListing>().associatedObject;
                if (objInListing == tempObj)
                    GameObject.Destroy(sceneItemList.transform.GetChild(i).gameObject);
            }

            // Reduce size of list for masking and scrollbar purposes
            if (sceneItemList.transform.childCount >= 4)
            {
                Vector2 newSize = sceneItemList.GetComponent<RectTransform>().sizeDelta;
                newSize.y -= 45;
                sceneItemList.GetComponent<RectTransform>().sizeDelta = newSize;
            }

            // If item was portal, remove from linked portals, and remove from portal list
            if (selectedItem.GetComponent<Item>().type == 6)
            {
                foreach (GameObject portal in listOfPortals)
                {
                    if (portal.GetComponent<Item>().linkedPortal == selectedItem)
                        portal.GetComponent<Item>().linkedPortal = null;
                }

                listOfPortals.Remove(selectedItem);
            }

            // Remove dragable from Dragables
            dragables.Remove(selectedItem.transform.parent.GetChild(0).gameObject);

            // Unselect item
            selectedItem.GetComponent<Item>().SetSelected(false);

            // Destroy game object from world
            GameObject.Destroy(tempObj.transform.parent.gameObject);
        }
    }

    // Remove a specific item from the world. Used when deleting an item from catalog.
    // Very similar to method before, should probably merge them
    // Just check if itemToRemove is null, and delete selected, otherwise delete argument obj
    public void RemoveItemFromScene(GameObject itemToRemove)
    {
        // Remove dragable from Dragables
        dragables.Remove(itemToRemove.transform.parent.GetChild(0).gameObject);

        // If item to delete is selected item, unselect and unparent camera
        if (selectedItem != null && selectedItem == itemToRemove)
        {
            selectedItem.GetComponent<Item>().SetSelected(false);
            // selectedItemCamera.transform.SetParent(null);
        }

        // If item was portal, remove from linked portals, and remove from portal list
        if (itemToRemove.GetComponent<Item>().type == 6)
        {
            foreach (GameObject portal in listOfPortals)
            {
                if (portal.GetComponent<Item>().linkedPortal == itemToRemove)
                    portal.GetComponent<Item>().linkedPortal = null;
            }

            listOfPortals.Remove(itemToRemove);
        }

        // Remove item from guide points if necessary
        ui.mainCanvas.GetComponent<GuideMenu>().RemoveItemFromPathBasedOnParent(itemToRemove);

        // Remove from list of items for guide building
        if (itemToRemove.GetComponent<Item>().type < 50)
            itemToRemove.GetComponent<Item>().itemListingForGuide.GetComponent<ButtonSceneItemListingGuide>().RemoveItemFromList();

        // Destroy game object from world
        GameObject.Destroy(itemToRemove.transform.parent.gameObject);
    }

    // Add an item listing to the List of World items menu
    public GameObject AddItemListing(GameObject objInListing, string listingItemName, string id)
    {
        return ui.AddItemListing(objInListing, listingItemName, id);
    }

    // ?? no idea what this is used in
    public void RemoveAllFromItemListing(string id)
    {
        int childCount = sceneItemList.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            string idInListing = sceneItemList.transform.GetChild(i).gameObject.GetComponent<SceneItemListing>().itemId;
            if (idInListing == id)
                GameObject.Destroy(sceneItemList.transform.GetChild(i).gameObject);
        }
    }

    // Update which button is selected in the media catalog
    public void UpdateTypeButtonSelection(int prevType, int newType)
    {
        if (prevType >= 0)
            typeButtons[prevType].GetComponent<ButtonCatalogType>().SetSelected(false);
        typeButtons[newType].GetComponent<ButtonCatalogType>().SetSelected(true);
    }

    // Used to save a scene to the database
    public void SaveSceneJson(string museumName)
    {
        saveSceneComponent.SaveSceneToJson(museumName);
    }

    // Set snapping mode on selected item
    public void SetSelectedSnapping(bool newValue)
    {
        draggedItemSnapped = newValue;
        selectedItem.GetComponent<Dragable>().snapping = newValue;
    }

    // Used when double clicking an item listing, moves camera to the item, in view/center screen
    public void MoveCameraToSelected()
    {
        Vector3 itemPosition = selectedItem.transform.position;
        cam.transform.position = itemPosition;

        cam.transform.position -= cam.transform.forward * 10;
    }

    // Clipping height slider adjustment
    public void HandleChangeClippingHeight()
    {
        changingClippingHeight = true;

        currentClippingHeight = ui.horizontalClippingSlider.GetComponent<Slider>().value;
        CheckItemBelowAboveWorkingLevel();
    }

    // Floor height slider adjustment
    public void HandleChangeFloorHeight()
    {
        changingClippingHeight = true;

        // Get the value from the slider
        currentWorkingFloorLevel = ui.floorHeightSlider.GetComponent<Slider>().value;
        currentWorkingFloorObject.transform.position = new Vector3(0, currentWorkingFloorLevel, 0);

        // Also change clipping height if the two are close enough together
        if (currentWorkingFloorLevel - 1 < currentClippingHeight)
        {
            currentClippingHeight = currentWorkingFloorLevel + 1;
            ui.horizontalClippingSlider.GetComponent<Slider>().value = currentClippingHeight;
        }

        // If would go above camera height, move camera along with the floor
        Vector3 camPos = cam.transform.position;
        if (currentWorkingFloorLevel + 1 >= camPos.y)
        {
            cam.transform.position = new Vector3(camPos.x, currentWorkingFloorLevel + 1, camPos.z);
        }
    }

    // Items are hidden if they are above the clipping height
    private void CheckItemBelowAboveWorkingLevel()
    {
        foreach (GameObject obj in dragables)
        {
            if (obj.transform.position.y < currentClippingHeight)
                obj.transform.parent.gameObject.SetActive(true);
            else
                obj.transform.parent.gameObject.SetActive(false);
        }
    }

    // Used to open the lighting catalog. Not sure why this is here, should just be in UI
    public void OpenLightingCatalog()
    {
        ui.OpenLightingCatalog();
    }

    // Used to change the html code shown on the side of the application on the website when in 
    // close proximity to an item with this widget
    [DllImport("__Internal")]
    private static extern void ChangeHtmlCode(string htmlCode);

    public void ChangeCurrentHtmlCode(string text)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            ChangeHtmlCode(text);
#else
        Debug.Log(text);
#endif
    }

    // Used to send a request to REACT to return the username and token of the logged user
    [DllImport("__Internal")]
    private static extern void GetUsernameAndToken();

    public void GetUsernameAndTokenLocal()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GetUsernameAndToken();
#else
        Debug.Log("get username and token called");
#endif
    }

    // Used to start the guide path from the beggining
    public void StartGuide()
    {
        // Change mode
        inGuideMode = true;
        List<GameObject> pathList = canvasMain.GetComponent<GuideMenu>().pathList;

        // Reset index of next target point
        nextIndexToReach = 1;

        // Update position values
        Vector3 startPosition = pathList[0].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position;
        lastPlayerPositionInGuide = startPosition;
        player.transform.position = lastPlayerPositionInGuide;

        // Start slow movement towards target
        StartCoroutine(WaitThenStartGuideMovement(pathList));
    }

    // Used to continue the guide path movement, or start fresh if no continuation point found
    public void ContinueGuide()
    {
        // Change mode
        inGuideMode = true;
        List<GameObject> pathList = canvasMain.GetComponent<GuideMenu>().pathList;

        // If it's 0, means that it was never started before. Treat it as a fresh start
        if (nextIndexToReach == 0)
        {
            Vector3 startPosition = pathList[0].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position;
            lastPlayerPositionInGuide = startPosition;
        }

        // Update player position to last position in guide
        player.transform.position = lastPlayerPositionInGuide;

        // Start slow movement towards target
        StartCoroutine(WaitThenStartGuideMovement(pathList));
    }

    private IEnumerator WaitThenStartGuideMovement(List<GameObject> pathList)
    {
        yield return new WaitForSeconds(.1f);
        StartCoroutine(GuideMovement(pathList));
    }

    // Slow movement towards next target point
    private IEnumerator GuideMovement(List<GameObject> pathList)
    {
        Vector3 nextPointPosition = pathList[nextIndexToReach].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position;

        // Move player towards the next point
        player.GetComponent<CharacterController>().Move((nextPointPosition - lastPlayerPositionInGuide).normalized * .01f);

        yield return new WaitForSeconds(.01f);

        // Update last player position
        lastPlayerPositionInGuide = player.transform.position;

        // If still hasn't exited guide mode, continue movement
        if (inGuideMode)
        {
            // Calculate distance to next point. If smaller than a certain threshold, move to next item
            if (Vector3.Distance(player.transform.position, nextPointPosition) <= .05f)
            {
                // If last point reached, stop guide
                if (nextIndexToReach < pathList.Count - 1)
                {
                    nextIndexToReach++;
                    StartCoroutine(GuideMovement(pathList));
                }
                else
                {
                    inGuideMode = false;
                    nextIndexToReach = 1;
                    lastPlayerPositionInGuide = pathList[0].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position;
                }
            }
            else
                StartCoroutine(GuideMovement(pathList));
        }
    }

    // Called by REACT to set the username in the controller
    public void SetUsername(string uname)
    {
        username = uname;
        Debug.Log(username + " " + token);
    }

    // Called by REACT to set the token in the controller
    public void SetToken(string tk)
    {
        token = tk;
        Debug.Log(username + " " + token);
    }

}
