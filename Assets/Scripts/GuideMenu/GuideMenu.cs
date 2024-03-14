using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideMenu : MonoBehaviour
{

    public Controller controller;
    public UI ui;

    public GameObject gridContentSceneItems;
    public GameObject gridContentGuidePath;

    public GameObject sceneItemPrefab;
    public GameObject pathItemPrefab;

    public GameObject fullDragablePrefab;

    // Hold list of items in the path list
    public List<GameObject> pathList;

    public GameObject drawLinePrefab;
    public List<GameObject> listOfLines;

    private int pathPointsWithoutParent;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = controller.ui;

        pathList = new List<GameObject>();
        listOfLines = new List<GameObject>();

        pathPointsWithoutParent = 0;
    }

    public GameObject AddItemListing(GameObject objInListing, string name, string id)
    {
        // Instantiate new item listing and parent to grid
        GameObject newListing = Instantiate(sceneItemPrefab);
        newListing.transform.SetParent(gridContentSceneItems.transform, false);

        // Set display name and associated game object to listing (for on click)
        newListing.GetComponent<ButtonSceneItemListingGuide>().SetItemName(name);
        newListing.GetComponent<ButtonSceneItemListingGuide>().SetItemId(id);
        newListing.GetComponent<ButtonSceneItemListingGuide>().associatedObject = objInListing;
        newListing.GetComponent<ButtonSceneItemListingGuide>().guideMenu = controller.canvasMain.GetComponent<GuideMenu>();

        return newListing;
    }

    public void AddNewPoint()
    {
        // Instantiate new item listing and parent to grid
        GameObject newPathItem = Instantiate(pathItemPrefab);
        newPathItem.transform.SetParent(gridContentGuidePath.transform, false);

        pathList.Add(newPathItem);

        // Set display name and index in list
        newPathItem.GetComponent<ButtonGuidePathItem>().SetItemName("Path Point " + pathPointsWithoutParent);
        newPathItem.GetComponent<ButtonGuidePathItem>().indexInPath = pathList.Count - 1;

        // Instantiate new dragable object and associate to entry
        Vector3 camPosition = controller.cam.transform.position;
        Vector3 instLocation = new Vector3(
                    camPosition.x + controller.cam.transform.forward.x * 20,
                    controller.currentWorkingFloorLevel,
                    camPosition.z + controller.cam.transform.forward.z * 20
                );

        GameObject newDragable = GameObject.Instantiate(fullDragablePrefab, instLocation, new Quaternion(0, 180, 0, 0));
        newDragable.GetComponent<Transform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);

        newPathItem.GetComponent<ButtonGuidePathItem>().associatedObject = newDragable;

        // Update arrows on all items
        foreach (GameObject entry in pathList)
            entry.GetComponent<ButtonGuidePathItem>().UpdateArrows(true);

        // Update path drawing lines
        UpdateDrawLines();

        // Update value of path points without parent
        pathPointsWithoutParent += 1;
    }

    public void AddToPath(GameObject objAtLocation, string displayName)
    {
        // Instantiate new item listing and parent to grid
        GameObject newPathItem = Instantiate(pathItemPrefab);
        newPathItem.transform.SetParent(gridContentGuidePath.transform, false);

        pathList.Add(newPathItem);

        // Set display name, index in list and assoiated object
        newPathItem.GetComponent<ButtonGuidePathItem>().SetItemName(displayName);
        newPathItem.GetComponent<ButtonGuidePathItem>().indexInPath = pathList.Count - 1;
        newPathItem.GetComponent<ButtonGuidePathItem>().parentObjectToFollow = objAtLocation;

        // Instantiate new dragable object and associate to entry
        Vector3 instLocation = objAtLocation.transform.position;

        GameObject newDragable = GameObject.Instantiate(fullDragablePrefab, instLocation, new Quaternion(0, 180, 0, 0));
        newDragable.GetComponent<Transform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);

        newPathItem.GetComponent<ButtonGuidePathItem>().associatedObject = newDragable;

        // Update parent object that path point is meant to follow
        newDragable.transform.GetChild(0).gameObject.GetComponent<Dragable>().guideParentObject = objAtLocation;
        newDragable.transform.GetChild(0).gameObject.GetComponent<Dragable>().guideParentFollowing = true;

        // Update arrows on all items
        foreach (GameObject entry in pathList)
            entry.GetComponent<ButtonGuidePathItem>().UpdateArrows(true);

        // Update path drawing lines
        UpdateDrawLines();
    }

    public void RemoveItemFromPathBasedOnParent(GameObject objParent)
    {
        List<int> indexesToRemove = new List<int>();

        // Find which items to remove from list
        for (int i = 0; i < pathList.Count; i++)
        {
            if (pathList[i].GetComponent<ButtonGuidePathItem>().parentObjectToFollow == objParent)
            {
                indexesToRemove.Add(i);
            }
        }

        // Remove items from list of path items in inverse order, because of index 
        if (indexesToRemove.Count > 0)
        {
            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                RemoveItemFromPath(indexesToRemove[i], false);

            // Update arrows first and last items
            if (pathList.Count > 0)
                pathList[0].GetComponent<ButtonGuidePathItem>().UpdateArrows(false);
            if (pathList.Count > 1)
                pathList[pathList.Count - 1].GetComponent<ButtonGuidePathItem>().UpdateArrows(false);

            // Update path drawing lines
            UpdateDrawLines();

            // Update scrollbar & scrollable list sizes
            GuideMenu guideMenu = ui.mainCanvas.GetComponent<GuideMenu>();
            Vector2 newSize = guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta;
            if (guideMenu.gridContentGuidePath.transform.childCount >= 7)
            {
                newSize.y = guideMenu.gridContentGuidePath.transform.childCount * 35;
            }
            else
            {
                newSize.y = 340;
            }
            guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta = newSize;
        }
    }

    // Removes from list and updates remaining items with new indexes
    public void RemoveItemFromPath(int indexInList, bool updateArrowsAndLines)
    {
        // Remove item from list
        GameObject removedObject = pathList[indexInList];
        pathList.RemoveAt(indexInList);

        // Update index of remaining items
        for (int i = indexInList; i < pathList.Count; i++)
            pathList[i].GetComponent<ButtonGuidePathItem>().indexInPath = i;

        // Remove dragable from world and item from displayed list
        Destroy(removedObject.GetComponent<ButtonGuidePathItem>().associatedObject);
        Destroy(removedObject);

        if (updateArrowsAndLines)
        {
            // Update arrows on all items
            foreach (GameObject entry in pathList)
                entry.GetComponent<ButtonGuidePathItem>().UpdateArrows(true);

            // Update path drawing lines
            UpdateDrawLines();
        }
    }

    public void MoveItemUpInPath(int indexInList)
    {
        // Swap positions and update indexes
        GameObject itemAtIndex = pathList[indexInList];
        GameObject itemBeforeIndex = pathList[indexInList - 1];

        pathList[indexInList] = itemBeforeIndex;
        pathList[indexInList - 1] = itemAtIndex;

        itemAtIndex.GetComponent<ButtonGuidePathItem>().indexInPath -= 1;
        itemBeforeIndex.GetComponent<ButtonGuidePathItem>().indexInPath += 1;

        // Update displayed list
        itemAtIndex.transform.SetSiblingIndex(indexInList - 1);
        itemBeforeIndex.transform.SetSiblingIndex(indexInList);

        // Update arrows on all items
        foreach (GameObject entry in pathList)
            entry.GetComponent<ButtonGuidePathItem>().UpdateArrows(false);

        // Update path drawing lines
        UpdateDrawLines();
    }

    public void MoveItemDownInPath(int indexInList)
    {
        // Swap positions and update indexes
        GameObject itemAtIndex = pathList[indexInList];
        GameObject itemAfterIndex = pathList[indexInList + 1];

        pathList[indexInList] = itemAfterIndex;
        pathList[indexInList + 1] = itemAtIndex;

        itemAtIndex.GetComponent<ButtonGuidePathItem>().indexInPath += 1;
        itemAfterIndex.GetComponent<ButtonGuidePathItem>().indexInPath -= 1;

        // Update displayed list
        itemAtIndex.transform.SetSiblingIndex(indexInList + 1);
        itemAfterIndex.transform.SetSiblingIndex(indexInList);

        // Update arrows on all items
        foreach (GameObject entry in pathList)
            entry.GetComponent<ButtonGuidePathItem>().UpdateArrows(false);

        // Update path drawing lines
        UpdateDrawLines();
    }

    // Update guide path line between all items 
    // Used when changing order of items in the path
    public void UpdateDrawLines()
    {
        foreach (GameObject line in listOfLines)
            Destroy(line);

        if (pathList.Count > 1)
        {
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                GameObject newLine = Instantiate(drawLinePrefab);
                newLine.transform.parent = null;
                listOfLines.Add(newLine);

                pathList[i].GetComponent<ButtonGuidePathItem>().associatedObject.transform.GetChild(0).gameObject.GetComponent<Dragable>().lineDrawOutbound = newLine;
                pathList[i + 1].GetComponent<ButtonGuidePathItem>().associatedObject.transform.GetChild(0).gameObject.GetComponent<Dragable>().lineDrawInbound = newLine;

                newLine.GetComponent<LineRenderer>().SetPosition(1, pathList[i].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position);
                newLine.GetComponent<LineRenderer>().SetPosition(0, pathList[i + 1].GetComponent<ButtonGuidePathItem>().associatedObject.transform.position);
            }
        }
    }


}
