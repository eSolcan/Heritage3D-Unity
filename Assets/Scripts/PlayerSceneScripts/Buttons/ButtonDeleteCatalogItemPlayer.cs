using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDeleteCatalogItemPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        GameObject objToGetComponentFrom = gameObject.transform.parent.GetChild(1).gameObject;
        switch (catalog.currentType)
        {
            case 0:
                objToGetComponentFrom.GetComponent<CatalogItem3DPlayer>().DeleteItem();
                break;
            case 1:
                objToGetComponentFrom.GetComponent<CatalogItemImagePlayer>().DeleteItem();
                break;
            default:
                break;
        }
    }
}
