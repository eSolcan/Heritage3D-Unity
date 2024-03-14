using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIMG : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        catalog.SetImgPath();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
