using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonNewLocation : ButtonCustom
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        guideMenu.AddNewPoint();
        this.gameObject.GetComponent<Image>().color = selectedColor;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseHovering)
            this.gameObject.GetComponent<Image>().color = hoverColor;
        else
        {
            this.gameObject.GetComponent<Image>().color = baseColor;
        }
    }
    
}

