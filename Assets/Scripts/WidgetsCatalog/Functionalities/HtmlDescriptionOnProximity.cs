using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HtmlDescriptionOnProximity : MonoBehaviour
{
    private Controller controller;
    private UI ui;
    private WidgetsCatalog widgetsCatalog;

    public string description;
    public float radiusForCollider;

    public SphereCollider sphereCollider;

    public bool playerInside;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = controller.ui;
        widgetsCatalog = controller.canvasMain.GetComponent<WidgetsCatalog>();

        radiusForCollider = 15f;

        // Apply collider trigger to current object
        sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = radiusForCollider;
        sphereCollider.enabled = false;

        // Open menu for user to write description
        widgetsCatalog.OpenHtmlDescriptionInsertWindow();

        playerInside = false;
    }

    void Update()
    {
        // Enable/Disable trigger collider based on player state
        if (controller.inPlayer && !sphereCollider.enabled)
            sphereCollider.enabled = true;
        else if (!controller.inPlayer && sphereCollider.enabled)
            sphereCollider.enabled = false;

        // Calculate distance to player and comapre to current html displayed to see if change is needed
        if (playerInside && controller.htmlDisplayObject != this.gameObject)
        {
            float distanceToPlayer = Vector3.Distance(controller.player.transform.position, this.transform.position);
            float distanceCurrentHtml = Vector3.Distance(controller.player.transform.position, controller.htmlDisplayObject.transform.position);
            if (distanceToPlayer < distanceCurrentHtml)
            {
                controller.htmlDisplayObject = this.gameObject;
                controller.ChangeCurrentHtmlCode(description);
            }
        }
    }

    // When player enters trigger are, display description on website side panel
    private void OnTriggerEnter(Collider other)
    {
        // If other collider is player
        if (other.gameObject.tag == "Player")
        {
            playerInside = true;
            if (controller.htmlDisplayObject == null)
            {
                controller.htmlDisplayObject = this.gameObject;
                controller.ChangeCurrentHtmlCode(description);
            }
        }
    }

    // When player exits trigger area, clear side panel
    private void OnTriggerExit(Collider other)
    {
        // If other collider is player
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.tag == "Player")
            {
                playerInside = false;

                controller.htmlDisplayObject = null;
            }

            controller.ChangeCurrentHtmlCode("");
        }
    }


}
