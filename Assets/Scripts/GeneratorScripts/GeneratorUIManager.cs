using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GeneratorUIManager allows to print the outcome of the generator on the UI
/// </summary>

public class GeneratorUIManager : Singleton<GeneratorUIManager>
{
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

        

        RectTransform contentRect = t.gameObject.GetComponent<RectTransform>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 tilePosition = new Vector3(-map.GetLength(0) / 2 + 0.5f + x, -map.GetLength(1) / 2 + 0.5f + y, 0)*100;
                GameObject newTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                RectTransform newTileRect = newTile.GetComponent<RectTransform>();

                newTileRect.localScale = Vector3.one * (1 - outlinePercent);// * (GetScale(Screen.width, Screen.height, new Vector2(1920,1080), .5f));
                Debug.Log(newTile.transform.localScale + "  "+ newTile.transform.lossyScale);
                newTileRect.SetParent(contentRect);
                newTileRect.anchoredPosition = tilePosition;
            }
        }

        contentRect.sizeDelta = new Vector2(map.GetLength(0)*100+ paddingContent, map.GetLength(1)*100+ paddingContent);
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
    }

    public void increaseScale(Transform t)
    {
        t.localScale = t.localScale * scaleFactor;
    }

    public void decreaseScale(Transform t)
    {
        t.localScale = t.localScale /(scaleFactor);
    }
}
