using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Defective.JSON;
using Unity.VisualScripting;
using System.IO;
using Dummiesman;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;


public class JsonLoadScene : MonoBehaviour
{

    public ControllerPlayerScene controller;

    public SceneSaveClass museum;

    public GameObject planeImage;
    public GameObject planeVideo;
    public GameObject colliderObj;
    public GameObject textObj;

    public GameObject exclamationObj;
    public GameObject arrowObj;

    public GameObject lightObj;
    public GameObject portalSimpleObj;
    public GameObject portalWithShaderObj;
    public GameObject hotspotObj;

    public GameObject guidePathPoint;

    public List<Doublet> listOfPortals;

    public Material[] skyboxMaterials;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        // Get all museum names and ids on start, and create entry for each
        StartCoroutine(GetAllMusemNamesRequest());
    }

    // Get all the names and IDs of created museums. Add an entry for each of the museums to the museum list
    private IEnumerator GetAllMusemNamesRequest()
    {
        yield return new WaitForSeconds(.01f);

        // Create JSON API request
        string url = controller.apiLinks.getMuseumNames;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // REQUIRED. Backend doesn't like it when JSON is empty, just add some trash field, doesn't mean anything
        json.AddField("id", "boop");

        // Convert into request and send it
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.downloadHandler.text);

            // If request failed, it is most likely due to backend being offline
            // Simply close menu and allow user input
            controller.ui.CloseMuseumListing();
        }
        else
        {
            // Get result and parse to JSON
            var result = request.downloadHandler.text;

            MuseumNameAndId museums = JsonUtility.FromJson<MuseumNameAndId>(result);

            UIPlayerScene ui = controller.ui;

            if (museums.listOfNames.Count > 0)
            {
                // Create entry for each museum
                for (int i = 0; i < museums.listOfNames.Count; i++)
                {
                    // Instantiate new museum listing and parent to grid
                    GameObject newListing = Instantiate(ui.museumListingPrefab);
                    newListing.transform.SetParent(ui.museumListingContent.transform, false);

                    // Set display name and id
                    newListing.GetComponent<MuseumListingPlayer>().SetMuseumName(museums.listOfNames[i]);
                    newListing.GetComponent<MuseumListingPlayer>().museumId = museums.listOfIds[i];
                }
            }
            // If no museums, unblock user movement
            else
            {
                // Close museum listing menu, also changes in catalog state, which allows player movement again
                controller.ui.CloseMuseumListing();
            }
        }
    }

    public void LoadSceneFromJson(string museumId)
    {
        StartCoroutine(MuseumGetAndLoadItems(museumId));
    }

    // Request all information about a specific museum given its id. Load all items, portals, etc into the scene
    private IEnumerator MuseumGetAndLoadItems(string id)
    {
        // Create JSON API request
        string url = controller.apiLinks.getMuseumById;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Add fields to the JSON with file and file type
        json.AddField("id", id);

        // Convert into request and send it
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Get result and parse to JSON
            var result = request.downloadHandler.text;

            museum = JsonUtility.FromJson<SceneSaveClass>(result);

            // Load all items as required
            foreach (JsonGameObject obj in museum.listOfObjects)
            {
                switch (obj.itemType)
                {
                    case 0:
                        StartCoroutine(AddObj(obj));
                        break;
                    case 1:
                        StartCoroutine(AddImage(obj));
                        break;
                    case 2:
                        StartCoroutine(AddVideo(obj));
                        break;
                    case 3:
                        StartCoroutine(AddAudio(obj));
                        break;
                    case 4:
                        AddText(obj);
                        break;
                    case 5:
                        AddMiscObj(obj);
                        break;
                    case 6:
                        AddPortal(obj);
                        break;
                    case 7:
                        AddPointLight(obj);
                        break;
                    case 8:
                        AddHotspot(obj);
                        break;
                    case 100:
                        AddCollider(obj);
                        break;
                    case 101:
                        AddCollider(obj);
                        break;
                    default:
                        break;
                }
            }

            // Load guide path
            foreach (Triplet location in museum.guidePathLocations)
            {
                Vector3 newLocation = new Vector3(location.xPosition, location.yPosition, location.zPosition);
                Vector3 newScale = new Vector3(.5f, .5f, .5f);
                GameObject newGuidePoint = Instantiate(guidePathPoint);

                newGuidePoint.transform.position = newLocation;
                newGuidePoint.transform.localScale = newScale;

                controller.guidePathList.Add(newGuidePoint);
            }

            // Link portals
            LinkPortals();

            // Apply Sky Texture
            ApplySkyTexutre(museum.skyTextureIndex);

            // Set player start position
            SetStartPosition();

            // Close museum listing menu
            controller.ui.CloseMuseumListing();
        }
    }

    private IEnumerator AddObj(JsonGameObject obj)
    {
        string baseMediaLink = controller.apiLinks.baseMediaLink;

        UnityWebRequest requestObj = UnityWebRequest.Get(baseMediaLink + obj.itemId + ".obj");
        UnityWebRequest requestMtl = UnityWebRequest.Get(baseMediaLink + obj.itemId + ".mtl");
        UnityWebRequest requestImg = UnityWebRequestTexture.GetTexture(baseMediaLink + obj.itemId + ".jpg");

        yield return requestObj.SendWebRequest();
        yield return requestMtl.SendWebRequest();
        yield return requestImg.SendWebRequest();

        if (
            requestObj.result != UnityWebRequest.Result.Success ||
            requestMtl.result != UnityWebRequest.Result.Success ||
            requestImg.result != UnityWebRequest.Result.Success
            )
            Debug.Log("Error request");
        else
        {
            // Open streams for the requests from file locations
            MemoryStream textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(requestObj.downloadHandler.text));
            MemoryStream textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(requestMtl.downloadHandler.text));

            // Load obj using OBJLoader plugin, and extract actual model to be used
            GameObject item = new OBJLoader().Load(textStreamObj, textStreamMtl);
            GameObject actualItem = item.transform.GetChild(0).gameObject;

            // Remove empty parent object and delete it
            actualItem.transform.SetParent(null);
            GameObject.Destroy(item);

            // Set layer to UI and adjust size to fit in item box, using Bound Size
            actualItem.layer = 0;

            // Apply texture to material
            Texture imageReceived = DownloadHandlerTexture.GetContent(requestImg);
            actualItem.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", imageReceived);

            // Set the required transform parameters
            SetTransform(actualItem, obj);

            // Add widgets
            AddWidgets(actualItem, obj);
        }
    }

    private IEnumerator AddImage(JsonGameObject obj)
    {
        string url = controller.apiLinks.baseMediaLink + obj.itemId + ".jpg";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Use texture downloader to get texture from image location
            Texture imageReceived = DownloadHandlerTexture.GetContent(request);

            // Instantiate plane
            GameObject objInstance = GameObject.Instantiate(planeImage);
            objInstance.name = obj.itemName;

            // Set the required transform parameters
            SetTransform(objInstance, obj);

            // Add texture
            objInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", imageReceived);

            // Add item listing
            controller.AddItemListing(objInstance, obj.itemName);

            // Add widgets
            AddWidgets(objInstance, obj);
        }
    }

    private IEnumerator AddVideo(JsonGameObject obj)
    {
        string url = controller.apiLinks.baseMediaLink + obj.itemId + ".mp4";

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Use audio downloader to get audio clip from clip location
            AudioClip audioReceived = DownloadHandlerAudioClip.GetContent(request);

            //Instantiate plane 
            GameObject objInstance = GameObject.Instantiate(planeVideo);
            objInstance.name = obj.itemName;

            // Set the required transform parameters
            SetTransform(objInstance, obj);

            //Add audio clip
            objInstance.GetComponent<AudioSource>().clip = audioReceived;
            objInstance.GetComponent<AudioSource>().Pause();

            // Add item listing
            controller.AddItemListing(objInstance, obj.itemName);

            // Add widgets
            AddWidgets(objInstance, obj);
        }
    }

    private IEnumerator AddAudio(JsonGameObject obj)
    {
        string url = controller.apiLinks.baseMediaLink + obj.itemId + ".mp4";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            //Instantiate plane 
            GameObject objInstance = GameObject.Instantiate(planeVideo);
            objInstance.name = obj.itemName;

            // Set the required transform parameters
            SetTransform(objInstance, obj);

            //Add video clip
            objInstance.GetComponent<VideoPlayer>().url = url;
            objInstance.GetComponent<VideoPlayer>().Pause();

            // Add item listing
            controller.AddItemListing(objInstance, obj.itemName);

            // Add widgets
            AddWidgets(objInstance, obj);
        }
    }

    private void AddText(JsonGameObject obj)
    {
        //Instantiate plane
        GameObject objInstance = GameObject.Instantiate(textObj);
        objInstance.name = obj.itemName;

        // Set the required transform parameters
        SetTransform(objInstance, obj);

        //Add text 
        objInstance.GetComponent<TextMeshPro>().text = obj.itemDescription;

        // Add item listing
        controller.AddItemListing(objInstance, obj.itemName);

        // Add widgets
        AddWidgets(objInstance, obj);
    }

    private void AddCollider(JsonGameObject obj)
    {
        //Instantiate plane 
        GameObject objInstance = GameObject.Instantiate(colliderObj);
        objInstance.name = "Collider";

        // Set the required transform parameters
        // Adjust layer to be visible to main camera
        objInstance.layer = 0;

        foreach (Transform child in objInstance.transform)
            child.gameObject.layer = 0;

        // Set item in correct location
        Vector3 newPos = new Vector3(obj.position[0], obj.position[1], obj.position[2]);
        objInstance.transform.position = newPos;

        // Set item correct rotation
        Vector3 newRotation = new Vector3(obj.rotation[0], obj.rotation[1], obj.rotation[2]);
        if (obj.itemType == 101)
            newRotation.x = 270;
        objInstance.transform.eulerAngles = newRotation;

        // Set item correct scale
        Vector3 newScale = new Vector3(obj.scale[0], obj.scale[1], obj.scale[2]);
        objInstance.transform.GetChild(0).localScale = newScale;

        // Add collider component 
        objInstance.transform.GetChild(0).gameObject.AddComponent<ColliderOnPreviewVisitor>();
    }

    private void AddMiscObj(JsonGameObject obj)
    {
        GameObject objInstance = null;

        //Instantiate desired object based on name 
        switch (obj.itemName)
        {
            case "Arrow":
                objInstance = GameObject.Instantiate(arrowObj);
                break;
            case "Exclamation":
                objInstance = GameObject.Instantiate(exclamationObj);
                break;
            default:
                break;
        }

        objInstance.name = obj.itemName;

        // Set the required transform parameters
        SetTransform(objInstance, obj);
    }

    private void AddPortal(JsonGameObject obj)
    {
        // Instantiate portal
        GameObject objInstance = GameObject.Instantiate(portalSimpleObj);
        objInstance.name = obj.itemName;

        // Set the required transform parameters
        SetTransform(objInstance, obj);

        // Save portal name and what it links to, to link at the end of item loading
        // Not linking right now as to prevent links to unloaded portals
        if (obj.linkedPortaName.Length > 0)
        {
            Doublet portalInfo = new Doublet();
            portalInfo.portalNameInScene = obj.itemName;
            portalInfo.portalNameToWhichToLink = obj.linkedPortaName;

            listOfPortals.Add(portalInfo);
        }
    }

    private void LinkPortals()
    {
        foreach (Doublet portalInfo in listOfPortals)
        {
            GameObject currPortal = GameObject.Find(portalInfo.portalNameInScene);
            GameObject linkedPortal = GameObject.Find(portalInfo.portalNameToWhichToLink);
            currPortal.transform.GetChild(1).GetComponent<PortalPlayerScene>().linkedPortal = linkedPortal;
        }
    }

    private void AddPointLight(JsonGameObject obj)
    {
        // Instantiate point light
        GameObject objInstance = GameObject.Instantiate(lightObj);
        objInstance.name = obj.itemName;

        // Set the required transform parameters
        SetTransform(objInstance, obj);

        // Set intensity and range
        objInstance.GetComponent<Light>().intensity = obj.lightIntensity;
        objInstance.GetComponent<Light>().range = obj.lightRange;
    }

    private void AddHotspot(JsonGameObject obj)
    {
        // Instantiate hotspot
        GameObject objInstance = GameObject.Instantiate(hotspotObj);
        objInstance.name = obj.itemName;
        objInstance.GetComponent<HotspotPlayer>().hotspotId = obj.itemId;

        // Set the required transform parameters
        SetTransform(objInstance, obj);
    }

    private void ApplySkyTexutre(int index)
    {
        RenderSettings.skybox = skyboxMaterials[index];
    }

    private void SetStartPosition()
    {
        Vector3 newStartPos = new Vector3(museum.startPosition[0], museum.startPosition[1], museum.startPosition[2]);
        controller.player.transform.position = newStartPos;
    }

    private void SetTransform(GameObject objInstance, JsonGameObject obj)
    {
        // Adjust layer to be visible to main camera
        objInstance.layer = 0;

        foreach (Transform child in objInstance.transform)
            child.gameObject.layer = 0;

        // Set item in correct location
        Vector3 newPos = new Vector3(obj.position[0], obj.position[1], obj.position[2]);
        objInstance.transform.position = newPos;

        // Set item correct rotation
        Vector3 newRotation = new Vector3(obj.rotation[0], obj.rotation[1], obj.rotation[2]);
        objInstance.transform.eulerAngles = newRotation;

        // Set item correct scale
        Vector3 newScale = new Vector3(obj.scale[0], obj.scale[1], obj.scale[2]);
        objInstance.transform.localScale = newScale;
    }

    private void AddWidgets(GameObject objInstance, JsonGameObject obj)
    {
        foreach (string comp in obj.addedComponents)
        {
            switch (comp)
            {
                case "RotateTowardsPlayer":
                    objInstance.AddComponent<RotateTowardsPlayerVisitor>();
                    break;
                case "SwayUpDown":
                    objInstance.AddComponent<SwayUpDownVisitor>();
                    break;
                case "HtmlDescriptionOnProximity":
                    HtmlDescriptionPlayer descComp = objInstance.AddComponent<HtmlDescriptionPlayer>();
                    descComp.description = obj.itemDescription;
                    break;
                default:
                    break;
            }
        }
    }

}

