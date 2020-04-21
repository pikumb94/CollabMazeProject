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
    public float offsetX;
    public float offsetY;

    protected ITypeGrid(Vector2Int[] dir, float oX, float oY)
    {
        directions = dir;
        offsetX = oX;
        offsetY = oY;
    }

    public Vector2Int[] getDirs()
    {
        return directions;
    }
}
