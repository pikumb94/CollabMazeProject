using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// GeneratorManager coordinates both UI and logic of every kind of generator attached.
/// </summary>

public class GeneratorManager : Singleton<GeneratorManager>
{
    public enum GeneratorEnum { CONNECTED, CELLULAR_AUTOMATA, PRIM, PRIM_CONNECTED };
    [HideInInspector]
    public GeneratorEnum activeGenerator = GeneratorEnum.CONNECTED;
    [HideInInspector]
    public IGenerator[] GeneratorsVect;

    public enum TypeGridEnum { SQUARE, HEXAGON, TRIANGLE };
    [HideInInspector]
    public TypeGridEnum activeTypeGrid = TypeGridEnum.SQUARE;
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
    private TileObject[,] tmpMapWBorder;
    protected GeneratorManager() {}

    void Start()
    {
        Content = MapHolder.transform.Find("BorderMask/Content").gameObject;
        GeneratorsVect = new IGenerator[] { connectedGenerator, cellularAutomataGenerator, primGenerator };
        TypeGridVect = new ITypeGrid[] { squareGrid};
        GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().refreshUIParams();
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
            /*
            if(map.GetLength(0)*map.GetLength(1)>625)
                GeneratorUIManager.Instance.printCompositeMap(Content.transform, TypeGridVect[(int)activeTypeGrid], GeneratorsVect[(int)activeGenerator].generateMap(),0);
            else
                GeneratorUIManager.Instance.printMap(Content.transform, TypeGridVect[(int)activeTypeGrid], GeneratorsVect[(int)activeGenerator].generateMap());
            */
            GeneratorsVect[(int)activeGenerator].generateMap();
            GeneratorsVect[(int)activeGenerator].postprocessMap();
            tmpMapWBorder = null; //since there's a new map, free the version with borders

            if (GeneratorsVect[(int)activeGenerator].useRandomSeed)
                GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().refreshUIParams();

            DataMap dataMap = MapEvaluator.computeMetrics(GeneratorsVect[(int)activeGenerator].getMap(), GeneratorsVect[(int)activeGenerator].TypeGrid, GeneratorsVect[(int)activeGenerator].startPos, GeneratorsVect[(int)activeGenerator].endPos);
            /*
            Content.transform.parent.Find("../SaveButton").gameObject.SetActive(true);
            Content.transform.parent.Find("../PlusButton").gameObject.SetActive(true);
            Content.transform.parent.Find("../MinusButton").gameObject.SetActive(true);
            */
            DisplayMainMap((GeneratorUIManager.Instance.isTrapsOnMapBorderToggleOn() ? getMapWTrapBorder() : GeneratorsVect[(int)activeGenerator].getMap()));
            GeneratorUIManager.Instance.showUIMapInfo(MapHolder.transform, dataMap);
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
        ParameterManager.Instance.MapToPlay = (GeneratorUIManager.Instance.isTrapsOnMapBorderToggleOn()? getMapWTrapBorder():GeneratorsVect[(int)activeGenerator].getMap());
        ParameterManager.Instance.GridType = GeneratorsVect[(int)activeGenerator].TypeGrid;
        SceneManager.LoadScene(AssembledLevelSceneName);
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

                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        textMap = textMap + map[y, map.GetLength(0)-1-x].type;
                    }
                    if (x < map.GetLength(1) - 1)
                    {
                        textMap = textMap + "\n";
                    }
                }
                string fileName = GeneratorsVect[(int)activeGenerator].seed.ToString() + "map.txt";
                File.WriteAllText(@textFilePath + "/" + fileName, textMap);

                GeneratorUIManager.Instance.showMessageDialogBox("Map \"" + fileName + "\" successfully saved at:\n" + textFilePath);
            }
            catch (Exception)
            {
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while saving the map at " + @textFilePath + ", please insert a valid path and check its permissions. ");
            }
        }
    }

    public TileObject[,] getMapWTrapBorder()
    {
        return (tmpMapWBorder==null?CreateMapWithBorder(): tmpMapWBorder);
    }

    private TileObject[,] CreateMapWithBorder()
    {
        switch(activeTypeGrid)
        {
            case TypeGridEnum.SQUARE:
                TileObject[,] Map =GeneratorsVect[(int)activeGenerator].getMap();
                tmpMapWBorder = new TileObject[Map.GetLength(0) + 2, Map.GetLength(1) + 2];

                for (int i = 0; i < tmpMapWBorder.GetLength(0); i++)
                {
                    for (int j = 0; j < tmpMapWBorder.GetLength(1); j++)
                    {
                        if (i > 0 && i < tmpMapWBorder.GetLength(0) - 1 && j > 0 && j < tmpMapWBorder.GetLength(1) - 1)
                            tmpMapWBorder[i, j].type = Map[i - 1, j - 1].type;
                        else
                            tmpMapWBorder[i, j].type = IGenerator.wallChar;
                    }
                }
            break;
            case TypeGridEnum.HEXAGON:
            break;
            case TypeGridEnum.TRIANGLE:
            break;
            default:
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Incorrect grid type.");
            break;
        }

        return tmpMapWBorder;
    }
}
