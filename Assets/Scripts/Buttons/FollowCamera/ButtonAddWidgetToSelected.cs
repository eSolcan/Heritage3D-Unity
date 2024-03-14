using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAddWidgetToSelected : ButtonCustom
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        ui.widgetCatalog.transform.parent.gameObject.GetComponent<WidgetsCatalog>().OpenWidgetCatalog();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseHovering)
            this.gameObject.GetComponent<Image>().color = baseColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

}
