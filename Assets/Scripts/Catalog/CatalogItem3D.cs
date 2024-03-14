using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatalogItem3D : MonoBehaviour
{
    private Controller controller;
    private UI ui;

    public int indexInList;
    public string id;
    public string itemName = "";
    public int type = 0;
    public GameObject nameDisplay;

    public GameObject sphere;

    private Catalog catalog;

    private float spinSpeed;
    private GameObject objToSpin;

    public GameObject itemInfoButton;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        catalog = GameObject.Find("CanvasMain").GetComponent<Catalog>();
        ui = controller.ui;
        spinSpeed = UnityEngine.Random.Range(.25f, .5f);
        objToSpin = this.gameObject.transform.Find("Object").gameObject;
    }

    void Update()
    {
        // Spin Spin
        objToSpin.transform.Rotate(new Vector3(0, spinSpeed * Time.timeScale, 0), Space.World);

    }

    public void AddToScene()
    {
        //Find the object in the button that will be instantiated
        GameObject obj = this.gameObject.transform.Find("Object").gameObject;

        //Instantiate sphere that will allow dragging of the object
        Vector3 instLocation = new Vector3(0, 0, 0);
        if (!controller.inPlayer)
        {
            Vector3 camPosition = controller.cam.transform.position;
            instLocation = new Vector3(
                    camPosition.x + controller.cam.transform.forward.x * 20,
                    controller.currentWorkingFloorLevel + .5f,
                    camPosition.z + controller.cam.transform.forward.z * 20
                );
        }
        else
            instLocation = controller.player.transform.position + controller.player.transform.forward * 5;
        GameObject newSphere = GameObject.Instantiate(sphere, instLocation, new Quaternion(0, 180, 0, 0));
        newSphere.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);

        //Instantiate the object and set as child
        GameObject objInstance = GameObject.Instantiate(obj.transform.GetChild(0).gameObject, new Vector3(0, 0, 0), new Quaternion(0, 180, 0, 0));
        objInstance.transform.SetParent(newSphere.transform, false);
        objInstance.name = id;

        // Add outline script
        Outline outline = objInstance.AddComponent<Outline>();
        outline.OutlineWidth = 5;

        //Make scale smaller and layer to be visible to main camera
        objInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
        if(type == 8)
            objInstance.transform.localScale = new Vector3(5f, .1f, 5f);
        objInstance.layer = 0;
        foreach (Transform child in objInstance.transform)
        {
            child.gameObject.layer = 0;
            foreach (Transform child2 in child)
            {
                child2.gameObject.layer = 0;
            }
        }

        // Add ID and Type
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().id = id;
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemName = itemName;
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().type = type;

        // Add item listing to list of scene items and items for path, when necessary
        GameObject newListing = controller.AddItemListing(newSphere.transform.GetChild(0).gameObject, itemName, id);
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListing = newListing.GetComponent<SceneItemListing>();

        if (type < 100)
        {
            GameObject newListingForPath = ui.GetComponent<GuideMenu>().AddItemListing(newSphere.transform.GetChild(0).gameObject, itemName, id);
            newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListingForGuide = newListingForPath;
        }

        controller.AddToScene(newSphere.transform.GetChild(0).gameObject);
        catalog.CloseCatalog();

        // If portal, do extra stuff (different scale and open naming menu)
        if (this.type == 6)
        {
            ThingsForPortalInstantiate(newSphere.transform.GetChild(0).gameObject);
            objInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
            objInstance.transform.localPosition = new Vector3(0, -1, 0);
        }
        // If hot spot, generate new ID
        else 
        {
            Guid randId = Guid.NewGuid();
            string randIdString = randId.ToString();
            newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().id = "Hotspot" + randIdString;
        }
    }

    public void SetName(string newName)
    {
        itemName = newName;
        nameDisplay.GetComponent<TextMeshProUGUI>().text = newName;
    }

    public void DeleteItem()
    {
        catalog.PreliminaryDeleteItemFromCatalog(indexInList, 0);
    }

    private void ThingsForPortalInstantiate(GameObject sphere)
    {
        ui.OpenPortalItemNameBox();
    }

    public void InfoButtonSetActive()
    {
        itemInfoButton.SetActive(true);
    }


}
