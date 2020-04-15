using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GeneratorManager coordinates both UI and logic of every kind of generator attached.
/// </summary>

public class GeneratorManager : Singleton<GeneratorManager>
{
    public ConnectedGenerator connectedGenerator;
    public SquareGrid squareGrid;
    public GameObject MapHolder;

    protected GeneratorManager() { }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateButtonPressed()
    {
        if(connectedGenerator.width >0 && connectedGenerator.height >0)
        {
            TileObject[,] map = connectedGenerator.initializeMap();
            GeneratorUIManager.Instance.printMap(MapHolder.transform, squareGrid, map);
        }
        else
        {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Height or length set to zero.");
        }
        
    }
}
