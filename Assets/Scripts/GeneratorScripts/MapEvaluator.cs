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

    public float obsToRoomIndex;
    public float obstacleClusteringIndex;

    public static bool operator ==(DataMap lhs, DataMap rhs)
    {
        // Check for null on left side.
        if (Object.ReferenceEquals(lhs, null))
        {
            if (Object.ReferenceEquals(rhs, null))
            {
                // null == null = true.
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator !=(DataMap lhs, DataMap rhs)
    {
        return !(lhs == rhs);
    }

}

public static class MapEvaluator
{

    public static string aggregateDataMap(DataMap dM)
    {
        string totalStepsString="Steps: "+ dM.totalSteps;
        string obstaclesCountString= "Walls: " + dM.obstaclesCount;
        string deadendCountString = "DeadEnds: "+dM.deadendCount;
        string chockeCountString="Chokes: "+dM.chockeCount;
        string obsToRoomIndexString="Walls/Rooms: "+ dM.obsToRoomIndex.ToString("0.###");
        string obstacleClusteringString="Room Cluster Index: "+dM.obstacleClusteringIndex.ToString("0.###");

        string separator = "   ";
        string result = totalStepsString;
        string[] ArrayDataString = new string[] {obstaclesCountString, deadendCountString, chockeCountString, obsToRoomIndexString, obstacleClusteringString };
        

        foreach (string s in ArrayDataString)
        {
            result = result + separator + s;
        }

        return result;
    }

    public static DataMap computeMetrics(TileObject[,] map, ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        DataMap dm = new DataMap();

        Vector2Int[] solSteps= Search_AStar(map, TypeGrid, start, end);
        if (solSteps.First()!=end && GeneratorManager.Instance.isAutosolverOn)
        {
            Utility.buildRoomsOnMap(map, walkSquareGrid(solSteps.First(), end));
            map[end.x, end.y].type = IGenerator.endChar;
            solSteps = Search_AStar(map, TypeGrid, start, end);
        }


        dm.solutionSteps = solSteps;
        dm.isTraversable = (dm.solutionSteps.First() == end);
        dm.totalSteps = dm.solutionSteps.Length-1;
        //Instead of doing same parsing on the map more than once to compute different metrics, we overlap all the code needed
        //in this two for loops to have a more performant program so repetition of code is not avoidable here
        HashSet<Vector2Int> AlreadyVisitedCells = new HashSet<Vector2Int>();
        List<HashSet<Vector2Int>> ObstacleConnectedRegions = new List<HashSet<Vector2Int>>();
        List<HashSet<Vector2Int>> RoomConnectedRegions = new List<HashSet<Vector2Int>>();

        int maxCellsCountObstacleRegion = 0;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].type != IGenerator.wallChar)
                {
                    Vector2Int[] NeighbourRooms= Utility.getNeighbours_General(map, new Vector2Int(x, y), TypeGrid, map.GetLength(0), map.GetLength(1));
                    if (NeighbourRooms.Length == 2)
                        dm.chockeCount++;
                    if (NeighbourRooms.Length == 1)
                        dm.deadendCount++;
                }
                else
                {
                    dm.obstaclesCount++;
                }
                
                //code to find all connected regions
                if (!AlreadyVisitedCells.Contains(new Vector2Int(x, y)))
                {
                    HashSet<Vector2Int> fFHS = floodFillGenMap(new Vector2Int(x, y), map, TypeGrid);
                    AlreadyVisitedCells.UnionWith(fFHS);

                    if (map[x, y].type != IGenerator.roomChar)
                    {
                        RoomConnectedRegions.Add(fFHS);
                    }

                    if (map[x, y].type == IGenerator.wallChar)
                    {
                        ObstacleConnectedRegions.Add(fFHS);
                        maxCellsCountObstacleRegion = (maxCellsCountObstacleRegion < ObstacleConnectedRegions.Count ? ObstacleConnectedRegions.Count : maxCellsCountObstacleRegion);
                    }
                }

                if (AlreadyVisitedCells.Count == map.GetLength(0) *map.GetLength(1))
                    break;
            }
            if (AlreadyVisitedCells.Count == map.GetLength(0) * map.GetLength(1))
                break;
        }

        dm.obsToRoomIndex = (float) dm.obstaclesCount / (map.GetLength(0) * map.GetLength(1) - dm.obstaclesCount);
        dm.obstacleClusteringIndex = (float) maxCellsCountObstacleRegion / dm.obstaclesCount;

        return dm;
    }

    //modified version of the A_star. If a solving path exists, returns it otherwise put the visited cells where the last one is the closest ot the end cell
    public static Vector2Int[] Search_AStar(TileObject[,] map, ITypeGrid TypeGrid, Vector2Int start, Vector2Int end)
    {
        Hashtable CameFrom = new Hashtable();
        SimplePriorityQueue<Vector2Int> frontier = new SimplePriorityQueue<Vector2Int>();

        LinkedList<Vector2Int> SolutionPath = new LinkedList<Vector2Int>();

        frontier.Enqueue(start, 0);

        CameFrom[start] = null;
        map[start.x, start.y].Cost = 0;

        Vector2Int closestCell = new Vector2Int(start.x, start.y);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            //TEORICAMENTE LA TROVA DA SOLA
            closestCell = (TypeGrid.heuristic(current, end) < TypeGrid.heuristic(closestCell, end) ? current : closestCell);

            if (current.Equals(end))
            {
                break;
            }
            foreach (Vector2Int next in Utility.getNeighbours_General(map, current, TypeGrid, map.GetLength(0), map.GetLength(1)))
            {
                double newCost = map[current.x, current.y].Cost + 1;
                if (!CameFrom.Contains(next) || newCost < map[next.x, next.y].Cost)
                {
                    map[next.x, next.y].Cost = (int)newCost;
                    double priority = newCost + TypeGrid.heuristic(next, end);
                    frontier.Enqueue(next, (float)priority);
                    CameFrom[next] = current;
                }
            }
        }

        //if a path exists, closestCell==end otherwise is the last cell expanded
        Vector2Int  cameValue = new Vector2Int(closestCell.x, closestCell.y);
        SolutionPath.AddLast(closestCell);

        while (CameFrom[cameValue] != null)
        {
            cameValue = (Vector2Int)CameFrom[cameValue];
            SolutionPath.AddLast(cameValue);
        }
        

        //if a path exists, the last cell is the end one
        return SolutionPath.ToArray();
    }

    //It returnes the connected squares that lay on the segment that go from p0 to p1 orthogonally adjacent
    public static Vector2Int[] walkSquareGrid(Vector2Int p0, Vector2Int p1)
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

    public static void findRoomObstacleConnectedRegions(List<HashSet<Vector2Int>> RoomConnectedRegions, List<HashSet<Vector2Int>> ObstacleConnectedRegions, TileObject[,] m, ITypeGrid typeGrid)
    {

        HashSet<Vector2Int> AlreadyVisitedCells = new HashSet<Vector2Int>();
        int width = m.GetLength(0);
        int height = m.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (!AlreadyVisitedCells.Contains(new Vector2Int(x, y)))
                {
                    HashSet<Vector2Int> fFHS = floodFillGenMap(new Vector2Int(x, y), m, typeGrid);
                    AlreadyVisitedCells.UnionWith(fFHS);

                    if (m[x, y].type != IGenerator.roomChar)
                    {
                        RoomConnectedRegions.Add(fFHS);
                    }

                    if (m[x, y].type == IGenerator.wallChar)
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
    }

    private static HashSet<Vector2Int> floodFillGenMap(Vector2Int startCell, TileObject[,] m, ITypeGrid typeGrid)
    {
        HashSet<Vector2Int> FloodedCells = new HashSet<Vector2Int>() { startCell };

        Queue<Vector2Int> queue = new Queue<Vector2Int>() { };
        queue.Enqueue(startCell);

        TileObject t = m[startCell.x, startCell.y];

        while (queue.Count > 0)
        {
            Vector2Int Cell = queue.Dequeue();
            Vector2Int[] Cells = Utility.getAllNeighbours_General(Cell, typeGrid, m.GetLength(0), m.GetLength(1));

            foreach (Vector2Int c in Cells)
            {
                if (t == m[c.x, c.y])
                {

                    if (!FloodedCells.Contains(c))
                        queue.Enqueue(c);
                    FloodedCells.Add(c);
                }

            }
        }

        return FloodedCells;
    }

    
}
