using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class AliasGameEvaluator : MonoBehaviour
{
    public GameObject MainMapGO;
    public Color lineColorBest;
    public Color lineColorBestWorst;
    private GameObject AliasContainerGO;
    private GameObject LineUIPrefab;

    private MapListManager aliasList;

    private int nodesCount;
    private TreeNode<Vector2Int, Dictionary<int, bool>> root;
    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> LeavesSet;
    private int maxDepth;

    private ParameterManager pMan;

    private void Start()
    {
        AliasContainerGO = AliasGeneratorManager.Instance.AliasDragAreas[0].gameObject;
        LineUIPrefab = GeneratorUIManager.Instance.LineUIPrefab;
        aliasList = AliasContainerGO.GetComponent<MapListManager>();
        pMan = ParameterManager.Instance;
    }

    public void AliasGameEvaluatorHandler()
    {
        LeavesSet=ConstructAliasTree();
        printBestWorstPaths();
    }

    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> ConstructAliasTree()
    {
        Dictionary<int, bool> initDic = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
            initDic.Add(m.Key, true);
        //set root node
        root = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(Vector2Int.zero,initDic),0,null);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> leafNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        Queue<TreeNode<Vector2Int, Dictionary<int, bool>>> frontier = new Queue<TreeNode<Vector2Int, Dictionary<int, bool>>>();

        frontier.Enqueue(root);
        visitedNodes.Add(Vector2Int.zero);
        //leafNodes.Add(root);
        TreeNode<Vector2Int, Dictionary<int, bool>> minLeafNode = root;

        while (frontier.Count > 0)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> CurrentNode = frontier.Dequeue();
            Vector2Int[] Cells = Utility.getAllNeighboursWOBoundCheck_General(CurrentNode.NodeKeyValue.Key, pMan.GridType, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1));
            List<Vector2Int> cellList = new List<Vector2Int>(Cells);
            //You can never come from outside the map => except the coming cells, you should have 3 elements with getAllNeighbours and the difference indicates the OoB cells.
            if(CurrentNode.ParentNode != null)
            {
                Vector2Int toRemove = Vector2Int.zero;
                foreach (var c in cellList)
                    if (CurrentNode.ParentNode.NodeKeyValue.Key == c)
                        toRemove = c;

                    cellList.Remove(toRemove);
            }
            

            foreach (var c in cellList)
            {
                if (!visitedNodes.Contains(c))
                {
                    Dictionary<int, bool> newDictionary = updateAliasDictionary(c, CurrentNode.NodeKeyValue.Value);
                    TreeNode<Vector2Int, Dictionary<int, bool>> Node;

                    if (newDictionary.Count - CurrentNode.NodeKeyValue.Value.Count ==0)
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, CurrentNode.NodeKeyValue.Value), CurrentNode.nodeDepth + 1, CurrentNode);
                    else
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, newDictionary), CurrentNode.nodeDepth+1, CurrentNode);

                    if (newDictionary.Count != 0)
                    {
                        if(Utility.in_bounds_General(c + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) &&
                            pMan.MapToPlay[c.x + pMan.StartCell.x, c.y + pMan.StartCell.y].type == IGenerator.roomChar)
                                frontier.Enqueue(Node);

                    }
                    else
                    {
                        leafNodes.Add(Node);
                    }
                    //
                    //Debug.Log(newDictionary.Count);
                    if (newDictionary.Count < minLeafNode.NodeKeyValue.Value.Count)
                        minLeafNode = Node;
                    //
                    visitedNodes.Add(Node.NodeKeyValue.Key);
                }

            }

        }

        //
        if(leafNodes.Count == 0)
        {
            leafNodes.Add(minLeafNode);
        }
        //
        return leafNodes;
    }

    public Dictionary<int, bool> updateAliasDictionary( Vector2Int stepAttempt, Dictionary<int, bool> currDic)
    {
        Dictionary<int, bool> newDictionary = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
        {
            if (currDic.ContainsKey(m.Key))
            {
                if ((!Utility.in_bounds_General(stepAttempt + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) &&
                    !Utility.in_bounds_General(stepAttempt + m.Value.start, m.Value.AliasMap.GetLength(0), m.Value.AliasMap.GetLength(1)))//both outside of the map

               || (Utility.in_bounds_General(stepAttempt + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) &&
                   Utility.in_bounds_General(stepAttempt + m.Value.start, m.Value.AliasMap.GetLength(0), m.Value.AliasMap.GetLength(1)) &&
                   pMan.MapToPlay[stepAttempt.x + pMan.StartCell.x, stepAttempt.y + pMan.StartCell.y] == m.Value.AliasMap[stepAttempt.x + m.Value.start.x, stepAttempt.y + m.Value.start.y])//both inside and same character
               )
                {
                    newDictionary.Add(m.Key, true);
                }
            }
            

        }

        return newDictionary;
    }

    public void printBestWorstPaths()
    {
        int minDepth=LeavesSet[0].nodeDepth, maxDepth=0;
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> minNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> maxNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();

        foreach (var l in LeavesSet)
        {
            if (l.nodeDepth > maxDepth)
            {
                maxDepth = l.nodeDepth;

                maxNodes.Clear();
                maxNodes.Add(l);
            }
            else
            {
                if(l.nodeDepth == maxDepth)
                    maxNodes.Add(l);
            }

            if (l.nodeDepth < minDepth)
            {
                minDepth = l.nodeDepth;
                minNodes.Clear();
                minNodes.Add(l);
            }
            else
            {
                if (l.nodeDepth == minDepth)
                    minNodes.Add(l);
            }
        }



        foreach (var l in maxNodes)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> tmp = l.ParentNode;
            List<Vector2Int> backtrackSolution = new List<Vector2Int>();
            backtrackSolution.Add(l.NodeKeyValue.Key);

            while (tmp != null)
            {
                backtrackSolution.Add(tmp.NodeKeyValue.Key);
                tmp = tmp.ParentNode;
            }
            GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
                backtrackSolution.ToArray(), GeneratorUIManager.Instance.originUIMap,
                ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
            LineGO.GetComponent<UILineRenderer>().color = lineColorBestWorst;
            //LineGO.GetComponent<RectTransform>().position += new Vector3(GeneratorUIManager.Instance.originUIMap.x, GeneratorUIManager.Instance.originUIMap.y,0);
        }

        foreach (var l in minNodes)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> tmp = l.ParentNode;
            List<Vector2Int> backtrackSolution = new List<Vector2Int>();
            backtrackSolution.Add(l.NodeKeyValue.Key);

            while (tmp != null)
            {
                backtrackSolution.Add(tmp.NodeKeyValue.Key);
                tmp = tmp.ParentNode;
            }
            GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
                backtrackSolution.ToArray(), GeneratorUIManager.Instance.originUIMap,
                ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
            LineGO.GetComponent<UILineRenderer>().color = lineColorBest;
            //LineGO.GetComponent<RectTransform>().position += new Vector3(GeneratorUIManager.Instance.originUIMap.x, GeneratorUIManager.Instance.originUIMap.y, 0);

        }
    }
}
