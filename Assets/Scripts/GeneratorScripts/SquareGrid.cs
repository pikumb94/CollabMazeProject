using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// SquareGrid class defines squared tiles.
/// </summary>

[System.Serializable]
public class SquareGrid : ITypeGrid
{
    
    public SquareGrid():base(new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) },
        new Vector2Int[4] { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1) }, 1f, 1f){
        offsetX = 1f;
        offsetY = 1f;
    }

    public override int heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
