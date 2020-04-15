using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This map generator can generate a map where all room tiles are connected.
/// </summary>

[System.Serializable]
public class ConnectedGenerator : IGenerator
{
    [SerializeField]
    protected Vector2Int startPos= new Vector2Int(0,0);
    [SerializeField]
    protected Vector2Int endPos;

    public ConnectedGenerator(ITypeGrid i, int w, int h) : base(i)
    {
        //width = w;
        //height = h;
        Vector2Int endPos = new Vector2Int(w, h);
        initializeMap();
    }

    public ConnectedGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY) : base(i)
    {
        ///width = w;
        //height = h;
        Vector2Int endPos = new Vector2Int(w, h);
        startPos.x = startPosX;
        startPos.y = startPosY;

        initializeMap();
    }

    public ConnectedGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY, int endPosX, int endPosY) : base(i)
    {
        //width = w;
        //height = h;
        Vector2Int endPos = new Vector2Int(w, h);
        startPos.x = startPosX;
        startPos.y = startPosY;
        endPos.x = endPosX;
        endPos.y = endPosY;

        initializeMap();
    }

    public override TileObject[,] initializeMap()
    {
        map = new TileObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].type = floorChar;
            }
        }

        if (startPos != null)
        {
            map[startPos.x, startPos.y].type = startChar;
        }

        if (endPos != null)
        {
            map[endPos.x, endPos.y].type = startChar;
        }

        return map;

    }

    public override TileObject[,] generateMap()
    {
        throw new System.NotImplementedException();
    }

}
