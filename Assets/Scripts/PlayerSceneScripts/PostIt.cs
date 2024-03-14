using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostIt : MonoBehaviour
{
    public ControllerPlayerScene controller;
    public GameObject player;
    private float speed = 1f;

    public TextMeshProUGUI promptText;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        player = GameObject.Find("Player");
        promptText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (player == null)
            player = GameObject.Find("Player");

        // If player is in the scene, rotate the item towards player
        if (player != null && player.activeSelf)
        {
            Transform itemTransform;
            if (this.gameObject.transform.parent != null)
                itemTransform = this.gameObject.transform.parent.GetChild(1);
            else
                itemTransform = this.gameObject.transform;

            // Determine which direction to rotate towards
            Vector3 targetDirection = player.transform.position - itemTransform.position;
            targetDirection.y = 0;

            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(itemTransform.forward, targetDirection, singleStep, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            Quaternion newRotation = Quaternion.LookRotation(newDirection);
            Vector3 newRotationEuler = newRotation.eulerAngles;

            itemTransform.localEulerAngles = new Vector3(90, 180 + newRotationEuler.y, newRotationEuler.z);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            controller.postit = this.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            controller.postit = null;
        }
    }
}
