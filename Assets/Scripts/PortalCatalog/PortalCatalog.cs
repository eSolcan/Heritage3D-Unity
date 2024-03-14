using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalCatalog : MonoBehaviour
{

    public Controller controller;
    private UI ui;

    private int currentPage = 1;
    private int maxPage = 1;
    public TextMeshProUGUI pageText;
    public GameObject noContentText;
    public GameObject grid;
    public GameObject itemNameAndUpload;
    public GameObject itemNameAndUploadCloseButton;
    public GameObject minimumThreeCharacters;

    public GameObject portalListingPrefab;

    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject uploadButton;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = controller.ui;

        LoadObjectsInGrid();
    }

    public void LoadObjectsInGrid()
    {
        int countExcludingSelected = controller.listOfPortals.Count - 1;
        ResetPages(countExcludingSelected);

        List<GameObject> listExcludingSelected = new List<GameObject>();

        foreach (GameObject obj in controller.listOfPortals)
        {
            if (!obj.Equals(controller.selectedItem))
            {
                listExcludingSelected.Add(obj);
            }
        }

        if (countExcludingSelected > 0)
        {
            noContentText.SetActive(false);

            for (int i = currentPage * 6 - 5; i <= countExcludingSelected && i <= currentPage * 6; i++)
            {
                // Instantiate new item card
                GameObject itemCard = GameObject.Instantiate(portalListingPrefab);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                // Change display name and apply assotiated sphere
                itemCard.transform.GetChild(1).GetComponent<PortalCatalogItem>().SetName(listExcludingSelected[i - 1].GetComponent<Item>().itemName);
                itemCard.transform.GetChild(1).GetComponent<PortalCatalogItem>().thisPortalSphere = listExcludingSelected[i - 1];

                // newObject.layer = 5;
            }
        }
        else
            noContentText.SetActive(true);
    }

    //Used to clear the objects in the catalog
    public void ClearContent()
    {
        // Clear items in catalog
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(grid.transform.GetChild(i).gameObject);
        }
    }

    private void ResetPages(int listSize)
    {
        currentPage = 1;
        if (listSize == 0)
            maxPage = 1;
        else
            maxPage = (listSize + 6 - 1) / 6;
        if (maxPage == 1)
            nextButton.GetComponent<Button>().interactable = false;
        else
            nextButton.GetComponent<Button>().interactable = true;
        previousButton.GetComponent<Button>().interactable = false;

        UpdatePageDisplay();
    }

    private void UpdatePageDisplay()
    {
        pageText.SetText("Page " + currentPage + "/" + maxPage);
    }
}
