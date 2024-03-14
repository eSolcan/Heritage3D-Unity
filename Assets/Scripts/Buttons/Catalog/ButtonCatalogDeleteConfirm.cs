using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCatalogDeleteConfirm : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(WaitThenClose());
    }

    private IEnumerator WaitThenClose()
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
        yield return new WaitForEndOfFrame();
        catalog.DeleteItemFromCatalog();
    }
}
