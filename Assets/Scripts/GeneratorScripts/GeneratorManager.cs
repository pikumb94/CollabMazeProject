using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// GeneratorManager coordinates both UI and logic of every kind of generator attached.
/// </summary>

public class GeneratorManager : MonoBehaviour
{
    public enum GeneratorEnum { CONNECTED, CELLULAR_AUTOMATA, PRIM };
    [HideInInspector]
    public GeneratorEnum activeGenerator = GeneratorEnum.CONNECTED;
    [HideInInspector]
    public IGenerator[] GeneratorsVect;

    
    [HideInInspector]
    public ITypeGrid.TypeGridEnum activeTypeGrid = ITypeGrid.TypeGridEnum.SQUARE;
    [HideInInspector]
    public ITypeGrid[] TypeGridVect;

    public ConnectedGenerator connectedGenerator;
    public CellularAutomataGenerator cellularAutomataGenerator;
    public PrimGenerator primGenerator;


    public SquareGrid squareGrid;

    [Header("Others")]
    public GameObject MapHolder;
    public String AssembledLevelSceneName;

    [HideInInspector]
    public GameObject Content;
    [HideInInspector]
    public bool isAutosolverOn=false;
    [HideInInspector]
    public bool inAliasGenerator = false;
    private DataMap dataMap;

    public static GeneratorManager Instance = null;
    protected GeneratorManager() {}

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(transform.gameObject);
    }

    private void InitGenMan()
    {
        Content = MapHolder.transform.Find("BorderMask/Content").gameObject;
        GeneratorsVect = new IGenerator[] { connectedGenerator, cellularAutomataGenerator, primGenerator };
        TypeGridVect = new ITypeGrid[] { squareGrid };

        connectedGenerator.TypeGrid = squareGrid;
        cellularAutomataGenerator.TypeGrid = squareGrid;
        primGenerator.TypeGrid = squareGrid;

        GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().InitUIPVC();
        GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().refreshUIParams();//IN SCENE LOADED!
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitGenMan();
    }

    public void generateButtonPressed()
    {
        GeneratorUIManager.Instance.disableGenerateButton();
        GeneratorUIManager.Instance.deleteMapOnUI(Content.transform);

        try
        {
            validateGeneratorParams(GeneratorsVect[(int)activeGenerator]);
            GeneratorsVect[(int)activeGenerator].TypeGrid = TypeGridVect[(int)activeTypeGrid];  //Typegrid change in generator
            TileObject[,] map = GeneratorsVect[(int)activeGenerator].initializeMap();

            GeneratorsVect[(int)activeGenerator].generateMap();
            GeneratorsVect[(int)activeGenerator].postprocessMap();
            

            if (GeneratorsVect[(int)activeGenerator].useRandomSeed)
                GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().refreshUIParams();

            dataMap = MapEvaluator.computeMetrics(GeneratorsVect[(int)activeGenerator].getMap(), GeneratorsVect[(int)activeGenerator].TypeGrid, GeneratorsVect[(int)activeGenerator].startPos, GeneratorsVect[(int)activeGenerator].endPos);
            DisplayMainMap((GeneratorUIManager.Instance.isTrapsOnMapBorderToggleOn() ? GeneratorsVect[(int)activeGenerator].getMapWTrapBorder() : GeneratorsVect[(int)activeGenerator].getMap()));
            GeneratorUIManager.Instance.showUIMapInfo(MapHolder.transform, dataMap, GeneratorsVect[(int)activeGenerator].TypeGrid);
        }
        catch (Exception e) {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message);
        }

        GeneratorUIManager.Instance.enableGenerateButton();
    }

    public void DisplayMainMap(TileObject[,] map)
    {
        GeneratorUIManager.Instance.deleteMapOnUI(Content.transform);
        GeneratorUIManager.Instance.DisplayMap(map, Content.transform, TypeGridVect[(int)activeTypeGrid]);
    }

    public UnityEngine.UI.Extensions.UILineRenderer[] DisplayMainMapKeepLines(TileObject[,] map)
    {
        UnityEngine.UI.Extensions.UILineRenderer[] Lines = GeneratorUIManager.Instance.deleteMapOnUIExceptLines(Content.transform);
        GeneratorUIManager.Instance.DisplayMap(map, Content.transform, TypeGridVect[(int)activeTypeGrid]);
        return Lines;
    }

    private void validateGeneratorParams(IGenerator g)
    {
        if (g.width <= 0 || g.height <= 0)
        {
            throw new Exception("Height or length set to zero or less.");
        }

        if(g.startPos == g.endPos)
        {
            throw new Exception("Start position match with end position.");
        }

        if (!g.in_bounds(g.startPos))
        {
            throw new Exception("Start position is outside of the grid.");
        }

        if (!g.in_bounds(g.endPos))
        {
            throw new Exception("End position is outside of the grid.");
        }
    }

    public void PlayButtonPressed()
    {
        GeneratorUIManager.Instance.savePlayParametersInManager();

        ParameterManager.Instance.GridType = GeneratorsVect[(int)activeGenerator].TypeGrid;
        ParameterManager.Instance.StartCell = GeneratorsVect[(int)activeGenerator].startPos;
        ParameterManager.Instance.EndCell = GeneratorsVect[(int)activeGenerator].endPos;

        if (!GeneratorManager.Instance.inAliasGenerator) {
            ParameterManager.Instance.MapToPlay = (GeneratorUIManager.Instance.isTrapsOnMapBorderToggleOn() ? GeneratorsVect[(int)activeGenerator].getMapWTrapBorder() : GeneratorsVect[(int)activeGenerator].getMap());
        }else
            ParameterManager.Instance.MapToPlay = (ParameterManager.Instance.IsTrapsOnMapBorder ? ParameterManager.Instance.MapToPlayWTrapBorder : ParameterManager.Instance.MapToPlay);

        ParameterManager.Instance.AliasMaps = (inAliasGenerator ? AliasGeneratorManager.Instance.AliasDragAreas[0].GetComponent<MapListManager>().getMapList() : AliasGeneratorManager.Instance.generateAliasOnTheFly());
        SceneManager.LoadScene(AssembledLevelSceneName);
    }
    
    
    public void BeforePlayButtonPressed(GameObject BeforePlayBox)
    {
        if (dataMap != null && Content.transform.childCount>0) { 
            if (dataMap.isTraversable)
            {
                BeforePlayBox.SetActive(true);
            }
            else
            {
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "No path from start to end. Generate another map or regenerate enabling the autosolver.");
            }
        }
        else
        {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Map not present. Generate a map first.");
        }
    }
    // Saves the map in a text file.
    public void SaveMapButtonPressed()
    {
        string textFilePath = Application.persistentDataPath;
        TileObject[,] map = GeneratorsVect[(int)activeGenerator].getMap();

        if (textFilePath == null && !Directory.Exists(textFilePath))
        {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while retrieving the folder, please insert a " + "valid path.");
        }
        else
        {
            try
            {
                string textMap = "";
                if (inAliasGenerator)
                {
                    for (int x = 0; x < map.GetLength(0); x++)
                    {
                        for (int y = 0; y < map.GetLength(1); y++)
                        {
                            textMap = textMap + map[y, map.GetLength(0) - 1 - x].type;
                        }
                        if (x < map.GetLength(1) - 1)
                        {
                            textMap = textMap + "\n";
                        }
                    }

                    Dictionary<int, StructuredAlias>.ValueCollection mapList = AliasGeneratorManager.Instance.AliasDragAreas[0].GetComponent<MapListManager>().getMapList();
                    textMap = textMap + "\n\n\n\n";

                    foreach (var m in mapList)
                    {
                        map = m.AliasMap;

                        for (int x = 0; x < map.GetLength(0); x++)
                        {
                            for (int y = 0; y < map.GetLength(1); y++)
                            {
                                textMap = textMap + map[y, map.GetLength(0) - 1 - x].type;
                            }
                            if (x < map.GetLength(1) - 1)
                            {
                                textMap = textMap + "\n";
                            }
                        }

                        textMap = textMap + "\n\n";
                    }
                }
                else
                {
                    for (int x = 0; x < map.GetLength(0); x++)
                    {
                        for (int y = 0; y < map.GetLength(1); y++)
                        {
                            textMap = textMap + map[y, map.GetLength(0) - 1 - x].type;
                        }
                        if (x < map.GetLength(1) - 1)
                        {
                            textMap = textMap + "\n";
                        }
                    }
                }
                

                string fileName = (inAliasGenerator?"Alias"+ParameterManager.Instance.rndSeed.ToString():GeneratorsVect[(int)activeGenerator].seed.ToString()) + "map.txt";
                File.WriteAllText(@textFilePath + "/" + fileName, textMap);

                GeneratorUIManager.Instance.showMessageDialogBox("Map \"" + fileName + "\" successfully saved at:\n" + textFilePath);
            }
            catch (Exception)
            {
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while saving the map at " + @textFilePath + ", please insert a valid path and check its permissions. ");
            }
        }


    }

    
}
