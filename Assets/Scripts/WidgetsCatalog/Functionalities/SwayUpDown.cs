using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayUpDown : MonoBehaviour
{
    private Controller controller;

    public float frequency = .0005f;
    private float sinValue = 0f;
    private bool climbing = true;

    private float baseYValue;
    private bool active = false;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        baseYValue = this.gameObject.transform.position.y;
    }

    void Update()
    {
        // If player is in the scene and item is not selected, move the item up and down
        if (controller.inPlayer && controller.player.activeSelf && controller.selectedItem != this.gameObject && !active)
        {
            baseYValue = this.gameObject.transform.parent.position.y;
            active = true;
            StartCoroutine(SwayThenSleep());
        }

        if (controller.inPlayer && !controller.player.activeSelf)
            active = false;
        else if (controller.inPlayer && controller.selectedItem == this.gameObject)
            active = false;

    }

    private IEnumerator SwayThenSleep()
    {

        // Get current transform
        Transform itemTransform;
        itemTransform = this.gameObject.transform.parent.GetChild(1).transform;

        // Calculate new position and change on the world object
        float valueToAdd = Mathf.Sin(sinValue);
        Vector3 newPosition = new Vector3(itemTransform.position.x, baseYValue + valueToAdd, itemTransform.position.z);

        this.gameObject.transform.parent.GetChild(1).transform.position = newPosition;

        // I don't know how this works anymore because I did it too long ago and yes I am smart, comments are good
        if (sinValue <= 1 && climbing)
            sinValue += frequency;
        else if (sinValue >= -1 && !climbing)
            sinValue -= frequency;

        if (sinValue > 1)
            climbing = false;
        else if (sinValue < -1)
            climbing = true;

        yield return new WaitForSeconds(frequency);

        if (active)
            StartCoroutine(SwayThenSleep());

    }
}
