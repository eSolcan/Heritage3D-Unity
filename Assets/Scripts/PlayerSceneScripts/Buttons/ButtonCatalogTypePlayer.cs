using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCatalogTypePlayer : ButtonCustomPlayer
{

    public int type = -1;

    public void SetSelected(bool val)
    {
        selected = val;

        if (selected)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        controller.canvasMain.GetComponent<CatalogPlayerScene>().DisplayContent(type);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
            this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
            this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
