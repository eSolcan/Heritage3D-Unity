using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Controller controller;

    private Renderer rendr;
    public bool selected;
    private UI ui;

    public bool isPortal;
    public GameObject linkedPortal;

    public string id;
    public string itemName;
    public int type;
    public List<string> addedWidgetComponents;

    private Color defaultColor;
    private Color selectedColor = new Color(0, 1f, 0f, 1);
    private Shader defaultShader;

    public GameObject[] directionArrows;

    public SceneItemListing itemListing;
    public GameObject itemListingForGuide;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        rendr = GetComponent<Renderer>();
        ui = controller.ui;
        defaultColor = rendr.material.color;
        addedWidgetComponents = new List<string>();
        selected = false;

        isPortal = false;
        linkedPortal = null;

        defaultShader = rendr.material.shader;

        if (this.type < 100 && !controller.inPlayer)
            this.SetSelected(true);

        if (controller.inPlayer)
        {
            foreach (GameObject arrow in directionArrows)
            {
                arrow.SetActive(false);
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!selected && !controller.inPlayer && !controller.inCatalog)
        {
            SetToSelectedColor();
        }
    }

    private void OnMouseExit()
    {
        if (!selected && !controller.inPlayer && !controller.inCatalog)
        {
            SetToDefaultColor();
        }
    }

    public void SetToDefaultColor()
    {
        rendr.material.color = defaultColor;
    }

    public void SetToSelectedColor()
    {
        rendr.material.color = selectedColor;
    }

    private void OnMouseDown()
    {
        if (!selected && !controller.inPlayer)
            SetSelected(true);
    }

    public void SetSelected(bool mode)
    {
        selected = mode;
        if (mode)
        {
            // If an object was already selected, desselect before selecting new one
            if (controller.selectedItem != null)
            {
                controller.selectedItem.GetComponent<Item>().SetSelected(false);
            }

            // If in any of the collider modes, exit
            if (controller.colliderModeFloors || controller.colliderModeWalls)
            {
                controller.colliderModeWalls = false;
                controller.colliderModeFloors = false;
                ui.lineDraw = false;
                ui.colliderButtonWalls.GetComponent<ButtonColliderModeWalls>().ResetColorButton();
                ui.colliderButtonFloors.GetComponent<ButtonColliderModeFloors>().ResetColorButton();
                ui.lineObjectInstance.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
                ui.lineObjectInstance.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));

                ui.lineObjectInstance.SetActive(false);
            }

            // Select item listing
            itemListing.SetToSelectedColor();
            itemListing.selected = true;

            // Set new selected item in controller
            controller.selectedItem = this.gameObject;

            // Change color of sphere and arrows to selected mode color
            rendr.material.color = selectedColor;

            // Enable outline
            if(this.transform.parent.GetChild(1).GetComponent<Outline>() != null)
                this.transform.parent.GetChild(1).GetComponent<Outline>().enabled = true;

            // Show UI elements when relevant
            if (!controller.inPlayer)
            {

                ui.selectedItemUIElements.SetActive(true);

                if (this.type == 6 && !ui.linkPortalButton.gameObject.activeSelf)
                    ui.linkPortalButton.gameObject.SetActive(true);

                ui.UpdateSliderNewlySelected((int)this.gameObject.transform.parent.GetChild(1).localRotation.eulerAngles.y);
            }
        }
        else
        {
            // Unselect item listing
            itemListing.SetToBaseColor();
            itemListing.selected = false;

            // Revert sphere and arrows to original color 
            rendr.material.color = defaultColor;

            ui.selectedItemUIElements.SetActive(false);
            ui.linkPortalButton.gameObject.SetActive(false);

            // Set no selected item
            controller.selectedItem = null;

            // Disable outline
            if(this.transform.parent.GetChild(1).GetComponent<Outline>() != null)
                this.transform.parent.GetChild(1).GetComponent<Outline>().enabled = false;
        }
    }

    // Apply/Remove "Always Visible" shader to item
    public void AlwaysVisibleShader(bool yes)
    {
        GameObject objToEdit = this.gameObject.transform.parent.GetChild(1).gameObject;
        Renderer renderer = objToEdit.GetComponent<Renderer>();

        if (yes)
        {
            // Portals and videos are special
            if (type == 6 || type == 2 || type == 3)
            {
                objToEdit.transform.GetChild(0).GetComponent<Renderer>().material.shader = controller.shaderAlwaysVisible;
            }
            else if (renderer != null)
                renderer.material.shader = controller.shaderAlwaysVisible;
            else
            {
                int nrChildren = objToEdit.transform.childCount;
                // for (int i = 0; i < nrChildren; i++)
                //     objToEdit.transform.GetChild(i).GetComponent<Renderer>().material.shader = controller.shaderAlwaysVisible;
                for (int i = 0; i < nrChildren; i++)
                    if (objToEdit.transform.GetChild(i).childCount == 0 || type >= 100)
                        objToEdit.transform.GetChild(i).GetComponent<Renderer>().material.shader = controller.shaderAlwaysVisible;
                    else
                        ShaderChildren(objToEdit.transform.GetChild(i).gameObject, controller.shaderAlwaysVisible);
            }
        }
        else
        {
            // Portals and videos are special
            if (type == 6 || type == 2 || type == 3)
            {
                objToEdit.transform.GetChild(0).GetComponent<Renderer>().material.shader = Shader.Find("Standard");
            }
            else if (renderer != null)
                renderer.material.shader = Shader.Find("Standard");
            else
            {
                int nrChildren = objToEdit.transform.childCount;
                for (int i = 0; i < nrChildren; i++)
                    if (objToEdit.transform.GetChild(i).childCount == 0 || type >= 100)
                        objToEdit.transform.GetChild(i).GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                    else
                        ShaderChildren(objToEdit.transform.GetChild(i).gameObject, Shader.Find("Standard"));
            }
        }

    }

    private void ShaderChildren(GameObject parentObj, Shader shaderToApply)
    {
        int nrChildren = parentObj.transform.childCount;
        for (int i = 0; i < nrChildren; i++)
            parentObj.transform.GetChild(i).GetComponent<Renderer>().material.shader = shaderToApply;
    }



}
