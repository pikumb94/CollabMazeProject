using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    public Canvas canvas;
    public GameObject ToMoveGameObj;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [HideInInspector]
    public GameObject OriginalParent;
    private bool inDropArea=false;
    [HideInInspector]
    public Color defaultColor;

    [HideInInspector]
    public KeyValuePair<int, StructuredAlias> MapOnDrag;

    private bool hasBeenDragged = false;

    private void Awake()
    {
        rectTransform = ToMoveGameObj.GetComponent<RectTransform>();
        canvasGroup = ToMoveGameObj.GetComponent<CanvasGroup>();

        //defaultColor = AliasGeneratorManager.Instance.AliasDragAreas[0].GetComponent<Image>().color;//default color of map content is the first of the list in alias manager
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        hasBeenDragged = true;
        MapOnDrag = OriginalParent.GetComponent<MapListManager>().removeMapFromDictionary(ToMoveGameObj.GetInstanceID());
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
            if (r.GetComponent<DropHandler>().droppable && Utility.rectOverlaps(rectTransform, r) && RectTransformUtility.RectangleContainsScreenPoint(r, Input.mousePosition))
            {
                Color c = Color.white;
                c.a = .25f;
                i.color = c;
                inDropArea |= true;
            }
            else
            {
                //Color c = Color.white;
                //c.a = 0f;
                i.color = defaultColor;
            }

            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!inDropArea) {
            Destroy(ToMoveGameObj);
            //update statistics
            AliasGeneratorManager.Instance.deleteBestWorstUILines();
            AliasGeneratorManager.Instance.gameObject.GetComponent<AliasGameEvaluator>().AliasGameEvaluatorHandler();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        hasBeenDragged = false;
        rectTransform.GetComponentInChildren<ScrollRect>().enabled = false;
        ToMoveGameObj.transform.SetParent(canvas.transform);
    }

    public void OnBeginDrag(ScrollRect sr)
    {
        Debug.Log("OnBeginDragSR");
        sr.enabled = false;
        ToMoveGameObj.transform.SetParent(canvas.transform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUP");
        if (!hasBeenDragged)
        {
            ToMoveGameObj.transform.SetParent(OriginalParent.transform.Find("AliasContent"));
            rectTransform.GetComponentInChildren<ScrollRect>().enabled = true;
        }
    }
}
