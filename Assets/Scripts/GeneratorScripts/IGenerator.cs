﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// IGenerator is an abstract class useful to implement a specific map generator.
/// </summary>

[Serializable]
public struct TileObject: IEquatable<TileObject>
{
    public char type;
    int? cost;

    public int Cost { get { return cost ?? -1; } set { cost = value; } }

    public static bool operator ==(TileObject obj1, TileObject obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(TileObject obj1, TileObject obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(TileObject other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return type == other.type;
    }

    public override bool Equals(object obj)
    {
        return base.Equals((TileObject)obj);
    }
}

[System.Serializable]
public abstract class IGenerator
{
    [HideInInspector]
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
    [TextArea]
    public string InfoGenerator = "";

    [HideInInspector]
    public const char roomChar = '.';
    [HideInInspector]
    public const char wallChar = '#';
    [HideInInspector]
    public const char startChar = '|';
    [HideInInspector]
    public const char endChar = '^';

    protected TileObject[,] map = null;
    protected TileObject[,] tmpMapWBorder = null;
    protected IGenerator(ITypeGrid i)
    {
        TypeGrid = i;
    }

    public bool in_bounds(Vector2Int id)
    {
        return 0 <= id.x && id.x < width && 0 <= id.y && id.y < height;
    }

    protected bool passable(Vector2Int id)  {
        return map[id.x,id.y].type != wallChar;
    }

    //this version get all neighbours walls excluded
    public Vector2Int[] getNeighbours(Vector2Int id)
    {
        Vector2Int[] results = new Vector2Int[/*TypeGrid.getDirs().Length*/] { };

        foreach(Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next= new Vector2Int( id.x + dir.x, id.y + dir.y);
            if (in_bounds(next) && passable(next))
            {
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

    //this version get all neighbours including walls
    public Vector2Int[] getAllMooreNeighbours(Vector2Int id)
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

        foreach (Vector2Int dir in TypeGrid.getDiags())
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

    

    private TileObject[,] CreateMapWithBorder()
    {
        switch (TypeGrid.gridType)
        {
            case ITypeGrid.TypeGridEnum.SQUARE:
                tmpMapWBorder = new TileObject[map.GetLength(0) + 2, map.GetLength(1) + 2];

                for (int i = 0; i < tmpMapWBorder.GetLength(0); i++)
                {
                    for (int j = 0; j < tmpMapWBorder.GetLength(1); j++)
                    {
                        if (i > 0 && i < tmpMapWBorder.GetLength(0) - 1 && j > 0 && j < tmpMapWBorder.GetLength(1) - 1)
                            tmpMapWBorder[i, j].type = map[i - 1, j - 1].type;
                        else
                            tmpMapWBorder[i, j].type = IGenerator.wallChar;
                    }
                }
                break;
            case ITypeGrid.TypeGridEnum.HEXAGON:
                break;
            case ITypeGrid.TypeGridEnum.TRIANGLE:
                break;
            default:
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Incorrect grid type.");
                break;
        }

        return tmpMapWBorder;
    }

    public int getWidth()
    {
        return width;
    }

    public abstract TileObject[,] initializeMap();
    public abstract TileObject[,] generateMap();
    public virtual TileObject[,] postprocessMap() { return map; } //a postprocessing operation of map is optionally available for inherited classes

    public virtual TileObject[,] generateAliasMap(TileObject[,] MainMap,HashSet<Vector2Int> CollisionCells) { return map; } //SWITCH TO ABSTRACT AS SOON AS POSSIBLE  

    public virtual TileObject[,] generateMapGeneral(bool isTrapsOnMapBorder) {

        initializeMap();
        generateMap();
        postprocessMap();
        return (isTrapsOnMapBorder ? getMapWTrapBorder() : getMap());
    }

    public void setBaseGeneratorParameters(ITypeGrid a, int b , int c, Vector2Int d, Vector2Int e, int f, bool g)
    {
         TypeGrid=a;
         width=b ;
         height = c;
         startPos = d;
         endPos=e;
         seed=f;
         useRandomSeed=g;
    }

    public TileObject[,] getMap()
    {
        return map;
    }

    public TileObject[,] getMapWTrapBorder()
    {
        return (tmpMapWBorder == null ? CreateMapWithBorder() : tmpMapWBorder);
    }
}
