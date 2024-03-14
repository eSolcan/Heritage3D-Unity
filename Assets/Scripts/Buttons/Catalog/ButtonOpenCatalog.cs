using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOpenCatalog : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        catalog.OpenCatalog();
    }

}
