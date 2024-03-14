using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HotSpotClass 
{
    public string hostpostId;
    public string museumId;

    public List<string> addedMedia;

    public HotSpotClass() { 
        addedMedia = new List<string>();
    }
}
