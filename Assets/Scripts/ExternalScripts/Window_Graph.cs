using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;

public class Window_Graph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    [SerializeField] private GameObject LineUIPrefab;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();

        gameObjectList = new List<GameObject>();
        /*
        //List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33, 50, 30, 60, 50, 40, 20, 5, 20, 10, 50, 30, 20, 11 };

        List<int> valueList = new List<int>() { 800,800,800,800,800,666,600,450,460,267,300,400,350,267,200,67,100,200,200,200,150,200,150,75 };
        List<float> valueListFloat = new List<float>();

            foreach (var v in valueList)
                valueListFloat.Add((float)v / 100);
        //ShowGraph(valueList, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
        ShowGraph(valueListFloat, -1, null,null);
        */
    }

    public void RemoveAllGraphs()
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
    }

    public void RefreshGraph(List<float> valueList, Color lineColor, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        RemoveAllGraphs();
        ShowGraph(valueList, lineColor, maxVisibleValueAmount, getAxisLabelX, getAxisLabelY);
    }

    public void ShowGraph(List<float> valueList, Color lineColor, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
        if (getAxisLabelX == null) {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null) {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        if (maxVisibleValueAmount <= 0) {
            maxVisibleValueAmount = valueList.Count;
        }
        
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];
        
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float value = valueList[i];
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0) {
            yDifference = 5f;
        }
        /*
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);
        */
        while (yMaximum % 10 != 0)
            yMaximum++;
        yMinimum = 0f; // Start the graph at zero

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;
        //
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        gameObjectList.Add(LineGO);
        LineGO.transform.SetParent(graphContainer, false);
        RectTransform lineRect = LineGO.GetComponent<RectTransform>();
        lineRect.anchorMax = Vector2.zero;
        lineRect.anchorMin = Vector2.zero;
        lineRect.pivot = Vector2.zero;
        UILineRenderer LineRenderer = LineGO.GetComponent<UILineRenderer>();
        LineRenderer.color = lineColor;
        //
        GameObject lastCircleGameObject = null;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);
            /*
            if (lastCircleGameObject != null) {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
                
            }*/
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -14f);
            labelX.GetComponent <TextMeshProUGUI>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, 0/*-3f*/);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
            //
            var pointlist = new List<Vector2>(LineRenderer.Points);
            pointlist.Add(new Vector2(xPosition, yPosition));
            LineRenderer.Points = pointlist.ToArray();
            //
        }

        int separatorCount = (int) yMaximum;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-14f, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0/*-4f*/, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    public void ShowGraphInBatch(List<Tuple<List<float>, Color>> LineBatch, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = LineBatch[0].Item1[0];
        int maxI = -1;
        for (int i = 0; i < LineBatch.Count; i++)
        {
            for (int j = 0; j < LineBatch[i].Item1.Count; j++)
            {
                float value = LineBatch[i].Item1[j];
                if (value > yMaximum)
                {
                    yMaximum = value;
                }

            }

            if (LineBatch[i].Item1.Count > maxVisibleValueAmount) { 
                maxVisibleValueAmount = LineBatch[i].Item1.Count;
                maxI = i;
            }
        }

        

        while (yMaximum % 10 != 0)
            yMaximum++;
        float yMinimum = 0f; // Start the graph at zero


        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;
        //
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        gameObjectList.Add(LineGO);
        LineGO.transform.SetParent(graphContainer, false);
        RectTransform lineRect = LineGO.GetComponent<RectTransform>();
        lineRect.anchorMax = Vector2.zero;
        lineRect.anchorMin = Vector2.zero;
        lineRect.pivot = Vector2.zero;
        UILineRenderer LineRenderer = LineGO.GetComponent<UILineRenderer>();
        LineRenderer.color = LineBatch[maxI].Item2;
        //
        GameObject lastCircleGameObject = null;

        for (int i = 0; i < LineBatch[maxI].Item1.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((LineBatch[maxI].Item1[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);
            /*
            if (lastCircleGameObject != null) {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
                
            }*/
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -14f);
            labelX.GetComponent<TextMeshProUGUI>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, 0/*-3f*/);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
            //
            var pointlist = new List<Vector2>(LineRenderer.Points);
            pointlist.Add(new Vector2(xPosition, yPosition));
            LineRenderer.Points = pointlist.ToArray();
            //
        }

        //write the rest of the lines
        for (int i = 0; i < LineBatch.Count && i != maxI; i++)
        {
            xIndex = 0;

            LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            gameObjectList.Add(LineGO);
            LineGO.transform.SetParent(graphContainer, false);
            lineRect = LineGO.GetComponent<RectTransform>();
            lineRect.anchorMax = Vector2.zero;
            lineRect.anchorMin = Vector2.zero;
            lineRect.pivot = Vector2.zero;
            LineRenderer = LineGO.GetComponent<UILineRenderer>();
            LineRenderer.color = LineBatch[i].Item2;

            for (int j = 0; j < LineBatch[i].Item1.Count; j++)
            {
                float xPosition = xSize + xIndex * xSize;
                float yPosition = ((LineBatch[i].Item1[j] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                gameObjectList.Add(circleGameObject);

                lastCircleGameObject = circleGameObject;

                xIndex++;
                //
                var pointlist = new List<Vector2>(LineRenderer.Points);
                pointlist.Add(new Vector2(xPosition, yPosition));
                LineRenderer.Points = pointlist.ToArray();
                //
            }
        }

        int separatorCount = (int)yMaximum;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-14f, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0/*-4f*/, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
