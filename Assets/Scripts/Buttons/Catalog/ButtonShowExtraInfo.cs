using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShowExtraInfo : ButtonCustom
{
    public GameObject infoWindow;

    public override void OnPointerEnter(PointerEventData eventData){
        infoWindow.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData){
        infoWindow.SetActive(false);
    }

}
