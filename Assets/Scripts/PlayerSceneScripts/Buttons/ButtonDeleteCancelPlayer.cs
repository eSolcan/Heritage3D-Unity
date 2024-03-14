using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonDeleteCancelPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(WaitThenDelete());
    }

    private IEnumerator WaitThenDelete(){
        this.gameObject.GetComponent<Image>().color = baseColor;
        yield return new WaitForEndOfFrame();
        catalog.CloseConfirmDelete();
    }
}
