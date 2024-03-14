using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public Camera cam;
    private ControllerPlayerScene controller;

    public float xRotation, yRotation;

    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    public bool teleporting;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        cam = controller.cam.GetComponent<Camera>();

        teleporting = false;
    }
 
    void Update()
    {
        if (!teleporting && !controller.inCatalog)
            UpdateCamera();
    }

    //Camera rotation for player first person mode
    void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * 1;// * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 1;// * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        yRotation += mouseX;

        //Rotate camera and player
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    }


}
