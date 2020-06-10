using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System;
using System.Linq;
using UnityEngine.UI;

public class AliasGameEvaluator : MonoBehaviour
{
    public GameObject MainMapGO;
    public GameObject CartesianGraphGO;
    public Color lineColorBest;
    public Color lineColorBestWorst;
    public Color lineColorAggressiveAgent;
    public Color lineColorPrudentAgent;
    public Color lineColorAverage;
    public GameObject ToggleLineContainerGO;

    private GameObject AliasContainerGO;
    private GameObject LineUIPrefab;

    private MapListManager aliasList;

    private int nodesCount;
    private TreeNode<Vector2Int, Dictionary<int, bool>> root;
    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> ZeroLeavesSet;
    private Dictionary<Vector2Int, TreeNode<Vector2Int, Dictionary<int, bool>>> LeavesSet;
    private List<Tuple<List<float>, Color>> ChartLines;
    private ParameterManager pMan;

    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> DupLeavesList;

    private List<List<Vector2Int>> BestPaths;
    private List<List<Vector2Int>> BestWorstPaths;

    private List<GameObject> LinesGO;

    private void Start()
    {
        
        AliasContainerGO = AliasGeneratorManager.Instance.AliasDragAreas[0].gameObject;
        LineUIPrefab = GeneratorUIManager.Instance.LineUIPrefab;
        aliasList = AliasContainerGO.GetComponent<MapListManager>();
        pMan = ParameterManager.Instance;
        ChartLines = new List<Tuple<List<float>, Color>>();
        LinesGO = new List<GameObject>();
    }

    public void AliasGameEvaluatorHandler()
    {
        ChartLines.Clear();
        DestroyMapLines();
        CartesianGraphGO.GetComponent<Window_Graph>().RemoveAllGraphs();

        if (aliasList.dictionaryMap.Count != 0)
        {
            
            //ZeroLeavesSet = ConstructAliasTreeWDuplicates();
            //ZeroLeavesSet = ConstructAliasTree();

            ZeroLeavesSet = ConstructAliasTreeWDuplicates();
            printBestWorstPaths();
            //buildAverageChartLine(); WORKS ONLY WITH ConstructAliasTree(); THAT BUILDS ALL THE LEAVES (but still not correctly)
            PrudentAgentPath();
            AggressiveAgentPath();
            CartesianGraphGO.GetComponent<Window_Graph>().ShowGraphInBatch(ChartLines,-1,null,null);
        }
        else
        {
            CartesianGraphGO.GetComponent<Window_Graph>().RemoveAllGraphs();
            //CartesianGraphGO.GetComponent<Window_Graph>().ShowGraph(new List<float>(){ 0}, lineColoraverage, -1, null, null);
        }
        refreshBlueToggle();
    }

    public void DestroyMapLines()
    {
        foreach (GameObject gameObject in LinesGO)
        {
            Destroy(gameObject);
        }
        LinesGO.Clear();
    }

    public void refreshBlueToggle()
    {
        Toggle[] toggles = ToggleLineContainerGO.GetComponentsInChildren<Toggle>();

        foreach (var l in toggles)
        {
            if(l.transform.Find("Background/Checkmark").GetComponent<Image>().color == Color.blue)
                l.gameObject.GetComponent<ToggleLineOnMap>().refreshLineReference();
        }
    }

    public void refreshToggle(GameObject lineGO)
    {
        Toggle[] toggles = ToggleLineContainerGO.GetComponentsInChildren<Toggle>();

        foreach (var l in toggles)
        {
            l.gameObject.GetComponent<ToggleLineOnMap>().refreshLineReference( lineGO);
        }
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

        LeavesSet = new Dictionary<Vector2Int, TreeNode<Vector2Int, Dictionary<int, bool>>>();
        LeavesSet.Add(root.NodeKeyValue.Key, root);

        while (frontier.Count > 0)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> CurrentNode = frontier.Dequeue();
            Vector2Int[] Cells = Utility.getAllNeighboursWOBoundCheck_General(CurrentNode.NodeKeyValue.Key, pMan.GridType);
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

                    
                    if(LeavesSet.ContainsKey(CurrentNode.NodeKeyValue.Key))
                        LeavesSet.Remove(CurrentNode.NodeKeyValue.Key);
                    LeavesSet.Add(Node.NodeKeyValue.Key, Node);
                    

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

    public Dictionary<int, bool> updateAliasDictionaryWReal(Vector2Int stepAttempt, Dictionary<int, bool> currDic)
    {
        Dictionary<int, bool> res = updateAliasDictionary(stepAttempt, currDic);
        res.Add(MainMapGO.GetInstanceID(), true);
        return res;
    }

    public void printBestWorstPaths()
    {
        int minDepth= ZeroLeavesSet[0].nodeDepth, maxDepth=0;
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> minNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> maxNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();

        foreach (var l in ZeroLeavesSet)
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

        List<float> BestChart = new List<float>();
        List<float> BestWorstChart = new List<float>();

        BestPaths = new List<List<Vector2Int>>();
        BestWorstPaths = new List<List<Vector2Int>>();

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

            BestWorstPaths.Add(backtrackSolution);
                        
            List<float> toChart = buildPathChartLine(backtrackSolution);
            toChart.Reverse();
            if (toChart.Count > BestWorstChart.Count)
            {
                for (int i = 0; i < toChart.Count; i++)
                {
                    if (i < BestWorstChart.Count)
                        BestWorstChart[i] += toChart[i];
                    else
                        BestWorstChart.Add(toChart[i]);
                }

            }
            else
            {
                for (int i = 0; i < toChart.Count; i++)
                {
                   BestWorstChart[i] += toChart[i];
                }
            }

            
        }

        for (int i = 0; i < BestWorstChart.Count; i++)
        {
            BestWorstChart[i] /= BestWorstPaths.Count;
        }
        ChartLines.Add(new Tuple<List<float>, Color>(BestWorstChart, lineColorBestWorst));

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
            BestPaths.Add(backtrackSolution);

            List<float> toChart = buildPathChartLine(backtrackSolution);
            toChart.Reverse();
            if (toChart.Count > BestChart.Count)
            {
                for (int i = 0; i < toChart.Count; i++)
                {
                    if (i < BestChart.Count)
                        BestChart[i] += toChart[i];
                    else
                        BestChart.Add(toChart[i]);
                }

            }
            else
            {
                for (int i = 0; i < toChart.Count; i++)
                {
                    BestChart[i] += toChart[i];
                }
            }
        }

        for (int i = 0; i < BestChart.Count; i++)
        {
            BestChart[i] /= BestPaths.Count;
        }
        ChartLines.Add(new Tuple<List<float>, Color>(BestChart, lineColorBest));

        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
            BestWorstPaths[rand.Next(0, Int32.MaxValue)%BestWorstPaths.Count].ToArray(), GeneratorUIManager.Instance.originUIMap,
            ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
        LineGO.GetComponent<UILineRenderer>().color = lineColorBestWorst;
        LinesGO.Add(LineGO);
        refreshToggle(LineGO);

        LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
            BestPaths[rand.Next(0, Int32.MaxValue) % BestPaths.Count].ToArray(), GeneratorUIManager.Instance.originUIMap,
            ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
        LineGO.GetComponent<UILineRenderer>().color = lineColorBest;
        LinesGO.Add(LineGO);
        refreshToggle(LineGO);
    }

    public List<float> buildPathChartLine(List<Vector2Int> pointList)
    {
        List<float> aliasPathCount = new List<float>();
        TreeNode<Vector2Int, Dictionary<int, bool>> tmp = ZeroLeavesSet.Find(x => x.NodeKeyValue.Key == pointList[0]);//(LeavesSet.ContainsKey(pointList[0])? LeavesSet[pointList[0]]:DupLeavesList.Find(x => x.NodeKeyValue.Key == pointList[0]));
        while (tmp!=null)
        {
            aliasPathCount.Add(tmp.NodeKeyValue.Value.Count);
            tmp = tmp.ParentNode;
        }
        return aliasPathCount;
    }

    public List<float> buildAverageChartLine()
    {
        Dictionary<int, List<TreeNode<Vector2Int, Dictionary<int, bool>>>> dicktionaryKeyStep = new Dictionary<int, List<TreeNode<Vector2Int, Dictionary<int, bool>>>>();
        HashSet<Vector2Int> VisitedTreeNodes = new HashSet<Vector2Int>();

        int maxDepth = 0;
        foreach(var leaf in LeavesSet.Values)
        {
            if (!dicktionaryKeyStep.ContainsKey(leaf.nodeDepth))
                dicktionaryKeyStep.Add(leaf.nodeDepth, new List<TreeNode<Vector2Int, Dictionary<int, bool>>>());

            dicktionaryKeyStep[leaf.nodeDepth].Add(leaf);

            if (leaf.nodeDepth > maxDepth)
                maxDepth = leaf.nodeDepth;

            VisitedTreeNodes.Add(leaf.NodeKeyValue.Key);
        }
        //dicktionaryKeyStep.Add(0, new List<TreeNode<Vector2Int, Dictionary<int, bool>>>() {root});
        int i = maxDepth;

        while (i > 0)
        {
            foreach (var node in dicktionaryKeyStep[i]) {
                if (!VisitedTreeNodes.Contains(node.ParentNode.NodeKeyValue.Key))
                {
                    if (dicktionaryKeyStep.ContainsKey(i - 1))
                    {
                        dicktionaryKeyStep[i - 1].Add(node.ParentNode);
                        VisitedTreeNodes.Add(node.ParentNode.NodeKeyValue.Key);
                    }
                    else
                    {
                        dicktionaryKeyStep.Add(i - 1, new List<TreeNode<Vector2Int, Dictionary<int, bool>>>());
                        dicktionaryKeyStep[i - 1].Add(node.ParentNode);
                        VisitedTreeNodes.Add(node.ParentNode.NodeKeyValue.Key);
                    }
                }

            }
            i--;
        }

        List<float> avgLine = new List<float>();


        for (i = 0; i < dicktionaryKeyStep.Count; i++)
        {
            int avg=0;
            foreach (var d in dicktionaryKeyStep[i])
                avg += d.NodeKeyValue.Value.Count;

            avgLine.Add((float)avg / (float)dicktionaryKeyStep[i].Count);
        }

        /*
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        avgLine.Reverse();
        Utility.displaySegmentedLineUI_General(LineGO, StatisticsUIContainer.GetComponent<RectTransform>(),avgLine.ToArray(), Vector3.zero,10,10);
        LineGO.GetComponent<UILineRenderer>().color = lineColoraverage;
        LineGO.GetComponent<UILineRenderer>().LineThickness = 5;*/

        ChartLines.Add(new Tuple<List<float>, Color>(avgLine, lineColorAverage));
        //CartesianGraphGO.GetComponent<Window_Graph>().ShowGraph(avgLine, lineColoraverage, -1, null, null);
        return avgLine;
    }

    public Tuple<float[],List<int>[]> getStepStatistics(Vector2Int stepAttempt, Dictionary<int, bool> currDic)
    {
        Dictionary<int, bool> newDictionary = new Dictionary<int, bool>();
        float[] stepStatistic = new float[] { 0, 0, 0 };//respectivly room, wall, out of border
        List<int>[] alisIDList = new List<int>[3] { new List<int>(), new List<int>(), new List<int>() };

        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
        {
            if (currDic.ContainsKey(m.Key))
            {

                if (!Utility.in_bounds_General(stepAttempt + m.Value.start, m.Value.AliasMap.GetLength(0), m.Value.AliasMap.GetLength(1)))
                {
                    stepStatistic[2]++;
                    alisIDList[2].Add(m.Key);
                }
                else if (m.Value.AliasMap[stepAttempt.x + m.Value.start.x, stepAttempt.y + m.Value.start.y].type != IGenerator.wallChar)
                {
                    stepStatistic[0]++;
                    alisIDList[0].Add(m.Key);
                }
                else if (m.Value.AliasMap[stepAttempt.x + m.Value.start.x, stepAttempt.y + m.Value.start.y].type == IGenerator.wallChar) {
                    stepStatistic[1]++;
                    alisIDList[1].Add(m.Key);
                }

            }

        }

        if (!Utility.in_bounds_General(stepAttempt + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)))
        {
            stepStatistic[2]++;
            alisIDList[2].Add(MainMapGO.GetInstanceID());
        }
        else if (pMan.MapToPlay[stepAttempt.x + pMan.StartCell.x, stepAttempt.y + pMan.StartCell.y].type != IGenerator.wallChar)
        {
            stepStatistic[0]++;
            alisIDList[0].Add(MainMapGO.GetInstanceID());
        }
        else if (pMan.MapToPlay[stepAttempt.x + pMan.StartCell.x, stepAttempt.y + pMan.StartCell.y].type == IGenerator.wallChar)
        {
            stepStatistic[1]++;
            alisIDList[1].Add(MainMapGO.GetInstanceID());
        }

        for (int i = 0; i < stepStatistic.Length; i++)
        {
            stepStatistic[i] /= currDic.Count;
        }

        return new Tuple<float[], List<int>[]>(stepStatistic, alisIDList);
    }

    public List<Tuple<Vector2Int, int>> PrudentAgentPath()
    {
        
        
        Dictionary<int, bool> playerDictionary = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
            playerDictionary.Add(m.Key, true);
        playerDictionary.Add(MainMapGO.GetInstanceID(), true);//?

        
        TreeNodeComplete<Vector2Int, Dictionary<int, bool>> root = new TreeNodeComplete<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(Vector2Int.zero, playerDictionary), 0, null);
        TreeNodeComplete<Vector2Int, Dictionary<int, bool>>  currCell = root;
        HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>() { root.NodeKeyValue.Key};

        List<Tuple<Vector2Int,int>> pathAgentWDicCount = new List<Tuple<Vector2Int, int>>();
        pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(root.NodeKeyValue.Key, playerDictionary.Count));

        bool allVisitedAndOnlyWalls = false;
        while (currCell.NodeKeyValue.Key + pMan.StartCell != pMan.EndCell && pathAgentWDicCount.Last().Item2 > 1)
        {
            //current treenode build so no init operations
            Vector2Int nextMove = new Vector2Int();

            //if out map or on Wall found...
            if (!Utility.in_bounds_General(currCell.NodeKeyValue.Key + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) || 
                pMan.MapToPlay[currCell.NodeKeyValue.Key.x + pMan.StartCell.x, currCell.NodeKeyValue.Key.y + pMan.StartCell.y].type == IGenerator.wallChar)
            {
                //pathDic already updated by the cycle before for currcell
                //visit cell updated
                //path updated: we trace the fact that we go back to the room from wall or out of map cells
                currCell = currCell.ParentNode;
                visitedCells.Add(currCell.NodeKeyValue.Key);
                pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                //currCell = currCell.ParentNode;
            }
            else//is a room cell current so seek for unvisited child using the smart policy
            {
                HashSet<Vector2Int> NeighCells = new HashSet<Vector2Int>(Utility.getAllNeighboursWOBoundCheck_General(currCell.NodeKeyValue.Key, pMan.GridType));
                //List<Vector2Int> cellList = new List<Vector2Int>(Cells);
                Dictionary<Vector2Int, Tuple<float[], List<int>[]>> StatisticsOnAttempts = new Dictionary<Vector2Int, Tuple<float[], List<int>[]>>();

                //if (currCell.ParentNode != null)
                    NeighCells.ExceptWith(visitedCells);

                if (NeighCells.Count != 0)
                {
                    //APPLY CUSTOM HEURISTIC----
                    foreach (var moveAttempt in NeighCells)
                    {
                        StatisticsOnAttempts.Add(moveAttempt, getStepStatistics(moveAttempt, playerDictionary));
                    }

                    List<Vector2Int> maxList = new List<Vector2Int>();
                    float max = 0;

                    foreach (var attemptStatistic in StatisticsOnAttempts)
                    {
                        float newMax = Math.Max(attemptStatistic.Value.Item1[0], attemptStatistic.Value.Item1[2]);
                        if (newMax > max)
                        {
                            max = newMax;
                            maxList.Clear();
                        }

                        if (newMax == max)
                            maxList.Add(attemptStatistic.Key);
                    }


                    if (maxList.Count == 1)
                    {

                        nextMove = maxList[0];
                    }
                    else
                    {
                        int minDstOfNxtMove = ParameterManager.Instance.GridType.heuristic(pMan.StartCell, pMan.EndCell);

                        foreach (var nextMvs in maxList)
                        {
                            List<int> mapsID = new List<int>();
                            /*foreach (var f in StatisticsOnAttempts[nextMvs].Item1) suppose to delete
                                if (f == max)
                                    mapsID.Add(StatisticsOnAttempts[nextMvs].Item2[i]);*/

                            for (int i = 0; i < StatisticsOnAttempts[nextMvs].Item1.Length; i++)
                            {
                                if (StatisticsOnAttempts[nextMvs].Item1[i] == max)
                                    mapsID.AddRange(StatisticsOnAttempts[nextMvs].Item2[i]);
                            }

                            int finalH = 0;

                            foreach (var id in mapsID)
                            {
                                if (aliasList.dictionaryMap.ContainsKey(id))
                                    finalH += ParameterManager.Instance.GridType.heuristic(aliasList.dictionaryMap[id].start + (nextMvs), aliasList.dictionaryMap[id].end);
                                else if (id == MainMapGO.GetInstanceID())
                                    finalH += ParameterManager.Instance.GridType.heuristic(pMan.StartCell + (nextMvs), pMan.EndCell);
                                else
                                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "MapID not found during agent path building.");
                            }
                            if (mapsID.Count>0)
                            {
                                finalH /= mapsID.Count;
                            }
                            else
                            {
                                allVisitedAndOnlyWalls = true;
                            }
                            

                            if (finalH < minDstOfNxtMove)
                            {
                                minDstOfNxtMove = finalH;
                                nextMove = nextMvs;
                            }

                        }

                    }
                    //END CUSTOM HEURISTIC---
                    if (allVisitedAndOnlyWalls)
                    {
                        currCell = currCell.ParentNode;
                        visitedCells.Add(currCell.NodeKeyValue.Key);
                        pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                    }
                    else
                    {
                        playerDictionary = updateAliasDictionaryWReal(nextMove, playerDictionary);
                        currCell = new TreeNodeComplete<Vector2Int, Dictionary<int, bool>>(nextMove, playerDictionary, currCell.nodeDepth + 1, currCell);
                        visitedCells.Add(currCell.NodeKeyValue.Key);
                        pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                    }

                    allVisitedAndOnlyWalls = false;
                }
                else
                {
                    //DEADENDFOUND BACKTRACK: go back from room deadend to parent
                    /*visitedCells.Add(currCell.ParentNode.NodeKeyValue.Key);
                    pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                    currCell = currCell.ParentNode;*/

                    currCell = currCell.ParentNode;
                    visitedCells.Add(currCell.NodeKeyValue.Key);
                    pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));

                }
                
            }

            //Update everything wrt new move found

        }


        Vector2Int[] stepsFromPlayerToExit = MapEvaluator.Search_AStar(pMan.MapToPlay, pMan.GridType, pathAgentWDicCount.Last().Item1 + pMan.StartCell, pMan.EndCell);
        List<Vector2Int> tmpArr = new List<Vector2Int>(stepsFromPlayerToExit);
        tmpArr.Reverse();
        stepsFromPlayerToExit = tmpArr.ToArray().Skip(1).ToArray();

        List<float> chartAgentLine = new List<float>();
        List<Vector2Int> agentPath = new List<Vector2Int>();
        foreach (var t in pathAgentWDicCount)
        {
            agentPath.Add(t.Item1);
            chartAgentLine.Add(t.Item2 - 1);
        }
        foreach (var t in stepsFromPlayerToExit)
        {
            agentPath.Add(t - pMan.StartCell);
            chartAgentLine.Add(0);
        }
        agentPath.Reverse();
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
                agentPath.ToArray(), GeneratorUIManager.Instance.originUIMap,
                ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
        LineGO.GetComponent<UILineRenderer>().color = lineColorPrudentAgent;

        ChartLines.Add(new Tuple<List<float>, Color>(chartAgentLine, lineColorPrudentAgent));
        LinesGO.Add(LineGO);
        refreshToggle(LineGO);
        return pathAgentWDicCount;
    }


    private List<TreeNode<Vector2Int, Dictionary<int, bool>>> ConstructAliasTreeWDuplicates()
    {
        Dictionary<int, bool> initDic = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
            initDic.Add(m.Key, true);
        //set root node
        root = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(Vector2Int.zero, initDic), 0, null);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        List<TreeNode<Vector2Int, Dictionary<int, bool>>> leafNodes = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        Queue<TreeNode<Vector2Int, Dictionary<int, bool>>> frontier = new Queue<TreeNode<Vector2Int, Dictionary<int, bool>>>();

        frontier.Enqueue(root);
        visitedNodes.Add(Vector2Int.zero);
        //leafNodes.Add(root);
        TreeNode<Vector2Int, Dictionary<int, bool>> minLeafNode = root;

        LeavesSet = new Dictionary<Vector2Int, TreeNode<Vector2Int, Dictionary<int, bool>>>();
        DupLeavesList = new List<TreeNode<Vector2Int, Dictionary<int, bool>>>();
        LeavesSet.Add(root.NodeKeyValue.Key, root);

        while (frontier.Count > 0)
        {
            TreeNode<Vector2Int, Dictionary<int, bool>> CurrentNode = frontier.Dequeue();
            Vector2Int[] Cells = Utility.getAllNeighboursWOBoundCheck_General(CurrentNode.NodeKeyValue.Key, pMan.GridType);
            List<Vector2Int> cellList = new List<Vector2Int>(Cells);
            //You can never come from outside the map => except the coming cells, you should have 3 elements with getAllNeighbours and the difference indicates the OoB cells.
            if (CurrentNode.ParentNode != null)
            {
                Vector2Int toRemove = Vector2Int.zero;
                foreach (var c in cellList)
                    if (CurrentNode.ParentNode.NodeKeyValue.Key == c)
                        toRemove = c;

                cellList.Remove(toRemove);
            }


            foreach (var c in cellList)
            {
                //Avoid self loops
                HashSet<Vector2Int> checkLoopSet = new HashSet<Vector2Int>();
                TreeNode<Vector2Int, Dictionary<int, bool>> tmp = CurrentNode;

                while (tmp != null)
                {
                    checkLoopSet.Add(tmp.NodeKeyValue.Key);
                    tmp = tmp.ParentNode;
                }
                //

                if (/*!visitedNodes.Contains(c)*/!checkLoopSet.Contains(c))
                {
                    Dictionary<int, bool> newDictionary = updateAliasDictionary(c, CurrentNode.NodeKeyValue.Value);
                    TreeNode<Vector2Int, Dictionary<int, bool>> Node;

                    if (newDictionary.Count - CurrentNode.NodeKeyValue.Value.Count == 0)
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, CurrentNode.NodeKeyValue.Value), CurrentNode.nodeDepth + 1, CurrentNode);
                    else
                        Node = new TreeNode<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(c, newDictionary), CurrentNode.nodeDepth + 1, CurrentNode);
                    /*
                    if (LeavesSet.ContainsKey(CurrentNode.NodeKeyValue.Key))
                        LeavesSet.Remove(CurrentNode.NodeKeyValue.Key);
                    LeavesSet.Add(Node.NodeKeyValue.Key, Node);
                    */

                    /*
                    if (LeavesSet.ContainsKey(CurrentNode.NodeKeyValue.Key))
                        LeavesSet.Remove(CurrentNode.NodeKeyValue.Key);

                    if (LeavesSet.ContainsKey(Node.NodeKeyValue.Key)) {
                        TreeNode<Vector2Int, Dictionary<int, bool>> duplicate = LeavesSet[Node.NodeKeyValue.Key];
                        if (duplicate.nodeDepth > Node.nodeDepth)
                        {
                            DupLeavesList.Add(Node);
                        }
                            
                        else
                        {
                            DupLeavesList.Add(duplicate);
                            LeavesSet.Remove(Node.NodeKeyValue.Key);
                            LeavesSet.Add(Node.NodeKeyValue.Key, Node);
                        }       
                    }*/

                    if (newDictionary.Count != 0)
                    {
                        if (Utility.in_bounds_General(c + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) &&
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
        if (leafNodes.Count == 0)
        {
            leafNodes.Add(minLeafNode);
        }
        //
        return leafNodes;
    }

    public List<Tuple<Vector2Int, int>> AggressiveAgentPath()
    {


        Dictionary<int, bool> playerDictionary = new Dictionary<int, bool>();
        foreach (KeyValuePair<int, StructuredAlias> m in aliasList.dictionaryMap)
            playerDictionary.Add(m.Key, true);
        playerDictionary.Add(MainMapGO.GetInstanceID(), true);//?


        TreeNodeComplete<Vector2Int, Dictionary<int, bool>> root = new TreeNodeComplete<Vector2Int, Dictionary<int, bool>>(new KeyValuePair<Vector2Int, Dictionary<int, bool>>(Vector2Int.zero, playerDictionary), 0, null);
        TreeNodeComplete<Vector2Int, Dictionary<int, bool>> currCell = root;
        HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>() { root.NodeKeyValue.Key };

        List<Tuple<Vector2Int, int>> pathAgentWDicCount = new List<Tuple<Vector2Int, int>>();
        pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(root.NodeKeyValue.Key, playerDictionary.Count));

        while (currCell.NodeKeyValue.Key + pMan.StartCell != pMan.EndCell && pathAgentWDicCount.Last().Item2 >1)
        {
            //current treenode build so no init operations
            Vector2Int nextMove = new Vector2Int();

            //if out map or on Wall found...
            if (!Utility.in_bounds_General(currCell.NodeKeyValue.Key + pMan.StartCell, pMan.MapToPlay.GetLength(0), pMan.MapToPlay.GetLength(1)) ||
                pMan.MapToPlay[currCell.NodeKeyValue.Key.x + pMan.StartCell.x, currCell.NodeKeyValue.Key.y + pMan.StartCell.y].type == IGenerator.wallChar)
            {
                //pathDic already updated by the cycle before for currcell
                //visit cell updated
                //path updated: we trace the fact that we go back to the room from wall or out of map cells
                currCell = currCell.ParentNode;
                visitedCells.Add(currCell.NodeKeyValue.Key);
                pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                //currCell = currCell.ParentNode;
            }
            else//is a room cell current so seek for unvisited child using the smart policy
            {
                HashSet<Vector2Int> NeighCells = new HashSet<Vector2Int>(Utility.getAllNeighboursWOBoundCheck_General(currCell.NodeKeyValue.Key, pMan.GridType));
                //List<Vector2Int> cellList = new List<Vector2Int>(Cells);
                Dictionary<Vector2Int, Tuple<float[], List<int>[]>> StatisticsOnAttempts = new Dictionary<Vector2Int, Tuple<float[], List<int>[]>>();

                //if (currCell.ParentNode != null)
                    NeighCells.ExceptWith(visitedCells);

                if (NeighCells.Count != 0)
                {
                    //APPLY CUSTOM HEURISTIC----
                    foreach (var moveAttempt in NeighCells)
                    {
                        StatisticsOnAttempts.Add(moveAttempt, getStepStatistics(moveAttempt, playerDictionary));
                    }

                    List<Vector2Int> minList = new List<Vector2Int>();
                    float min = 1;

                    foreach (var attemptStatistic in StatisticsOnAttempts)
                    {
                        //float newMax = Math.Min(attemptStatistic.Value.Item1[0], Math.Min(attemptStatistic.Value.Item1[1], attemptStatistic.Value.Item1[2]));
                        float[] list = { attemptStatistic.Value.Item1[0], attemptStatistic.Value.Item1[1], attemptStatistic.Value.Item1[2] };
                        list = list.Where(i => i != 0).ToArray();

                        float newMin = list.Min();
                        if (newMin < min)
                        {
                            min = newMin;
                            minList.Clear();
                        }

                        if (newMin == min)
                            minList.Add(attemptStatistic.Key);
                    }


                    if (minList.Count == 1)
                    {

                        nextMove = minList[0];
                    }
                    else
                    {
                        int minDstOfNxtMove = ParameterManager.Instance.GridType.heuristic(pMan.StartCell, pMan.EndCell);

                        foreach (var nextMvs in minList)
                        {
                            List<int> mapsID = new List<int>();
                            /*foreach (var f in StatisticsOnAttempts[nextMvs].Item1) suppose to delete
                                if (f == max)
                                    mapsID.Add(StatisticsOnAttempts[nextMvs].Item2[i]);*/

                            for (int i = 0; i < StatisticsOnAttempts[nextMvs].Item1.Length; i++)
                            {
                                if (StatisticsOnAttempts[nextMvs].Item1[i] == min)
                                    mapsID.AddRange(StatisticsOnAttempts[nextMvs].Item2[i]);
                            }

                            int finalH = 0;

                            foreach (var id in mapsID)
                            {
                                if (aliasList.dictionaryMap.ContainsKey(id))
                                    finalH += ParameterManager.Instance.GridType.heuristic(aliasList.dictionaryMap[id].start + (nextMvs), aliasList.dictionaryMap[id].end);
                                else if (id == MainMapGO.GetInstanceID())
                                    finalH += ParameterManager.Instance.GridType.heuristic(pMan.StartCell + (nextMvs), pMan.EndCell);
                                else
                                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "MapID not found during agent path building.");
                            }
                            finalH /= mapsID.Count;

                            if (finalH < minDstOfNxtMove)
                            {
                                minDstOfNxtMove = finalH;
                                nextMove = nextMvs;
                            }

                        }

                    }
                    //END CUSTOM HEURISTIC---

                    playerDictionary = updateAliasDictionaryWReal(nextMove, playerDictionary);
                    currCell = new TreeNodeComplete<Vector2Int, Dictionary<int, bool>>(nextMove, playerDictionary, currCell.nodeDepth + 1, currCell);
                    visitedCells.Add(currCell.NodeKeyValue.Key);
                    pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                }
                else
                {
                    //DEADENDFOUND BACKTRACK: go back from room deadend to parent
                    /*visitedCells.Add(currCell.ParentNode.NodeKeyValue.Key);
                    pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));
                    currCell = currCell.ParentNode;*/

                    currCell = currCell.ParentNode;
                    visitedCells.Add(currCell.NodeKeyValue.Key);
                    pathAgentWDicCount.Add(new Tuple<Vector2Int, int>(currCell.NodeKeyValue.Key, playerDictionary.Count));

                }

            }

            //Update everything wrt new move found

        }

        Vector2Int[] stepsFromPlayerToExit = MapEvaluator.Search_AStar(pMan.MapToPlay, pMan.GridType, pathAgentWDicCount.Last().Item1+ pMan.StartCell, pMan.EndCell);
        List<Vector2Int> tmpArr = new List<Vector2Int>(stepsFromPlayerToExit);
        tmpArr.Reverse();
        stepsFromPlayerToExit = tmpArr.ToArray().Skip(1).ToArray();

        List<float> chartAgentLine = new List<float>();
        List<Vector2Int> agentPath = new List<Vector2Int>();
        foreach (var t in pathAgentWDicCount)
        {
            agentPath.Add(t.Item1);
            chartAgentLine.Add(t.Item2 - 1);
        }
        foreach (var t in stepsFromPlayerToExit)
        {
            agentPath.Add(t-pMan.StartCell);
            chartAgentLine.Add(0);
        }
        agentPath.Reverse();
        GameObject LineGO = Instantiate(LineUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Utility.displaySegmentedLineUI(LineGO, MainMapGO.transform.Find("BorderMask/Content").GetComponent<RectTransform>(),
                agentPath.ToArray(), GeneratorUIManager.Instance.originUIMap,
                ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.x, ParameterManager.Instance.GridType.TilePrefab.GetComponent<RectTransform>().sizeDelta.y);
        LineGO.GetComponent<UILineRenderer>().color = lineColorAggressiveAgent;

        ChartLines.Add(new Tuple<List<float>, Color>(chartAgentLine, lineColorAggressiveAgent));
        LinesGO.Add(LineGO);
        refreshToggle(LineGO);
        return pathAgentWDicCount;
    }
}
