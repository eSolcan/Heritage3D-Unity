using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatalogItem3DPlayer : MonoBehaviour
{
    private ControllerPlayerScene controller;
    private UIPlayerScene ui;

    public int indexInList;
    public string id;
    public string itemName = "";
    public int type = 0;
    public GameObject nameDisplay;

    private CatalogPlayerScene catalog;

    private float spinSpeed;
    private GameObject objToSpin;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        catalog = GameObject.Find("CanvasMain").GetComponent<CatalogPlayerScene>();
        ui = controller.ui;
        spinSpeed = Random.Range(.05f, .1f);
        objToSpin = this.gameObject.transform.Find("Object").gameObject;
    }

    void Update()
    {
        // Spin Spin
        objToSpin.transform.Rotate(new Vector3(0, spinSpeed * Time.timeScale, 0), Space.World);
    }

    public void AddToScene()
    {
        if (controller.hotspot != null)
        {
            //Find the object in the button that will be instantiated
            GameObject obj = this.gameObject.transform.Find("Object").gameObject;

            //Instantiate the object
            Vector3 instLocation = controller.hotspot.transform.position;

            GameObject objInstance = GameObject.Instantiate(obj.transform.GetChild(0).gameObject, instLocation + controller.hotspot.transform.up, new Quaternion(0, 180, 0, 0));

            ItemHotspot itemHotspot = objInstance.AddComponent<ItemHotspot>();
            itemHotspot.itemId = obj.transform.parent.gameObject.GetComponent<CatalogItem3DPlayer>().id;
            itemHotspot.itemType = obj.transform.parent.gameObject.GetComponent<CatalogItem3DPlayer>().type;

            //Make scale smaller and layer to be visible to main camera
            objInstance.transform.localScale = new Vector3(.01f, .01f, .01f);
            objInstance.layer = 0;
            // objInstance.transform.position = instLocation;
            foreach (Transform child in objInstance.transform)
            {
                child.gameObject.layer = 0;
                foreach (Transform child2 in child)
                {
                    child2.gameObject.layer = 0;
                }
            }

            controller.AddToHotspot(objInstance, type);
            catalog.CloseCatalog();
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

}
