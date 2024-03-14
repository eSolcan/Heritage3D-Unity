using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetRotateToPlayer : MonoBehaviour
{
    private Controller controller;
    private UI ui;

    private Color baseColor;
    private Color selectedColor;

    // private string compToAdd;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = controller.ui;

        baseColor = this.gameObject.GetComponent<Image>().color;
        selectedColor = new Color(.4f, .58f, .94f, 1);

        // compToAdd = "_RotateTowardsPlayer";
    }

    public void UpdateBackgroundColor()
    {
        Item item = controller.selectedItem.GetComponent<Item>();
        foreach (string componentName in item.addedWidgetComponents)
        {
            if (componentName.Equals("RotateTowardsPlayer"))
                this.gameObject.GetComponent<Image>().color = selectedColor;
            else
                this.gameObject.GetComponent<Image>().color = baseColor;
        }
    }

    // Used to add a new component to currently selected item
    public void AddToSelectedItem()
    {
        if (controller.selectedItem != null)
        {
            Item item = controller.selectedItem.GetComponent<Item>();
            bool alreadyHasWidget = false;

            foreach (string componentName in item.addedWidgetComponents)
            {
                // Set boolean state of wether widget already added or not
                if (componentName.Equals("RotateTowardsPlayer"))
                {
                    alreadyHasWidget = true;
                    break;
                }
            }

            // Add/Remove widget based on boolean value previously determined
            if (!alreadyHasWidget)
            {
                controller.selectedItem.AddComponent<RotateTowardsPlayer>();
                controller.selectedItem.GetComponent<Item>().addedWidgetComponents.Add("RotateTowardsPlayer");
                this.gameObject.GetComponent<Image>().color = selectedColor;
            }
            else
            {
                Destroy(controller.selectedItem.GetComponent<RotateTowardsPlayer>());
                controller.selectedItem.GetComponent<Item>().addedWidgetComponents.Remove("RotateTowardsPlayer");
                this.gameObject.GetComponent<Image>().color = baseColor;
            }
        }
    }
}
