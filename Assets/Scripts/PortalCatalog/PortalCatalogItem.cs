using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalCatalogItem : MonoBehaviour
{
    private Controller controller;

    public int indexInList;
    public string id;
    public string itemName = "";
    // private int type = 6;
    public GameObject nameDisplay;

    public GameObject thisPortalSphere;

    private Color baseColor;
    private Color selectedColor;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        baseColor = this.gameObject.GetComponent<Image>().color;
        selectedColor = new Color(.4f, .58f, .94f, 1);

        UpdateBackgroundColor();
    }

    public void UpdateBackgroundColor()
    {
        if (controller.selectedItem.GetComponent<Item>().linkedPortal == thisPortalSphere)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void LinkPortal()
    {
        if (controller.selectedItem != null)
        {
            controller.selectedItem.GetComponent<Item>().linkedPortal = thisPortalSphere;
            // thisPortalSphere.GetComponent<Item>().linkedPortal = controller.selectedItem;
            controller.ui.ClosePortalCatalog();
        }
    }

    public void SetName(string newName)
    {
        itemName = newName;
        nameDisplay.GetComponent<TextMeshProUGUI>().text = newName;
    }


}
