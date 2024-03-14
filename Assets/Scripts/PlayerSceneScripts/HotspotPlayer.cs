using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using Defective.JSON;
using System.IO;
using Dummiesman;
using UnityEngine.UI;

// THIS SYSTEM IS INCREDIBLY FLAWED AND I WAS TOO LAZY TOO FIX IT, SORRY - Eric

// Ideally there would be limitations to 1 item per user per hotspot, but that's not the case,
// as this can be easily circumvented with a simple page reload. No checks are done on the backend
// to see if a user has uploaded to that hotspot already.

public class HotspotPlayer : MonoBehaviour
{

    public ControllerPlayerScene controller;

    public GameObject objInHotspot;
    public GameObject plane;

    public GameObject playerInteractText;

    public string deleteItemText;

    public List<GameObject> objsToCycle;
    public List<GameObject> objsToCycleInstances;
    public int currentObjIndex;

    public string hotspotId;

    public float sleepTimeForCycle;

    public bool cyclingItems;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        playerInteractText = controller.player.transform.GetChild(1).GetChild(0).gameObject;

        deleteItemText = "[E] Delete added item from hotspot";

        currentObjIndex = 0;

        sleepTimeForCycle = 10f;

        cyclingItems = false;

        StartCoroutine(GetItemsThenStartCycle());
    }

    private IEnumerator GetItemsThenStartCycle()
    {
        // Create JSON API request
        string url = controller.apiLinks.queryHotspotItems;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Add fields to the JSON with file and file type
        json.AddField("hotspotId", hotspotId);

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

            SoVeryTired items = JsonUtility.FromJson<SoVeryTired>(result);

            foreach (DoubletButForHotspotMedia mediaItem in items.addedMedia)
            {
                string link = controller.apiLinks.baseMediaLink + mediaItem.mediaId;
                string linkFull = link + "." + mediaItem.mediaType;

                switch (mediaItem.mediaType)
                {
                    case "obj":
                        string linkMtl = link + ".mtl";
                        string linkImg = link + ".jpg";
                        StartCoroutine(GetObj(linkFull, linkMtl, linkImg, mediaItem.mediaId, mediaItem.mediaId, false));
                        break;
                    case "jpg":
                        StartCoroutine(GetImg(linkFull, mediaItem.mediaId, mediaItem.mediaId, false));
                        break;
                    default:
                        break;
                }
            }

            yield return new WaitForSeconds(1f);
            StartCoroutine(IncreaseScaleOverTime());
        }
    }

    private IEnumerator GetObj(string urlObj, string urlMtl, string urlImg, string id, string name, bool uploadingFresh)
    {
        UnityWebRequest requestObj = UnityWebRequest.Get(urlObj);
        UnityWebRequest requestMtl = UnityWebRequest.Get(urlMtl);
        UnityWebRequest requestImg = UnityWebRequestTexture.GetTexture(urlImg);

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

            actualItem.transform.position = this.transform.position + this.transform.up * 1.5f;

            actualItem.transform.localEulerAngles = new Vector3(90, this.gameObject.transform.eulerAngles.y, 0);
            objsToCycleInstances.Add(actualItem);

            // Apply texture to material
            Texture imageReceived = DownloadHandlerTexture.GetContent(requestImg);
            actualItem.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", imageReceived);

            actualItem.SetActive(false);
        }
    }

    private IEnumerator GetImg(string url, string id, string name, bool uploadingFresh)
    {
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

            //Instantiate 
            Vector3 instLocation = this.transform.position;

            GameObject objInstance = GameObject.Instantiate(plane, instLocation + this.transform.up, new Quaternion(0, 0, 0, 0));

            //Adjust scale and layer to be visible to main camera
            objInstance.transform.localScale = new Vector3(.01f, .01f, .01f);
            objInstance.transform.localEulerAngles = new Vector3(90, 0, 0);

            objInstance.transform.position = this.transform.position + this.transform.up * 1.5f;

            objInstance.transform.localEulerAngles = new Vector3(90, this.gameObject.transform.eulerAngles.y, 0);
            objsToCycleInstances.Add(objInstance);

            //Add texture
            Texture texture = imageReceived;
            objInstance.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

            //Make plane to have same aspect ratio as image in texture
            float ratio = (float)texture.width / (float)texture.height;
            Vector3 currentScale = objInstance.transform.localScale;
            objInstance.transform.localScale = new Vector3(currentScale.x * ratio, currentScale.y, currentScale.z);

            objInstance.SetActive(false);

        }
    }

    private IEnumerator ObjectCycling()
    {
        yield return new WaitForSeconds(sleepTimeForCycle);
        StartCoroutine(ReduceScaleOverTime());
    }

    private IEnumerator IncreaseScaleOverTime()
    {
        if (!cyclingItems && objsToCycleInstances.Count > 0)
            cyclingItems = true;

        if (cyclingItems)
        {
            if (!objsToCycleInstances[currentObjIndex].activeSelf)
                objsToCycleInstances[currentObjIndex].SetActive(true);

            if (currentObjIndex < objsToCycleInstances.Count)
                objsToCycleInstances[currentObjIndex].transform.localScale *= 1.02f;
            else
                currentObjIndex = 0;

            yield return new WaitForEndOfFrame();

            if (objsToCycleInstances[currentObjIndex].transform.localScale.x < .1f)
                StartCoroutine(IncreaseScaleOverTime());
            else
                StartCoroutine(ObjectCycling());
        }
    }

    private IEnumerator ReduceScaleOverTime()
    {
        if (currentObjIndex < objsToCycleInstances.Count)
            objsToCycleInstances[currentObjIndex].transform.localScale *= .98f;
        else
            currentObjIndex = 0;

        yield return new WaitForEndOfFrame();

        if (objsToCycleInstances[currentObjIndex].transform.localScale.x > .001f)
            StartCoroutine(ReduceScaleOverTime());
        else
        {
            objsToCycleInstances[currentObjIndex].SetActive(false);

            currentObjIndex++;
            if (currentObjIndex >= objsToCycleInstances.Count)
                currentObjIndex = 0;

            StartCoroutine(IncreaseScaleOverTime());
        }
    }

    public void AddObj(GameObject objToAdd, int type)
    {

        // If an item was already added, remove it (and tell database to also remote from hotspot's media)
        if (objInHotspot != null)
        {
            objInHotspot.SetActive(false);
            RemoveObj(objInHotspot);
        }

        objInHotspot = objToAdd;
        objsToCycleInstances.Add(objToAdd);
        objToAdd.SetActive(false);

        ItemHotspot itemHotspot = objToAdd.GetComponent<ItemHotspot>();
        StartCoroutine(AddMediaToHotspot(itemHotspot.itemId, itemHotspot.itemType));

        if(!cyclingItems)
            StartCoroutine(IncreaseScaleOverTime());
    }

    public void RemoveObj(GameObject objToRemove)
    {
        objsToCycleInstances.Remove(objToRemove);
        objToRemove.SetActive(false);
        StartCoroutine(RemoveMediaFromHotspot(objToRemove.GetComponent<ItemHotspot>().itemId));

        if (objsToCycleInstances.Count == 0)
            cyclingItems = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            controller.hotspot = this.gameObject.GetComponent<HotspotPlayer>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && controller.hotspot == this.gameObject.GetComponent<HotspotPlayer>())
        {
            controller.hotspot = null;
        }
    }

    private IEnumerator AddMediaToHotspot(string mediaId, int mediaType)
    {
        // Create JSON API request
        string url = controller.apiLinks.addHotspotItem;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Backend doesn't like empty (0 lenght) fields for some reason
        string token = controller.token;
        if (token.Length < 1)
        {
            token = "this is a token, yes ok";
        }

        DoubletButForHotspotMedia newHotspotItem = new DoubletButForHotspotMedia();
        newHotspotItem.mediaId = mediaId;
        newHotspotItem.mediaType = mediaType == 0 ? "obj" : "jpg";

        // Add fields to the JSON with file and file type
        json.AddField("item", JsonUtility.ToJson(newHotspotItem));
        json.AddField("hotspotId", hotspotId);
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

            if (!cyclingItems)
                StartCoroutine(IncreaseScaleOverTime());

            Debug.Log(result);
        }
    }

    private IEnumerator RemoveMediaFromHotspot(string mediaId)
    {
        // Need this delay, otherwise replacing items was causing issues, not deleting previous item in hotspot
        yield return new WaitForSeconds(1f);

        // Create JSON API request
        string url = controller.apiLinks.removeHotspotItem;
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
        json.AddField("mediaId", mediaId);
        json.AddField("hotspotId", hotspotId);
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
