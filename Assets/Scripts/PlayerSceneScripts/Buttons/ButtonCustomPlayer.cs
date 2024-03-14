using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustomPlayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ControllerPlayerScene controller;
    public UIPlayerScene ui;
    public CatalogPlayerScene catalog;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    public bool selected = false;

    public int indexInPathList;

    public bool mouseHovering;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        ui = controller.ui;
        catalog = controller.canvasMain.GetComponent<CatalogPlayerScene>();

        mouseHovering = false;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        mouseHovering = true;
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        mouseHovering = false;
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {

    }

    public virtual void ResetToBaseColor()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
