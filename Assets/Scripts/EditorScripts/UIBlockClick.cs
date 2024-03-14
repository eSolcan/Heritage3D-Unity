using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBlockClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   Controller controller;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // On pointer enter, set block state to true
    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.cursorOverSideMenu = true;

        // If it's the scrollable item list, block scroll movement
        if(this.gameObject.name.Equals("SceneItemsListPanel"))
            controller.blockScrollCameraMovement = true;
    }

    // On pointer enter, set block state to false
    public void OnPointerExit(PointerEventData eventData)
    {
        controller.cursorOverSideMenu = false;

        // If it's the scrollable item list, unblock scroll movement
        if(this.gameObject.name.Equals("SceneItemsListPanel"))
            controller.blockScrollCameraMovement = false;
    }
}
