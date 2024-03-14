using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScaleSelected : ButtonCustom
{
    public bool increaseScale;

    private bool operating = false;

    // Upon clicking, start "operating", will loop increase/decrease scale while button is held down
    public override void OnPointerDown(PointerEventData eventData)
    {
        operating = true;
        this.gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(ChangeScaleLoop());
    }

    // On button click release, stop looping and revert color to normal
    public override void OnPointerUp(PointerEventData eventData)
    {
        operating = false;
        if (mouseHovering)
            this.gameObject.GetComponent<Image>().color = hoverColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    // Color changes
    public override void OnPointerExit(PointerEventData eventData)
    {
        mouseHovering = false;
        if (operating)
            this.gameObject.GetComponent<Image>().color = selectedColor;
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    // Recursive scale change looping while button is held
    private IEnumerator ChangeScaleLoop()
    {
        if (increaseScale)
            ui.IncreaseScale();
        else
            ui.DecreaseScale();

        yield return new WaitForSeconds(.02f);
        if (operating)
            StartCoroutine(ChangeScaleLoop());
    }


}
