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

    public bool borderIsObstacle;

    public int roomThreshold;
    public int obstacleThreshold;

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

        if(borderIsObstacle)
            wallCount = wallCount + (TypeGrid.getDirs().Length - Neighbours.Length);

        return wallCount;
    }

    public override TileObject[,] postprocessMap()
    {
        
        if (obstacleThreshold > 0 || roomThreshold > 0)
        {

            HashSet<Vector2Int> AlreadyVisitedCells = new HashSet<Vector2Int>();
            List<HashSet<Vector2Int>> ObstacleConnectedRegions = new List<HashSet<Vector2Int>>();
            List<HashSet<Vector2Int>> RoomConnectedRegions = new List<HashSet<Vector2Int>>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (!AlreadyVisitedCells.Contains(new Vector2Int(x, y)))
                    {
                        HashSet<Vector2Int> fFHS = floodFillGenMap(new Vector2Int(x, y));
                        AlreadyVisitedCells.UnionWith(fFHS);

                        if (map[x, y].type != wallChar)
                        {
                            RoomConnectedRegions.Add(fFHS);
                        }

                        if (map[x, y].type == wallChar)
                        {
                            ObstacleConnectedRegions.Add(fFHS);
                        }
                    }

                    if (AlreadyVisitedCells.Count == width * height)
                        break;
                }
                if (AlreadyVisitedCells.Count == width * height)
                    break;
            }

            if (roomThreshold > 0)
            {
                foreach(HashSet<Vector2Int> rCR in RoomConnectedRegions)
                {
                    if(rCR.Count <= roomThreshold)
                    {
                        foreach(Vector2Int cell in rCR)
                        {
                            map[cell.x, cell.y].type  = wallChar;
                        }
                    }
                }
            }

            if (obstacleThreshold > 0)
            {
                foreach (HashSet<Vector2Int> oCR in ObstacleConnectedRegions)
                {
                    if (oCR.Count <= obstacleThreshold)
                    {
                        foreach (Vector2Int cell in oCR)
                        {
                            map[cell.x, cell.y].type = roomChar;
                        }
                    }
                }
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

    private HashSet<Vector2Int> floodFillGenMap(Vector2Int startCell)
    {
        HashSet<Vector2Int> FloodedCells = new HashSet<Vector2Int>() {startCell};

        Queue<Vector2Int> queue = new Queue<Vector2Int>() {};
        queue.Enqueue(startCell);

        TileObject t = map[startCell.x, startCell.y];

        while (queue.Count > 0)
        {
            Vector2Int Cell = queue.Dequeue();
            Vector2Int[] Cells = getAllNeighbours(Cell);

            foreach(Vector2Int c in Cells)
            {
                if(t==map[c.x, c.y])
                {
                    
                    if(!FloodedCells.Contains(c))
                        queue.Enqueue(c);
                    FloodedCells.Add(c);
                }

            }
        }

        return FloodedCells;
    }
}
