using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSceneItemListingGuide : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Controller controller;
    public GuideMenu guideMenu;

    public GameObject associatedObject;

    public GameObject nameDisplay;
    public string itemName;
    public string itemId;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        guideMenu = controller.canvasMain.GetComponent<GuideMenu>();

        // Increase size of list if needed
        if (guideMenu.gridContentSceneItems.transform.childCount > 7)
        {
            Vector2 newSize = guideMenu.gridContentSceneItems.GetComponent<RectTransform>().sizeDelta;
            newSize.y += 45;
            guideMenu.gridContentSceneItems.GetComponent<RectTransform>().sizeDelta = newSize;
        }
    }

    // Used to set the display of the item in the listing of items
    public void SetItemName(string itemNameDisplay)
    {
        itemName = itemNameDisplay;
        nameDisplay.GetComponent<TextMeshProUGUI>().text = itemNameDisplay;
    }

    public void SetItemId(string i)
    {
        itemId = i;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        guideMenu.AddToPath(associatedObject, itemName);
        this.gameObject.GetComponent<Image>().color = selectedColor;
    }

    public void RemoveItemFromList()
    {
        // Decrease size of list
        if (guideMenu.gridContentSceneItems.transform.childCount >= 7)
        {
            Vector2 newSize = guideMenu.gridContentSceneItems.GetComponent<RectTransform>().sizeDelta;
            newSize.y -= 45;
            guideMenu.gridContentSceneItems.GetComponent<RectTransform>().sizeDelta = newSize;
        }

        GameObject.Destroy(this.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

}
