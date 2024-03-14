using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonColliderModeWalls : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.ColliderModeWalls();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (controller.colliderModeWalls)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void ResetColorButton()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

}
