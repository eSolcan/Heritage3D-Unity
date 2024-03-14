using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOpenCloseSceneItemListingPlayer : ButtonCustomPlayer
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.OpenCloseSideMenuItemListing();
    }
}
