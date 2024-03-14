using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSnapPoints : MonoBehaviour
{

    private Controller controller;
    private GameObject parentSphere;

    private Vector3 rotationBeforeSnapping;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        parentSphere = this.gameObject.transform.parent.parent.GetChild(0).gameObject;
    }

    private void OnMouseEnter()
    {
        bool selectedItemIsCollider = false;
        if (controller.selectedItem != null)
        {
            selectedItemIsCollider = controller.selectedItem.GetComponent<Item>().type == 100 || controller.selectedItem.GetComponent<Item>().type == 101;

            if (!selectedItemIsCollider && controller.draggingSelectedItem && !controller.draggedItemSnapped)
            {

                // Save rotation before snapping, to be re-applied after exiting snapping
                rotationBeforeSnapping = controller.selectedItem.transform.parent.GetChild(1).eulerAngles;

                // Apply new position of snapped object
                controller.selectedItem.transform.parent.position = this.transform.position;
                controller.selectedItem.transform.parent.position += this.transform.up * 1f;

                // Apply new rotation of snapped object
                GameObject objToRotate = controller.selectedItem.transform.parent.GetChild(1).gameObject;

                float yRotation = this.transform.parent.GetChild(1).eulerAngles.y;
                Vector3 newRotation = new Vector3(objToRotate.transform.eulerAngles.x, yRotation, objToRotate.transform.eulerAngles.z);
                objToRotate.transform.eulerAngles = newRotation;

                // Set snapping
                controller.SetSelectedSnapping(true);

                // Update plane position on dragable
                controller.selectedItem.GetComponent<Dragable>().SetPlanePosition(controller.selectedItem.transform.position.y);
            }
        }
    }

    private void OnMouseExit()
    {
        // Snapping of Colliders onto other colliders not supported
        if (controller.draggingSelectedItem && (controller.selectedItem.GetComponent<Item>().type != 100 && controller.selectedItem.GetComponent<Item>().type != 101))
        {
            // Re-apply snapping to dragged object
            controller.selectedItem.transform.parent.GetChild(1).eulerAngles = rotationBeforeSnapping;

            // Set snapping
            controller.SetSelectedSnapping(false);
        }
    }
}
