using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableStartY : MonoBehaviour
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

    private Vector2 mousePositionPrevious;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        cam = Camera.main;
        fullDragableTransform = sphereDragable.fullDragableTransform;
        plane = new Plane(Vector3.right, planePosition);

        mousePositionPrevious = Input.mousePosition;

        this.gameObject.GetComponent<Renderer>().material.renderQueue = 3001;
    }

    void Update()
    {
        if (held)
        {

            // Get mouse position
            Vector2 currentPosition = Input.mousePosition;
            float deltaPositionY = currentPosition.y - mousePositionPrevious.y;
            mousePositionPrevious = currentPosition;

            Vector3 vec3DeltaY = new Vector3(0, deltaPositionY / 10, 0);

            fullDragableTransform.position += vec3DeltaY;

            sphereDragable.SetPlanePosition(this.transform.position.y - .5f);
        }
    }


    private void OnMouseDown()
    {
        held = true;
        Cursor.visible = false;

        mousePositionPrevious = Input.mousePosition;

        //Change plane for raycast, incase the camera was rotated
        planePosition = fullDragableTransform.position.y;
        plane.SetNormalAndPosition(Vector3.right, new Vector3(fullDragableTransform.position.x, planePosition, fullDragableTransform.position.y));

    }

    private void OnMouseUp()
    {
        held = false;
        Cursor.visible = true;
    }

    
}
