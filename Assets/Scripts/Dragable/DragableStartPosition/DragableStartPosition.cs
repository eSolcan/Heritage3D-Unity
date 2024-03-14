using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableStartPosition : MonoBehaviour
{
    private Controller controller;

    private Camera cam;
    private bool held;
    public bool snapping;

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    public float planePosition;
    private Plane plane;

    public Transform fullDragableTransform;

    public GameObject arrowObj;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        // planePosition = -1;
        planePosition = controller.currentWorkingFloorLevel;

        cam = Camera.main;
        plane = new Plane(Vector3.down, planePosition);

        snapping = false;

        // Make dragable and big red arrow to be always visible
        this.gameObject.GetComponent<Renderer>().material.renderQueue = 3003;
        arrowObj.GetComponent<Renderer>().material.renderQueue = 3003;
    }

    void Update()
    {
        controller.playerStartPosition = this.transform.position;
        // controller.playerStartPosition.y += 1.05f;

        if (held && !snapping)
        {
            screenPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
            }
            fullDragableTransform.position = worldPosition;
        }
    }

    private void OnMouseDown()
    {
        held = true;
        Cursor.visible = false;
    }

    private void OnMouseUp()
    {
        held = false;
        Cursor.visible = true;
    }

    public float GetPlanePosition()
    {
        return planePosition;
    }

    public void SetPlanePosition(float newValue)
    {
        planePosition = newValue;
        plane.SetNormalAndPosition(Vector3.down, new Vector3(0, planePosition, 0));
    }

}
