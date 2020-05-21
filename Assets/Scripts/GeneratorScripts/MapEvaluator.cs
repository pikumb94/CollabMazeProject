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
    public DataMap computeMetrics(TileObject[,] map, ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        DataMap dm = new DataMap();

        Vector2Int[] solSteps= Search_AStar(map, TypeGrid, start, end);
        if (solSteps.Last()!=end && GeneratorManager.Instance.isAutosolverOn)
        {
            Utility.buildRoomsOnMap(map, walkSquareGrid(solSteps.Last(), end));
            map[end.x, end.y].type = IGenerator.endChar;
            solSteps = Search_AStar(map, TypeGrid, start, end);
        }


        dm.solutionSteps = solSteps;
        dm.isTraversable = (dm.solutionSteps.Last() == end);
        dm.totalSteps = dm.solutionSteps.Length-1;


        return dm;
    }

    //modified version of the A_star. If a solving path exists, returns it otherwise put the visited cells where the last one is the closest ot the end cell
    public Vector2Int[] Search_AStar(TileObject[,] map, ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> SolutionSteps = new HashSet<Vector2Int>();
        SimplePriorityQueue<Vector2Int> frontier = new SimplePriorityQueue<Vector2Int>();

        frontier.Enqueue(start, 0);

        SolutionSteps.Add(start);
        map[start.x, start.y].Cost = 0;

        Vector2Int closestCell = new Vector2Int(start.x, start.y);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            closestCell = (TypeGrid.heuristic(current, end) < TypeGrid.heuristic(closestCell, end) ? current : closestCell);

            if (current.Equals(end))
            {
                break;
            }
            foreach (Vector2Int next in Utility.getNeighbours_General(map, current, TypeGrid, map.GetLength(0), map.GetLength(1)))
            {
                double newCost = map[current.x, current.y].Cost + 1;
                if (!SolutionSteps.Contains(next) || newCost < map[next.x, next.y].Cost)
                {
                    map[next.x, next.y].Cost = (int)newCost;
                    double priority = newCost + TypeGrid.heuristic(next, end);
                    frontier.Enqueue(next, (float)priority);
                    SolutionSteps.Add(current);
                }
            }
        }

        SolutionSteps.Add(closestCell);//if a path exists, the last cell is the end one
        return SolutionSteps.ToArray();
    }

    //It returnes the connected squares that lay on the segment that go from p0 to p1 orthogonally adjacent
    public Vector2Int[] walkSquareGrid(Vector2Int p0, Vector2Int p1)
    {
        var dx = p1.x - p0.x;
        var dy = p1.y - p0.y;
        var nx = Mathf.Abs(dx);
        var ny = Mathf.Abs(dy);
        var sign_x = dx > 0 ? 1 : -1;
        var sign_y = dy > 0 ? 1 : -1;

        var p = new Vector2Int(p0.x, p0.y);
        List<Vector2Int> points = new List<Vector2Int>();
        points.Add(new Vector2Int(p.x, p.y));

        for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            if ((0.5 + ix) / nx < (0.5 + iy) / ny)
            {
                // next step is horizontal
                p.x += sign_x;
                ix++;
            }
            else
            {
                // next step is vertical
                p.y += sign_y;
                iy++;
            }
            points.Add(new Vector2Int(p.x,p.y));
        }

        return points.ToArray();
    }
}
