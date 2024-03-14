using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementEditor : MonoBehaviour
{
    private Controller controller;

    public GameObject player;

    private CharacterController playerController;

    public float movementSpeed = 8f;

    Vector3 velocity;
    // public float gravity = -9.81f;

    public bool teleporting;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        playerController = player.GetComponent<CharacterController>();

        //Set time scale to normal, just in case
        Time.timeScale = 1f;

        teleporting = false;
    }

    void Update()
    {
        if (!controller.inCatalog && !teleporting && !controller.inGuideMode)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;

            if (Input.GetKey(KeyCode.Space))
                y = 1;
            else if (Input.GetKey(KeyCode.LeftShift))
                y = -1;

            Vector3 dir = new Vector3(x, y, z);
            UpdateMovement(dir);

            // If an object is being held, update the plane position for dragable in edit mode
            if (controller.selectedItem)
                controller.selectedItem.GetComponent<Dragable>().SetPlanePosition(transform.position.y);
        }
    }


    void UpdateMovement(Vector3 direction)
    {

        Transform playerTransform = playerController.transform;

        // Vector3 move = playerTransform.right * direction.x + playerTransform.forward * direction.z;
        Vector3 move = playerTransform.right * direction.x + playerTransform.forward * direction.z + playerTransform.up * direction.y;

        // Vector3 move = playerTransform.right * x + playerTransform.forward * z;

        playerController.Move(move * movementSpeed * Time.deltaTime);

        // velocity.y += gravity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerController.Move(velocity * Time.deltaTime);
    }
}
