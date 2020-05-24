using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverDisplayText : MonoBehaviour
{
    public GameObject DialogBoxInfo;
    [TextArea]
    public string textToDisplay="";
    bool isMouseHover = false;
    TextMeshProUGUI TextComp;
    RectTransform BoxRect;

    void Start()
    {
        TextComp = DialogBoxInfo.GetComponentInChildren<TextMeshProUGUI>();
        BoxRect = DialogBoxInfo.GetComponent<RectTransform>();

    }

    public void OnMouseEnterCallback()
    {
        /*
        if(TextComp.canvas.transform.GetChild(TextComp.canvas.transform.childCount-1).gameObject != DialogBoxInfo.transform)//if dialog box is not displayed
        {
            isMouseHover = true;

            if (textToDisplay == "")
                TextComp.text = GeneratorManager.Instance.GeneratorsVect[(int)GeneratorManager.Instance.activeGenerator].InfoGenerator;
            else
                TextComp.text = textToDisplay;

            RectTransform RectTxtCmp = TextComp.gameObject.GetComponent<RectTransform>();
            if (TextComp.preferredHeight > RectTxtCmp.sizeDelta.y)
                RectTxtCmp.sizeDelta = new Vector2(RectTxtCmp.sizeDelta.x, TextComp.preferredHeight);// TextComp.renderedWidth
            if (TextComp.preferredHeight < RectTxtCmp.sizeDelta.y)
                RectTxtCmp.sizeDelta = new Vector2(RectTxtCmp.sizeDelta.x, BoxRect.sizeDelta.y);
            DialogBoxInfo.transform.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;

            Invoke("DisplayDialogBox", 1f);
        }
        else
        {
            isMouseHover = false;
            DisplayDialogBox();
        }*/
        
        isMouseHover = true;

        if (textToDisplay == "")
            TextComp.text = GeneratorManager.Instance.GeneratorsVect[(int)GeneratorManager.Instance.activeGenerator].InfoGenerator;
        else
            TextComp.text = textToDisplay;

        RectTransform RectTxtCmp = TextComp.gameObject.GetComponent<RectTransform>();
        if (TextComp.preferredHeight > RectTxtCmp.sizeDelta.y)
            RectTxtCmp.sizeDelta = new Vector2(RectTxtCmp.sizeDelta.x, TextComp.preferredHeight);// TextComp.renderedWidth
        if (TextComp.preferredHeight < RectTxtCmp.sizeDelta.y)
            RectTxtCmp.sizeDelta = new Vector2(RectTxtCmp.sizeDelta.x, BoxRect.sizeDelta.y);
        DialogBoxInfo.transform.Find("Scrollbar").GetComponent<Scrollbar>().value = 1;

        Invoke("DisplayDialogBox", 1f);
    }

    public void OnMouseExitCallback()
    {

        isMouseHover = false;
        DisplayDialogBox();


    }

    public void DisplayDialogBox()
    {
        if (isMouseHover) {
            /*
            DialogBoxInfo.transform.position = gameObject.transform.position + new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);*/
            DialogBoxInfo.transform.parent.SetParent(gameObject.transform);

            Vector3 mousePos = Input.mousePosition;
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(TextComp.canvas.transform as RectTransform,Input.mousePosition, TextComp.canvas.worldCamera,out mousePos);

            DialogBoxInfo.transform.position = mousePos+ new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);// + new Vector3(-2,2,0);//gameObject.transform.position; /*new Vector3(pos.x,pos.y,0)+*/ // new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);
            Vector3 overflowOffsets = Utility.GetGUIElementOffset(DialogBoxInfo.GetComponent<RectTransform>());
            DialogBoxInfo.transform.position = DialogBoxInfo.transform.position + overflowOffsets;
            //DialogBoxInfo.transform.SetParent(TextComp.canvas.transform);
            //DialogBoxInfo.transform.SetAsLastSibling();
            DialogBoxInfo.transform.parent.GetComponent<Canvas>().sortingOrder = 1;
        }

        else
        {
            //DialogBoxInfo.transform.SetParent(TextComp.canvas.transform);
            //DialogBoxInfo.transform.SetAsFirstSibling();
            DialogBoxInfo.transform.parent.GetComponent<Canvas>().sortingOrder = -1;
        }
            
    }

    private IEnumerator countdownTimer(float seconds)
    {
        float duration = seconds;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            if (!isMouseHover)
                break;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
            
        }

        if (normalizedTime >= 1)
            isMouseHover = true;
        else
            isMouseHover = false;

        DisplayDialogBox();
    }

}
