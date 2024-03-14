using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOpenMuseumNameDialog : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.OpenMuseumNameTextBox();
    }



}