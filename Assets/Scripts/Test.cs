using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class Test : ButtonCustom
{
    // private UnityEngine.Video.VideoPlayer videoPlayer;
    // private string status;

    // public Shader shader;

    // public Material material;

    // void Update()
    // {
    //     Plane plane = new Plane(transform.up, transform.position);
    //     Vector4 planeVisulization = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    //     material.SetVector("_Plane", planeVisulization);
    // }

    [DllImport("__Internal")]
    private static extern void ChangeHtmlCode(string htmlCode);

    public override void OnPointerDown(PointerEventData eventData)
    {
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
            ChangeHtmlCode(Time.time + "");
        #endif
    }

    void Update(){
        Debug.Log(this.transform.rotation);
    }





}
