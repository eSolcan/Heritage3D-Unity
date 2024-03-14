using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCloseGuideMenu : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.CloseGuideMenu();
    }
}
