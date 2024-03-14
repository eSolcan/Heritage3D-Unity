using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ControllerPlayerScene : MonoBehaviour
{
    public API_Links apiLinks;

    public GameObject cam;

    public bool inPlayer;
    public bool inCatalog;
    public bool inTopViewCamera;
    public bool inInputBox;

    public GameObject player;

    public UIPlayerScene ui;

    private Vector3 previousCameraPosition;
    private Quaternion previousCameraRotation;

    private float camNearClipPlaneTopView = 5f;
    private float camNearClipPlaneNormalView = 0.3f;

    private Vector3 cameraDragOrigin;
    private Vector3 cameraDragDifference;

    public Vector3 camCenterPointGround;

    private Vector2 mousePositionPrevious;
    private bool rotatingCameraSleep;
    // private float currentCameraRotationDegrees;
    private float currentCameraRotationDegreesX;
    private float currentCameraRotationDegreesY;

    public float currentClippingHeight;

    public GameObject playerDarkScreenFade;

    public GameObject canvasMain;

    public bool inGuideMode;
    public Vector3 lastPlayerPositionInGuide;
    public int nextIndexToReach;

    public List<GameObject> listOfItems;

    public List<GameObject> typeButtons;

    public List<GameObject> guidePathList;

    public bool cameraZooming;
    public Vector3 cameraZoomStartPosition;
    public int interpolationFramesCount;
    private int elapsedFrames;

    public HotspotPlayer hotspot;

    public GameObject postit;

    public GameObject htmlDisplayObject;

    public JsonLoadScene loadSceneComponent;

    public GameObject videoPlayer;
    public GameObject audioPlayer;

    public string username;
    public string token;

    public List<GameObject> buildings;
    private bool changingBuildings;

    public bool changingClippingHeight;


    void Start()
    {
        apiLinks = this.gameObject.GetComponent<API_Links>();

        changingBuildings = false;

        inPlayer = false;
        inCatalog = true;
        inTopViewCamera = false;
        inInputBox = false;

        mousePositionPrevious = Input.mousePosition;
        rotatingCameraSleep = false;
        currentCameraRotationDegreesX = 30;
        currentCameraRotationDegreesY = -180;

        HandleChangeClippingHeight();

        inGuideMode = false;
        nextIndexToReach = 0;

        listOfItems = new List<GameObject>();

        cameraZooming = false;
        interpolationFramesCount = 60;
        elapsedFrames = 0;

        changingClippingHeight = false;

        username = "";
        token = "";

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

        if (!inPlayer && !inCatalog && Input.GetMouseButton(2))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - cameraDragOrigin);
            Vector3 move = new Vector3(pos.x * 20f, 0, pos.y * 20f);

            // If past a certain angle looking up/down, consider up to be horizontal
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

        if (!inPlayer && Input.GetMouseButtonUp(2))
        {
            Cursor.visible = true;
        }
    }

    void Update()
    {
        // If in preview mode and C is pressed, open Catalog
        if (!inCatalog && !inInputBox && inPlayer && Input.GetKeyDown(KeyCode.C))
        {
            inCatalog = true;
            ui.gameObject.GetComponent<CatalogPlayerScene>().OpenCatalog();
        }

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
        if (!inPlayer && !inCatalog && !cameraZooming)
        {
            CameraMovement();
        }

        // If in preview mode and ESCAPE is pressed
        if (inPlayer && !inCatalog && !inInputBox && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            // Change camera mode to edit
            SetCameraMode(false);
        }

        // If in Top View mode and ESCAPE/Q is pressed, exit
        if (!inPlayer && inTopViewCamera && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            // Change camera mode to edit
            SetCameraTopView();
        }

        // If in catalog and ESCAPE/Q is pressed, exit
        if (inPlayer && inCatalog && !inInputBox && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
        {
            // Change camera mode to edit
            ui.CloseCatalog();
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

        // If inside a hotspot and an item was added and E is pressed, remove item from hostpot
        if (hotspot != null && !inCatalog && Input.GetKeyDown(KeyCode.E) && hotspot.objInHotspot != null)
        {
            GameObject temp = hotspot.objInHotspot;
            hotspot.objInHotspot = null;
            hotspot.GetComponent<HotspotPlayer>().RemoveObj(temp);
            // temp.SetActive(false);
        }

        // If close to a postit note and E is pressed, remove said note
        if (postit != null && !inCatalog && Input.GetKeyDown(KeyCode.E))
        {
            GameObject.Destroy(postit);
            postit = null;
        }

        // Hide/Show buildings keybind combination ("secret")
        if (!changingBuildings && Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.K))
        {
            StartCoroutine(ChangeBuildingThenWait());
        }

        // Camera zoom in/out
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

    public void CameraRotation()
    {
        if (!rotatingCameraSleep)
        {
            // Hide mouse cursor
            Cursor.visible = false;

            // Get mouse position
            Vector2 currentPosition = Input.mousePosition;

            // Calculate new deltas and update previous position
            float deltaPositionX = currentPosition.x - mousePositionPrevious.x;
            float deltaPositionY = currentPosition.y - mousePositionPrevious.y;

            mousePositionPrevious = currentPosition;

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

    private IEnumerator SleepThenChangeRotateCameraBool()
    {
        yield return new WaitForSeconds(.01f * Time.deltaTime);
        rotatingCameraSleep = false;
    }

    public void CameraZoom()
    {
        Vector3 transformVec = cam.transform.forward;

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

    private IEnumerator CameraZoomLerp(Vector3 startPlayerPosition, Vector3 targetPlayerPosition)
    {
        // Lerp player towards target location
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
        Vector3 interpolatedPosition = Vector3.Lerp(startPlayerPosition, targetPlayerPosition, interpolationRatio);

        cam.transform.position = interpolatedPosition;

        yield return new WaitForEndOfFrame();

        elapsedFrames += 1;

        // Check distance between player and target. If at target, stop, otherwise, move again
        if (Vector3.Distance(cam.transform.position, targetPlayerPosition) <= .01f)
        {
            cameraZooming = false;
            elapsedFrames = 0;
        }
        else
            StartCoroutine(CameraZoomLerp(startPlayerPosition, targetPlayerPosition));

    }

    public void CameraMovement()
    {
        Vector3 transformVec = new Vector3(0, 0, 0);
        float multiplier = 0f;

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

        // Space and Control (vertical movement)
        if (Input.GetKey(KeyCode.Space) && !inTopViewCamera)
        {
            cam.transform.position += new Vector3(0, 1, 0) * multiplier * 2 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && !inTopViewCamera)
        {
            cam.transform.position -= new Vector3(0, 1, 0) * multiplier * 2 * Time.deltaTime;
        }
    }

    public void SetCameraMode(bool newMode)
    {
        // If in top view, exit first. This will update Top View button in side menu, along with camera clipping plane
        if (inTopViewCamera)
            SetCameraTopView();

        hotspot = null;

        // Update various modes and buttons
        inPlayer = newMode;

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

            player.GetComponent<PlayerMovementPointClick>().targetPositionObj.SetActive(false);

            // Reset video/audio players if any was active
            if (videoPlayer != null)
            {
                videoPlayer.GetComponent<VideoPlayerControls>().Reset();
            }

            if (audioPlayer != null)
            {
                audioPlayer.GetComponent<AudioPlayerControls>().Reset();
            }
        }

        player.GetComponent<PlayerMovement>().lastMovementTime = Time.time;

        // Hide/Show various UI elements
        ui.sideMenu.SetActive(!inPlayer);
        ui.sideMenuItemListing.SetActive(!inPlayer);
        ui.qKeyIcon.SetActive(inPlayer);
        ui.exitIcon.SetActive(inPlayer);
        ui.cKeyIcon.SetActive(inPlayer);
        ui.addObjectButton.SetActive(inPlayer);

        ui.crosshair.SetActive(inPlayer);

        player.SetActive(newMode);

        // Update cursor
        Cursor.visible = !newMode;
        if (newMode)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

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
            ui.topViewCameraButton.GetComponent<ButtonCameraTopViewPlayer>().ResetToBaseColor();
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

    // Used when double clicking an item listing, moves camera to the item, in view/center screen
    public void MoveCameraToSelected(GameObject objToGetTo)
    {
        Vector3 itemPosition = objToGetTo.transform.position;
        cam.transform.position = itemPosition;

        cam.transform.position -= cam.transform.forward * 10;
    }

    public void HandleChangeClippingHeight()
    {
        changingClippingHeight = true;

        currentClippingHeight = ui.horizontalClippingSlider.GetComponent<Slider>().value;
        CheckItemBelowAboveWorkingLevel();
    }

    private void CheckItemBelowAboveWorkingLevel()
    {
        foreach (GameObject obj in listOfItems)
        {
            if (obj.transform.position.y < currentClippingHeight)
                obj.SetActive(true);
            else
                obj.SetActive(false);
        }
    }


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

    [DllImport("__Internal")]
    private static extern void GetUsernameAndToken();

    public void GetUsernameAndTokenLocal(string text)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            GetUsernameAndToken();
#else
        Debug.Log("get username and token called");
#endif
    }


    public void StartGuide()
    {
        inGuideMode = true;

        nextIndexToReach = 1;

        Vector3 startPosition = guidePathList[0].transform.position;
        lastPlayerPositionInGuide = startPosition;
        player.transform.position = lastPlayerPositionInGuide;

        StartCoroutine(WaitThenStartGuideMovement());
    }

    public void ContinueGuide()
    {
        if (guidePathList.Count > 0)
        {
            inGuideMode = true;

            // If it's 0, means that it was never started before. Treat it as a fresh start
            if (nextIndexToReach == 0)
            {
                Vector3 startPosition = guidePathList[0].transform.position;
                lastPlayerPositionInGuide = startPosition;
            }

            // Update player position to last position in guide
            player.transform.position = lastPlayerPositionInGuide;

            // Start guide movement
            StartCoroutine(WaitThenStartGuideMovement());
        }
    }

    private IEnumerator WaitThenStartGuideMovement()
    {
        yield return new WaitForSeconds(.1f);
        StartCoroutine(GuideMovement());
    }

    private IEnumerator GuideMovement()
    {
        Vector3 nextPointPosition = guidePathList[nextIndexToReach].transform.position;

        // Move player towards the next point
        player.GetComponent<CharacterController>().Move((nextPointPosition - lastPlayerPositionInGuide).normalized * .01f);

        yield return new WaitForSeconds(.01f);

        // Update last player position
        lastPlayerPositionInGuide = player.transform.position;

        // If still hasn't exited guide mode, continue movement
        if (inGuideMode)
        {
            // Calculate distance to next point. If smaller than a certain threshold, move to next item
            if (Vector3.Distance(player.transform.position, nextPointPosition) <= .1f)
            {
                // If last point reached, stop guide
                if (nextIndexToReach < guidePathList.Count - 1)
                {
                    nextIndexToReach++;
                    StartCoroutine(GuideMovement());
                }
                else
                {
                    inGuideMode = false;
                    nextIndexToReach = 1;
                    lastPlayerPositionInGuide = guidePathList[0].transform.position;
                }
            }
            else
                StartCoroutine(GuideMovement());
        }
    }

    public void UpdateTypeButtonSelection(int prevType, int newType)
    {
        if (prevType >= 0)
            typeButtons[prevType].GetComponent<ButtonCatalogTypePlayer>().SetSelected(false);
        typeButtons[newType].GetComponent<ButtonCatalogTypePlayer>().SetSelected(true);
    }

    public void AddToHotspot(GameObject objToAdd, int type)
    {
        hotspot.AddObj(objToAdd, type);
    }

    public void LoadSceneJson(string museumId)
    {
        loadSceneComponent.LoadSceneFromJson(museumId);
    }

    public void AddItemListing(GameObject objInListing, string listingItemName)
    {
        ui.AddItemListing(objInListing, listingItemName);
    }

    public void SetUsernamAndToken(string uname, string tk)
    {
        username = uname;
        token = tk;
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
