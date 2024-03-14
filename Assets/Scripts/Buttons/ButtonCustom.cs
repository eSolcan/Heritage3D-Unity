using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Controller controller;
    public UI ui;
    public Catalog catalog;
    public WidgetsCatalog widgetsCatalog;
    public PortalCatalog portalCatalog;
    public GuideMenu guideMenu;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    public bool selected = false;

    public int indexInPathList;

    public bool mouseHovering;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = GameObject.Find("CanvasMain").GetComponent<UI>();
        catalog = GameObject.Find("CanvasMain").GetComponent<Catalog>();
        widgetsCatalog = controller.canvasMain.GetComponent<WidgetsCatalog>();
        portalCatalog = ui.portalCatalog.GetComponent<PortalCatalog>();
        guideMenu = ui.gameObject.GetComponent<GuideMenu>();

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
