using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCameraTopViewPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        controller.SetCameraTopView();
        if (controller.inTopViewCamera)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (controller.inTopViewCamera)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
