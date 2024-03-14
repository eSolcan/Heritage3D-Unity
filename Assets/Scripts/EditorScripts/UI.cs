using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Camera cam;
    private Controller controller;
    public GameObject mainCanvas;

    public GameObject catalog;
    public GameObject widgetCatalog;
    public GameObject lightingCatalog;
    public GameObject portalCatalog;
    public GameObject infoScreen;
    public GameObject guideMenu;

    public GameObject selectedItemUIElements;
    public Slider rotationSlider;
    public GameObject increaseScaleButton;
    public GameObject decreaseScaleButton;
    public GameObject linkPortalButton;

    public float scaleIncreasePerClick = .05f;

    public GameObject lineObject;
    public GameObject lineObjectInstance;
    public bool lineDraw;
    private float currentLineLevelY;
    private Vector3 lineStartPosition;
    public GameObject colliderPlaneWall;
    public GameObject colliderPlaneFloor;
    public GameObject sphere;

    public GameObject colliderButtonWalls;
    public GameObject colliderButtonFloors;
    public GameObject previewButton;
    public GameObject setStartPosButton;

    public GameObject addObjectButton;
    public GameObject lightingButton;
    public GameObject saveMuseumButton;
    public GameObject infoMenuButton;
    public GameObject guideMenuButton;
    public GameObject cKeyIcon;
    public GameObject lKeyIcon;
    public GameObject qKeyIcon;
    public GameObject gKeyIcon;
    public GameObject exitIcon;

    public GameObject itemListingPrefab;
    public GameObject itemListingContent;

    public GameObject topViewCameraButton;

    public GameObject sideMenu;
    public bool sideMenuOpen = false;
    private bool sideMenuInAnimation = false;
    private float sideMenuMinPosition;
    private float sideMenuMaxPosition;

    public GameObject sideMenuItemListing;
    public bool sideMenuItemListingOpen = false;
    private bool sideMenuItemListingInAnimation = false;

    private Vector3 colliderMultiPointStartLocation;

    public Vector3 camCenterPointGround;
    public GameObject groundForCameraRaycast;

    private float previousLeftClickTime;
    private float doubleClickTime = .2f;

    private float previousRightClickDownTime;
    private float previousRightClipUpTime;

    public GameObject colliderForWall;

    public GameObject horizontalClippingSlider;
    public GameObject floorHeightSlider;

    public GameObject portalItemNameAndUpload;
    public GameObject minimumThreeCharacters;

    public GameObject startPositionObj;

    public GameObject sideMenuTargetLocationOut;
    public GameObject sideMenuTargetLocationIn;

    public Vector3 wallDrawStartPosition;

    public GameObject museumNameAndUpload;
    public GameObject minimumThreeCharactersMuseum;
    public GameObject buttonSubmitMuseumName;

    void Start()
    {
        cam = Camera.main;
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        colliderButtonWalls = GameObject.Find("ColliderButtonWalls");
        colliderButtonFloors = GameObject.Find("ColliderButtonFloors");
        previewButton = GameObject.Find("PreviewButton");
        setStartPosButton = GameObject.Find("SetStartingPositionButton");

        addObjectButton = GameObject.Find("AddItemButton");
        lightingButton = GameObject.Find("ButtonChangeLighting");
        infoMenuButton = GameObject.Find("InfoButton");
        guideMenuButton = GameObject.Find("GuideMenuButton");
        saveMuseumButton = GameObject.Find("SaveMenuButton");

        lineDraw = false;
        currentLineLevelY = controller.currentWorkingFloorLevel;
        colliderMultiPointStartLocation = new Vector3(0, 0, 0);

        camCenterPointGround = new Vector3(0, 0, 0);
        controller.camCenterPointGround = camCenterPointGround;
        groundForCameraRaycast.SetActive(false);

        previousLeftClickTime = 0f;
        previousRightClickDownTime = 0f;
        previousRightClipUpTime = 0f;

        lineObjectInstance = Instantiate(lineObject);
        lineObjectInstance.SetActive(false);

        sideMenuMinPosition = sideMenu.transform.localPosition.x;
        sideMenuMaxPosition = sideMenuMinPosition + 195f;

    }

    void Update()
    {
        // If cursor over UI element, ignore everything
        if (controller.cursorOverSideMenu)
            return;

        // Start point of line. Can't draw if an item is selected
        if (Input.GetMouseButtonDown(0) && (controller.colliderModeWalls || controller.colliderModeFloors) && controller.selectedItem == null && !lineDraw)
        {
            lineDraw = true;
            currentLineLevelY = controller.currentWorkingFloorLevel;
            lineObjectInstance.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (new Plane(Vector3.down, currentLineLevelY).Raycast(ray, out float distance))
            {
                lineStartPosition = ray.GetPoint(distance);
                lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, lineStartPosition);

                wallDrawStartPosition = lineStartPosition;
            }
            return;
        }

        // While drawing line, adjust end point to follow cursor
        if (lineDraw && (controller.colliderModeWalls || controller.colliderModeFloors))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // float tempY = lineObjectInstance.GetComponent<LineRenderer>().GetPosition(0).y;
            if (new Plane(Vector3.down, currentLineLevelY).Raycast(ray, out float distance))
            {
                lineStartPosition = ray.GetPoint(distance);
                lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, lineStartPosition);
            }
        }

        // -------------------------------------------- COLLIDER WALL INSTANTIATE --------------------------------------------

        // End point of line
        if (Input.GetMouseButtonDown(0) && controller.colliderModeWalls && lineDraw)
        {
            // Update previous click time, for double click detection
            bool stopDrawing = false;
            if (Time.time - previousLeftClickTime <= doubleClickTime)
            {
                stopDrawing = true;
            }

            // Disable line draw mode
            lineDraw = false;

            // Find center point of line
            Vector3 pos1 = lineObjectInstance.GetComponent<LineRenderer>().GetPosition(0);
            Vector3 pos2 = lineObjectInstance.GetComponent<LineRenderer>().GetPosition(1);

            // Get distance between start and end points
            float distanceBetweenPoints = Vector3.Distance(pos1, pos2) / 10;

            // Only draw collider if minimum distance is met
            if (distanceBetweenPoints > .02f)
            {
                // Instantiate new collider plane at center. Offset Y center to be higher than base level
                Vector3 center = (pos2 + pos1) / 2;
                center += new Vector3(0, 2, 0);
                GameObject newPlane = GameObject.Instantiate(colliderPlaneWall, center, colliderPlaneWall.transform.localRotation);

                // Add outline
                Outline outline = newPlane.AddComponent<Outline>();
                outline.OutlineWidth = 5;
                outline.enabled = false;

                newPlane.transform.GetChild(0).localScale = new Vector3(distanceBetweenPoints, .1f, .4f);
                newPlane.transform.GetChild(1).localScale = new Vector3(distanceBetweenPoints, .1f, .4f);

                // Find angle between the two points and apply to planes
                float angle = (float)(Math.Atan2(pos2.z - pos1.z, pos2.x - pos1.x) * (180 / Math.PI)); //gives in degrees

                newPlane.transform.localEulerAngles = new Vector3(0, -angle, 0);

                // Instantiate sphere that will allow dragging of the object
                GameObject newSphere = GameObject.Instantiate(sphere, newPlane.transform.position, new Quaternion(0, 180, 0, 0));
                newSphere.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);

                // Set collider as child of dragable
                newPlane.transform.SetParent(newSphere.transform);

                // Add dragable to list of dragables
                string newId = "ColliderPlaneWall" + DateTime.UtcNow.Millisecond;
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().id = newId;
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().type = 100;

                GameObject newListing = controller.AddItemListing(newSphere.transform.GetChild(0).gameObject, "Collider Wall", newId);
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListing = newListing.GetComponent<SceneItemListing>();

                controller.AddToScene(newSphere.transform.GetChild(0).gameObject);

                // Add collider to list of coliders and disable line object
                // controller.AddCollider(newPlane);
                lineObjectInstance.SetActive(false);

                // Add collider points to wall based on its length
                AddCollidersToWall(newPlane, distanceBetweenPoints);

                // If in multi point mode, re-enable linedraw and continue drawing until exit collider mode click
                if (!stopDrawing)
                {
                    lineDraw = true;

                    lineObjectInstance.SetActive(true);

                    if (colliderMultiPointStartLocation != new Vector3(0, 0, 0))
                        colliderMultiPointStartLocation = lineStartPosition;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (new Plane(Vector3.down, currentLineLevelY).Raycast(ray, out float distance))
                    {
                        lineStartPosition = ray.GetPoint(distance);
                        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, lineStartPosition);
                    }
                }
                previousLeftClickTime = Time.time;
            }
        }

        // -------------------------------------------- COLLIDER FLOOR INSTANTIATE --------------------------------------------

        // End point of line
        if (Input.GetMouseButtonDown(0) && controller.colliderModeFloors && lineDraw)
        {
            // Disable line draw mode
            lineDraw = false;

            // Find center point of line
            Vector3 pos1 = lineObjectInstance.GetComponent<LineRenderer>().GetPosition(0);
            Vector3 pos2 = lineObjectInstance.GetComponent<LineRenderer>().GetPosition(1);

            // Get distance between points
            float distanceBetweenPoints = Vector3.Distance(pos1, pos2) / 10;
            float distanceBetweenX = Mathf.Abs(pos1.x - pos2.x) / 10;
            float distanceBetweenZ = Mathf.Abs(pos1.z - pos2.z) / 10;

            // Only draw collider if minimum distance is met
            if (distanceBetweenPoints > .02f)
            {
                // Instantiate new collider plane at center. Offset Y center to be higher than base level
                Vector3 center = (pos2 + pos1) / 2;

                GameObject newPlane = GameObject.Instantiate(colliderPlaneFloor, center, colliderPlaneFloor.transform.localRotation);

                // Add outline
                Outline outline = newPlane.AddComponent<Outline>();
                outline.OutlineWidth = 5;
                outline.enabled = false;

                newPlane.transform.GetChild(0).localScale = new Vector3(distanceBetweenX, .1f, distanceBetweenZ);
                newPlane.transform.GetChild(1).localScale = new Vector3(distanceBetweenX, .1f, distanceBetweenZ);

                newPlane.transform.localEulerAngles = new Vector3(0, 0, 0);

                // Instantiate sphere that will allow dragging of the object
                GameObject newSphere = GameObject.Instantiate(sphere, newPlane.transform.position, new Quaternion(0, 180, 0, 0));
                newSphere.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);

                // Set collider as child of dragable
                newPlane.transform.SetParent(newSphere.transform);

                // Add dragable to list of dragables
                string newId = "ColliderPlaneFloor" + DateTime.UtcNow.Millisecond;
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().id = newId;
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().type = 101;

                GameObject newListing = controller.AddItemListing(newSphere.transform.GetChild(0).gameObject, "Collider Wall", newId);
                newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListing = newListing.GetComponent<SceneItemListing>();

                controller.AddToScene(newSphere.transform.GetChild(0).gameObject);

                // Disable line object
                lineObjectInstance.SetActive(false);

                previousLeftClickTime = Time.time;
            }
        }

        // Update right click down time
        if (Input.GetMouseButtonDown(1) && (controller.colliderModeWalls || controller.colliderModeFloors) && lineDraw)
        {
            previousRightClickDownTime = Time.time;
        }

        // Update right click up time, and if time matches, exit linedraw
        if (Input.GetMouseButtonUp(1) && (controller.colliderModeWalls || controller.colliderModeFloors) && lineDraw)
        {
            previousRightClipUpTime = Time.time;
            if (previousRightClipUpTime - previousRightClickDownTime <= doubleClickTime)
            {
                lineDraw = false;

                lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
                lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

                lineObjectInstance.SetActive(false);
            }
        }
    }

    // When a collider wall is instantiated, add colliders to the plane.
    // These are same width, and the amount added is based on the size of the wall
    // Also serve as snapping points for other items
    private void AddCollidersToWall(GameObject newPlane, float planeSize)
    {
        float howManyColliders = planeSize * 10;
        for (int i = 0; i < howManyColliders - 1; i++)
        {
            GameObject newCollider = Instantiate(colliderForWall);
            newCollider.transform.SetParent(newPlane.transform.GetChild(0), false);
            newCollider.transform.localPosition = new Vector3(-4.2f + (i / planeSize), .8f, 0);
            float scaleNewWhyThisTakeSoLongToFigureOut = 1 / newPlane.transform.GetChild(0).transform.localScale.x;
            newCollider.transform.localScale = new Vector3(scaleNewWhyThisTakeSoLongToFigureOut, 1, 1);
        }

        for (int i = 0; i < howManyColliders - 1; i++)
        {
            GameObject newCollider = Instantiate(colliderForWall);
            newCollider.transform.SetParent(newPlane.transform.GetChild(1), false);
            newCollider.transform.localPosition = new Vector3(-4.2f + (i / planeSize), .8f, 0);
            float scaleNewWhyThisTakeSoLongToFigureOut = 1 / newPlane.transform.GetChild(1).transform.localScale.x;
            newCollider.transform.localScale = new Vector3(scaleNewWhyThisTakeSoLongToFigureOut, 1, 1);
        }
    }

    public void UpdateCameraGroundCenterPoint()
    {
        Vector3 camPosition = cam.transform.position;

        groundForCameraRaycast.SetActive(true);

        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, 7);

        groundForCameraRaycast.SetActive(false);

        camCenterPointGround = hit.point;
        controller.camCenterPointGround = camCenterPointGround;
    }

    // Used to rotate the currently selected item
    public void RotateSelected()
    {
        if (controller.selectedItem != null)
        {
            Vector3 originalRotation = controller.selectedItem.transform.parent.GetChild(1).localEulerAngles;
            controller.selectedItem.transform.parent.GetChild(1).localEulerAngles = new Vector3(originalRotation.x, rotationSlider.value, originalRotation.z);
        }
    }

    // Update slider to be in line with newly selected item
    public void UpdateSliderNewlySelected(int rotationValue)
    {
        rotationSlider.value = rotationValue;
    }

    // Increase scale of selected item
    public void IncreaseScale()
    {
        if (controller.selectedItem != null)
        {
            // Increase scale
            controller.selectedItem.transform.parent.GetChild(1).localScale *= 1.01f;

            // If its a light source, also change its radius and intensity (not affected by scale)
            if (controller.selectedItem.GetComponent<Item>().type == 7)
            {
                controller.selectedItem.transform.parent.GetChild(1).gameObject.GetComponent<Light>().range *= 1.01f;
                controller.selectedItem.transform.parent.GetChild(1).gameObject.GetComponent<Light>().intensity *= 1.01f;
            }
        }
    }

    // Decrease scale of selected item
    public void DecreaseScale()
    {
        if (controller.selectedItem != null)
        {
            // Decreas scale until a certain threshold
            Vector3 currentScale = controller.selectedItem.transform.parent.GetChild(1).localScale;
            if (currentScale.x > .05f || currentScale.y > .05f || currentScale.z > .05f)
                controller.selectedItem.transform.parent.GetChild(1).localScale *= 0.99f;

            // If its a light source, also change its radius and intensity (not affected by scale)
            if (controller.selectedItem.GetComponent<Item>().type == 7)
            {
                controller.selectedItem.transform.parent.GetChild(1).gameObject.GetComponent<Light>().range *= 0.99f;
                controller.selectedItem.transform.parent.GetChild(1).gameObject.GetComponent<Light>().intensity *= 0.99f;
            }
        }
    }

    // Enter/Exit collider mode walls
    public void ColliderModeWalls()
    {
        lineDraw = false;

        // If other collider mode, disable
        if (controller.colliderModeFloors)
            ColliderModeFloors();

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        // If an item was selected, unselect before entering collider mode
        if (controller.selectedItem != null)
            controller.selectedItem.GetComponent<Item>().SetSelected(false);

        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

        // Change collider mode in controller and disable visibility of draw line
        controller.colliderModeWalls = !controller.colliderModeWalls;
        lineObjectInstance.SetActive(controller.colliderModeWalls);

        if (!controller.colliderModeWalls)
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();

        // colliderButtonMulti.GetComponent<ButtonColliderModeMulti>().SetClickable(controller.colliderMode);
    }

    // Enter/Exit collider mode floors
    public void ColliderModeFloors()
    {
        lineDraw = false;

        // If other collider mode, disable
        if (controller.colliderModeWalls)
            ColliderModeWalls();

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        // If an item was selected, unselect before entering collider mode
        if (controller.selectedItem != null)
            controller.selectedItem.GetComponent<Item>().SetSelected(false);

        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

        // Change collider mode in controller and disable visibility of draw line
        controller.colliderModeFloors = !controller.colliderModeFloors;
        lineObjectInstance.SetActive(controller.colliderModeFloors);

        if (!controller.colliderModeFloors)
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();

        // colliderButtonMulti.GetComponent<ButtonColliderModeMulti>().SetClickable(controller.colliderMode);
    }

    // Close UI elements and unselect item when opening the catalog
    public void OpenCatalog()
    {
        catalog.SetActive(true);

        if (this.GetComponent<Catalog>().currentType == 4)
            this.GetComponent<Catalog>().inputField.GetComponent<TMP_InputField>().text = "";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        guideMenuButton.SetActive(false);
        saveMuseumButton.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        if (controller.colliderModeWalls || controller.colliderModeFloors)
        {
            controller.colliderModeWalls = false;
            controller.colliderModeFloors = false;
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
        }

        if (controller.selectedItem)
        {
            controller.selectedItem.GetComponent<Item>().SetSelected(false);
        }
    }

    // Reactivate UI elements when closing catalog
    public void CloseCatalog()
    {
        catalog.SetActive(false);

        if (!controller.inPlayer)
        {
            sideMenu.SetActive(true);
            sideMenuItemListing.SetActive(true);
            guideMenuButton.SetActive(true);

            gKeyIcon.SetActive(true);
            // infoMenuButton.SetActive(true);
        }
        else
        {
            qKeyIcon.SetActive(true);
            exitIcon.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        saveMuseumButton.SetActive(true);

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);

    }

    // Close UI elements and unselect item when opening the widget catalog
    public void OpenWidgetCatalog()
    {
        widgetCatalog.SetActive(true);

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        saveMuseumButton.SetActive(false);
        guideMenuButton.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        selectedItemUIElements.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);

        controller.inCatalog = true;
    }

    // Reactivate UI elements when closing widget catalog
    public void CloseWidgetCatalog()
    {
        widgetCatalog.SetActive(false);

        sideMenu.SetActive(true);
        sideMenuItemListing.SetActive(true);

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        guideMenuButton.SetActive(true);
        saveMuseumButton.SetActive(true);

        selectedItemUIElements.SetActive(true);

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);
        gKeyIcon.SetActive(true);

        controller.inCatalog = false;
    }

    public GameObject AddItemListing(GameObject objInListing, string name, string id)
    {
        // Instantiate new item listing and parent to grid
        GameObject newListing = Instantiate(itemListingPrefab);
        newListing.transform.SetParent(itemListingContent.transform, false);

        // Set display name and associated game object to listing (for on click)
        newListing.GetComponent<SceneItemListing>().SetItemName(name);
        newListing.GetComponent<SceneItemListing>().SetItemId(id);
        newListing.GetComponent<SceneItemListing>().associatedObject = objInListing;

        return newListing;
    }

    public void OpenCloseSideMenu()
    {
        if (sideMenuOpen)
        {
            sideMenuMaxPosition = sideMenu.transform.localPosition.x;
            sideMenuMinPosition = sideMenuMaxPosition - 195;
        }
        else
        {
            sideMenuMinPosition = sideMenu.transform.localPosition.x;
            sideMenuMaxPosition = sideMenuMinPosition + 195;
        }

        if (!sideMenuInAnimation)
        {
            sideMenuInAnimation = true;
            sideMenuOpen = !sideMenuOpen;
            StartCoroutine(SideMenuAnimation());
        }
    }

    // Sliding movement for opening/closing the side menu
    private IEnumerator SideMenuAnimation()
    {
        // If true, means is opening
        if (sideMenuOpen && sideMenu.transform.localPosition.x < sideMenuTargetLocationOut.transform.localPosition.x)
            sideMenu.transform.localPosition += new Vector3(500f, 0, 0) * Time.deltaTime;
        else if (sideMenuOpen && sideMenu.transform.localPosition.x >= sideMenuTargetLocationOut.transform.localPosition.x)
        {
            sideMenuInAnimation = false;
            Vector3 currPosition = sideMenu.transform.localPosition;
            sideMenu.transform.localPosition = new Vector3(sideMenuTargetLocationOut.transform.localPosition.x, currPosition.y, currPosition.z);
        }
        else if (!sideMenuOpen && sideMenu.transform.localPosition.x > sideMenuTargetLocationIn.transform.localPosition.x)
            sideMenu.transform.localPosition -= new Vector3(500f, 0, 0) * Time.deltaTime;
        else if (!sideMenuOpen && sideMenu.transform.localPosition.x <= sideMenuTargetLocationIn.transform.localPosition.x)
        {
            sideMenuInAnimation = false;
            Vector3 currPosition = sideMenu.transform.localPosition;
            sideMenu.transform.localPosition = new Vector3(sideMenuTargetLocationIn.transform.localPosition.x, currPosition.y, currPosition.z);
        }

        yield return new WaitForSeconds(.0005f);
        if (sideMenuInAnimation)
            StartCoroutine(SideMenuAnimation());
    }

    public void OpenCloseSideMenuItemListing()
    {
        if (sideMenuItemListingOpen)
        {
            sideMenuMaxPosition = sideMenuItemListing.transform.localPosition.x;
            sideMenuMinPosition = sideMenuMaxPosition - 195;
        }
        else
        {
            sideMenuMinPosition = sideMenuItemListing.transform.localPosition.x;
            sideMenuMaxPosition = sideMenuMinPosition + 195;
        }

        if (!sideMenuItemListingInAnimation)
        {
            sideMenuItemListingInAnimation = true;
            sideMenuItemListingOpen = !sideMenuItemListingOpen;
            StartCoroutine(SideMenuItemListingAnimation());
        }
    }

    // Sliding movement for opening/closing the side menu item listing
    private IEnumerator SideMenuItemListingAnimation()
    {
        // If true, means is opening
        if (sideMenuItemListingOpen && sideMenuItemListing.transform.localPosition.x < sideMenuTargetLocationOut.transform.localPosition.x)
            sideMenuItemListing.transform.localPosition += new Vector3(500f, 0, 0) * Time.deltaTime;
        else if (sideMenuItemListingOpen && sideMenuItemListing.transform.localPosition.x >= sideMenuTargetLocationOut.transform.localPosition.x)
        {
            sideMenuItemListingInAnimation = false;
            Vector3 currPosition = sideMenuItemListing.transform.localPosition;
            sideMenuItemListing.transform.localPosition = new Vector3(sideMenuTargetLocationOut.transform.localPosition.x, currPosition.y, currPosition.z);
        }
        else if (!sideMenuItemListingOpen && sideMenuItemListing.transform.localPosition.x > sideMenuTargetLocationIn.transform.localPosition.x)
            sideMenuItemListing.transform.localPosition -= new Vector3(500f, 0, 0) * Time.deltaTime;
        else if (!sideMenuItemListingOpen && sideMenuItemListing.transform.localPosition.x <= sideMenuTargetLocationIn.transform.localPosition.x)
        {
            sideMenuItemListingInAnimation = false;
            Vector3 currPosition = sideMenuItemListing.transform.localPosition;
            sideMenuItemListing.transform.localPosition = new Vector3(sideMenuTargetLocationIn.transform.localPosition.x, currPosition.y, currPosition.z);
        }

        yield return new WaitForSeconds(.0005f);
        if (sideMenuItemListingInAnimation)
            StartCoroutine(SideMenuItemListingAnimation());
    }

    // public void OpenInfoScreen()
    // {
    //     controller.inCatalog = true;

    //     infoScreen.SetActive(true);

    //     sideMenu.SetActive(false);
    //     sideMenuItemListing.SetActive(false);

    //     addObjectButton.SetActive(false);
    //     lightingButton.SetActive(false);
    //     guideMenuButton.SetActive(false);
    //     saveMuseumButton.SetActive(false);

    //     // If start position mode, disable
    //     if (controller.inSetStartPosition)
    //         SetStartPositionToggle();

    //     if (controller.colliderModeWalls || controller.colliderModeFloors)
    //     {
    //         controller.colliderModeWalls = false;
    //         controller.colliderModeFloors = false;
    //         colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
    //         colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
    //     }

    //     if (controller.selectedItem)
    //     {
    //         controller.selectedItem.GetComponent<Item>().SetSelected(false);
    //     }
    // }

    // public void CloseInfoScreen()
    // {
    //     infoScreen.SetActive(false);

    //     if (!controller.inPlayer)
    //     {
    //         sideMenu.SetActive(true);
    //         sideMenuItemListing.SetActive(true);
    //     }

    //     addObjectButton.SetActive(true);
    //     lightingButton.SetActive(true);
    //     guideMenuButton.SetActive(true);
    //     saveMuseumButton.SetActive(true);

    //     controller.inCatalog = false;
    // }

    public void OpenLightingCatalog()
    {
        controller.inCatalog = true;

        lightingCatalog.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        saveMuseumButton.SetActive(false);
        guideMenuButton.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        if (controller.colliderModeWalls || controller.colliderModeFloors)
        {
            controller.colliderModeWalls = false;
            controller.colliderModeFloors = false;
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
        }

        if (controller.selectedItem)
        {
            controller.selectedItem.GetComponent<Item>().SetSelected(false);
        }
    }

    public void CloseLightingCatalog()
    {
        lightingCatalog.SetActive(false);

        if (!controller.inPlayer)
        {
            sideMenu.SetActive(true);
            sideMenuItemListing.SetActive(true);

            // infoMenuButton.SetActive(true);
            guideMenuButton.SetActive(true);

            gKeyIcon.SetActive(true);
        }
        else
        {
            qKeyIcon.SetActive(true);
            exitIcon.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        saveMuseumButton.SetActive(true);

        controller.inCatalog = false;

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);
    }

    public void OpenPortalCatalog()
    {
        controller.inCatalog = true;

        portalCatalog.SetActive(true);

        StartCoroutine(WaitThenLoadPortals());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        guideMenuButton.SetActive(false);
        selectedItemUIElements.SetActive(false);
        saveMuseumButton.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        if (controller.colliderModeWalls || controller.colliderModeFloors)
        {
            controller.colliderModeWalls = false;
            controller.colliderModeFloors = false;
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
        }
    }

    // Needed to wait a bit before loading, otherwise was loading duplicates for some reason
    private IEnumerator WaitThenLoadPortals()
    {
        yield return new WaitForEndOfFrame();

        portalCatalog.GetComponent<PortalCatalog>().ClearContent();
        portalCatalog.GetComponent<PortalCatalog>().LoadObjectsInGrid();
    }

    public void ClosePortalCatalog()
    {
        portalCatalog.SetActive(false);

        if (!controller.inPlayer)
        {
            sideMenu.SetActive(true);
            sideMenuItemListing.SetActive(true);
            selectedItemUIElements.SetActive(false);

            // infoMenuButton.SetActive(true);

            guideMenuButton.SetActive(true);

            gKeyIcon.SetActive(true);
        }
        else
        {
            qKeyIcon.SetActive(true);
            exitIcon.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        saveMuseumButton.SetActive(true);

        controller.inCatalog = false;

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);
    }

    public void OpenPortalItemNameBox()
    {
        portalCatalog.GetComponent<PortalCatalog>().itemNameAndUpload.SetActive(true);

        GameObject button = GameObject.Find("SubmitPortalName");
        button.GetComponent<ButtonSubmitPortalName>().ResetToBaseColor();

        controller.inCatalog = true;
        controller.inInputBox = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        saveMuseumButton.SetActive(false);
        guideMenuButton.SetActive(false);
        selectedItemUIElements.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        if (controller.colliderModeWalls || controller.colliderModeFloors)
        {
            controller.colliderModeWalls = false;
            controller.colliderModeFloors = false;
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
        }
    }

    public void AddPortal(GameObject button)
    {
        // Get the name to display on the item to be uploaded and clear the text input box
        string portalName = portalItemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        portalName = portalName.Replace("\n", " ");
        portalItemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";

        // Check if title meets minimum size requirement
        string trimmedMediaName = portalName.Replace(" ", "");

        if (trimmedMediaName.Length < 3)
        {
            StartCoroutine(TextSleepOff(minimumThreeCharacters));
            return;
        }
        else
        {
            button.GetComponent<Image>().color = button.GetComponent<ButtonSubmitPortalName>().baseColor;

            controller.selectedItem.GetComponent<Item>().itemName = "Portal - " + portalName;
            controller.selectedItem.GetComponent<Item>().itemListing.SetItemName("Portal - " + portalName);
            controller.listOfPortals.Add(controller.selectedItem);
            CloseItemNameTextBox();
            ClosePortalCatalog();

            controller.inInputBox = false;

            selectedItemUIElements.SetActive(true);
        }
    }

    // Coroutine to show a UI item for a period of time, then hide it. Commonly used for showing error messages
    private IEnumerator TextSleepOff(GameObject itemToShowAndHide)
    {
        itemToShowAndHide.SetActive(true);
        yield return new WaitForSeconds(5);
        itemToShowAndHide.SetActive(false);
    }

    // Used to close the item name dialog. Clears text and closes window
    public void CloseItemNameTextBox()
    {
        portalItemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
        portalItemNameAndUpload.SetActive(false);
    }

    // Used to open the Dialog for setting museum name and saving museum
    public void OpenMuseumNameTextBox()
    {
        // Set in input box state
        controller.inCatalog = true;
        controller.inInputBox = true;

        // Hide other UI elements
        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        saveMuseumButton.SetActive(false);
        guideMenuButton.SetActive(false);
        selectedItemUIElements.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);

        // Open museum name dialog and clear text
        museumNameAndUpload.SetActive(true);
        museumNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";

        // Clear button color
        buttonSubmitMuseumName.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);;
    }

    // Used to close the Dialog for setting museum name and saving museum
    public void CloseMuseumNameTextBox()
    {
        // Set in input box state
        controller.inCatalog = false;
        controller.inInputBox = false;

        // Unhide other UI elements
        sideMenu.SetActive(true);
        sideMenuItemListing.SetActive(true);

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        saveMuseumButton.SetActive(true);
        guideMenuButton.SetActive(true);
        selectedItemUIElements.SetActive(true);

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);
        gKeyIcon.SetActive(true);

        // Hide museum name dialog and clear text
        museumNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
        museumNameAndUpload.SetActive(false);
    }

    public void SaveMuseum()
    {
        // Get the name to save clear the text input box
        string museumName = museumNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        museumName = museumName.Replace("\n", " ");

        // Check if name meets minimum size requirement
        string trimmedName = museumName.Replace(" ", "");

        if (trimmedName.Length < 3)
        {
            StartCoroutine(TextSleepOff(minimumThreeCharactersMuseum));
            return;
        }
        else
        {
            controller.SaveSceneJson(museumName);
            museumNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
            CloseMuseumNameTextBox();
        }
    }

    public void OpenGuideMenu()
    {
        controller.inCatalog = true;

        guideMenu.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        sideMenu.SetActive(false);
        sideMenuItemListing.SetActive(false);

        addObjectButton.SetActive(false);
        lightingButton.SetActive(false);
        guideMenuButton.SetActive(false);
        saveMuseumButton.SetActive(false);

        cKeyIcon.SetActive(false);
        lKeyIcon.SetActive(false);
        gKeyIcon.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);

        // If start position mode, disable
        if (controller.inSetStartPosition)
            SetStartPositionToggle();

        if (controller.colliderModeWalls || controller.colliderModeFloors)
        {
            controller.colliderModeWalls = false;
            controller.colliderModeFloors = false;
            colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
            colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
        }

        if (controller.selectedItem)
        {
            controller.selectedItem.GetComponent<Item>().SetSelected(false);
        }
    }

    public void CloseGuideMenu()
    {
        controller.inCatalog = false;

        guideMenu.SetActive(false);

        if (!controller.inPlayer)
        {
            sideMenu.SetActive(true);
            sideMenuItemListing.SetActive(true);
            guideMenuButton.SetActive(true);

            gKeyIcon.SetActive(true);
        }
        else
        {
            qKeyIcon.SetActive(true);
            exitIcon.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        addObjectButton.SetActive(true);
        lightingButton.SetActive(true);
        saveMuseumButton.SetActive(true);

        cKeyIcon.SetActive(true);
        lKeyIcon.SetActive(true);
    }

    public void SetStartPositionToggle()
    {
        // If collider mode, disable
        if (controller.colliderModeWalls)
            ColliderModeWalls();
        else if (controller.colliderModeFloors)
            ColliderModeFloors();

        // If selected item, unselect
        if (controller.selectedItem != null)
            controller.selectedItem.GetComponent<Item>().SetSelected(false);

        // If an item was selected, unselect before entering "set start position" mode
        if (controller.selectedItem != null)
            controller.selectedItem.GetComponent<Item>().SetSelected(false);

        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

        // Change mode state
        controller.inSetStartPosition = !controller.inSetStartPosition;

        // Reset button color if required
        if (!controller.inSetStartPosition)
            setStartPosButton.GetComponent<ButtonSetStartPosition>().ResetColorButton();

        // Enable/Disable obj for start position
        startPositionObj.SetActive(controller.inSetStartPosition);
    }

}
