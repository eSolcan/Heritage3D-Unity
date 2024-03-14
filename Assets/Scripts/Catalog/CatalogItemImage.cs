using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemImage : MonoBehaviour
{
    private Controller controller;

    public int indexInList;
    public string id;
    public string itemName = "";
    private int type = 1;
    public GameObject nameDisplay;

    public GameObject sphere;
    public GameObject plane;

    private Catalog catalog;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        catalog = GameObject.Find("CanvasMain").GetComponent<Catalog>();
    }

    public void AddToScene()
    {
        //Get texture to be added to world
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

        //Instantiate plane and set as child
        GameObject objInstance = GameObject.Instantiate(plane);
        objInstance.transform.SetParent(newSphere.transform, false);
        objInstance.name = id;

        //Adjust scale and layer to be visible to main camera
        objInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
        objInstance.layer = 0;

        //Add texture
        Texture texture = obj.GetComponent<RawImage>().texture;
        objInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

        // Add outline script
        Outline outline = objInstance.AddComponent<Outline>();
        outline.OutlineWidth = 5;

        //Make plane to have same aspect ratio as image in texture
        float ratio = (float)texture.width / (float)texture.height;
        Vector3 currentScale = objInstance.transform.localScale;
        objInstance.transform.localScale = new Vector3(currentScale.x * ratio, currentScale.y, currentScale.z);

        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().id = id;
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemName = itemName;
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().type = type;

        GameObject newListing = controller.AddItemListing(newSphere.transform.GetChild(0).gameObject, itemName, id);
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListing = newListing.GetComponent<SceneItemListing>();

        if (type < 100)
        {
            GameObject newListingForPath = controller.ui.GetComponent<GuideMenu>().AddItemListing(newSphere.transform.GetChild(0).gameObject, itemName, id);
            newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemListingForGuide = newListingForPath;
        }

        controller.AddToScene(newSphere.transform.GetChild(0).gameObject);
        catalog.CloseCatalog();
    }

    public void SetName(string newName)
    {
        itemName = newName;
        nameDisplay.GetComponent<TextMeshProUGUI>().text = newName;
    }

    public void DeleteItem()
    {
        catalog.PreliminaryDeleteItemFromCatalog(indexInList, 1);
    }
}
