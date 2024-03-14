using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraEditor : MonoBehaviour
{
    public Camera cam;
    private Controller controller;

    public float xRotation, yRotation;

    public bool teleporting;
    
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        cam = controller.cam.GetComponent<Camera>();

        teleporting = false;
    }

    void Update()
    {
        if (!controller.inCatalog && !teleporting)
            UpdateCamera();        
    }

    //Camera rotation for player first person mode
    void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * 1;// * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 1;// * Time.deltaTime;

        // Limit vertical rotation to 180 degrees (90 up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        //Rotate camera and player
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    }
}
