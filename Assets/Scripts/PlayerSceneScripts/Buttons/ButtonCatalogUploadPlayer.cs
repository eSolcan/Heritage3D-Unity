using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCatalogUploadPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(WaitThenOpen());
    }

    private IEnumerator WaitThenOpen()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        yield return new WaitForEndOfFrame();
        controller.canvasMain.GetComponent<CatalogPlayerScene>().OpenItemNameTextBox();
    }
}
