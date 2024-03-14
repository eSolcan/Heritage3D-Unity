using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableY : MonoBehaviour
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

    private Vector2 mousePositionPrevious;

    private Item parentItemComponent;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        cam = Camera.main;
        fullDragableTransform = sphereDragable.fullDragableTransform;
        // plane = new Plane(-cam.transform.position, planePosition);
        plane = new Plane(Vector3.right, planePosition);

        mousePositionPrevious = Input.mousePosition;

        parentItemComponent = this.transform.parent.gameObject.GetComponent<Item>();

        this.gameObject.GetComponent<Renderer>().material.renderQueue = 3001;
    }

    void Update()
    {
        if (held)
        {
            // screenPosition = Input.mousePosition;

            // Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            // if (plane.Raycast(ray, out float distance))
            // {
            //     worldPosition = ray.GetPoint(distance);
            // }

            // sphereDragable.SetPlanePosition(worldPosition.y);
            // fullDragableTransform.position = new Vector3(fullDragableTransform.position.x, worldPosition.y, fullDragableTransform.position.z);

            // Get mouse position
            Vector2 currentPosition = Input.mousePosition;
            float deltaPositionY = currentPosition.y - mousePositionPrevious.y;
            mousePositionPrevious = currentPosition;

            Vector3 vec3DeltaY = new Vector3(0, deltaPositionY / 10, 0);

            fullDragableTransform.position += vec3DeltaY;

            sphereDragable.SetPlanePosition(this.transform.position.y - .5f);

            sphereDragable.UpdateLineDraws();
        }
    }


    private void OnMouseDown()
    {
        held = true;
        Cursor.visible = false;

        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inCatalog)
            parentItemComponent.SetSelected(true);

        mousePositionPrevious = Input.mousePosition;

        //Change plane for raycast, incase the camera was rotated
        planePosition = fullDragableTransform.position.y;
        plane.SetNormalAndPosition(Vector3.right, new Vector3(fullDragableTransform.position.x, planePosition, fullDragableTransform.position.y));

        if (parentItemComponent != null && !controller.inCatalog)
            StartCoroutine(sphereDragable.WaitThenShader(true));
    }

    private void OnMouseUp()
    {
        held = false;
        Cursor.visible = true;

        if (parentItemComponent != null && !controller.inCatalog)
            StartCoroutine(sphereDragable.WaitThenShader(false));
    }

    private void OnMouseEnter()
    {
        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inPlayer && !controller.inCatalog)
        {
            parentItemComponent.SetToSelectedColor();
        }
    }

    private void OnMouseExit()
    {
        if (parentItemComponent != null && !parentItemComponent.selected && !controller.inPlayer && !controller.inCatalog)
        {
            parentItemComponent.SetToDefaultColor();
        }
    }
}
