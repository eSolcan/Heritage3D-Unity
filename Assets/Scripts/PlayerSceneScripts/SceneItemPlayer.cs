using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneItemPlayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private ControllerPlayerScene controller;

    public GameObject associatedObject;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    public bool hovering;

    public GameObject nameDisplay;


    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;

        MoveCameraToObject();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hovering)
            this.gameObject.GetComponent<Image>().color = hoverColor;
        else
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

    public void MoveCameraToObject()
    {
        Vector3 itemPosition = associatedObject.transform.position;
        controller.cam.transform.position = itemPosition;

        controller.cam.transform.position -= controller.cam.transform.forward * 2;
    }

    public void SetItemName(string itemNameDisplay)
    {
        nameDisplay.GetComponent<TextMeshProUGUI>().text = itemNameDisplay;
    }


}
