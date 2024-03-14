using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOpenCatalogPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        controller.canvasMain.GetComponent<CatalogPlayerScene>().OpenCatalog();
    }

}