using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public Controller controller;

    public Camera cam;

    private float rayDistance = 5;
    [SerializeField]
    private LayerMask rayHitMask;

    public TextMeshProUGUI promptText;
    public Transform whereItemHeldStays;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

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

            if (controller != null)
            {
                // I hate this so much, if staircase very funny
                if (controller.heldItem == null && Physics.Raycast(ray, out hitInfo, rayDistance, rayHitMask))
                {
                    // If target of raycast is interactable
                    if (hitInfo.collider.GetComponent<Interactable>() != null && !controller.inGuideMode)
                    {
                        // Change prompt text
                        promptText.text = hitInfo.collider.GetComponent<Interactable>().promptMessage;

                        // If E is pressed, make item to be held by the user
                        if (hitInfo.collider.GetComponent<Item>() != null && Input.GetKeyDown(KeyCode.E))
                        {
                            controller.heldItem = hitInfo.collider.transform.parent.gameObject;
                            controller.heldItem.transform.SetParent(whereItemHeldStays);
                            controller.heldItem.transform.localPosition = new Vector3(0, 0, 0);

                            // Also make it to be selected
                            hitInfo.collider.transform.gameObject.GetComponent<Item>().SetSelected(true);
                        }
                        // If it's a guide point, start guide on E and continue guide on P
                        else if (!controller.inGuideMode && Input.GetKeyDown(KeyCode.R))
                        {
                            controller.StartGuide();
                        }
                        else if (!controller.inGuideMode && Input.GetKeyDown(KeyCode.E))
                        {
                            controller.ContinueGuide();
                        }
                    }
                }

                // If an item is being held and E is pressed, drop the item in its current position
                else if (controller.heldItem != null && Input.GetKeyDown(KeyCode.E))
                {
                    controller.heldItem.transform.SetParent(null);

                    // Not necessary, just in case something broke with the main rotation fix in controller
                    controller.heldItem.transform.eulerAngles = new Vector3(0, 180, 0);

                    if (controller.selectedItem != null)
                        controller.selectedItem.GetComponent<Item>().SetSelected(false);
                    controller.heldItem = null;
                }

                // If an item is being held, change prompt to Drop Item
                if (controller.heldItem != null)
                    promptText.text = "[E] Drop Item";
            }

            else
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
        }
    }
}
