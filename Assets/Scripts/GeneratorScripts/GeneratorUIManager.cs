using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using System;
/// <summary>
/// GeneratorUIManager allows to print the outcome of the generator on the UI
/// </summary>

public class GeneratorUIManager : Singleton<GeneratorUIManager>
{

    public Color roomColor;
    public Color wallColor;
    public Color startColor;
    public Color endColor;

    public Color correctMessageColor;
    public Color errorMessageColor;

    [Range(0, 1)]
    public float outlinePercent;
    public int paddingContent;
    public float scaleFactorResizeButtons;
    public int thresholdToCompositeImage = 625;

    public Button generateButton;
    public GameObject ErrorDialogBox;
    public GameObject MessageDialogBox;
    public GameObject BeforePlayDialogBox;
    //public CursorLoadingScript cursorLoadingScript;

    public Toggle TrapsOnMapBorderToggle;
    public GameObject LineUIPrefab;

    private Vector2 originUIMap;

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

        t.localScale = Vector3.one;

        RectTransform contentRect = t.gameObject.GetComponent<RectTransform>();

        float displX = (map.GetLength(0) % 2 == 0 ? .5f : 0f);
        float displY = (map.GetLength(1) % 2 == 0 ? .5f : 0f);

        RectTransform prefabRect = g.TilePrefab.GetComponent<RectTransform>();

        

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector3 tilePosition = new Vector3((-map.GetLength(0) / 2 + displX + x) * g.offsetX * prefabRect.sizeDelta.x, (-map.GetLength(1) / 2 + displY + y) * g.offsetY * prefabRect.sizeDelta.y, 0);

                GameObject newTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                applyTileColor(newTile, map[x, y].type);
                RectTransform newTileRect = newTile.GetComponent<RectTransform>();
                newTileRect.SetParent(contentRect);

                newTileRect.localScale = Vector3.one * (1 - outlinePercent);// * (GetScale(Screen.width, Screen.height, new Vector2(1920,1080), .5f));
                newTileRect.anchoredPosition = tilePosition;

                if (map[x, y].type == IGenerator.startChar)
                    originUIMap = new Vector2(tilePosition.x, tilePosition.y);
            }
        }

        ScaleToFitContainerWPadding(contentRect, map, g);

    }

    public void printCompositeMap(Transform t, ITypeGrid g, TileObject[,] map, int pixelOffset)
    {

        t.localScale = Vector3.one;

        RectTransform contentRect = t.gameObject.GetComponent<RectTransform>();
        GameObject sampleTile = Instantiate(g.TilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        Image tileImg = sampleTile.GetComponent<Image>();

        Texture2D prefabTexture = new Texture2D((int)tileImg.mainTexture.width, (int)tileImg.mainTexture.height, TextureFormat.ARGB32, false);
        Texture2D a = (Texture2D)tileImg.mainTexture;
        prefabTexture.SetPixels(a.GetPixels());
        Utility.TextureScale.Point(prefabTexture, prefabTexture.width / 4, prefabTexture.height / 4);

        //Texture2D prefabTexture = (Texture2D) tileImg.mainTexture;

        float displX = (map.GetLength(0) % 2 == 0 ? .5f : 0f);
        float displY = (map.GetLength(1) % 2 == 0 ? .5f : 0f);

        Texture2D compositeTexture = new Texture2D(map.GetLength(0) * (int)prefabTexture.width, map.GetLength(1) * (int)prefabTexture.height, TextureFormat.ARGB32, false);
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
                Vector3 tilePosition = new Vector3((/*-map.GetLength(0) / 2 + displX */+x) * g.offsetX * prefabTexture.width, (/*-map.GetLength(1) / 2 + displY + */y) * g.offsetY * prefabTexture.height, 0);

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
                        originUIMap = new Vector2(tilePosition.x, tilePosition.y);
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

        ScaleToFitContainer(contentRect);

        GameObject.Destroy(sampleTile);
    }

    private float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
    {
        return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
               Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
    }

    public void ScaleToFitContainerWPadding(RectTransform contentRect, TileObject[,] map, ITypeGrid tG)
    {
        RectTransform prefabRect = tG.TilePrefab.GetComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.sizeDelta = new Vector2(map.GetLength(0) * prefabRect.sizeDelta.x + paddingContent, map.GetLength(1) * prefabRect.sizeDelta.y + paddingContent);

        ScaleToFitContainer(contentRect);
    }

    public void ScaleToFitContainer(RectTransform contentRect)
    {
        bool hasDecreased = false;
        while (contentRect.sizeDelta.x * contentRect.localScale.x > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y > contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height) {

            decreaseScale(contentRect);
            hasDecreased = true;
        }
        if (hasDecreased)
            return;

        while (contentRect.sizeDelta.x * contentRect.localScale.x < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y < contentRect.parent.gameObject.GetComponent<RectTransform>().rect.height)
            increaseScale(contentRect);

        decreaseScale(contentRect);
    }

    public void ScaleToFitContainer(RectTransform contentRect, Rect containerRect)
    {
        bool hasDecreased = false;
        while (contentRect.sizeDelta.x * contentRect.localScale.x > containerRect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y > containerRect.height)
        {

            decreaseScale(contentRect);
            hasDecreased = true;
        }
        if (hasDecreased)
            return;

        while (contentRect.sizeDelta.x * contentRect.localScale.x < containerRect.width ||
            contentRect.sizeDelta.y * contentRect.localScale.y < containerRect.height)
            increaseScale(contentRect);

        decreaseScale(contentRect);
    }

    public void increaseScale(RectTransform t)
    {
        //if(t.sizeDelta.x*t.localScale.x < t.parent.gameObject.GetComponent<RectTransform>().rect.width)
        t.localScale = t.localScale * scaleFactorResizeButtons;
    }

    public void decreaseScale(RectTransform t)
    {
        //if (t.sizeDelta.x * t.localScale.x > t.parent.gameObject.GetComponent<RectTransform>().rect.width)
        t.localScale = t.localScale / (scaleFactorResizeButtons);
    }

    public void applyTileColor(GameObject tileGO, char roomType)
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

    public void showUIMapInfo(Transform mapHolderTransform, DataMap DataM, ITypeGrid t)
    {
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Utility.displaySegmentedLineUI(LineGO, mapHolderTransform.Find("BorderMask/Content").GetComponent<RectTransform>(), DataM.solutionSteps, originUIMap, t.TilePrefab.GetComponent<RectTransform>().sizeDelta.x);

        mapHolderTransform.Find("SaveButton").gameObject.SetActive(true);
        mapHolderTransform.Find("PlusButton").gameObject.SetActive(true);
        mapHolderTransform.Find("MinusButton").gameObject.SetActive(true);
        GameObject DialogBoxGO = mapHolderTransform.Find("BorderMask/MapDataBox").gameObject;
        Animator animatorDB = DialogBoxGO.GetComponent<Animator>();

        TextMeshProUGUI textDataDB = mapHolderTransform.Find("BorderMask/MapDataBox/DataDialogBox").gameObject.GetComponent<TextMeshProUGUI>();
        textDataDB.text = MapEvaluator.aggregateDataMap(DataM);

        if (DataM.isTraversable) {
            mapHolderTransform.GetComponent<Image>().color = correctMessageColor;
            DialogBoxGO.GetComponent<Image>().color = correctMessageColor;

            if (animatorDB != null)
            {
                animatorDB.SetBool("Show",true);
                animatorDB.SetBool("Error", false);
            }
            else
            {
                throw new Exception("Animator is missing on "+ DialogBoxGO .name+ " Game Object.");
            }
            
        }
        else
        {
            mapHolderTransform.GetComponent<Image>().color = errorMessageColor;
            DialogBoxGO.GetComponent<Image>().color = errorMessageColor;

            if (animatorDB != null)
            {
                animatorDB.SetBool("Show", true);
                animatorDB.SetBool("Error", true);
            }
            else
            {
                throw new Exception("Animator is missing on " + DialogBoxGO.name + " Game Object.");
            }
        }
    }

    public void hideUIMapInfo(Transform mapHolderTransform)
    {
        mapHolderTransform.Find("SaveButton").gameObject.SetActive(false);
        mapHolderTransform.Find("PlusButton").gameObject.SetActive(false);
        mapHolderTransform.Find("MinusButton").gameObject.SetActive(false);
        GameObject DialogBoxGO = mapHolderTransform.Find("BorderMask/MapDataBox").gameObject;
        Animator animatorDB = DialogBoxGO.GetComponent<Animator>();

        mapHolderTransform.GetComponent<Image>().color = correctMessageColor;
        DialogBoxGO.GetComponent<Image>().color = correctMessageColor;

        if (animatorDB != null)
        {
            animatorDB.SetBool("Show", false);
        }
        else
        {
            throw new Exception("Animator is missing on " + DialogBoxGO.name + " Game Object.");
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

    public void deleteMapOnUI(Transform t)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public UILineRenderer[] deleteMapOnUIExceptLines(Transform t)
    {
        List<UILineRenderer> listLines = new List<UILineRenderer>();
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                UILineRenderer line = child.GetComponent<UILineRenderer>();
                if (line == null)
                    GameObject.Destroy(child.gameObject);
                else
                    listLines.Add(line);
            }
        }
        return listLines.ToArray();
    }

    public void showDialogErrorBox(string s)
    {
        ErrorDialogBox.SetActive(true);
        ErrorDialogBox.transform.Find("ErrorMessage").GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void showMessageDialogBox(string s)
    {
        MessageDialogBox.SetActive(true);
        MessageDialogBox.transform.Find("MessageDialog").GetComponent<TMPro.TextMeshProUGUI>().text = s;
    }

    public void savePlayParametersInManager()
    {
        TMP_InputField[] ParamsIF = BeforePlayDialogBox.GetComponentsInChildren<TMP_InputField>();
        Toggle areObsTrav = BeforePlayDialogBox.GetComponentInChildren<Toggle>();

        ParameterManager.Instance.countdownSecondsParam = Int32.Parse(ParamsIF[0].text);
        ParameterManager.Instance.penaltySecondsParam = Int32.Parse(ParamsIF[1].text);
        ParameterManager.Instance.areObstacleTraversableParam = areObsTrav.isOn;

    }

    public void DisplayMap(TileObject[,] map,Transform container, ITypeGrid gridType)
    {
        if (map.GetLength(0) * map.GetLength(1) > thresholdToCompositeImage)
            printCompositeMap(container, gridType, map, 0);
        else
            printMap(container, gridType, map);
    }

    public void trapsOnMapBorderHandler(Toggle t)
    {
        GeneratorManager GM = GeneratorManager.Instance;
        UILineRenderer[] Lines;
        if (t.isOn)
        {
            Lines=GM.DisplayMainMapKeepLines(GM.GeneratorsVect[(int)GM.activeGenerator].getMapWTrapBorder());
        }
        else
        {
            Lines=GM.DisplayMainMapKeepLines(GM.GeneratorsVect[(int)GM.activeGenerator].getMap());
        }

        foreach (UILineRenderer l in Lines)
            l.transform.SetAsLastSibling();
    }

    public bool isTrapsOnMapBorderToggleOn()
    {
        return TrapsOnMapBorderToggle.isOn;
    }

    public bool toggleUIAnimator(Animator a)
    {
        bool isOn = a.GetBool("isOn");
        isOn = !isOn;
        a.SetBool("isOn", isOn);
        return isOn;
    }

    public void loadAliasParamsInParamManager(GameObject ParamsContainer)
    {
        GeneratorManager genM = GeneratorManager.Instance;
        ParameterManager p = ParameterManager.Instance;
        TMP_InputField[] InpFields = ParamsContainer.GetComponentsInChildren<TMP_InputField>();
        Toggle t = ParamsContainer.GetComponentInChildren<Toggle>();
        TMP_Dropdown DropD = ParamsContainer.GetComponentInChildren<TMP_Dropdown>();

        //Save map params
        p.MapToPlay = (GeneratorUIManager.Instance.isTrapsOnMapBorderToggleOn() ? genM.GeneratorsVect[(int)genM.activeGenerator].getMapWTrapBorder() : genM.GeneratorsVect[(int)genM.activeGenerator].getMap());
        p.GridType = genM.GeneratorsVect[(int)genM.activeGenerator].TypeGrid;
        p.StartCell = genM.GeneratorsVect[(int)genM.activeGenerator].startPos;
        p.EndCell = genM.GeneratorsVect[(int)genM.activeGenerator].endPos;

        //Save alias generation params
        p.aliasNum = Int32.Parse(InpFields[0].text);
        p.minStepsSolution = Int32.Parse(InpFields[1].text);
        p.maxStepsSolution = Int32.Parse(InpFields[2].text);
        p.allowAutosolverForAlias = t.isOn;
        if (DropD.value == 0)
        {
            p.considerSimilar = true;
            p.considerNovelty = false;
        }
        else if (DropD.value == 1)
        {
            p.considerSimilar = false;
            p.considerNovelty = true;
        }
        else if (DropD.value >= 2)
        {
            p.considerSimilar = true;
            p.considerNovelty = true;
        }

    }
}
