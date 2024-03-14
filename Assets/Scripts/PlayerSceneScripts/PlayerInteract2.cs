using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteract2 : MonoBehaviour
{
    public ControllerPlayerScene controller;

    public Camera cam;

    private float rayDistance = 5;
    [SerializeField]
    private LayerMask rayHitMask;

    public TextMeshProUGUI promptText;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        cam = controller.cam.GetComponent<Camera>();
    }

    void Update()
    {
        promptText.text = "";

        if (!controller.inCatalog)
        {
            // Create raycast from camera origin facing foward
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, rayDistance, rayHitMask) && hitInfo.collider.GetComponent<Interactable>() != null && !controller.inGuideMode)
            {
                // Change prompt text
                promptText.text = hitInfo.collider.GetComponent<Interactable>().promptMessage;

                if (!controller.inGuideMode && Input.GetKeyDown(KeyCode.R))
                {
                    controller.StartGuide();
                }
                else if (!controller.inGuideMode && Input.GetKeyDown(KeyCode.E))
                {
                    controller.ContinueGuide();
                }
            }
            else if (controller.hotspot != null)
            {
                if (controller.hotspot.objInHotspot != null)
                    promptText.text = "[E] Remove added item from hotspot";
                else
                    promptText.text = "[C] Add item to hostpot";
            }
            else if (controller.postit != null)
                promptText.text = "[E] Delete Note";
        }

    }
}
