using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonFileBrowserNormalPlayer : ButtonCustomPlayer
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (catalog.currentType == 0 && !catalog.ValidToSubmitObj())
        {
            this.gameObject.GetComponent<Image>().color = selectedColor;
            StartCoroutine(catalog.TextSleepOff("Please select one of each file"));
        }
        else
        {
            this.gameObject.GetComponent<Image>().color = baseColor;
            StartCoroutine(WaitThenUpload());
        }

    }

    private IEnumerator WaitThenUpload()
    {
        yield return new WaitForEndOfFrame();
        catalog.UploadObject();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (mouseHovering)
        {
            this.gameObject.GetComponent<Image>().color = hoverColor;
        }
        else
            this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void UpdateCurrentType(int newType)
    {
        if (newType == 0)
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Submit";
        else
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Select File";
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mouseHovering = true;
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        mouseHovering = false;
        this.gameObject.GetComponent<Image>().color = baseColor;
    }
}
