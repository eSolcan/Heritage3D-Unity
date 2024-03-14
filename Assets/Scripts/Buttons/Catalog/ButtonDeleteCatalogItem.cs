using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDeleteCatalogItem : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        GameObject objToGetComponentFrom = gameObject.transform.parent.GetChild(1).gameObject;
        switch (catalog.currentType)
        {
            case 0:
                objToGetComponentFrom.GetComponent<CatalogItem3D>().DeleteItem();
                break;
            case 1:
                objToGetComponentFrom.GetComponent<CatalogItemImage>().DeleteItem();
                break;
            case 2:
                objToGetComponentFrom.GetComponent<CatalogItemVideo>().DeleteItem();
                break;
            case 3:
                objToGetComponentFrom.GetComponent<CatalogItemAudio>().DeleteItem();
                break;
            default:
                break;
        }
    }
}
