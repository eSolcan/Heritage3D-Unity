using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MuseumListingPlayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private ControllerPlayerScene controller;

    public string museumId;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    public bool hovering;

    public GameObject nameDisplay;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();

        // Increase size of list if needed
        if (controller.ui.itemListingContent.transform.childCount > 4)
        {
            Vector2 newSize = controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta;
            newSize.y += 45;
            controller.ui.itemListingContent.GetComponent<RectTransform>().sizeDelta = newSize;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        controller.LoadSceneJson(museumId);
        this.gameObject.GetComponent<Image>().color = selectedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void SetMuseumName(string musemNameDisplay)
    {
        nameDisplay.GetComponent<TextMeshProUGUI>().text = musemNameDisplay;
    }

    private IEnumerator WaitThenSubmit(){
        
        yield return new WaitForEndOfFrame();
        controller.ui.CloseMuseumListing();
    }
}
