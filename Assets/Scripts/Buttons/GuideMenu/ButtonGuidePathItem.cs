using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGuidePathItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Controller controller;
    private GuideMenu guideMenu;

    public GameObject associatedObject;
    public GameObject parentObjectToFollow;

    public GameObject nameDisplay;
    public string itemId;

    public bool selected;

    public Color baseColor = new Color(1f, 1f, 1f, 1);
    public Color hoverColor = new Color(.7f, .82f, .92f, 1);
    public Color selectedColor = new Color(.4f, .58f, .94f, 1);

    public int indexInPath;

    public GameObject upArrow;
    public GameObject downArrow;

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        guideMenu = controller.ui.GetComponent<GuideMenu>();

        // Increase size of list if needed
        if (guideMenu.gridContentGuidePath.transform.childCount > 7)
        {
            Vector2 newSize = guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta;
            newSize.y += 45;
            guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta = newSize;
        }

        // UpdateArrows(true);
    }

    public void UpdateArrows(bool sleepYes)
    {
        if (sleepYes)
            StartCoroutine(WaitThenUpdateArrows());
        else
        {
            if (guideMenu.pathList.Count == 1)
            {
                upArrow.SetActive(false);
                downArrow.SetActive(false);
            }
            else if (indexInPath == 0)
            {
                upArrow.SetActive(false);
                downArrow.SetActive(true);
            }
            else if (indexInPath == guideMenu.pathList.Count - 1)
            {
                downArrow.SetActive(false);
                upArrow.SetActive(true);
            }
            else
            {
                upArrow.SetActive(true);
                downArrow.SetActive(true);
            }
        }
    }

    private IEnumerator WaitThenUpdateArrows()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if (guideMenu.pathList.Count == 1)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(false);
        }
        else if (indexInPath == 0)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(true);
        }
        else if (indexInPath == guideMenu.pathList.Count - 1)
        {
            downArrow.SetActive(false);
            upArrow.SetActive(true);
        }
        else
        {
            upArrow.SetActive(true);
            downArrow.SetActive(true);
        }
    }

    // Used to set the display of the item in the listing of items
    public void SetItemName(string itemNameDisplay)
    {
        nameDisplay.GetComponent<TextMeshProUGUI>().text = itemNameDisplay;
    }

    public void SetItemId(string i)
    {
        itemId = i;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // guideMenu.AddToPath();
        // this.gameObject.GetComponent<Image>().color = selectedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = baseColor;
    }

    public void RemoveItemFromPath()
    {
        guideMenu.RemoveItemFromPath(indexInPath, true);

        // Decrease size of list
        if (guideMenu.gridContentGuidePath.transform.childCount >= 7)
        {
            Vector2 newSize = guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta;
            newSize.y -= 45;
            guideMenu.gridContentGuidePath.GetComponent<RectTransform>().sizeDelta = newSize;
        }

        GameObject.Destroy(this.gameObject);
    }

    public void MoveItemUpInPath()
    {
        guideMenu.MoveItemUpInPath(indexInPath);

    }

    public void MoveItemDownInPath()
    {
        guideMenu.MoveItemDownInPath(indexInPath);

    }
}
