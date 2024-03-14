using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPreviewMode : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        controller.SetCameraMode(true);
    }
}
