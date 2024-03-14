using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.Networking;
using TMPro;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Dummiesman;
using UnityEngine.UI;
using Defective.JSON;


public class LoadFromLink : MonoBehaviour
{

    public GameObject image;
    public string[] objPath;

    public string username;
    public string token;

    private const string BUCKET_URL_HTTPS = "http://localhost:3000/userMedia/";

    private const string BASE_LINK = "http://localhost:3000";

    private const string REST_UPLOAD_FILE = BASE_LINK + "/media/upload";

    void Start()
    {
        username = "user";
        token = "404e4914-79db-4500-a017-eadea1b2df65";
    }


#if UNITY_WEBGL && !UNITY_EDITOR
    // WebGL
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void UploadObject(){
        UploadFile(gameObject.name, "OnFileUpload", ".jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url){
        Debug.Log("In OnFileUpload url " + url );
        // Debug.Log("new system uri " + new System.Uri(url) );
        // Debug.Log("new system uri absolute " + new System.Uri(url).AbsoluteUri );
        StartCoroutine(TryingSomething2(url));
        // StartCoroutine(TryingSomething1(new System.Uri(objPath[0]).AbsoluteUri));
        // StartCoroutine(TryingSomething1(new System.Uri(url).AbsoluteUri));
    }

#else

    public void UploadObject()
    {
        // objPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg", false);
        objPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg,png", false);
        StartCoroutine(TryingSomething1(objPath[0]));
    }

#endif

    private IEnumerator TryingSomething1(string fileLocation)
    {
        Debug.Log(fileLocation);
        // Create JSON API request
        string url = REST_UPLOAD_FILE;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Turn file into bytes
        byte[] fileBytes = File.ReadAllBytes(fileLocation);

        // Convert the byte array to a base64-encoded string
        string base64String = System.Convert.ToBase64String(fileBytes);

        // Add fields to the JSON with file and file type
        json.AddField("bytes", base64String);
        json.AddField("name", "currentMediaName");
        json.AddField("fileType", "jpg");
        json.AddField("username", username);
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
            // Get result and parse to JSON, and extract file info
            var result = request.downloadHandler.text;
            ReceivedItemInfo data = JsonUtility.FromJson<ReceivedItemInfo>(result);
            Debug.Log("File uploaded.");
        }
    }




    private IEnumerator TryingSomething2(string url)
    {
        UnityWebRequest link = UnityWebRequestTexture.GetTexture(url);
        yield return link.SendWebRequest();

        if (link.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(link.error);
        }
        else
        {
            Texture imageReceived = ((DownloadHandlerTexture)link.downloadHandler).texture;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(imageReceived.width / 4, imageReceived.height / 4);
            image.GetComponent<RawImage>().texture = imageReceived;

            byte[] bytes = ((DownloadHandlerTexture)link.downloadHandler).data;
            StartCoroutine(TryingSomething3(bytes));
        }
    }


    private IEnumerator TryingSomething3(byte[] bytes)
    {
        // Create JSON API request
        string url = REST_UPLOAD_FILE;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Turn file into bytes
        byte[] fileBytes = bytes;

        // Convert the byte array to a base64-encoded string
        string base64String = System.Convert.ToBase64String(fileBytes);

        // Add fields to the JSON with file and file type
        json.AddField("bytes", base64String);
        json.AddField("name", "currentMediaName");
        json.AddField("fileType", "jpg");
        json.AddField("username", username);
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
            // Get result and parse to JSON, and extract file info
            var result = request.downloadHandler.text;
            ReceivedItemInfo data = JsonUtility.FromJson<ReceivedItemInfo>(result);
            Debug.Log("File uploaded.");
        }
    }



}
