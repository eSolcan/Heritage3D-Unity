using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Defective.JSON;
using TMPro;


public class JsonSaveScene : MonoBehaviour
{

    private Controller controller;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // Sweep through all added items to the world, and save relevant info about each
    // Also save info about guide path, sky texture, portals, etc
    public void SaveSceneToJson(string museumName)
    {
        // Create class to hold info about current scene
        SceneSaveClass sceneToSave = new SceneSaveClass(controller.username);

        // Set scene name to what user wrote
        sceneToSave.sceneName = museumName;
        // sceneToSave.sceneName = "sceneNameGivenByUserEventually";

        // Cycle through all the added game objects to the scene and save info regarding each
        foreach (GameObject gameObject in controller.dragables)
        {
            JsonGameObject newObjJson = new JsonGameObject();

            GameObject gameObjInScene = gameObject.transform.parent.GetChild(1).gameObject;

            // Save ID and Type
            newObjJson.itemId = gameObject.GetComponent<Item>().id;
            newObjJson.itemName = gameObject.GetComponent<Item>().itemName;
            newObjJson.itemType = gameObject.GetComponent<Item>().type;

            // Save position
            newObjJson.position.Add(gameObjInScene.transform.position.x);
            newObjJson.position.Add(gameObjInScene.transform.position.y);
            newObjJson.position.Add(gameObjInScene.transform.position.z);

            // Save rotation in euler angles
            newObjJson.rotation.Add(gameObjInScene.transform.eulerAngles.x);
            newObjJson.rotation.Add(gameObjInScene.transform.eulerAngles.y);
            newObjJson.rotation.Add(gameObjInScene.transform.eulerAngles.z);

            // Save scale
            if (gameObject.GetComponent<Item>().type == 100 || gameObject.GetComponent<Item>().type == 101)
            {
                newObjJson.scale.Add(gameObjInScene.transform.GetChild(0).localScale.x);
                newObjJson.scale.Add(gameObjInScene.transform.GetChild(0).localScale.y);
                newObjJson.scale.Add(gameObjInScene.transform.GetChild(0).localScale.z);
            }
            else
            {
                newObjJson.scale.Add(gameObjInScene.transform.localScale.x / 2);
                newObjJson.scale.Add(gameObjInScene.transform.localScale.y / 2);
                newObjJson.scale.Add(gameObjInScene.transform.localScale.z / 2);
            }

            // Save added components
            List<string> listOfComponents = gameObject.GetComponent<Item>().addedWidgetComponents;

            foreach (string comp in listOfComponents)
            {
                newObjJson.addedComponents.Add(comp);

                // If description widget, also add what is the description
                if (comp.Equals("HtmlDescriptionOnProximity"))
                {
                    newObjJson.itemDescription = gameObject.GetComponent<HtmlDescriptionOnProximity>().description;
                }
            }

            // If item is of category TEXT, also save text info
            if (gameObject.GetComponent<Item>().type == 4)
            {
                string textToSave = gameObjInScene.GetComponent<TextMeshPro>().text;
                textToSave = textToSave.Replace("\n", " ");
                newObjJson.itemDescription = textToSave;
            }

            // If item is of category PORTAL, also save to which portal it links
            if (gameObject.GetComponent<Item>().type == 6)
            {
                if (gameObject.GetComponent<Item>().linkedPortal != null)
                    newObjJson.linkedPortaName = gameObject.GetComponent<Item>().linkedPortal.GetComponent<Item>().itemName;
            }

            // If item is of category POINT LIGHT, also save intensity and range
            if (gameObject.GetComponent<Item>().type == 7)
            {
                Light lightComp = gameObject.transform.parent.GetChild(1).gameObject.GetComponent<Light>();
                newObjJson.lightIntensity = lightComp.intensity;
                newObjJson.lightRange = lightComp.range;
            }

            // If item is of category HOT SPOT, add database entry for this. Will save visitor media
            if (gameObject.GetComponent<Item>().type == 8)
            {
                HotSpotClass newHotspot = new HotSpotClass();
                newHotspot.hostpostId = gameObject.GetComponent<Item>().id;
                newHotspot.museumId = sceneToSave.id;
                StartCoroutine(HotspotUploadAPI(newHotspot));
            }

            // Add Json Item to list of objects
            sceneToSave.listOfObjects.Add(newObjJson);
        }

        // Save start location
        sceneToSave.startPosition.Add(controller.playerStartPosition.x);
        sceneToSave.startPosition.Add(controller.playerStartPosition.y + .65f);
        sceneToSave.startPosition.Add(controller.playerStartPosition.z);

        // Save guide path points
        List<GameObject> pathList = controller.canvasMain.GetComponent<GuideMenu>().pathList;
        foreach (GameObject pathPoint in pathList)
        {

            Vector3 newPosition = pathPoint.GetComponent<ButtonGuidePathItem>().associatedObject.transform.position;
            Triplet coords = new Triplet(newPosition.x, newPosition.y, newPosition.z);
            sceneToSave.guidePathLocations.Add(coords);
        }

        // Save sky texture name
        sceneToSave.skyTextureIndex = controller.currentSkyboxIndex;

        StartCoroutine(MuseumUploadAPI(sceneToSave));
    }

    // Rest API call to upload museum to database
    private IEnumerator MuseumUploadAPI(SceneSaveClass museum)
    {

        // Create JSON API request
        string url = controller.apiLinks.uploadMuseum;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Backend doesn't like empty (0 lenght) fields for some reason
        string token = controller.token;
        if (token.Length < 1)
        {
            token = "this is a token, yes ok";
        }

        // Add fields to the JSON with file and file type
        json.AddField("museum", JsonUtility.ToJson(museum));
        json.AddField("token", token);

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

            Debug.Log(result);
        }
    }

    // Rest API call to add Hot Spot entry
    private IEnumerator HotspotUploadAPI(HotSpotClass hotspot)
    {

        // Create JSON API request
        string url = controller.apiLinks.uploadHotspot;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Backend doesn't like empty (0 lenght) fields for some reason
        string token = controller.token;
        if (token.Length < 1)
        {
            token = "this is a token, yes ok";
        }

        // Add fields to the JSON with file and file type
        json.AddField("hostpost", JsonUtility.ToJson(hotspot));
        json.AddField("token", token);

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

            Debug.Log(result);
        }
    }

}
