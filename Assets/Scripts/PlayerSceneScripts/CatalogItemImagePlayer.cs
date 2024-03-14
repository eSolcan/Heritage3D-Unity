using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItemImagePlayer : MonoBehaviour
{
    private ControllerPlayerScene controller;

    public int indexInList;
    public string id;
    public string itemName = "";
    public int type = 1;
    public GameObject nameDisplay;

    public GameObject plane;

    private CatalogPlayerScene catalog;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        catalog = GameObject.Find("CanvasMain").GetComponent<CatalogPlayerScene>();
    }

    public void AddToScene()
    {
        if (controller.hotspot != null)
        {
            //Get texture to be added to world
            GameObject obj = this.gameObject.transform.Find("Object").gameObject;

            //Instantiate 
            Vector3 instLocation = controller.hotspot.transform.position;

            GameObject objInstance = GameObject.Instantiate(plane, instLocation + controller.hotspot.transform.up, new Quaternion(0, 0, 0, 0));

            ItemHotspot itemHotspot = objInstance.AddComponent<ItemHotspot>();
            itemHotspot.itemId = obj.transform.parent.gameObject.GetComponent<CatalogItemImagePlayer>().id;
            itemHotspot.itemType = obj.transform.parent.gameObject.GetComponent<CatalogItemImagePlayer>().type;

            //Adjust scale and layer to be visible to main camera
            objInstance.transform.localScale = new Vector3(.01f, .01f, .01f);
            objInstance.transform.localEulerAngles = new Vector3(90, 0, 0);

            //Add texture
            Texture texture = obj.GetComponent<RawImage>().texture;
            objInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

            //Make plane to have same aspect ratio as image in texture
            float ratio = (float)texture.width / (float)texture.height;
            Vector3 currentScale = objInstance.transform.localScale;
            objInstance.transform.localScale = new Vector3(currentScale.x * ratio, currentScale.y, currentScale.z);

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
        catalog.PreliminaryDeleteItemFromCatalog(indexInList, 1);
    }

}
