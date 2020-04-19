using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This map generator can generate a map according to the Cellular Automata algorithm.
/// </summary>

[System.Serializable]
public class CellularAutomataGenerator : IGenerator
{
    [Range(0, 1)]
    public float obstaclePercent;

    public int iterationsNumber;

    public int thresholdWall=4;

    public bool borderIsWall;

    public CellularAutomataGenerator(ITypeGrid i, int w, int h) : base(i)
    {
        Vector2Int endPos = new Vector2Int(w, h);
        initializeMap();
    }

    public CellularAutomataGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY) : base(i)
    {
        Vector2Int endPos = new Vector2Int(w, h);
        startPos.x = startPosX;
        startPos.y = startPosY;

        initializeMap();
    }

    public CellularAutomataGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY, int endPosX, int endPosY) : base(i)
    {
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
        //Map initialization.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].type = roomChar;
            }
        }

        RandomFillMap();

        return map;

    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = (int) Time.time;
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].type = (pseudoRandom.Next(0, 100) < obstaclePercent * 100) ? wallChar : roomChar;
            }
        }
    }

    public override TileObject[,] generateMap()
    {
        for (int i = 0; i < iterationsNumber; i++)
        {
            SmoothMap();
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

    void SmoothMap()
    {
        Stack<Vector2Int> locationsRoom = new Stack<Vector2Int>();
        Stack<Vector2Int> locationsWall = new Stack<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > thresholdWall)
                    //map[x, y].type = wallChar;
                    locationsWall.Push(new Vector2Int(x, y));
                else if (neighbourWallTiles < thresholdWall)
                    //map[x, y].type = roomChar;
                    locationsRoom.Push(new Vector2Int(x, y));

            }
        }

        foreach (Vector2Int c in locationsWall)
        {
            map[c.x,c.y].type = wallChar;
        }

        foreach (Vector2Int c in locationsRoom)
        {
            map[c.x, c.y].type = roomChar;
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        /*
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += (map[neighbourX,neighbourY].type==wallChar) ? 1 : 0;
                    }
                }
                
            }
        }*/
        Vector2Int[] Neighbours = getAllNeighbours(new Vector2Int(gridX, gridY));

        foreach (Vector2Int neigh in Neighbours)
        {
            wallCount += (map[neigh.x, neigh.y].type == wallChar) ? 1 : 0;
        }

        if(borderIsWall)
            wallCount = wallCount + (TypeGrid.getDirs().Length - Neighbours.Length);

        return wallCount;
    }

}
