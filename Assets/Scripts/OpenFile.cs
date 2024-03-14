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


public class OpenFile : MonoBehaviour
{

    public GameObject model;
    private string[] objPath;
    private string[] mtlPath;
    public GameObject sphere;
    public GameObject image;

#if UNITY_WEBGL && !UNITY_EDITOR
    // WebGL
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void UploadObject(){
        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
    }

    // Called from browser
    public void OnFileUpload(string url){
        StartCoroutine(OBJLoader(url));
    }


#else

    public void UploadObject()
    {
        // paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        objPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        // if (objPath.Length > 0)
        // StartCoroutine(OBJLoader(new System.Uri(objPath[0]).AbsoluteUri));
    }

    public void UploadMaterial()
    {
        // mtlPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mtl", false);
        mtlPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mtl", false);
        // if (mtlPath.Length > 0)
        // StartCoroutine(OutputRoutineOpen2(new System.Uri(mtlPath[0]).AbsoluteUri));
    }

    public void FullLoad()
    {
        if (objPath.Length > 0 && mtlPath.Length > 0)
        {
            StartCoroutine(FullLoad(new System.Uri(objPath[0]).AbsoluteUri, new System.Uri(mtlPath[0]).AbsoluteUri));
        }
        // if(objPath.Length > 0)
        //     StartCoroutine(OBJLoader(new System.Uri(objPath[0]).AbsoluteUri));
    }


#endif

    // OBJ Loader
    private IEnumerator OBJLoader(string url)
    {
        UnityWebRequest objReq = UnityWebRequest.Get(url);
        yield return objReq.SendWebRequest();

        if (objReq.result != UnityWebRequest.Result.Success)
            Debug.Log("ERROR: " + objReq.error);
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(objReq.downloadHandler.text));
            if (model != null)
                Destroy(model);
            model = new OBJLoader().Load(textStream);

            // model.transform.localScale = new Vector3(-1, 1, 1);
            model.transform.rotation = new Quaternion(0, 180, 0, 0);


            // Create file and write to disk
            string fileName = "/123.obj";
            // string path = "C:/Users/Eric/Desktop/Test/" + fileName;
            string path = Application.streamingAssetsPath + fileName;
            FileStream fstream = File.Create(path);
            byte[] info = Encoding.UTF8.GetBytes(objReq.downloadHandler.text);
            fstream.Write(info);
        }
    }



    // OBJ + MTL loader
    private IEnumerator FullLoad(string objP, string mtlP)
    {
        UnityWebRequest objReq = UnityWebRequest.Get(objP);
        UnityWebRequest mtlReq = UnityWebRequest.Get(mtlP);
        // UnityWebRequest textureReq = UnityWebRequestTexture.GetTexture("https://storage.googleapis.com/heritage3d.appspot.com/trafariauser1681813735684.jpeg");

        yield return objReq.SendWebRequest();
        yield return mtlReq.SendWebRequest();
        // yield return textureReq.SendWebRequest();

        if (objReq.result != UnityWebRequest.Result.Success || mtlReq.result != UnityWebRequest.Result.Success)
            Debug.Log("ERROR");
        else
        {
            MemoryStream objTextStream = new MemoryStream(Encoding.UTF8.GetBytes(objReq.downloadHandler.text));
            MemoryStream mtlTextStream = new MemoryStream(Encoding.UTF8.GetBytes(mtlReq.downloadHandler.text));

            // Load obj using OBJLoader plugin, and extract actual model to be used
            GameObject item = new OBJLoader().Load(objTextStream, mtlTextStream);
            GameObject actualItem = item.transform.GetChild(0).gameObject;

            // Remove empty parent object and delete it
            actualItem.transform.SetParent(null);
            GameObject.Destroy(item);

            model = actualItem;

            // model = new OBJLoader().Load(objTextStream, mtlTextStream);

            // model.transform.rotation = new Quaternion(0, 180, 0, 0);

            // GameObject newSphere = GameObject.Instantiate(sphere, new Vector3(0, 0, 0), new Quaternion(0, 180, 0, 0));
            // newSphere.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            // model.GetComponent<Transform>().parent = newSphere.transform;

            // Texture imageReceived = DownloadHandlerTexture.GetContent(textureReq);
            // actualItem.gameObject.GetComponent<Renderer>().material.SetTexture("_Main", imageReceived);
        }

    }

    // https://storage.googleapis.com/heritage3d.appspot.com/full.obj

    
    public void SetTextureItem(){
        StartCoroutine(GetTextr());
    }

    private IEnumerator GetTextr(){
        UnityWebRequest textureReq = UnityWebRequestTexture.GetTexture("https://storage.googleapis.com/heritage3d.appspot.com/trafariauser1681813735684.jpeg");
        yield return textureReq.SendWebRequest();

        Texture imageReceived = DownloadHandlerTexture.GetContent(textureReq);
        model.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", imageReceived);
    }
}
