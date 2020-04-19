using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// IGenerator is an abstract class useful to implement a specific map generator.
/// </summary>

[Serializable]
public struct TileObject
{
    public char type;
}

[System.Serializable]
public abstract class IGenerator
{
    [SerializeField]
    public ITypeGrid TypeGrid;
    [SerializeField]
    public int width= 0;
    [SerializeField]
    public int height= 0;
    [SerializeField]
    public Vector2Int startPos = new Vector2Int(0, 0);
    [SerializeField]
    public Vector2Int endPos;
    [SerializeField]
    public int seed;
    [SerializeField]
    public bool useRandomSeed;

    [HideInInspector]
    public const char roomChar = '.';
    [HideInInspector]
    public const char wallChar = '#';
    [HideInInspector]
    public const char startChar = '|';
    [HideInInspector]
    public const char endChar = '^';

    protected TileObject[,] map = null;

    protected IGenerator(ITypeGrid i)
    {
        TypeGrid = i;
    }

    public bool in_bounds(Vector2Int id)
    {
        return 0 <= id.x && id.x < width && 0 <= id.y && id.y < height;
    }

    protected bool passable(Vector2Int id)  {
        return map[id.x,id.y].type == wallChar;
    }

    //this version get all neighbours walls excluded
    public Vector2Int[] getNeighbours(Vector2Int id)
    {
        Vector2Int[] results = new Vector2Int[TypeGrid.getDirs().Length];

        foreach(Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next= new Vector2Int( id.x + dir.x, id.y + dir.y);
            if (in_bounds(next) && passable(next))
            {
                results[results.Length]=next;
            }
        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }

    //this version get all neighbours including walls
    public Vector2Int[] getAllNeighbours(Vector2Int id)
    {
        Vector2Int[] results = new Vector2Int[] { };

        foreach (Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (in_bounds(next))
            {
                //results[results.Length] = next;
                Array.Resize(ref results, results.Length + 1);
                results[results.GetUpperBound(0)] = next;
            }
        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }

    public int getWidth()
    {
        return width;
    }
    public abstract TileObject[,] initializeMap();
    public abstract TileObject[,] generateMap();
}
