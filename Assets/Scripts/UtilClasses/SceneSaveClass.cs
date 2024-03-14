

using System;
using System.Collections.Generic;

[System.Serializable]
public class SceneSaveClass
{
    public string username;

    public string id;

    public string sceneName;
    public List<float> startPosition;
    public List<Triplet> guidePathLocations;

    public List<JsonGameObject> listOfObjects;

    public int skyTextureIndex;

    public SceneSaveClass(string uname)
    {
        username = uname;

        id = Guid.NewGuid().ToString();

        startPosition = new List<float>();
        guidePathLocations = new List<Triplet>();
        listOfObjects = new List<JsonGameObject>();
    }

}
