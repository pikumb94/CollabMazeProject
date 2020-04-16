using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This map generator can generate a map where all room tiles are connected.
/// </summary>

[System.Serializable]
public class ConnectedGenerator : IGenerator
{
    [Range(0, 1)]
    public float obstaclePercent;

    private List<Vector2Int> allTileCoords;
    private Queue<Vector2Int> shuffledTileCoords;
    private Vector2Int mapCentre;

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
        //Initialize vector of all tile's locations.
        allTileCoords = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allTileCoords.Add(new Vector2Int(x, y));
            }
        }
        //Shuffle the vector of all tile's locations.
        shuffledTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Vector2Int((int)width / 2, (int)height / 2);


        map = new TileObject[width, height];
        //Map initialization.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].type = roomChar;
            }
        }

        //Fix start position
        if (startPos != null)
        {
            map[startPos.x, startPos.y].type = startChar;
        }

        //Fix end position
        if (endPos != null)
        {
            map[endPos.x, endPos.y].type = endChar;
        }

        return map;

    }

    public override TileObject[,] generateMap()
    {
        throw new System.NotImplementedException();
    }

}
