using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonUploadDescription : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        widgetsCatalog.UpdateDescription();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseHovering)
            this.gameObject.GetComponent<Image>().color = hoverColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mouseHovering = true;
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        mouseHovering = false;
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
