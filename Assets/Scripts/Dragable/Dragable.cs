using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragable : MonoBehaviour
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

    public bool guideParentFollowing;
    public GameObject guideParentObject;
    public GameObject lineDrawInbound;
    public GameObject lineDrawOutbound;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        // planePosition = -1;
        planePosition = controller.currentWorkingFloorLevel;
        if (this.GetComponent<Item>() != null && this.GetComponent<Item>().type == 100)
            planePosition += 2;

        cam = Camera.main;
        plane = new Plane(Vector3.down, planePosition);

        snapping = false;

        guideParentFollowing = false;

        // If guide point, shouldnt be visible before normal dragable item (should stay inside, since it's smaller)
        if (this.gameObject.GetComponent<Item>() != null)
            this.gameObject.GetComponent<Renderer>().material.renderQueue = 3003;
        else
            this.gameObject.GetComponent<Renderer>().material.renderQueue = 3002;
    }

    void Update()
    {
        if (held && !snapping)
        {
            screenPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
            }
            fullDragableTransform.position = worldPosition;

            // If appropriate, update line draw positions
            UpdateLineDraws();
        }

        // If current dragable is a guide point with a parent attached, always follow parent's location
        if (guideParentObject != null)
        {
            this.transform.position = guideParentObject.transform.position;
            UpdateLineDraws();
        }
    }

    // If part of guide path, update line draw
    public void UpdateLineDraws()
    {
        if (lineDrawInbound != null)
            lineDrawInbound.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);

        if (lineDrawOutbound != null)
            lineDrawOutbound.GetComponent<LineRenderer>().SetPosition(1, this.transform.position);
    }

    private void OnMouseDown()
    {
        if (!controller.inPlayer && !controller.inCatalog)
        {
            if (controller.colliderModeWalls)
                controller.ui.ColliderModeWalls();

            if (controller.colliderModeFloors)
                controller.ui.ColliderModeFloors();

            held = true;
            Cursor.visible = false;

            if (this.GetComponent<Item>() != null && !controller.inCatalog)
            {
                controller.draggingSelectedItem = true;
                StartCoroutine(WaitThenShader(true));
            }
        }
    }

    private void OnMouseUp()
    {
        if (!controller.inPlayer)
        {
            held = false;
            snapping = false;
            Cursor.visible = true;
            if (this.GetComponent<Item>() != null && !controller.inCatalog)
            {
                controller.draggingSelectedItem = false;
                StartCoroutine(WaitThenShader(false));
            }
        }
    }

    public IEnumerator WaitThenShader(bool applyShader)
    {
        if (this.GetComponent<Item>() != null)
        {
            yield return new WaitForSeconds(.01f);
            if (controller.selectedItem.GetComponent<Item>().type != 4)
                controller.selectedItem.GetComponent<Item>().AlwaysVisibleShader(applyShader);
        }
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

    public void EnableDisableSideArrows(bool value)
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(value);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(value);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(value);
    }

}
