using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GeneratorManager coordinates both UI and logic of every kind of generator attached.
/// </summary>

public class GeneratorManager : Singleton<GeneratorManager>
{
    public ConnectedGenerator connectedGenerator;
    public SquareGrid squareGrid;
    public GameObject Content;
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

        try{

            validateGeneratorParams(connectedGenerator);

            TileObject[,] map = connectedGenerator.initializeMap();
            
            GeneratorUIManager.Instance.printMap(Content.transform, squareGrid, connectedGenerator.generateMap());

        } catch (Exception e) {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message);
        }

        
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
}
