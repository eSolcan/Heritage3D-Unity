using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableZ : MonoBehaviour
{
    private Camera cam;
    private bool held;
    private Controller controller;

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    public float planePosition = -1;
    private Plane plane;

    public Dragable sphereDragable;
    public Transform fullDragableTransform;

    private Item parentItemComponent;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        cam = Camera.main;
        fullDragableTransform = sphereDragable.fullDragableTransform;
        plane = new Plane(Vector3.down, planePosition);

        parentItemComponent = this.transform.parent.gameObject.GetComponent<Item>();

        this.gameObject.GetComponent<Renderer>().material.renderQueue = 3001;
    }

    void Update()
    {
        if (held)
        {
            screenPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
            }
            fullDragableTransform.position = new Vector3(fullDragableTransform.position.x, sphereDragable.GetPlanePosition(), worldPosition.z);

            sphereDragable.UpdateLineDraws();
        }
    }

    private void OnMouseDown()
    {
        held = true;
        Cursor.visible = false;

        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inCatalog)
            parentItemComponent.SetSelected(true);

        //Change plane for raycast, height was increased
        plane.SetNormalAndPosition(Vector3.down, new Vector3(0, sphereDragable.GetPlanePosition(), 0));

        if (parentItemComponent != null && !controller.inCatalog)
            StartCoroutine(sphereDragable.WaitThenShader(true));
    }

    private void OnMouseUp()
    {
        held = false;
        Cursor.visible = true;

        if (parentItemComponent != null)
            StartCoroutine(sphereDragable.WaitThenShader(false));
    }

    private void OnMouseEnter()
    {
        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inPlayer)
        {
            parentItemComponent.SetToSelectedColor();
        }
    }

    private void OnMouseExit()
    {
        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inPlayer)
        {
            parentItemComponent.SetToDefaultColor();
        }
    }
}
