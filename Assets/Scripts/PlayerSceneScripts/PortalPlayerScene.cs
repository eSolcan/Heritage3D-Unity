using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalPlayerScene : MonoBehaviour
{
    private ControllerPlayerScene controller;
    public PlayerCamera playerCamController;
    public PlayerMovement playerMovementController;

    private float screenFadeSleep;

    public GameObject linkedPortal;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        playerCamController = controller.player.GetComponent<PlayerCamera>();
        playerMovementController = controller.player.GetComponent<PlayerMovement>();

        screenFadeSleep = .3f;
    }

    void Update()
    {
        this.gameObject.GetComponent<MeshCollider>().enabled = controller.inPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player
        if (other.gameObject.layer == 31)
        {
            // If it has a valid linked portal
            if (linkedPortal != null)
            {
                // Block player movement and camera rotation
                playerCamController.teleporting = true;
                playerMovementController.teleporting = true;

                GameObject player = controller.player;
                Transform actualPortalTransform = linkedPortal.transform.GetChild(1);

                // Fade to screen to black then teleport to other portal
                StartCoroutine(FadeThenTeleport(player, actualPortalTransform));
            }
        }
    }

    private IEnumerator FadeThenTeleport(GameObject player, Transform actualPortalTransform)
    {
        RawImage rawImage = controller.playerDarkScreenFade.GetComponent<RawImage>();
        Color updatedColor = new Color(0, 0, 0, 0);

        // Increase alpha of image untill fully opaque
        while (rawImage.color.a < 1.1f)
        {
            updatedColor.a += .01f;
            rawImage.color = updatedColor;
            yield return new WaitForSeconds(screenFadeSleep * Time.deltaTime);
        }

        // Force full dark screen
        updatedColor.a = 1;
        rawImage.color = updatedColor;

        // Then teleport player
        // Move player to other portal
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = actualPortalTransform.position + actualPortalTransform.forward;

        player.GetComponent<CharacterController>().enabled = true;

        // Update camera rotation
        playerCamController.xRotation = 0;
        playerCamController.yRotation = actualPortalTransform.eulerAngles.y;

        yield return new WaitForSeconds(.2f);

        // And reverse alpha of image
        StartCoroutine(UnfadeAfterTeleport());
    }

    private IEnumerator UnfadeAfterTeleport()
    {
        RawImage rawImage = controller.playerDarkScreenFade.GetComponent<RawImage>();
        Color updatedColor = new Color(0, 0, 0, 0);

        // Increase alpha of image untill fully opaque
        while (rawImage.color.a >= 0.05f)
        {
            updatedColor.a -= .005f;
            rawImage.color = updatedColor;
            yield return new WaitForSeconds(screenFadeSleep * Time.deltaTime);
        }

        // Force full transparency
        updatedColor.a = 0;
        rawImage.color = updatedColor;

        // Unlock movement and camera rotation
        playerCamController.teleporting = false;
        playerMovementController.teleporting = false;
    }
}
