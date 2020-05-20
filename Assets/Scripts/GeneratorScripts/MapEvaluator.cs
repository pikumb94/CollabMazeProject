using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public struct DataMap
{
    public bool isTraversable;
    public Vector2Int[] solutionSteps;
    public int totalSteps;
    public int obstaclesCount;
    public int deadendCount;
    public int chockeCount;

    public int obsToRoomIndex;
    public int obstacleClusteringIndex;
}

public class MapEvaluator
{
    public DataMap computeMetrics(TileObject[,] map,ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        DataMap dm = new DataMap();
        dm.solutionSteps = Search_AStar(map,TypeGrid,start,end);
        dm.totalSteps = dm.solutionSteps.Length;
        dm.isTraversable = (dm.solutionSteps.Length != 0);
        return dm;
    }

    public Vector2Int[] Search_AStar(TileObject[,] map, ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> SolutionSteps = new HashSet<Vector2Int>();
        SimplePriorityQueue<Vector2Int> frontier = new SimplePriorityQueue<Vector2Int>();

        frontier.Enqueue(start, 0);

        SolutionSteps.Add(start);
        map[start.x,start.y].Cost = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current.Equals(end))
            {
                break;
            }
            foreach (Vector2Int next in Utility.getNeighbours_General(map, current, TypeGrid, map.GetLength(0), map.GetLength(1)) )
            {
                double newCost = map[current.x, current.y].Cost + 1;
                if (!SolutionSteps.Contains(next) || newCost < map[next.x, next.y].Cost)
                {
                    map[next.x, next.y].Cost = (int) newCost;
                    double priority = newCost + TypeGrid.heuristic(next, end);
                    frontier.Enqueue(next, (float)priority);
                    SolutionSteps.Add(current);
                }
            }
        }
        

        return (frontier.Count == 0 ? new Vector2Int[] { } :SolutionSteps.ToArray());//we encode the abscence of a solution returning an empty array
    }
}
