using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public GameObject ToMoveGameObj;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [HideInInspector]
    public GameObject OriginalParent;
    private bool inDropArea=false;

    private void Awake()
    {
        rectTransform = ToMoveGameObj.GetComponent<RectTransform>();
        canvasGroup = ToMoveGameObj.GetComponent<CanvasGroup>();
        if(ToMoveGameObj.transform.parent != null)
            OriginalParent = ToMoveGameObj.transform.parent.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        inDropArea = false;

        foreach (RectTransform r in AliasGeneratorManager.Instance.AliasDragAreas)
        {
            Image i = r.GetComponent<Image>();
            if (Utility.rectOverlaps(rectTransform, r) && RectTransformUtility.RectangleContainsScreenPoint(r, Input.mousePosition)) {
                Color c = Color.white;
                c.a = .25f;
                i.color = c;
                inDropArea |= true;
            }
            else {
                Color c = Color.white;
                c.a = 0f;
                i.color = c;
            }
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!inDropArea)
            Destroy(ToMoveGameObj);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        rectTransform.GetComponentInChildren<ScrollRect>().enabled = false;
        ToMoveGameObj.transform.SetParent(canvas.transform);
    }

    public void OnBeginDrag(ScrollRect sr)
    {
        Debug.Log("OnBeginDrag");
        sr.enabled = false;
        ToMoveGameObj.transform.SetParent(canvas.transform);
    }

     
}
