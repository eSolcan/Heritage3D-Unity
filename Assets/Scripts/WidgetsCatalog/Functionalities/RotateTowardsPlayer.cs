using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{

    public GameObject player;
    private float speed = 1f;

    void Start()
    {
        player = GameObject.Find("PlayerEditor");
    }

    void Update()
    {
        if (player == null)
            player = GameObject.Find("PlayerEditor");

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

            if (this.GetComponent<Item>() != null)
            {
                // If applied to image or video, keep upright
                if (this.GetComponent<Item>().type == 1 || this.GetComponent<Item>().type == 2)
                {
                    itemTransform.localEulerAngles = new Vector3(90, 180 + newRotationEuler.y, newRotationEuler.z);
                }

                // If applied to text, keep upright and invert half rotation (stupid, not sure why it does that)
                if (this.GetComponent<Item>().type == 4)
                {
                    itemTransform.localEulerAngles = new Vector3(0, 180 + newRotationEuler.y, newRotationEuler.z);
                }
            }
            else
                itemTransform.localEulerAngles = new Vector3(90, 180 + newRotationEuler.y, newRotationEuler.z);

        }

    }
}
