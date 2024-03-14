using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyClipping : MonoBehaviour
{
    private Controller controller;
    private ControllerPlayerScene controllerPlayerScene;


    void Start()
    {
        // Will find either of the controllers, depending if it's the EDITOR or VISITOR application
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        controllerPlayerScene = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
    }

    void Update()
    {
        // Will apply the clipping height based on the slider value

        if (controller != null)
        {
            if (controller.changingClippingHeight)
            {
                Plane plane = new Plane(new Vector3(0, 1, 0), new Vector3(0, controller.currentClippingHeight, 0));
                Vector4 planeVisulization = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
                this.gameObject.GetComponent<Renderer>().material.SetVector("_Plane", planeVisulization);
            }
        }
        else
        {
            if (controllerPlayerScene.changingClippingHeight)
            {
                Plane plane = new Plane(new Vector3(0, 1, 0), new Vector3(0, controllerPlayerScene.currentClippingHeight, 0));
                Vector4 planeVisulization = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
                this.gameObject.GetComponent<Renderer>().material.SetVector("_Plane", planeVisulization);
            }
        }

    }




}
