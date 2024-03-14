using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOnPreview : MonoBehaviour
{

    private Controller controller;
    private Collider thisCollider;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        thisCollider = this.GetComponent<Collider>();
    }

    void Update()
    {
        if (
            (controller.inPlayer && !thisCollider.enabled) ||
            (!controller.inPlayer && controller.draggingSelectedItem && controller.selectedItem.GetComponent<Item>().type != 100)
        )
            thisCollider.enabled = true;
        else if (
            (!controller.inPlayer && thisCollider.enabled) ||
            (!controller.inPlayer && !controller.draggingSelectedItem)
        )
            thisCollider.enabled = false;
    }
}
