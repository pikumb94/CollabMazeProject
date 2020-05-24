using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        Invoke("DisplayDialogBox",1f);
    }

    public void OnMouseExitCallback()
    {
        isMouseHover = false;
        DisplayDialogBox();
    }

    public void DisplayDialogBox()
    {
        if (isMouseHover) {
            //DialogBoxInfo.SetActive(true);
            /*
            DialogBoxInfo.transform.position = gameObject.transform.position + new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);
            DialogBoxInfo.transform.SetParent(gameObject.transform);*/
            DialogBoxInfo.transform.position = gameObject.transform.position + new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);
            Vector3 overflowOffsets = Utility.GetGUIElementOffset(DialogBoxInfo.GetComponent<RectTransform>());
            DialogBoxInfo.transform.position = DialogBoxInfo.transform.position + overflowOffsets;
            DialogBoxInfo.transform.SetParent(TextComp.canvas.transform);
            DialogBoxInfo.transform.SetAsLastSibling();
        }

        else
        {
            //DialogBoxInfo.SetActive(false);
            DialogBoxInfo.transform.SetParent(TextComp.canvas.transform);
            DialogBoxInfo.transform.SetAsFirstSibling();
        }
            
    }
}
