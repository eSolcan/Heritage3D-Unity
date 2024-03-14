using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class API_Links : MonoBehaviour
{

    public string baseLink;
    public string baseMediaLink;

    public string uploadMuseum;
    public string uploadHotspot;

    public string getMuseumNames;
    public string getMuseumById;

    public string uploadMedia;
    public string uploadMediaObj;
    public string deleteMedia;
    public string queryFileNames;

    public string registerHotspot;
    public string queryHotspotItems;
    public string addHotspotItem;
    public string removeHotspotItem;


    void Start()
    {
        // For use on local development
        baseLink = "http://localhost:3000";

        // For use on deployment, replace with public IP/DNS
        // baseLink = "http://x.x.x.x:3000";

        baseMediaLink = baseLink + "/userMedia/";

        getMuseumNames = baseLink + "/query/museumNames/";
        getMuseumById = baseLink + "/query/museumSpecific/";

        uploadMuseum = baseLink + "/museum/register";
        uploadHotspot = baseLink + "/hotspot/register";

        uploadMedia = baseLink + "/media/upload";
        uploadMediaObj = baseLink + "/media/uploadobj";
        deleteMedia = baseLink + "/media/delete";
        queryFileNames = baseLink + "/query/mediaNames";

        registerHotspot = baseLink + "/hotspot/register";
        queryHotspotItems = baseLink + "/hotspot/queryItemNames";
        addHotspotItem = baseLink + "/hotspot/addItem";
        removeHotspotItem = baseLink + "/hotspot/removeItem";

    }

}
