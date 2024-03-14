using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonObjPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        catalog.SetObjPath();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
