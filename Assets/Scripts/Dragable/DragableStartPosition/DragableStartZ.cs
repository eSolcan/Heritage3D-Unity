using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableStartZ : MonoBehaviour
{
    private Camera cam;
    private bool held;
    private Controller controller;

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    public float planePosition = -1;
    private Plane plane;

    public DragableStartPosition sphereDragable;
    public Transform fullDragableTransform;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        cam = Camera.main;
        fullDragableTransform = sphereDragable.fullDragableTransform;
        plane = new Plane(Vector3.down, planePosition);

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
        }
    }

    private void OnMouseDown()
    {
        held = true;
        Cursor.visible = false;

        //Change plane for raycast, height was increased
        plane.SetNormalAndPosition(Vector3.down, new Vector3(0, sphereDragable.GetPlanePosition(), 0));

    }

    private void OnMouseUp()
    {
        held = false;
        Cursor.visible = true;
    }


}
