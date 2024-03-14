using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public ControllerPlayerScene controller;

    public GameObject player;

    private CharacterController playerController;

    public float movementSpeed = 8f;

    Vector3 velocity;
    public float gravity = -9.81f;
    public float jumpHeight = 1f;

    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    public float lastMovementTime;
    private float timeForIdle;

    public bool teleporting;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        playerController = player.GetComponent<CharacterController>();

        //Set time scale to normal, just in case
        Time.timeScale = 1f;

        lastMovementTime = Time.time;

        timeForIdle = 20f;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, 0, z);

        if (controller.inPlayer && playerController.enabled && !controller.inGuideMode && !controller.inCatalog && !teleporting)
        {
            UpdateMovement(dir);
            CheckIfIdle();
        }
    }

    void UpdateMovement(Vector3 direction)
    {
        //Sphere at around feet level to check ground collision
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //If grounded, have a constant negative y velocity
        if (isGrounded && velocity.y < -1)
        {
            velocity.y = -1f;
        }

        Transform playerTransform = playerController.transform;

        Vector3 move = playerTransform.right * direction.x + playerTransform.forward * direction.z;

        playerController.Move(move * movementSpeed * Time.deltaTime);

        // Update last movement time
        if (move.x != 0 || move.y != 0 || move.z != 0)
        {
            lastMovementTime = Time.time;
        }

        // Jump
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            lastMovementTime = Time.time;
        }

        velocity.y += gravity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        playerController.Move(velocity * Time.deltaTime);
    }

    private void CheckIfIdle()
    {
        // Check if idle and continue guide mode if not already in it
        if (Time.time - lastMovementTime >= timeForIdle && !controller.inGuideMode)
            controller.ContinueGuide();
    }
}
