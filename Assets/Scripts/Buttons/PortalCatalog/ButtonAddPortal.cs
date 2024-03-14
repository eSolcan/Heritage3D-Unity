using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAddPortal : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        // ui.OpenItemNameTextBox();
        this.gameObject.GetComponent<Image>().color = selectedColor;
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
