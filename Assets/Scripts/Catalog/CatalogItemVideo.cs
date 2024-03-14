using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class CatalogItemVideo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Controller controller;

    public int indexInList;
    public string id;
    public string itemName = "";
    private int type = 2;
    public GameObject nameDisplay;

    public GameObject sphere;
    public GameObject plane;

    private Catalog catalog;

    public VideoPlayer videoPlayer;

    public Texture videoTexture;
    public Texture baseTexture;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        catalog = GameObject.Find("CanvasMain").GetComponent<Catalog>();
    }

    public void AddToScene()
    {
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
        objInstance.transform.GetChild(0).localScale = new Vector3(.5f, 1f, 0.281f);
        objInstance.transform.GetChild(1).localScale = new Vector3(.5f, 1f, 0.281f);
        // objInstance.layer = 0;

        //Add video clip
        objInstance.GetComponent<VideoPlayer>().url = obj.GetComponent<VideoPlayer>().url;
        objInstance.GetComponent<VideoPlayer>().Pause();

        // Add outline script
        Outline outline = objInstance.AddComponent<Outline>();
        outline.OutlineWidth = 5;

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



    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.transform.GetChild(1).gameObject.GetComponent<RawImage>().texture = videoTexture;
        
        videoPlayer.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.transform.GetChild(1).gameObject.GetComponent<RawImage>().texture = baseTexture;
        
        videoPlayer.Stop();
    }

    public void SetName(string newName)
    {
        itemName = newName;
        nameDisplay.GetComponent<TextMeshProUGUI>().text = newName;
    }

    public void DeleteItem()
    {
        catalog.PreliminaryDeleteItemFromCatalog(indexInList, 2);
    }
}
