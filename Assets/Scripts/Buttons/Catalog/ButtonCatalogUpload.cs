using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCatalogUpload : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(WaitThenOpen());
    }

    private IEnumerator WaitThenOpen()
    {
        catalog.OpenItemNameTextBox();
        yield return new WaitForEndOfFrame();
    }
}
