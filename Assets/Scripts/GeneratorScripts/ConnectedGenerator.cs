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
        Vector2Int endPos = new Vector2Int(w, h);
        initializeMap();
    }

    public ConnectedGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY) : base(i)
    {
        Vector2Int endPos = new Vector2Int(w, h);
        startPos.x = startPosX;
        startPos.y = startPosY;

        initializeMap();
    }

    public ConnectedGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY, int endPosX, int endPosY) : base(i)
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
        //Initialize vector of all tile's locations.
        allTileCoords = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (pos != startPos && pos != endPos)
                    allTileCoords.Add(new Vector2Int(x, y));
            }
        }
        //Shuffle the vector of all tile's locations.
        shuffledTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(allTileCoords.ToArray(), useRandomSeed, seed));
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
        bool[,] obstacleMap = new bool[(int)width, (int)height];

        int obstacleCount = (int)(width * height * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2Int randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                map[randomCoord.x, randomCoord.y].type = wallChar;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        return map;
    }

    public Vector2Int GetRandomCoord()
    {
        Vector2Int randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Vector2Int(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(width * height - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

}
