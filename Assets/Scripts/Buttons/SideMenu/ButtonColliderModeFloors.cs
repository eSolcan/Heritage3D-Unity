using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonColliderModeFloors : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.ColliderModeFloors();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (controller.colliderModeFloors)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void ResetColorButton()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

}
