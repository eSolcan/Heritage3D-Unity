using System.Collections;
using System.Collections.Generic;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.InteropServices;
using Dummiesman;

public class ButtonOpenFileBrowser : ButtonCustom
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        // string[] imagePath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg,png", false);
        UploadObject();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    // WebGL
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void UploadObject(){
        UploadFile(gameObject.name, "OnFileUpload", ".jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url)
    {
        
    }

#else

    void UploadObject()
    {
        string[] imagePath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg,png", false);
    }
#endif

}


