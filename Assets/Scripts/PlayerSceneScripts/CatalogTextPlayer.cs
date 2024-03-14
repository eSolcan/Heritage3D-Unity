using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CatalogTextPlayer : MonoBehaviour
{
    private ControllerPlayerScene controller;

    // private int type = 4;

    public GameObject text;

    private CatalogPlayerScene catalog;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerPlayerScene>();
        catalog = GameObject.Find("CanvasMain").GetComponent<CatalogPlayerScene>();
    }

    public void AddTextToWorld(string textToAdd)
    {

        //Instantiate 
        Vector3 instLocation = new Vector3(0, 0, 0);
        instLocation = controller.player.transform.position;

        GameObject objInstance = GameObject.Instantiate(text);
        objInstance.transform.position = controller.player.transform.position;

        //Adjust scale and layer to be visible to main camera
        objInstance.transform.localScale = new Vector3(.15f, .2f, .20f);
        objInstance.layer = 0;

        //Add text to text
        objInstance.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = textToAdd;
        objInstance.transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text = textToAdd;

        // Wait then close catalog - this is so that point and click movement isn't activated when pressing the upload button
        StartCoroutine(WaitThenCloseCatalog());
    }

    private IEnumerator WaitThenCloseCatalog(){
        yield return new WaitForEndOfFrame();
        catalog.CloseCatalog();
    }
}
