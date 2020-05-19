using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ITypeGrid is an abstract class that specifies which kind of tiles is used for a specific map generation.
/// Offset X and Y are used by the UI component to correctly print the map composed by a specific tile.
/// </summary>

[System.Serializable]
public abstract class ITypeGrid
{
    public GameObject TilePrefab;
    protected  Vector2Int[] directions;
    protected Vector2Int[] diagonals;
    public float offsetX;
    public float offsetY;

    [Header("Map Assembling Objects")]
    public GameObject InGameTilePrefab;
    public GameObject InGameObstaclePrefab;
    public GameObject InGameEndPrefab;
    public float inGameOffsetX;
    public float inGameOffsetY;

    protected ITypeGrid(Vector2Int[] dir, Vector2Int[] diag, float oX, float oY)
    {
        directions = dir;
        diagonals = diag;
        offsetX = oX;
        offsetY = oY;
    }

    public Vector2Int[] getDirs()
    {
        return directions;
    }

    public Vector2Int[] getDiags()
    {
        return diagonals;
    }
}
