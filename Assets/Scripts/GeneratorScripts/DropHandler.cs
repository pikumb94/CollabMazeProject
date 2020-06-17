using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public bool droppable;
    private Rect rectCellContainer;
    private Color defaultColor;

    private MapListManager dicMaps;
    
    public void Start()
    {
        GridLayoutGroup GrLGr = GetComponentInChildren<GridLayoutGroup>();
        rectCellContainer = new Rect(Vector2.zero, GrLGr.cellSize);
        defaultColor = GetComponent<Image>().color;
        dicMaps = GetComponent<MapListManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (droppable && eventData.pointerDrag.name.Contains("DragNDrop"))
        {
            
            CanvasGroup canvasGroup = eventData.pointerDrag.GetComponent<CanvasGroup>();
            GameObject AliasElement = eventData.pointerDrag.transform.parent.parent.gameObject;
            //if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition)){
            if (eventData.pointerDrag.transform.parent.GetComponent<ScrollRect>() != null)
                eventData.pointerDrag.transform.parent.GetComponent<ScrollRect>().enabled = true;

            AliasElement.transform.SetParent(transform.GetChild(0).transform);

            //}
            if (!dicMaps.dictionaryMap.ContainsKey(AliasElement.GetInstanceID()))
            {
                dicMaps.addMapToDictionary(eventData.pointerDrag.GetComponent<DragHandler>().MapOnDrag.Value, AliasElement.GetInstanceID());
                
                eventData.pointerDrag.GetComponent<DragHandler>().OriginalParent = gameObject;
                //update statistics
                AliasGeneratorManager.Instance.deleteBestWorstUILines();
                AliasGeneratorManager.Instance.gameObject.GetComponent<AliasGameEvaluator>().AliasGameEvaluatorHandler();
            }
                
            Image i = GetComponent<Image>();
            //Color c = Color.white;
            //c.a = 0f;
            i.color = defaultColor;


            RectTransform contentRect = eventData.pointerDrag.transform.parent.Find("Content").GetComponent<RectTransform>();
            GeneratorUIManager.Instance.ScaleToFitContainer(contentRect, rectCellContainer);
        }
        else
        {
            //Debug.Log("ONDROP_Outsidearea");
        }
        
    }

}
