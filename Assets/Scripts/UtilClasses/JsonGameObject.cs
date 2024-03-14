
using System.Collections.Generic;

[System.Serializable]
public class JsonGameObject
{

    public string itemId;
    public string itemName;
    public int itemType;
    public string itemDescription;

    public float lightIntensity;
    public float lightRange;

    public List<float> position;
    public List<float> rotation;
    public List<float> scale;

    public List<string> addedComponents;

    public string linkedPortaName;

    public JsonGameObject() { 
        rotation = new List<float>();
        position = new List<float>();
        scale = new List<float>();

        addedComponents = new List<string>();
    }

}
