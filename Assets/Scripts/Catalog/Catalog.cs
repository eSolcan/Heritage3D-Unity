using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Dummiesman;
using SFB;
using System.IO;
using System.Text;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.Video;
using Defective.JSON;
using System.Runtime.InteropServices;
using System;



public class Catalog : MonoBehaviour
{

    private Controller controller;
    private UI ui;

    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject uploadButton;

    // public List<GameObject> typeButtons;
    public int currentType;

    public GameObject grid;

    public GameObject objects3dItem;
    public GameObject imagesItem;
    public GameObject videosItem;
    public GameObject audiosItem;
    public GameObject textItem;
    public GameObject miscItem;

    public List<CatalogItem<GameObject>> objects3dList;
    private List<CatalogItem<Texture>> imagesList;
    private List<CatalogItem<string>> videosList;
    private List<CatalogItem<AudioClip>> audiosList;
    public List<GameObject> miscList;

    private int currentPage = 1;
    private int maxPage = 1;
    public TextMeshProUGUI pageText;
    public GameObject noContentText;

    public GameObject inputField;
    public GameObject itemNameAndUpload;

    private string currentMediaName;

    public GameObject confirmDeleteWindow;

    private int typeToDelete;
    private int indexToDelete;

    public GameObject minimumThreeCharacters;

    public GameObject objButton;
    public GameObject mtlButton;
    public GameObject imgButton;

    private string objPath = "";
    private string mtlPath = "";
    private string imgPath = "";

    public GameObject selectFileButton;

    private bool itemsLoaded;

    private byte[] bytesMedia1;
    private byte[] bytesMedia2;
    private byte[] bytesMedia3;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        ui = GameObject.Find("CanvasMain").GetComponent<UI>();

        //Create empty lists for all types.
        objects3dList = new List<CatalogItem<GameObject>>();
        imagesList = new List<CatalogItem<Texture>>();
        videosList = new List<CatalogItem<string>>();
        audiosList = new List<CatalogItem<AudioClip>>();

        ResetPages(0);

        currentType = 0;

        //Load items in current window
        DisplayContent(currentType);

        controller.UpdateTypeButtonSelection(-1, currentType);

        itemsLoaded = false;
    }

    // Open catalog window. Checks if a user is logged and retrieves the items of said user, if required
    public void OpenCatalog()
    {
        // Set user in catalog and ask UI to enable catalog elements
        controller.inCatalog = true;
        ui.OpenCatalog();

        // If user is logged and synced, retrieve items
        if (!controller.username.Equals("") && !itemsLoaded)
        {
            itemsLoaded = true;
            StartCoroutine(QueryNamesOnStart());
        }
    }

    // Close catalog window
    public void CloseCatalog()
    {
        controller.inCatalog = false;
        CloseItemNameTextBox();
        ui.CloseCatalog();
    }

    // Used to clear the objects in the catalog
    public void ClearContent()
    {
        // Clear items in catalog
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(grid.transform.GetChild(i).gameObject);
        }
    }

    // Move to previous page
    public void PreviousPage()
    {
        // Enable next page button
        if (!nextButton.GetComponent<Button>().interactable)
            nextButton.GetComponent<Button>().interactable = true;

        if (currentPage > 1)
            currentPage--;

        // If on the first page, disable previous page button (there is no more "previous" page than current)
        if (currentPage == 1)
            previousButton.GetComponent<Button>().interactable = false;

        // Update what is being displayed in terms of items, as well as the page number at the bottom
        UpdateContentDisplay();
        UpdatePageDisplay();
    }

    // Move to next page
    public void NextPage()
    {
        // Enable previous page button
        if (!previousButton.GetComponent<Button>().interactable)
            previousButton.GetComponent<Button>().interactable = true;

        if (currentPage < maxPage)
            currentPage++;

        // If on the last page, disable next page button (there is no more "next" page than current)
        if (currentPage == maxPage)
            nextButton.GetComponent<Button>().interactable = false;

        // Update what is being displayed in terms of items, as well as the page number at the bottom
        UpdateContentDisplay();
        UpdatePageDisplay();
    }

    // Update what items are being shown on the screen
    private void UpdateContentDisplay()
    {
        // Clear all items first
        ClearContent();

        // ?? not sure why this is here
        if (inputField.activeSelf)
            inputField.SetActive(false);

        // Load items based on the current tab
        switch (currentType)
        {
            case 0:
                LoadObjectsInGrid();
                break;
            case 1:
                LoadImagesInGrid();
                break;
            case 2:
                LoadVideosInGrid();
                break;
            case 3:
                LoadAudiosInGrid();
                break;
            case 4:
                DisplayText();
                break;
            case 5:
                LoadMiscInGrid();
                break;
            default:
                break;
        }
    }

    // Display items based on the current tab selection
    public void DisplayContent(int type)
    {
        if (currentType != type)
        {
            controller.UpdateTypeButtonSelection(currentType, type);

            currentType = type;

            CloseItemNameTextBox();

            // Update page count based on the selected tab type
            switch (type)
            {
                case 0:
                    ResetPages(objects3dList.Count);
                    break;
                case 1:
                    ResetPages(imagesList.Count);
                    break;
                case 2:
                    ResetPages(videosList.Count);
                    break;
                case 3:
                    ResetPages(audiosList.Count);
                    break;
                case 5:
                    ResetPages(miscList.Count);
                    break;
                default:
                    break;
            }

            // If on miscelaneous tab, hide upload button, as this type can't be uploaded
            if (type == 5)
                uploadButton.SetActive(false);
            else if (!uploadButton.activeSelf)
                uploadButton.SetActive(true);

            selectFileButton.GetComponent<ButtonFileBrowserNormal>().UpdateCurrentType(type);
            UpdateContentDisplay();
        }
    }

    // Display text UI elements in the catalog, tab 4
    public void DisplayText()
    {
        //Hide no items text
        noContentText.SetActive(false);

        //Hide page related elements
        pageText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);

        //Show text box
        inputField.SetActive(true);

        // This is not ideal, but is an easy way to block exi
        controller.inInputBox = true;
    }

    // Load miscelaneous items in the catalog
    private void LoadMiscInGrid()
    {
        // Disable "no items" text
        noContentText.SetActive(false);

        // Display items
        for (int i = currentPage * 6 - 5; i <= miscList.Count && i <= currentPage * 6; i++)
        {
            //Instantiate both new item card and object to show in card
            GameObject itemCard = GameObject.Instantiate(miscItem);
            GameObject newObject = GameObject.Instantiate(miscList[i - 1]);

            //Set index, name and id of item to respective item card
            CatalogItem3D item = itemCard.transform.GetChild(1).GetComponent<CatalogItem3D>();
            item.indexInList = i - 1;
            item.id = miscList[i - 1].name;
            item.SetName(miscList[i - 1].name);
            item.type = 5;

            // Very hard coded, not the best solution for when new Widgets have to be added
            switch (item.id)
            {
                case "Portal":
                    item.type = 6;
                    break;
                case "PortalSimpleCenter":
                    item.type = 6;
                    break;
                case "PointLight":
                    item.type = 7;
                    break;
                case "Hotspot":
                    item.type = 8;
                    break;
                default:
                    break;
            }

            //Set item card parent to grid
            itemCard.transform.SetParent(grid.transform, false);

            //Get position where to place new object inside item card
            Transform placeHere = itemCard.transform.Find("Item Button").transform.Find("Object").transform;

            //Set parent and local position
            newObject.transform.SetParent(placeHere, false);
            newObject.transform.localPosition = new Vector3(0, 0, 0);

            // Portal pivot and scale are very weird, force change
            if (item.type == 6)
            {
                newObject.transform.localPosition = new Vector3(0, -1, 0);
                newObject.transform.localScale = new Vector3(.2f, .2f, .2f);
            }

            // If hotspot, activate UI help element
            if (item.type == 8)
            {
                item.InfoButtonSetActive();
            }
        }
    }

    // Reset page number display based on a list size
    private void ResetPages(int listSize)
    {
        if (currentType != 4)
        {
            //Show page related elements
            pageText.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(true);

            //Hide text input field 
            inputField.SetActive(false);
            controller.inInputBox = false;
        }

        // Update pages based on 6 items per page value
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

    // Used to upload files to the app as well as to the database
#if UNITY_WEBGL && !UNITY_EDITOR
    // WebGL call
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void UploadObject(){
        
        // Get the name to display on the item to be uploaded and clear the text input box
        currentMediaName = itemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        itemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";

        // Check if title meets minimum size requirement
        string trimmedMediaName = currentMediaName.Replace(" ", "");
        trimmedMediaName = currentMediaName.Replace("\n", "");

        if (trimmedMediaName.Length < 3)
        {
            StartCoroutine(TextSleepOff("Title must be at least 3 characters long"));
            return;
        }
        else
        {
            switch (currentType)
            {
                case 0:
                    if (objPath.Length == 0 || mtlPath.Length == 0 || imgPath.Length == 0)
                    {
                        Debug.Log("At least one of the URLs is invalid.");
                        break;
                    }
                    else
                    {
                        StartCoroutine(OBJLoader(objPath, mtlPath, imgPath, currentMediaName, currentMediaName, true));
                        break;
                    }
                case 1:
                    UploadFile(gameObject.name, "OnFileUpload", ".jpg", false);
                    break;
                case 2:
                    UploadFile(gameObject.name, "OnFileUpload", ".mp4", false);
                    break;
                case 3:
                    UploadFile(gameObject.name, "OnFileUpload", ".mp3", false);
                    break;
                default:
                    break;
            }

            CloseItemNameTextBox();
        }
    }

    // Called from browser
    public void OnFileUpload(string url)
    {
        switch (currentType)
        {
            case 1:
                StartCoroutine(ImageLoader(url, currentMediaName, currentMediaName, true));
                break;
            case 2:
                StartCoroutine(VideoLoader(url, currentMediaName, currentMediaName, true));
                break;
            case 3:
                StartCoroutine(AudioLoader(url, currentMediaName, currentMediaName, true));
                break;
            case 4:
                this.GetComponent<CatalogText>().AddTextToWorld(inputField.GetComponent<TMP_InputField>().text);
                inputField.GetComponent<TMP_InputField>().text = "";
                break;
            default:
                break;
        }
    }

#else

    public void UploadObject()
    {
        // Get the name to display on the item to be uploaded and clear the text input box
        currentMediaName = itemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        itemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";

        // Check if title meets minimum size requirement
        string trimmedMediaName = currentMediaName.Replace(" ", "");

        if (trimmedMediaName.Length < 3)
        {
            StartCoroutine(TextSleepOff("Title must be at least 3 characters long"));
            return;
        }
        else
        {
            switch (currentType)
            {
                case 0:
                    if (objPath.Length > 0 && mtlPath.Length > 0 && imgPath.Length > 0)
                    {
                        StartCoroutine(OBJLoader(
                            new System.Uri(objPath).AbsoluteUri,
                            new System.Uri(mtlPath).AbsoluteUri,
                            new System.Uri(imgPath).AbsoluteUri,
                            currentMediaName,
                            currentMediaName,
                            true));
                    }
                    break;
                case 1:
                    string[] imagePath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg,png", false);
                    if (imagePath.Length > 0)
                    {
                        StartCoroutine(ImageLoader(new System.Uri(imagePath[0]).AbsoluteUri, currentMediaName, currentMediaName, true));
                    }
                    break;
                case 2:
                    string[] videoPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mp4,flv,mov", false);
                    if (videoPath.Length > 0)
                    {
                        StartCoroutine(VideoLoader(new System.Uri(videoPath[0]).AbsoluteUri, currentMediaName, currentMediaName, true));
                    }
                    break;
                case 3:
                    string[] audioPath = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mp3", false);
                    if (audioPath.Length > 0)
                    {
                        StartCoroutine(AudioLoader(new System.Uri(audioPath[0]).AbsoluteUri, currentMediaName, currentMediaName, true));
                    }
                    break;
                case 4:
                    this.GetComponent<CatalogText>().AddTextToWorld(inputField.GetComponent<TMP_InputField>().text);
                    inputField.GetComponent<TMP_InputField>().text = "";
                    break;
                default:
                    break;
            }

            CloseItemNameTextBox();
        }
    }

#endif

    // Used to open the modal text window for inserting the item name
    public void OpenItemNameTextBox()
    {
        // Text does not support this, if text simply add item to world
        if (currentType != 4)
        {
            itemNameAndUpload.SetActive(true);
            controller.inInputBox = true;
        }
        else
            this.GetComponent<CatalogText>().AddTextToWorld(inputField.GetComponent<TMP_InputField>().text);

        // If added item typeis OBJ, have 3 more buttons shows, one for each required item to be added
        bool extraButtonsVisible = false;
        if (currentType == 0)
        {
            extraButtonsVisible = true;
        }

        objButton.SetActive(extraButtonsVisible);
        mtlButton.SetActive(extraButtonsVisible);
        imgButton.SetActive(extraButtonsVisible);
    }

    // Close modal text window for inserting the item name
    public void CloseItemNameTextBox()
    {
        itemNameAndUpload.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
        itemNameAndUpload.SetActive(false);
        controller.inInputBox = false;
    }

    // Load OBJ items in the grid
    private void LoadObjectsInGrid()
    {
        if (objects3dList.Count > 0)
        {
            noContentText.SetActive(false);

            for (int i = currentPage * 6 - 5; i <= objects3dList.Count && i <= currentPage * 6; i++)
            {
                //Instantiate both new item card and object to show in card
                GameObject itemCard = GameObject.Instantiate(objects3dItem);
                GameObject newObject = GameObject.Instantiate(objects3dList[i - 1].item);

                //Set index, name and id of item to respective item card
                CatalogItem3D item = itemCard.transform.GetChild(1).GetComponent<CatalogItem3D>();
                item.indexInList = i - 1;
                item.id = objects3dList[i - 1].id;
                item.SetName(objects3dList[i - 1].name);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                //Get position where to place new object inside item card
                Transform placeHere = itemCard.transform.Find("Item Button").transform.Find("Object").transform;

                //Set parent, local position and layer to UI
                newObject.transform.SetParent(placeHere, false);
                newObject.transform.localPosition = new Vector3(0, 0, 0);
                newObject.layer = 5;

                // Adjust size based on bounds
                Vector3 newObjBoundsSize = newObject.GetComponent<Renderer>().bounds.size;
                float maxBound = Mathf.Max(newObjBoundsSize.x, newObjBoundsSize.y, newObjBoundsSize.z);
                newObject.transform.localScale /= maxBound;
            }
        }
        else
            noContentText.SetActive(true);
    }

    // Load IMAGE items in the grid
    private void LoadImagesInGrid()
    {
        if (imagesList.Count > 0)
        {
            noContentText.SetActive(false);

            for (int i = currentPage * 6 - 5; i <= imagesList.Count && i <= currentPage * 6; i++)
            {
                //Instantiate new item card
                GameObject itemCard = GameObject.Instantiate(imagesItem);

                //Set index, name and id of item to respective item card
                CatalogItemImage item = itemCard.transform.GetChild(1).GetComponent<CatalogItemImage>();
                item.indexInList = i - 1;
                item.id = imagesList[i - 1].id;
                item.SetName(imagesList[i - 1].name);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                //Get texture location in item card and add texture from list
                GameObject obj = itemCard.transform.Find("Item Button").transform.Find("Object").gameObject;

                //Ideally should get the bounds of the parent, so that if resizes happen it will adjust accordingly
                var maxWidth = 160;
                var maxHeight = 120;
                float ratio = 0;
                if (imagesList[i - 1].item.width > imagesList[i - 1].item.height)
                {
                    ratio = imagesList[i - 1].item.width / maxWidth;
                    obj.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, imagesList[i - 1].item.height / ratio);
                }
                else
                {
                    ratio = imagesList[i - 1].item.height / maxHeight;
                    obj.GetComponent<RectTransform>().sizeDelta = new Vector2(imagesList[i - 1].item.width / ratio, maxHeight);
                }
                obj.GetComponent<RawImage>().texture = imagesList[i - 1].item;
            }
        }
        else
            noContentText.SetActive(true);
    }

    // Load VIDEO items in the grid
    private void LoadVideosInGrid()
    {
        if (videosList.Count > 0)
        {
            noContentText.SetActive(false);

            for (int i = currentPage * 6 - 5; i <= videosList.Count && i <= currentPage * 6; i++)
            {
                //Instantiate new item card
                GameObject itemCard = GameObject.Instantiate(videosItem);

                //Set index, name and id of item to respective item card
                CatalogItemVideo item = itemCard.transform.GetChild(1).GetComponent<CatalogItemVideo>();
                item.indexInList = i - 1;
                item.id = videosList[i - 1].id;
                item.SetName(videosList[i - 1].name);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                //Get video player location in item card and add video from list
                VideoPlayer videoPlayer = itemCard.transform.Find("Item Button").transform.Find("Object").GetComponent<VideoPlayer>();

                //Adjust size
                RectTransform rectTrans = itemCard.transform.Find("Item Button").transform.Find("Object").GetComponent<RectTransform>();
                rectTrans.sizeDelta = new Vector2(160, 160);

                videoPlayer.url = videosList[i - 1].item;
            }
        }
        else
            noContentText.SetActive(true);
    }

    // Load AUDIO items in the grid
    private void LoadAudiosInGrid()
    {
        if (audiosList.Count > 0)
        {
            noContentText.SetActive(false);

            for (int i = currentPage * 6 - 5; i <= audiosList.Count && i <= currentPage * 6; i++)
            {
                //Instantiate new item card
                GameObject itemCard = GameObject.Instantiate(audiosItem);

                //Set index, name and id of item to respective item card
                CatalogItemAudio item = itemCard.transform.GetChild(1).GetComponent<CatalogItemAudio>();
                item.indexInList = i - 1;
                item.id = audiosList[i - 1].id;
                item.SetName(audiosList[i - 1].name);

                //Set item card parent to grid
                itemCard.transform.SetParent(grid.transform, false);

                //Get video player location in item card and add video from list
                AudioSource audioSource = itemCard.transform.Find("Item Button").transform.Find("Object").GetComponent<AudioSource>();

                audioSource.clip = audiosList[i - 1].item;
            }
        }
        else
            noContentText.SetActive(true);
    }

    // Rest request to the database to get item
    private IEnumerator OBJLoader(string urlObj, string urlMtl, string urlImg, string id, string name, bool uploadingFresh)
    {
        UnityWebRequest requestObj = UnityWebRequest.Get(urlObj);
        UnityWebRequest requestMtl = UnityWebRequest.Get(urlMtl);
        UnityWebRequest requestImg = UnityWebRequestTexture.GetTexture(urlImg);

        yield return requestObj.SendWebRequest();
        yield return requestMtl.SendWebRequest();
        yield return requestImg.SendWebRequest();

        if (
            requestObj.result != UnityWebRequest.Result.Success ||
            requestMtl.result != UnityWebRequest.Result.Success ||
            requestImg.result != UnityWebRequest.Result.Success
            )
            Debug.Log("Error request");
        else
        {
            // Open streams for the requests from file locations
            MemoryStream textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(requestObj.downloadHandler.text));
            MemoryStream textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(requestMtl.downloadHandler.text));

            // Load obj using OBJLoader plugin, and extract actual model to be used
            GameObject item = new OBJLoader().Load(textStreamObj, textStreamMtl);
            GameObject actualItem = item.transform.GetChild(0).gameObject;

            // Remove empty parent object and delete it
            actualItem.transform.SetParent(null);
            GameObject.Destroy(item);

            // Set layer to UI and adjust size to fit in item box, using Bound Size
            actualItem.layer = 5;
            Vector3 maxBoundSize = actualItem.transform.gameObject.GetComponent<Renderer>().bounds.size;
            float maxBoundValue = Mathf.Max(maxBoundSize.x, maxBoundSize.y, maxBoundSize.z);
            Vector3 currentScale = actualItem.transform.localScale;
            actualItem.transform.localScale = currentScale / maxBoundValue * 1.4f;

            // Create new class instance to store item info and add to list
            CatalogItem<GameObject> newItem = new CatalogItem<GameObject>(id, name, actualItem);
            objects3dList.Add(newItem);

            // Apply texture to material
            Texture imageReceived = DownloadHandlerTexture.GetContent(requestImg);
            actualItem.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", imageReceived);
            actualItem.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");

            // Clear file paths
            objPath = "";
            mtlPath = "";
            imgPath = "";

            // Update catalog content and pages
            UpdateContentDisplay();
            ResetPages(objects3dList.Count);

            // If uploading item for the first time, transform request to bytes and send to database
            if (uploadingFresh)
            {
                byte[] bytesObj = requestObj.downloadHandler.data;
                byte[] bytesMtl = requestMtl.downloadHandler.data;
                byte[] bytesImg = requestImg.downloadHandler.data;
                StartCoroutine(ObjUploadAPI(bytesObj, bytesMtl, bytesImg));
            }
        }
    }

    // Rest request to the database to get item
    private IEnumerator ImageLoader(string url, string id, string name, bool uploadingFresh)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Use texture downloader to get texture from image location
            Texture imageReceived = DownloadHandlerTexture.GetContent(request);

            // Create new class instance to store item info and add to list
            CatalogItem<Texture> newItem = new CatalogItem<Texture>(id, name, imageReceived);
            imagesList.Add(newItem);

            // Update catalog content and pages
            UpdateContentDisplay();
            ResetPages(imagesList.Count);

            // If uploading item for the first time, transform request to bytes and send to database
            if (uploadingFresh)
            {
                byte[] bytes = request.downloadHandler.data;
                StartCoroutine(FileUploadAPI(bytes, "jpg"));
            }
        }
    }

    // Rest request to the database to get item
    private IEnumerator VideoLoader(string url, string id, string name, bool uploadingFresh)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Create new class instance to store item info and add to list
            CatalogItem<string> newItem = new CatalogItem<string>(id, name, url);
            videosList.Add(newItem);

            // Update catalog content and pages
            UpdateContentDisplay();
            ResetPages(videosList.Count);

            // If uploading item for the first time, transform request to bytes and send to database
            if (uploadingFresh)
            {
                byte[] bytes = request.downloadHandler.data;
                StartCoroutine(FileUploadAPI(bytes, "mp4"));
            }
        }
    }

    // Rest request to the database to get item
    private IEnumerator AudioLoader(string url, string id, string name, bool uploadingFresh)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Use audio downloader to get audio clip from clip location
            AudioClip audioReceived = DownloadHandlerAudioClip.GetContent(request);

            // Create new class instance to store item info and add to list
            CatalogItem<AudioClip> newItem = new CatalogItem<AudioClip>(id, name, audioReceived);
            audiosList.Add(newItem);

            // Update catalog content and pages
            UpdateContentDisplay();
            ResetPages(audiosList.Count);

            // If uploading item for the first time, transform request to bytes and send to database
            if (uploadingFresh)
            {
                byte[] bytes = request.downloadHandler.data;
                StartCoroutine(FileUploadAPI(bytes, "mp3"));
            }
        }
    }

    // Rest request to the database to upload an item
    private IEnumerator FileUploadAPI(byte[] bytes, string fileType)
    {
        // Create JSON API request
        string url = controller.apiLinks.uploadMedia;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Turn file into bytes
        // byte[] fileBytes = File.ReadAllBytes(fileLocation);
        byte[] fileBytes = bytes;

        // Convert the byte array to a base64-encoded string
        string base64String = System.Convert.ToBase64String(fileBytes);

        // Add fields to the JSON with file and file type
        json.AddField("bytes", base64String);
        json.AddField("name", currentMediaName);
        json.AddField("fileType", fileType);
        json.AddField("username", controller.username);
        json.AddField("token", controller.token);

        // Convert into request and send it
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Get result and parse to JSON, and extract file info
            var result = request.downloadHandler.text;
            ReceivedItemInfo data = JsonUtility.FromJson<ReceivedItemInfo>(result);

            // Name of item already set, just need to set id from returned response
            switch (currentType)
            {
                case 1:
                    imagesList[imagesList.Count - 1].id = data.id;
                    break;
                case 2:
                    videosList[videosList.Count - 1].id = data.id;
                    break;
                case 3:
                    audiosList[audiosList.Count - 1].id = data.id;
                    break;
                default:
                    break;
            }

            UpdateContentDisplay();
            Debug.Log("File uploaded.");
        }
    }

    private IEnumerator ObjUploadAPI(byte[] bytesObj, byte[] bytesMtl, byte[] bytesImg)
    {
        // Create JSON API request
        string url = controller.apiLinks.uploadMediaObj;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Turn files into bytes
        byte[] objBytes = bytesObj;
        byte[] mtlBytes = bytesMtl;
        byte[] imgBytes = bytesImg;

        // Convert the byte arrays to a base64-encoded strings
        string base64StringObj = System.Convert.ToBase64String(objBytes);
        string base64StringMtl = System.Convert.ToBase64String(mtlBytes);
        string base64StringImg = System.Convert.ToBase64String(imgBytes);

        // Add fields to the JSON with file and file type
        json.AddField("bytesObj", base64StringObj);
        json.AddField("bytesMtl", base64StringMtl);
        json.AddField("bytesImg", base64StringImg);
        json.AddField("name", currentMediaName);
        json.AddField("username", controller.username);
        json.AddField("token", controller.token);

        // Convert into request and send it
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // Get result and parse to JSON, and extract file info
            var result = request.downloadHandler.text;
            ReceivedItemInfo data = JsonUtility.FromJson<ReceivedItemInfo>(result);

            objects3dList[objects3dList.Count - 1].id = data.id;

            UpdateContentDisplay();
            Debug.Log("OBJ, MTL and Texture file uploaded.");
        }
    }

    // Used to query the files of a logged user on the first opening of the media catalog
    private IEnumerator QueryNamesOnStart()
    {
        // Create JSON request
        string url = controller.apiLinks.queryFileNames;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Add username and token field
        json.AddField("username", controller.username);
        json.AddField("token", controller.token);

        // Stringify
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Send request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {

            // Get result and parse to JSON, and extract list
            var result = request.downloadHandler.text;

            ReceivedData data = JsonUtility.FromJson<ReceivedData>(result);
            List<ReceivedItemInfo> listOfItemInfo = data.listOfItems;

            // If list was returned non empty, load items from list
            if (listOfItemInfo != null && listOfItemInfo.Count > 0)
            {
                noContentText.SetActive(false);

                // Add to list of objects depending on file type
                foreach (ReceivedItemInfo itemInfo in listOfItemInfo)
                {
                    string link = controller.apiLinks.baseMediaLink + itemInfo.id;
                    string linkFull = link + "." + itemInfo.type;

                    switch (itemInfo.type)
                    {
                        case "obj":
                            string linkMtl = link + ".mtl";
                            string linkImg = link + ".jpg";
                            StartCoroutine(OBJLoader(linkFull, linkMtl, linkImg, itemInfo.id, itemInfo.name, false));
                            break;
                        case "jpg":
                            StartCoroutine(ImageLoader(linkFull, itemInfo.id, itemInfo.name, false));
                            break;
                        case "mp4":
                            StartCoroutine(VideoLoader(linkFull, itemInfo.id, itemInfo.name, false));
                            break;
                        case "mp3":
                            StartCoroutine(AudioLoader(linkFull, itemInfo.id, itemInfo.name, false));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void PreliminaryDeleteItemFromCatalog(int index, int type)
    {
        typeToDelete = type;
        indexToDelete = index;
        CloseItemNameTextBox();
        OpenConfirmDelete();
    }

    // Delete an item from catalog. Also removes item from world, if any added, as well as from the database
    public void DeleteItemFromCatalog()
    {
        switch (typeToDelete)
        {
            case 0:
                StartCoroutine(DeleteItemAPI(objects3dList[indexToDelete].id, "obj"));
                DeleteItemFromWorld(objects3dList[indexToDelete].id);
                objects3dList.RemoveAt(indexToDelete);
                break;
            case 1:
                StartCoroutine(DeleteItemAPI(imagesList[indexToDelete].id, "jpg"));
                DeleteItemFromWorld(imagesList[indexToDelete].id);
                imagesList.RemoveAt(indexToDelete);
                break;
            case 2:
                StartCoroutine(DeleteItemAPI(videosList[indexToDelete].id, "mp4"));
                DeleteItemFromWorld(videosList[indexToDelete].id);
                videosList.RemoveAt(indexToDelete);
                break;
            case 3:
                StartCoroutine(DeleteItemAPI(audiosList[indexToDelete].id, "mp3"));
                DeleteItemFromWorld(audiosList[indexToDelete].id);
                audiosList.RemoveAt(indexToDelete);
                break;
            default:
                break;
        }

        CloseConfirmDelete();
        UpdateContentDisplay();
    }

    // Search all added items and remove based on given ID
    private void DeleteItemFromWorld(string id)
    {
        // Pass through all listing items in list of scene items and remove matching ones
        controller.RemoveAllFromItemListing(id);

        // Pass through all game objects in the scene and remove ones with matching name
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (gameObj.name == id)
            {
                Item itemComponent = gameObj.transform.parent.GetChild(0).gameObject.GetComponent<Item>();
                if (itemComponent != null)
                {
                    GameObject.Destroy(gameObj.transform.parent.gameObject);
                }
            }
        }
    }

    // Rest request to remove a specific item given it's ID 
    private IEnumerator DeleteItemAPI(string id, string type)
    {
        // Create JSON request
        string url = controller.apiLinks.deleteMedia;
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Content-Type", "application/json");
        JSONObject json = new JSONObject();

        // Add username field
        json.AddField("id", id);
        json.AddField("fileType", type);
        json.AddField("username", controller.username);
        json.AddField("token", controller.token);

        // Stringify
        string jsonStr = json.ToString();
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Send request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            if (request.downloadHandler != null)
                Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("File deleted.");
        }
    }

    // Open the item delete confirmation window
    public void OpenConfirmDelete()
    {
        confirmDeleteWindow.SetActive(true);
    }

    // Close the item delete confirmation window
    public void CloseConfirmDelete()
    {
        confirmDeleteWindow.SetActive(false);
    }

    // Show the "minimum char length" text for a period of time
    public IEnumerator TextSleepOff(string text)
    {
        minimumThreeCharacters.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
        minimumThreeCharacters.SetActive(true);
        yield return new WaitForSeconds(5);
        minimumThreeCharacters.SetActive(false);
    }


#if UNITY_WEBGL && !UNITY_EDITOR

    public void SetObjPath(){
        UploadFile(gameObject.name, "ObjCringe", ".obj", false);
    }

    public void SetMtlPath(){
        UploadFile(gameObject.name, "MtlCringe", ".mtl", false);
    }

    public void SetImgPath(){
        UploadFile(gameObject.name, "ImgCringe", ".jpg", false);
    }

    public void ObjCringe(string url){
        objPath = url;
    }

    public void MtlCringe(string url){
        mtlPath = url;
    }

    public void ImgCringe(string url){
        imgPath = url;
    }


#else

    public void SetObjPath()
    {
        string[] objPathTemp = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if (objPathTemp.Length > 0)
        {
            objPath = objPathTemp[0];
        }
    }

    public void SetMtlPath()
    {
        string[] mtlPathTemp = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mtl", false);
        if (mtlPathTemp.Length > 0)
        {
            mtlPath = mtlPathTemp[0];
        }
    }

    public void SetImgPath()
    {
        string[] imgPathTemp = StandaloneFileBrowser.OpenFilePanel("Open File", "", "jpg,png", false);
        if (imgPathTemp.Length > 0)
        {
            imgPath = imgPathTemp[0];
        }
    }

#endif

    public bool ValidToSubmitObj()
    {
        return (objPath.Length > 0 && mtlPath.Length > 0 && imgPath.Length > 0);
    }

}
