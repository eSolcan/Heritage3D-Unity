using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementPointClick : MonoBehaviour
{
    public ControllerPlayerScene controller;

    public Vector3 startPlayerPosition;
    public Vector3 currentPlayerPosition;
    public Vector3 targetPlayerPosition;

    public bool teleporting;

    public GameObject targetPositionObj;

    public int interpolationFramesCount;
    private int elapsedFrames;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        teleporting = false;

        interpolationFramesCount = 60;
        elapsedFrames = 0;
    }


    void Update()
    {
        // Draw raycast every frame for ground check
        Ray ray = controller.cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;

        // If something hit, check if it was floor (using tag check), and draw prefab for target location
        if (controller.inPlayer && !teleporting && !controller.inGuideMode && !controller.inCatalog && Physics.Raycast(ray, out hitPoint))
        {
            if (hitPoint.collider.gameObject.tag == "Ground")
            {
                targetPlayerPosition = hitPoint.point;

                // Increase target height so player isn't teleported into the floor
                targetPlayerPosition.y += 1.08f;

                // Update target position object
                if (!targetPositionObj.activeSelf && !teleporting)
                    targetPositionObj.SetActive(true);

                targetPositionObj.transform.position = targetPlayerPosition;

                // If player pressed LButton, teleport to location
                if (!teleporting && !controller.inCatalog && !controller.inInputBox && Input.GetMouseButtonDown(0))
                {
                    // Disable player controller and set target location
                    controller.player.GetComponent<CharacterController>().enabled = false;
                    startPlayerPosition = this.gameObject.transform.position;

                    targetPositionObj.SetActive(false);

                    teleporting = true;
                    StartCoroutine(TeleportThenActivatePlayer());
                }
            }
        }
        else if (targetPositionObj.activeSelf)
            targetPositionObj.SetActive(false);
    }

    private IEnumerator TeleportThenActivatePlayer()
    {

        // Lerp player towards target location
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
        Vector3 interpolatedPosition = Vector3.Lerp(startPlayerPosition, targetPlayerPosition, interpolationRatio);

        controller.player.transform.position = interpolatedPosition;

        yield return new WaitForEndOfFrame();

        elapsedFrames += 1;

        // Update last movement time for idle check
        this.gameObject.GetComponent<PlayerMovement>().lastMovementTime = Time.time;

        // Check distance between player and target. If at target, stop, otherwise, move again
        if (Vector3.Distance(controller.player.transform.position, targetPlayerPosition) <= .01f)
        {
            // Turn player controller back on and confirm teleportation end
            controller.player.GetComponent<CharacterController>().enabled = true;
            teleporting = false;
            elapsedFrames = 0;
        }
        else
            StartCoroutine(TeleportThenActivatePlayer());

    }
}
