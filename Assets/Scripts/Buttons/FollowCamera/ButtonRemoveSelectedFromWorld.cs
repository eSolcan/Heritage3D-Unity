using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonRemoveSelectedFromWorld : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        controller.RemoveSelectedFromWorld();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseHovering)
            this.gameObject.GetComponent<Image>().color = baseColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;

    }

}
