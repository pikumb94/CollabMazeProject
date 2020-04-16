using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GeneratorUIManager allows to print the outcome of the generator on the UI
/// </summary>

public class GeneratorUIManager : Singleton<GeneratorUIManager>
{
    public Color roomColor;
    public Color wallColor;
    public Color startColor;
    public Color endColor;

    [Range(0, 1)]
    public float outlinePercent;
    public int paddingContent;
    public float scaleFactor;

    protected GeneratorUIManager() { }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void printMap(Transform t, ITypeGrid g, TileObject[,] map)
    {

        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        t.localScale = Vector3.one;

        RectTransform contentRect = t.gameObject.GetComponent<RectTransform>();

        float displX = (map.GetLength(0) % 2 == 0 ? .5f : 0f);
        float displY = (map.GetLength(1) % 2 == 0 ? .5f : 0f);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 tilePosition = new Vector3(-map.GetLength(0) / 2 + displX + x, -map.GetLength(1) / 2 + displY + y, 0)*100;
                GameObject newTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                applyTileColor(newTile,map[x,y].type);
                RectTransform newTileRect = newTile.GetComponent<RectTransform>();
                newTileRect.localScale = Vector3.one * (1 - outlinePercent);// * (GetScale(Screen.width, Screen.height, new Vector2(1920,1080), .5f));
                newTileRect.SetParent(contentRect);
                newTileRect.anchoredPosition = tilePosition;
            }
        }

        contentRect.sizeDelta = new Vector2(map.GetLength(0)*100+ paddingContent, map.GetLength(1)*100+ paddingContent);

        while (contentRect.sizeDelta.x * contentRect.localScale.x < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height)
            increaseScale(contentRect);

        while (contentRect.sizeDelta.x * contentRect.localScale.x > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height)
            decreaseScale(contentRect);

        t.parent.Find("../PlusButton").gameObject.SetActive(true);
        t.parent.Find("../MinusButton").gameObject.SetActive(true);
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
    }

    public void increaseScale(RectTransform t)
    {
        //if(t.sizeDelta.x*t.localScale.x < t.parent.gameObject.GetComponent<RectTransform>().rect.width)
            t.localScale = t.localScale * scaleFactor;
    }

    public void decreaseScale(RectTransform t)
    {
        //if (t.sizeDelta.x * t.localScale.x > t.parent.gameObject.GetComponent<RectTransform>().rect.width)
            t.localScale = t.localScale /(scaleFactor);
    }

    private void applyTileColor(GameObject tileGO, char roomType)
    {
        switch (roomType)
        {
            case IGenerator.roomChar:
                tileGO.GetComponent<Image>().color = roomColor;
                break;
            case IGenerator.wallChar:
                tileGO.GetComponent<Image>().color = wallColor;
                break;
            case IGenerator.startChar:
                tileGO.GetComponent<Image>().color = startColor;
                break;
            case IGenerator.endChar:
                tileGO.GetComponent<Image>().color = endColor;
                break;
            default:
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Room type does not match with any of the predefined types.");
                break;
        }
    }
}
