using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerScene : MonoBehaviour
{
    private Camera cam;
    private ControllerPlayerScene controller;
    public GameObject mainCanvas;

    public GameObject catalog;
    public GameObject addObjectButton;
    public GameObject loadSceneButton;

    public GameObject cKeyIcon;
    public GameObject qKeyIcon;
    public GameObject exitIcon;

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

    public GameObject horizontalClippingSlider;

    public GameObject crosshair;

    public GameObject sideMenuTargetLocationOut;
    public GameObject sideMenuTargetLocationIn;

    public GameObject itemListingPrefab;

    public GameObject museumListing;
    public GameObject museumListingContent;
    public GameObject museumListingPrefab;

    void Start()
    {
        cam = Camera.main;
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        sideMenuMinPosition = sideMenu.transform.localPosition.x;
        sideMenuMaxPosition = sideMenuMinPosition + 195f;
    }

    void Update()
    {

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

    // Close UI elements and unselect item when opening the catalog
    public void OpenCatalog()
    {
        catalog.SetActive(true);

        if (this.GetComponent<CatalogPlayerScene>().currentType == 4)
            this.GetComponent<CatalogPlayerScene>().inputField.GetComponent<TMP_InputField>().text = "";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        cKeyIcon.SetActive(false);
        addObjectButton.SetActive(false);
        // loadSceneButton.SetActive(false);
        qKeyIcon.SetActive(false);
        exitIcon.SetActive(false);
        crosshair.SetActive(false);

    }

    // Reactivate UI elements when closing catalog
    public void CloseCatalog()
    {
        catalog.SetActive(false);

        controller.inCatalog = false;
        controller.inInputBox = false;

        cKeyIcon.SetActive(true);
        addObjectButton.SetActive(true);
        // loadSceneButton.SetActive(true);

        qKeyIcon.SetActive(true);
        exitIcon.SetActive(true);
        crosshair.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        controller.player.GetComponent<PlayerMovement>().lastMovementTime = Time.time;

    }

    // Add item listing to list of scene items
    public void AddItemListing(GameObject objInListing, string listingItemName)
    {
        // Instantiate new item listing and parent to grid
        GameObject newListing = Instantiate(itemListingPrefab);
        newListing.transform.SetParent(itemListingContent.transform, false);

        // Set display name and associated game object to listing (for on click)
        newListing.GetComponent<SceneItemPlayer>().SetItemName(listingItemName);
        newListing.GetComponent<SceneItemPlayer>().associatedObject = objInListing;
    }

    // public void OpenMuseumListing(){
    // }

    // Used to close museum item listing
    public void CloseMuseumListing(){
        // Update in catalog state
        controller.inCatalog = false;

        // Show other UI elements
        sideMenu.SetActive(true);
        sideMenuItemListing.SetActive(true);

        // Close museum list window
        museumListing.SetActive(false);
    }

}
