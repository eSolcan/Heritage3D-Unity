using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOnPreviewVisitor : MonoBehaviour
{
    private ControllerPlayerScene controller;
    private Collider thisCollider;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        thisCollider = this.GetComponent<Collider>();
    }

    void Update()
    {
        if (controller.inPlayer && !thisCollider.enabled)
            thisCollider.enabled = true;
        else if (!controller.inPlayer && thisCollider.enabled)
            thisCollider.enabled = false;
    }
}
