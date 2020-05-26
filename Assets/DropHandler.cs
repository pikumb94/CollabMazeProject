using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        Debug.Log("ONDROP");
        CanvasGroup canvasGroup = eventData.pointerDrag.GetComponent<CanvasGroup>();

        if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
        {
            eventData.pointerDrag.transform.parent.GetComponent<ScrollRect>().enabled = true;
            eventData.pointerDrag.transform.parent.parent.SetParent(transform);
        }

        Image i = GetComponent<Image>();
        Color c = Color.white;
        c.a = 0f;
        i.color = c;
    }
    /*
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("ONDROP");

        CanvasGroup canvasGroup = eventData.pointerDrag.GetComponent<CanvasGroup>();

        if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
        {
            eventData.pointerDrag.GetComponentInChildren<ScrollRect>().enabled = true;
            eventData.pointerDrag.transform.SetParent(transform);
        }
    }*/
}
