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
    public float scaleFactorResizeButtons;
    
    public Button generateButton;
    //public CursorLoadingScript cursorLoadingScript;

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

        RectTransform prefabRect = g.TilePrefab.GetComponent<RectTransform>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 tilePosition = new Vector3((-map.GetLength(0) / 2 + displX + x)*g.offsetX * prefabRect.sizeDelta.x, ( -map.GetLength(1) / 2 + displY + y) * g.offsetY * prefabRect.sizeDelta.y, 0);

                GameObject newTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                applyTileColor(newTile,map[x,y].type);
                RectTransform newTileRect = newTile.GetComponent<RectTransform>();
                newTileRect.SetParent(contentRect);
                
                newTileRect.localScale = Vector3.one * (1 - outlinePercent);// * (GetScale(Screen.width, Screen.height, new Vector2(1920,1080), .5f));
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

    public void printCompositeMap(Transform t, ITypeGrid g, TileObject[,] map, int pixelOffset)
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
        GameObject sampleTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        Image tileImg = sampleTile.GetComponent<Image>();

        Texture2D prefabTexture = new Texture2D((int)tileImg.mainTexture.width,(int)tileImg.mainTexture.height, TextureFormat.ARGB32, false);
        Texture2D a = (Texture2D)tileImg.mainTexture;
        prefabTexture.SetPixels(a.GetPixels());
        Utility.TextureScale.Point(prefabTexture, prefabTexture.width / 4, prefabTexture.height / 4);

        //Texture2D prefabTexture = (Texture2D) tileImg.mainTexture;

        float displX = (map.GetLength(0) % 2 == 0 ? .5f : 0f);
        float displY = (map.GetLength(1) % 2 == 0 ? .5f : 0f);

        Texture2D compositeTexture = new Texture2D(map.GetLength(0)*(int)prefabTexture.width, map.GetLength(1) * (int)prefabTexture.height, TextureFormat.ARGB32, false);
        Color fillColor = Color.clear;
        Color[] fillPixels = new Color[compositeTexture.width * compositeTexture.height];

        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }


        compositeTexture.SetPixels(fillPixels);
        compositeTexture.Apply();

        Texture2D textureTileRoom = (Texture2D)prefabTexture;
        Texture2D textureTileWall = Utility.BlendOnWhite(textureTileRoom, wallColor);
        Texture2D textureTileStart = Utility.BlendOnWhite(textureTileRoom, startColor);
        Texture2D textureTileEnd = Utility.BlendOnWhite(textureTileRoom, endColor);


        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 tilePosition = new Vector3((/*-map.GetLength(0) / 2 + displX */+ x) * g.offsetX * prefabTexture.width, (/*-map.GetLength(1) / 2 + displY + */y) * g.offsetY * prefabTexture.height, 0);

                switch (map[x, y].type)
                {
                    case IGenerator.roomChar:
                        Utility.CompositeTopOnBottomAtPos(compositeTexture, textureTileRoom, new Vector2Int((int)tilePosition.x, (int)tilePosition.y));
                        break;
                    case IGenerator.wallChar:
                        Utility.CompositeTopOnBottomAtPos(compositeTexture, textureTileWall, new Vector2Int((int)tilePosition.x, (int)tilePosition.y));
                        break;
                    case IGenerator.startChar:
                        Utility.CompositeTopOnBottomAtPos(compositeTexture, textureTileStart, new Vector2Int((int)tilePosition.x, (int)tilePosition.y));
                        break;
                    case IGenerator.endChar:
                        Utility.CompositeTopOnBottomAtPos(compositeTexture, textureTileEnd, new Vector2Int((int)tilePosition.x, (int)tilePosition.y));
                        break;
                    default:
                        ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Room type does not match with any of the predefined types.");
                        break;
                }

                
            }
        }

        GameObject NewObj = new GameObject(); //Create the GameObject
        Image NewImage = NewObj.AddComponent<Image>(); //Add the Image Component script
        NewImage.sprite = Sprite.Create(compositeTexture, new Rect(0, 0, compositeTexture.width, compositeTexture.height), Vector2.zero);//Set the Sprite of the Image Component on the new GameObject
        NewObj.GetComponent<RectTransform>().SetParent(contentRect); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        NewObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        NewObj.SetActive(true); //Activate the GameObject

        //contentRect.sizeDelta = new Vector2(map.GetLength(0) * prefabRect.sizeDelta.x, map.GetLength(1) * prefabRect.sizeDelta.y);
        contentRect.sizeDelta = NewObj.GetComponent<RectTransform>().sizeDelta;

        while (contentRect.sizeDelta.x * contentRect.localScale.x < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height)
            increaseScale(contentRect);

        while (contentRect.sizeDelta.x * contentRect.localScale.x > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height)
            decreaseScale(contentRect);

        t.parent.Find("../PlusButton").gameObject.SetActive(true);
        t.parent.Find("../MinusButton").gameObject.SetActive(true);

        GameObject.Destroy(sampleTile);
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
    }

    public void increaseScale(RectTransform t)
    {
        //if(t.sizeDelta.x*t.localScale.x < t.parent.gameObject.GetComponent<RectTransform>().rect.width)
            t.localScale = t.localScale * scaleFactorResizeButtons;
    }

    public void decreaseScale(RectTransform t)
    {
        //if (t.sizeDelta.x * t.localScale.x > t.parent.gameObject.GetComponent<RectTransform>().rect.width)
            t.localScale = t.localScale /(scaleFactorResizeButtons);
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

    public void disableGenerateButton()
    {
        generateButton.interactable = false;
        //cursorLoadingScript.enableCursor(true);
        //Cursor.visible = false;
        
    }

    public void enableGenerateButton()
    {
        //Cursor.visible = true;
        //cursorLoadingScript.enableCursor(false);
        generateButton.interactable = true;
    }
}
