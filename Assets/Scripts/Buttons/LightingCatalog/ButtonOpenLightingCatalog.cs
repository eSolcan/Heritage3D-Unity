using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOpenLightingCatalog : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        controller.OpenLightingCatalog();
    }
}
