using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayUpDownVisitor : MonoBehaviour
{

    public float frequency = .001f;
    private float sinValue = 0f;
    private bool climbing = true;

    private float baseYValue;
    private bool active = false;


    void Start()
    {
        baseYValue = this.gameObject.transform.position.y;
        active = true;
        StartCoroutine(SwayThenSleep());
    }

    private IEnumerator SwayThenSleep()
    {
        // Get current transform
        Transform itemTransform;
        itemTransform = this.gameObject.transform;

        // Calculate new position and change on the world object
        float valueToAdd = Mathf.Sin(sinValue);
        Vector3 newPosition = new Vector3(itemTransform.position.x, baseYValue + valueToAdd, itemTransform.position.z);

        this.gameObject.transform.position = newPosition;

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
