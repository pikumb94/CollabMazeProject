using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliasGameEvaluator : MonoBehaviour
{

    public GameObject AliasContainerGO;
    private MapListManager aliasList;

    private int nodesCount;
    private TreeNode<Vector2Int, Dictionary<int, bool>> root;
    private HashSet<Vector2Int> LeavesSet;
    private int maxDepth;

    private ParameterManager pMan;
    private void Start()
    {
        aliasList = AliasContainerGO.GetComponent<MapListManager>();
        pMan = ParameterManager.Instance;
    }
    
    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> ConstructAliasTree()
    {
        Dictionary<int, bool> initDic = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
            initDic.Add(m.Key, true);
        //set root node
        TreeNode<Vector2Int, Dictionary<int, bool>> root = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(Vector2Int.zero,initDic),0,null);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> leafNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        Queue<TreeNode<Vector2Int, Dictionary<int, bool>>> frontier = new Queue<TreeNode<Vector2Int, Dictionary<int, bool>>>();

        frontier.Enqueue(root);
        visitedNodes.Add(Vector2Int.zero);
        leafNodes.Add(root);

        while (frontier.Count > 0)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> CurrentNode = frontier.Dequeue();
            Vector2Int[] Cells = Utility.getAllNeighboursWOBoundCheck_General(CurrentNode.NodeKeyValue.Key, pMan.GridType, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1));
            List<Vector2Int> cellList = new List<Vector2Int>(Cells);
            //You can never come from outside the map => except the coming cells, you should have 3 elements with getAllNeighbours and the difference indicates the OoB cells.
            if(CurrentNode.ParentNode != null)
            {
                foreach (var c in cellList)
                    if (CurrentNode.ParentNode.NodeKeyValue.Key == c)
                        cellList.Remove(c);
            }
            

            foreach (var c in cellList)
            {
                if (!visitedNodes.Contains(c))
                {
                    Dictionary<int, bool> newDictionary = updateAliasDictionary(c);
                    TreeNode<Vector2Int, Dictionary<int, bool>> Node;

                    if (newDictionary.Count - CurrentNode.NodeKeyValue.Value.Count ==0)
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, CurrentNode.NodeKeyValue.Value), CurrentNode.nodeDepth + 1, CurrentNode);
                    else
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, newDictionary), CurrentNode.nodeDepth+1, CurrentNode);

                    if (newDictionary.Count != 0)
                    {
                        frontier.Enqueue(Node);

                    }
                    else
                    {
                        leafNodes.Add(Node);
                    }
                        
                    
                    visitedNodes.Add(Node.NodeKeyValue.Key);
                }

            }

        }
        /*
        came_from = { }
                came_from[start] = None

        while not frontier.empty():
           current = frontier.get()
           for next in graph.neighbors(current):
              if next not in came_from:
                frontier.put(next)
                 came_from[next] = current*/

        return leafNodes;
    }

    public Dictionary<int, bool> updateAliasDictionary( Vector2Int stepAttempt)
    {
        Dictionary<int, bool> newDictionary = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
        {
            if( (   Utility.in_bounds_General(stepAttempt+ pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) && 
                    Utility.in_bounds_General(stepAttempt + m.Value.start, m.Value.AliasMap.GetLength(0), m.Value.AliasMap.GetLength(1)))
               || pMan.MapToPlay[stepAttempt.x+pMan.StartCell.x, stepAttempt.y + pMan.StartCell.y] == m.Value.AliasMap[stepAttempt.x + m.Value.start.x, stepAttempt.y + m.Value.start.y])
            {
                newDictionary.Add(m.Key, true);
            }

        }

        return newDictionary;
    }
}
