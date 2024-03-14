using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUploadScene : ButtonCustom
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(WaitThenSave());
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    private IEnumerator WaitThenSave(){
        ui.SaveMuseum();
        yield return new WaitForEndOfFrame();
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
