using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
/// <summary>
/// This map generator can generate a map according to the Cellular Automata algorithm.
/// </summary>

[System.Serializable]
public class PrimGenerator : IGenerator
{
    [Range(0, 1)]
    public float obstacleToRemovePercent;

    [HideInInspector]
    public const char pillarChar = 'I';

    private Tuple<int, int, int> GridColoring = Tuple.Create<int,int,int>(2,0,2);
    private char[,] ColorLookupTable;
    private HashSet<Vector2Int> wallSet;
    public PrimGenerator(ITypeGrid i, int w, int h) : base(i)
    {
        Vector2Int endPos = new Vector2Int(w, h);
        initializeMap();
    }

    public PrimGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY) : base(i)
    {
        Vector2Int endPos = new Vector2Int(w, h);
        startPos.x = startPosX;
        startPos.y = startPosY;

        initializeMap();
    }

    public PrimGenerator(ITypeGrid i, int w, int h, int startPosX, int startPosY, int endPosX, int endPosY) : base(i)
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
        tmpMapWBorder = null;
        ColorLookupTable = new char[/*GridColoring.Item1, GridColoring.Item3*/,] { { roomChar, wallChar}, { wallChar, endChar}};

        map = new TileObject[width, height];
        //Map initialization: mark all walls as closed
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].type = ColorLookupTable[x% ColorLookupTable.GetLength(0),y % ColorLookupTable.GetLength(1)];
            }
        }


        return map;

    }

    public override TileObject[,] generateMap()
    {
        HashSet<Vector2Int> pathSet = new HashSet<Vector2Int>();
        //Select a room from the set of rooms, and add it to the "path": in our case the start position
        pathSet.Add(new Vector2Int(startPos.x, startPos.y));

        wallSet = new HashSet<Vector2Int>();
        //Add the four walls of the previous added room to the "wall list". 
        //This is the list that we keep processing until it is empty.
        insertWallsInSet(wallSet, new Vector2Int(startPos.x, startPos.y));

        if (useRandomSeed)
        {
            System.Random rand = new System.Random((int)DateTime.Now.Ticks);
            seed = rand.Next(Int32.MinValue, Int32.MaxValue);
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        while (wallSet.Count > 0) //While the wall list is not empty
        {
            //Select a wall from the list: in our case randomly
            Vector2Int wall = wallSet.ElementAt(pseudoRandom.Next(0,wallSet.Count));
            //Find the rooms adjacent to the wall
            Vector2Int[] rooms = getNeighbours(wall);

            //Find the rooms adjacent to the wall.
            Vector2Int pathRoom = new Vector2Int();
            int count = 0;
            foreach(Vector2Int r in rooms)
            {
                if (!pathSet.Contains(r) && map[r.x,r.y].type==roomChar)//here rooms include pillars so the second condition takes only rooms
                {
                    pathRoom = r;
                    count++;
                }
            }

            if (count <= 1) {//If there are two adjacent rooms, and exactly one of them is not in the path
                if (count == 1)
                {
                    //Mark the wall as "Open" (i.e. room)
                    map[wall.x, wall.y].type = roomChar;
                    //Add the unvisited room to the path
                    pathSet.Add(pathRoom);
                    //Add the walls adjacent to the unvisited room to the wall list
                    insertWallsInSet(wallSet, pathRoom);
                }
                //if count is equal to zero, probably we're on the boarder or the next room is already in the path
            } else {
                throw new Exception("One wall has more than one room as neighbour.");
            }

            //Remove the wall from the wall list
            wallSet.Remove(wall);
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x, y].type == endChar)
                map[x, y].type = wallChar;

                if (map[x, y].type == wallChar)
                    wallSet.Add(new Vector2Int(x, y));
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
            wallSet.Remove(new Vector2Int(endPos.x, endPos.y));
            map[endPos.x, endPos.y].type = endChar;
        }

        return map;
    }

    private void insertWallsInSet(HashSet<Vector2Int> wallSet, Vector2Int room)
    {
        Vector2Int[] neigh = getAllNeighbours(room);
        foreach(Vector2Int v in neigh)
        {
            if (map[v.x, v.y].type == wallChar)
                wallSet.Add(v);
        }
    }

    public override TileObject[,] postprocessMap()
    {
        if (obstacleToRemovePercent > 0)
        {
            int n = (int)(obstacleToRemovePercent * (float)wallSet.Count);
            System.Random pseudoRandom = new System.Random(seed.GetHashCode());//IS PSEUDORANDOM

            while (n > 0)
            {
                Vector2Int wallToRemove = wallSet.ElementAt(pseudoRandom.Next(0, wallSet.Count));
                map[wallToRemove.x, wallToRemove.y].type = roomChar;
                wallSet.Remove(wallToRemove);
                n--;
            }
        }
        return map;
    }

    public TileObject[,] generateMapGeneral(bool isTrapsOnMapBorder, float obsToRemPerc, bool useS, int Seed)
    {
        obstacleToRemovePercent = obsToRemPerc;
        useRandomSeed = useS;
        seed = Seed;

        return base.generateMapGeneral(isTrapsOnMapBorder);
    }
}
