using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SceneItemListing : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    private Controller controller;

    // public string itemName;
    public GameObject associatedObject;

    public GameObject nameDisplay;
    public string itemId;

    public bool selected;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    private float previousLeftClickTime;
    private float doubleClickTime = .2f;
    private bool waiting;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        // Increase size of list if needed
        if (controller.ui.itemListingContent.transform.childCount > 4)
        {
            Vector2 newSize = controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta;
            newSize.y += 45;
            controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta = newSize;
        }

        // Set selected, if not a collider
        StartCoroutine(CringeWait());

        previousLeftClickTime = 0f;
        waiting = false;
    }

    private IEnumerator CringeWait()
    {
        yield return new WaitForSeconds(.02f);
        if (controller.selectedItem != null && controller.selectedItem.GetComponent<Item>().type < 100)
            selected = true;
    }

    // Used to set the display of the item in the listing of items
    public void SetItemName(string itemNameDisplay)
    {
        nameDisplay.GetComponent<TextMeshProUGUI>().text = itemNameDisplay;
    }

    public void SetItemId(string i)
    {
        itemId = i;
    }

    // On click
    public void OnPointerDown(PointerEventData eventData)
    {
        bool justSelected = false;

        if (!waiting)
        {
            if (associatedObject != null && !selected)
            {
                associatedObject.GetComponent<Item>().SetSelected(true);
                justSelected = true;
            }

            StartCoroutine(WaitThenProcede(justSelected));
        }

        // Update click time
        previousLeftClickTime = Time.time;
    }

    private IEnumerator WaitThenProcede(bool justSelected)
    {
        waiting = true;

        yield return new WaitForSeconds(doubleClickTime + .05f);
        if (Time.time - previousLeftClickTime <= doubleClickTime)
        {
            controller.MoveCameraToSelected();
            controller.ui.UpdateCameraGroundCenterPoint();
        }
        else if (selected && !justSelected)
            associatedObject.GetComponent<Item>().SetSelected(false);

        waiting = false;
    }

    public void RemoveItemFromScene()
    {
        controller.RemoveItemFromScene(associatedObject);

        // Decrease size of list
        if (controller.ui.itemListingContent.transform.childCount >= 4)
        {
            Vector2 newSize = controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta;
            newSize.y -= 45;
            controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta = newSize;
        }

        GameObject.Destroy(this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        associatedObject.GetComponent<Item>().SetToSelectedColor();
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            associatedObject.GetComponent<Item>().SetToDefaultColor();
            this.gameObject.GetComponent<Image>().color = baseColor;
        }
        else
            this.gameObject.GetComponent<Image>().color = selectedColor;

    }

    public void SetToBaseColor()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void SetToSelectedColor()
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
    }

}
