using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IDropHandler
{
    private Rect rectCellContainer;
    public void Awake()
    {
        GridLayoutGroup GrLGr = GetComponent<GridLayoutGroup>();
        rectCellContainer = new Rect(Vector2.zero, GrLGr.cellSize);
    }
    public void OnDrop(PointerEventData eventData)
    {

        Debug.Log("ONDROP");
        CanvasGroup canvasGroup = eventData.pointerDrag.GetComponent<CanvasGroup>();

        if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
        {
            if(eventData.pointerDrag.transform.parent.GetComponent<ScrollRect>()!=null)
                eventData.pointerDrag.transform.parent.GetComponent<ScrollRect>().enabled = true;
            eventData.pointerDrag.transform.parent.parent.SetParent(transform);
            
        }

        Image i = GetComponent<Image>();
        Color c = Color.white;
        c.a = 0f;
        i.color = c;


        RectTransform contentRect = eventData.pointerDrag.transform.parent.Find("Content").GetComponent<RectTransform>();
        GeneratorUIManager.Instance.ScaleToFitContainer(contentRect, rectCellContainer);
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
