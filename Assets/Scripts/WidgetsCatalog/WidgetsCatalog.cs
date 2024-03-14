using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WidgetsCatalog : MonoBehaviour
{
    private Controller controller;
    private UI ui;

    public GameObject grid;

    public GameObject previousButton;
    public GameObject nextButton;

    private int currentPage = 1;
    private int maxPage = 1;
    public TextMeshProUGUI pageText;

    public List<PairWidgetInfo> widgets3D;
    public List<PairWidgetInfo> widgetsImage;
    public List<PairWidgetInfo> widgetsVideo;
    public List<PairWidgetInfo> widgetsAudio;
    public List<PairWidgetInfo> widgetsText;

    public GameObject widgetItemPrefab;

    public GameObject htmlTextInsertWindow;
    public GameObject minimumThreeCharsWindow;

    public List<string> blacklistedWordInHtml;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = GameObject.Find("CanvasMain").GetComponent<UI>();

        widgets3D = new List<PairWidgetInfo>();
        widgetsImage = new List<PairWidgetInfo>();
        widgetsVideo = new List<PairWidgetInfo>();
        widgetsAudio = new List<PairWidgetInfo>();
        widgetsText = new List<PairWidgetInfo>();

        // Add widget to each category of item. Very inefficient ofc, but it is what it is
        widgets3D.Add(new PairWidgetInfo("Object will sway up and down", "SwayUpDown"));
        widgets3D.Add(new PairWidgetInfo("Add a description to the object that will be shown on proximity", "HtmlDescriptionOnProximity"));

        widgetsImage.Add(new PairWidgetInfo("Object will sway up and down", "SwayUpDown"));
        widgetsImage.Add(new PairWidgetInfo("Object will constantly rotate towards the player's location", "RotateTowardsPlayer"));
        widgetsImage.Add(new PairWidgetInfo("Add a description to the object that will be shown on proximity", "HtmlDescriptionOnProximity"));

        widgetsVideo.Add(new PairWidgetInfo("Object will constantly rotate towards the player's location", "RotateTowardsPlayer"));
        widgetsVideo.Add(new PairWidgetInfo("Add a description to the object that will be shown on proximity", "HtmlDescriptionOnProximity"));

        widgetsAudio.Add(new PairWidgetInfo("Add a description to the object that will be shown on proximity", "HtmlDescriptionOnProximity"));

        widgetsText.Add(new PairWidgetInfo("Object will constantly rotate towards the player's location", "RotateTowardsPlayer"));
    }

    private void ResetPages(int listSize)
    {
        //Show page related elements
        pageText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);

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

    public void OpenWidgetCatalog()
    {
        ui.OpenWidgetCatalog();
        UpdateContentDisplay();
    }

    public void CloseWidgetCatalog()
    {
        if (!controller.inInputBox)
        {
            htmlTextInsertWindow.SetActive(false);
            ui.CloseWidgetCatalog();
        }
    }

    // Used to clear the objects in the widget catalog
    public void ClearContent()
    {
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(grid.transform.GetChild(i).gameObject);
        }
    }

    public void PreviousPage()
    {
        if (!nextButton.GetComponent<Button>().interactable)
            nextButton.GetComponent<Button>().interactable = true;

        if (currentPage > 1)
            currentPage--;
        if (currentPage == 1)
            previousButton.GetComponent<Button>().interactable = false;

        UpdateContentDisplay();
        UpdatePageDisplay();
    }

    public void NextPage()
    {
        if (!previousButton.GetComponent<Button>().interactable)
            previousButton.GetComponent<Button>().interactable = true;

        if (currentPage < maxPage)
            currentPage++;
        if (currentPage == maxPage)
            nextButton.GetComponent<Button>().interactable = false;

        UpdateContentDisplay();
        UpdatePageDisplay();
    }

    private void UpdateContentDisplay()
    {
        ClearContent();

        int selectedItemType = controller.selectedItem.GetComponent<Item>().type;
        switch (selectedItemType)
        {
            case 0:
                LoadItemsInGrid(widgets3D);
                break;
            case 1:
                LoadItemsInGrid(widgetsImage);
                break;
            case 2:
                LoadItemsInGrid(widgetsVideo);
                break;
            case 3:
                LoadItemsInGrid(widgetsAudio);
                break;
            case 4:
                LoadItemsInGrid(widgetsText);
                break;
            default:
                break;
        }
    }

    private void LoadItemsInGrid(List<PairWidgetInfo> list)
    {
        if (list.Count > 0)
            for (int i = 0; i < list.Count; i++)
            {
                // Instantiate new item card and update description and component name
                GameObject itemCard = GameObject.Instantiate(widgetItemPrefab);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                // Update information of item card
                itemCard.transform.GetChild(0).gameObject.GetComponent<WidgetToAdd>().SetDisplayText(list[i].widgetDescription);
                itemCard.transform.GetChild(0).gameObject.GetComponent<WidgetToAdd>().SetAttachedComponent(list[i].widgetComponent);

                // Update item card background color to represent if it's added or not to selected item 
                // itemCard.transform.GetChild(0).gameObject.GetComponent<WidgetToAdd>().UpdateBackgroundColor();
            }
    }

    [Serializable]
    public class PairWidgetInfo
    {
        public string widgetDescription;
        public string widgetComponent;

        public PairWidgetInfo(string desc, string comp)
        {
            widgetDescription = desc;
            widgetComponent = comp;
        }
    }

    public void OpenHtmlDescriptionInsertWindow()
    {
        controller.inInputBox = true;

        htmlTextInsertWindow.SetActive(true);

        htmlTextInsertWindow.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
    }

    public void CloseHtmlDescriptionInsertWindow()
    {
        controller.inInputBox = false;

        htmlTextInsertWindow.SetActive(false);
    }

    public void UpdateDescription()
    {
        string description = htmlTextInsertWindow.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        string finalDescription = htmlTextInsertWindow.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;

        description = description.Replace(" ", "");
        description = description.ToLower();

        // Checks for description length and scripting, then update description
        if (description.Length < 3)
        {
            StartCoroutine(TextSleepOff("Description must be at least 3 characters long"));
            return;
        }

        foreach (string word in blacklistedWordInHtml)
        {
            // if (description.Contains("<script"))
            if (description.Contains(word))
            {
                StartCoroutine(TextSleepOff("Javascript scripts are not allowed in the description"));
                return;
            }
        }

        controller.selectedItem.GetComponent<HtmlDescriptionOnProximity>().description = finalDescription;
        CloseHtmlDescriptionInsertWindow();
    }

    public IEnumerator TextSleepOff(string text)
    {
        minimumThreeCharsWindow.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
        minimumThreeCharsWindow.SetActive(true);
        yield return new WaitForSeconds(5);
        minimumThreeCharsWindow.SetActive(false);
    }

}
