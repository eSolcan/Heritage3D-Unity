using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HtmlDescriptionPlayer : MonoBehaviour
{
    private ControllerPlayerScene controller;
    private UIPlayerScene ui;

    public string description;

    public bool playerInside;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        ui = controller.ui;

        playerInside = false;
    }

    void Update()
    {
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
