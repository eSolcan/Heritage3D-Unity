using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatalogText : MonoBehaviour
{
    private Controller controller;

    private int type = 4;

    public GameObject sphere;
    public GameObject text;

    private Catalog catalog;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        catalog = GameObject.Find("CanvasMain").GetComponent<Catalog>();
    }

    public void AddTextToWorld(string textToAdd)
    {
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

        //Instantiate text object and set as child
        GameObject objInstance = GameObject.Instantiate(text);
        objInstance.transform.SetParent(newSphere.transform, false);

        //Adjust scale and layer to be visible to main camera
        objInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
        objInstance.layer = 0;

        //Add text to text (very informative, I know)
        objInstance.GetComponent<TextMeshPro>().text = textToAdd;

        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().type = type;
        newSphere.transform.GetChild(0).gameObject.GetComponent<Item>().itemName = "Text";

        string itemName = "Text";
        string id = "Text" + System.DateTime.Now.Millisecond;

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

}
