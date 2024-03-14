using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingCatalog : MonoBehaviour
{
    private Controller controller;
    public UI ui;

    public Material[] skyboxMaterials;

    void Start(){
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = GameObject.Find("CanvasMain").GetComponent<UI>();
    }
    
    public void ApplyLightingSettings(int index){
        RenderSettings.skybox = skyboxMaterials[index];
        controller.currentSkyboxIndex = index;
        ui.CloseLightingCatalog();
    }
}
