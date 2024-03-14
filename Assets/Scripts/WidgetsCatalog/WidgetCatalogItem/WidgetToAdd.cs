using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WidgetToAdd : MonoBehaviour
{
    private Controller controller;
    private UI ui;

    private Color baseColor;
    private Color selectedColor;

    public string componentAttached;
    public string displayText;

    public GameObject textDisplayObject;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = controller.ui;

        baseColor = this.gameObject.GetComponent<Image>().color;
        selectedColor = new Color(.4f, .58f, .94f, 1);

        // displayText = "";
        // componentAttached = "";
        UpdateBackgroundColor();
    }

    public void SetDisplayText(string textDisplay)
    {
        displayText = textDisplay;
        textDisplayObject.GetComponent<TextMeshProUGUI>().text = textDisplay;
    }

    public void SetAttachedComponent(string comp)
    {
        componentAttached = comp;
    }

    public void UpdateBackgroundColor()
    {
        Item item = controller.selectedItem.GetComponent<Item>();
        if (item.addedWidgetComponents.Contains(componentAttached))
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;

    }

    // Used to add a new component to currently selected item
    public void AddToSelectedItem()
    {
        if (controller.selectedItem != null && !controller.inInputBox)
        {
            Item item = controller.selectedItem.GetComponent<Item>();
            bool alreadyHasWidget = false;

            foreach (string componentName in item.addedWidgetComponents)
            {
                // Set boolean state of wether widget already added or not
                if (componentName.Equals(componentAttached))
                {
                    alreadyHasWidget = true;
                    break;
                }
            }

            // Add/Remove widget based on boolean value previously determined
            if (!alreadyHasWidget)
            {
                switch (componentAttached)
                {
                    case "SwayUpDown":
                        controller.selectedItem.AddComponent<SwayUpDown>();
                        break;
                    case "RotateTowardsPlayer":
                        controller.selectedItem.AddComponent<RotateTowardsPlayer>();
                        break;
                    case "HtmlDescriptionOnProximity":
                        controller.selectedItem.AddComponent<HtmlDescriptionOnProximity>();
                        break;
                    default:
                        break;
                }
                controller.selectedItem.GetComponent<Item>().addedWidgetComponents.Add(componentAttached);
                this.gameObject.GetComponent<Image>().color = selectedColor;
            }
            else
            {
                switch (componentAttached)
                {
                    case "SwayUpDown":
                        Destroy(controller.selectedItem.GetComponent<SwayUpDown>());
                        break;
                    case "RotateTowardsPlayer":
                        Destroy(controller.selectedItem.GetComponent<RotateTowardsPlayer>());
                        break;
                    case "HtmlDescriptionOnProximity":
                        Destroy(controller.selectedItem.GetComponent<HtmlDescriptionOnProximity>().sphereCollider);
                        Destroy(controller.selectedItem.GetComponent<HtmlDescriptionOnProximity>());
                        controller.ChangeCurrentHtmlCode("");
                        break;
                    default:
                        break;
                }
                controller.selectedItem.GetComponent<Item>().addedWidgetComponents.Remove(componentAttached);
                this.gameObject.GetComponent<Image>().color = baseColor;
            }
        }
    }
}
