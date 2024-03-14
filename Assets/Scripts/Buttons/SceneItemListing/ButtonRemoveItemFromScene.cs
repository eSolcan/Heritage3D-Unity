using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonRemoveItemFromScene : ButtonCustom, IPointerDownHandler //, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{

    private SceneItemListing itemListingParent;

    void Start()
    {
        itemListingParent = this.transform.parent.gameObject.GetComponent<SceneItemListing>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        itemListingParent.RemoveItemFromScene();
    }

}
