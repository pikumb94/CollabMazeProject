﻿using System.Collections;
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
    private Transform originalParent;
    void Start()
    {
        if(DialogBoxInfo!= null)
        {
            TextComp = DialogBoxInfo.GetComponentInChildren<TextMeshProUGUI>();
            BoxRect = DialogBoxInfo.GetComponent<RectTransform>();
        }
        originalParent = DialogBoxInfo.transform.parent.parent;
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

            DialogBoxInfo.transform.parent.SetParent(gameObject.transform);

            Vector3 mousePos = this.transform.position;

            DialogBoxInfo.transform.position = mousePos+ new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);// + new Vector3(-2,2,0);//gameObject.transform.position; /*new Vector3(pos.x,pos.y,0)+*/ // new Vector3(BoxRect.sizeDelta.x / 2, -BoxRect.sizeDelta.y / 2, 0);
            Vector3 overflowOffsets = Utility.GetGUIElementOffset(DialogBoxInfo.GetComponent<RectTransform>());
            DialogBoxInfo.transform.position = DialogBoxInfo.transform.position + overflowOffsets;

            DialogBoxInfo.transform.parent.GetComponent<Canvas>().sortingOrder = 1;
        }

        else
        {
            DialogBoxInfo.transform.parent.SetParent(originalParent);
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
