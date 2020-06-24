using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Priority_Queue;

public struct StructuredAlias
{
    public TileObject[,] AliasMap;
    public Vector2Int start;
    public Vector2Int end;
    public float similarityDistance;

    public StructuredAlias(TileObject[,] a, Vector2Int b, Vector2Int c, float d)
    {
        AliasMap = a;
        start = b;
        end = c;
        similarityDistance = d;
    }

    public StructuredAlias(TileObject[,] a)
    {
        AliasMap = a;
        start = new Vector2Int(-1,-1);
        end = new Vector2Int(-1, -1);
        similarityDistance = -1;
    }
}

public class AliasGeneratorManager : MonoBehaviour/*Singleton<AliasGeneratorManager>*/
{
    static readonly int MAX_ALIAS = 1000;
    static readonly int MAX_ALIASMASKS = 5;

    static readonly int MAX_OPT_ALIAS;//should not be less than BATCH_ALIASES: ACTUALLY NOT USED!

    public int BatchAliasNumber = 5;
    static System.Random pRNG_Alias = new System.Random(0);

    public GameObject[] AliasToggleAnimations;
    public RectTransform[] AliasDragAreas;
    private HashSet<Vector2Int>[] K_CollisionSet;
    private HashSet<Vector2Int> BaseAliasCollisionMask;
    public GameObject AliasPrefab;
    public GameObject ToggleLinesContainer;
    private TileObject[,] mainMap;
    private ITypeGrid gridType;
    private GeneratorManager genMan;
    private SimplePriorityQueue<TileObject[,]> SimilarMapsQueue;

    public static AliasGeneratorManager Instance = null;
    protected AliasGeneratorManager() { }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        genMan = GeneratorManager.Instance;
    }

    public void AliasGenUIToggleAnim()
    {
        foreach (GameObject g in AliasToggleAnimations)
        {
            GeneratorUIManager.Instance.toggleUIAnimator(g.GetComponent<Animator>());
        }
        genMan.inAliasGenerator = !genMan.inAliasGenerator;
    }

    public void GenerateAndTestAliasMaps()
    {
        
        
        Vector2Int startMainMap = ParameterManager.Instance.StartCell;
        int width = ParameterManager.Instance.MapToPlay.GetLength(0);
        int height = ParameterManager.Instance.MapToPlay.GetLength(1);


        BaseAliasCollisionMask = getMainMapKMaxMinCells(mainMap, gridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.minStepsSolution, startMainMap, 0f);
        HashSet<Vector2Int>[,] AliasCollisionMaskMatrix = new HashSet<Vector2Int>[ParameterManager.Instance.maxStepsSolution - ParameterManager.Instance.minStepsSolution, MAX_ALIASMASKS];

        if (ParameterManager.Instance.maxStepsSolution > ParameterManager.Instance.minStepsSolution)
        {
            for (int h = 0; h < ParameterManager.Instance.maxStepsSolution - ParameterManager.Instance.minStepsSolution; h++)
            {
                for (int k = 0; k < MAX_ALIASMASKS; k++)
                {
                    AliasCollisionMaskMatrix[h, k] = getMainMapKMaxMinCells(mainMap, gridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.minStepsSolution + h + 1, startMainMap, ((k + 1) * 1.0f) / (MAX_ALIASMASKS)); ;
                }
            }
        }
        else
        {
            AliasCollisionMaskMatrix = null;
        }

        //Map initialization.
        int i = 0;

        

        Vector2Int startAlias = ParameterManager.Instance.StartCell;
        Vector2Int endAlias = ParameterManager.Instance.EndCell;

        
        while (i < MAX_ALIAS)
        {
            //define here the width, height, start and end  of the chosen map
            TileObject[,] aliasMap = new TileObject[width, height];

            genMan.connectedGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);
            genMan.cellularAutomataGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);
            genMan.primGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);

            switch (i % 3)
            {
                case 0:
                    aliasMap = genMan.connectedGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble());
                    break;
                case 1:
                    aliasMap = genMan.cellularAutomataGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble(), 1 + i % 5, 5, i % 2 == 1, 0, 0);
                    break;
                case 2:
                    aliasMap = genMan.primGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble());
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "AliasManager: no map generator found.");
                    break;
            }

            //Apply mask...
            //from zero to kMinStep and up to some cells at kMaxStep 
            if (ParameterManager.Instance.maxStepsSolution > ParameterManager.Instance.minStepsSolution)
            {
                for (int x = 0; x < aliasMap.GetLength(0); x++)
                {
                    for (int y = 0; y < aliasMap.GetLength(1); y++)
                    {
                        if (Utility.in_bounds_General(new Vector2Int(startMainMap.x + x, startMainMap.y + y), width, height))
                            if (AliasCollisionMaskMatrix[i % AliasCollisionMaskMatrix.GetLength(0), i % AliasCollisionMaskMatrix.GetLength(1)].Contains(new Vector2Int(x - startAlias.x, y - startAlias.y)))
                                aliasMap[x, y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + x, startMainMap.y + y].type;
                    }
                }
            }
            else
            {
                //only from zero to kMinStep
                /*
                for (int x = 0; x < aliasMap.GetLength(0); x++)
                {
                    for (int y = 0; y < aliasMap.GetLength(1); y++)
                    {
                        if (Utility.in_bounds_General(new Vector2Int(startMainMap.x + x, startMainMap.y + y), width, height) && BaseAliasCollisionMask.Contains(new Vector2Int(x - startAlias.x, y - startAlias.y)))
                            aliasMap[x, y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + x, startMainMap.y + y].type;
                    }
                }*/
                foreach(var m in BaseAliasCollisionMask)
                {
                    if(Utility.in_bounds_General(new Vector2Int(startMainMap.x + m.x, startMainMap.y + m.y), mainMap.GetLength(0), mainMap.GetLength(1)) &&
                        Utility.in_bounds_General(new Vector2Int(startAlias.x + m.x, startAlias.y + m.y), aliasMap.GetLength(0), aliasMap.GetLength(1)))
                        aliasMap[startAlias.x + m.x, startAlias.y + m.y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + m.x, startMainMap.y + m.y].type;
                }
            }

            if (MapEvaluator.isEndReachable(aliasMap, gridType, startAlias, endAlias, ParameterManager.Instance.allowAutosolverForAlias).First() == endAlias)
            {//if the map has a path from start to end, add it
                float dst = MapEvaluator.BinaryMapSimilarity(mainMap, aliasMap, startMainMap, startAlias);
                /*
                int mapWCount = 0;
                int aliasWCount = 0;
                for (int h = 0; h < mainMap.GetLength(0); h++)
                {
                    for (int k = 0; k < mainMap.GetLength(1); k++)
                    {
                        if (mainMap[h, k].type == IGenerator.wallChar)
                            mapWCount++;
                        if (aliasMap[h, k].type == IGenerator.wallChar)
                            aliasWCount++;
                    }
                }

                dst = dst + Math.Abs(mapWCount - aliasWCount);
                */
                SimilarMapsQueue.Enqueue(aliasMap, dst);
            }
            i++;
        }
    }

    public void CollabGameGeneration()
    {

        mainMap = ParameterManager.Instance.MapToPlay;
        gridType = ParameterManager.Instance.GridType;
        SimilarMapsQueue = new SimplePriorityQueue<TileObject[,]>();

        K_CollisionSet = MapEvaluator.BuildKCollisionVec(mainMap, gridType, ParameterManager.Instance.StartCell, Mathf.Max(ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.maxStepsSolution));
        
        if (ParameterManager.Instance.isOptimizerOn) {
            List<TileObject[,]> alises = new List<TileObject[,]>();
            BaseAliasCollisionMask = getMainMapKMaxMinCells(mainMap, gridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.StartCell, 0f);

            foreach (var a in AliasGeneratorManager.Instance.GetComponent<AliasGameEvaluator>().AliasGameOptimizerHandler())
                SimilarMapsQueue.Enqueue(a.Value.AliasMap, a.Value.similarityDistance);
        }
            else
            GenerateAndTestAliasMaps();


        int i = 0;
        Vector2Int startMainMap = ParameterManager.Instance.StartCell;
        TileObject[,] tmpStrAlias;
        while (i< ParameterManager.Instance.aliasNum)
        {
            if (ParameterManager.Instance.considerSimilar)
            {
                float dst = SimilarMapsQueue.GetPriority(SimilarMapsQueue.First());
                tmpStrAlias = SimilarMapsQueue.Dequeue();

                Utility.renderAliasOnUI(AliasDragAreas[0].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst), AliasPrefab, true);
                i++;
            }

            if (ParameterManager.Instance.considerNovelty)
            {
                tmpStrAlias = SimilarMapsQueue.Last();
                float dst = SimilarMapsQueue.GetPriority(tmpStrAlias);
                SimilarMapsQueue.Remove(tmpStrAlias);

                Utility.renderAliasOnUI(AliasDragAreas[0].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst), AliasPrefab, true);
                i++;
            }
            
        }

        i = 0;
        while (SimilarMapsQueue.Count > 0 && i < BatchAliasNumber )
        {
            float dst = SimilarMapsQueue.GetPriority(SimilarMapsQueue.First());
            tmpStrAlias = SimilarMapsQueue.Dequeue();

            Utility.renderAliasOnUI(AliasDragAreas[1].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst),AliasPrefab, true);
            i++;
        }

        i = 0;
        while (SimilarMapsQueue.Count > 0 && i < BatchAliasNumber)
        {
            tmpStrAlias = SimilarMapsQueue.Last();
            float dst = SimilarMapsQueue.GetPriority(tmpStrAlias);
            SimilarMapsQueue.Remove(tmpStrAlias);

            Utility.renderAliasOnUI(AliasDragAreas[2].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst),AliasPrefab, true);
            
            i++;
        }
        
        //reset horizontal and vertical bars if exists
        ScrollRect sR = AliasDragAreas[0].GetComponent<ScrollRect>();
        if (sR != null)
        {
            Scrollbar hSb = sR.horizontalScrollbar;
            Scrollbar vSb = sR.verticalScrollbar;

            if (hSb != null)
                hSb.value = .99f;

            if (vSb != null)
                vSb.value = .99f;
        }

        gameObject.GetComponent<AliasGameEvaluator>().AliasGameEvaluatorHandler();
    }

    public void refershSimilarMapsBatch()
    {
        int i = 0;
        TileObject[,] tmpStrAlias;
        Vector2Int startMainMap = ParameterManager.Instance.StartCell;

        GeneratorUIManager.Instance.deleteMapOnUI(AliasDragAreas[1].GetChild(0));
        AliasDragAreas[1].GetComponent<MapListManager>().dictionaryMap.Clear();

        if(SimilarMapsQueue.Count<=0)
            GenerateAndTestAliasMaps();

        while (i < BatchAliasNumber)
        {
            float dst = SimilarMapsQueue.GetPriority(SimilarMapsQueue.First());
            tmpStrAlias = SimilarMapsQueue.Dequeue();

            Utility.renderAliasOnUI(AliasDragAreas[1].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst), AliasPrefab, true);
            i++;
        }
    }

    public void refershNoveltyMapsBatch()
    {
        int i = 0;
        TileObject[,] tmpStrAlias;
        Vector2Int startMainMap = ParameterManager.Instance.StartCell;

        GeneratorUIManager.Instance.deleteMapOnUI(AliasDragAreas[2].GetChild(0));
        AliasDragAreas[2].GetComponent<MapListManager>().dictionaryMap.Clear();

        if (SimilarMapsQueue.Count <= 0)
            GenerateAndTestAliasMaps();

        while (i < BatchAliasNumber)
        {
            tmpStrAlias = SimilarMapsQueue.Last();
            float dst = SimilarMapsQueue.GetPriority(tmpStrAlias);
            SimilarMapsQueue.Remove(tmpStrAlias);

            Utility.renderAliasOnUI(AliasDragAreas[2].GetChild(0).GetComponent<RectTransform>(), ParameterManager.Instance.GridType, new StructuredAlias(tmpStrAlias, startMainMap, ParameterManager.Instance.EndCell, dst), AliasPrefab, true);

            i++;
        }
    }

    //Needs K_CollisionSet to be relative to the (0,0) cell!
    private HashSet<Vector2Int> getMainMapKMaxMinCells(TileObject[,] mainMap, ITypeGrid TypeGrid, int kMinStep, int kMaxStep, Vector2Int startCell, float percBacktrack)
    {
        HashSet<Vector2Int> UnionCollisionSet = new HashSet<Vector2Int>();
        if (K_CollisionSet == null || K_CollisionSet.Length == 0)
            ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR,"No collision set for the main map has been built.");

        int i = 0;
        int width = mainMap.GetLength(0);
        int height = mainMap.GetLength(1);

        //Ensure same cells in minimum kMinStep steps: EVERYTHING IS WRT ORIGIN CELL
        while (i <= kMinStep)
        {
            foreach(Vector2Int p in K_CollisionSet[i])
            {
                //Vector2Int x = p + startCell; 
                //if (Utility.in_bounds_General(p, width, height))
                    UnionCollisionSet.Add(p);
            }
            
            i++;
        }

        //Ensure one or more paths from kMinStep up to kMaxStep: WE DON'T USE IT. IF USE IT CONSIDER WRT TO ORIGIN
        if (kMaxStep > kMinStep)
        {
            HashSet<Vector2Int> toBacktrack= new HashSet<Vector2Int>();
            HashSet<Vector2Int> nextBacktrack = new HashSet<Vector2Int>();
            foreach (Vector2Int p in K_CollisionSet[kMaxStep])
            {
                Vector2Int x = p + startCell;
                if (Utility.in_bounds_General(x, width, height))
                    toBacktrack.Add(x);
            }

            if (toBacktrack.Count > 0)
            {
                int toRemove = (int)((1- percBacktrack)* (float)toBacktrack.Count);
                
                
                //remove from toBacktrack in order to have percbacktrack elements in the set
                for (i = 0; i < toRemove; i++)
                {
                    toBacktrack.Remove(toBacktrack.ElementAt(pRNG_Alias.Next(0, toBacktrack.Count)));
                }

                UnionCollisionSet.UnionWith(toBacktrack);

                //backtrack and reconstruct the paths
                for(i=0; i<kMaxStep-kMinStep; i++)
                {
                    
                    foreach(Vector2Int v in toBacktrack)
                    {
                        HashSet<Vector2Int> a = new HashSet<Vector2Int>(Utility.getAllNeighbours_General(v, TypeGrid, width, height));
                        HashSet<Vector2Int> TmpSet = new HashSet<Vector2Int>(K_CollisionSet[kMaxStep - i - 1]);
                        TmpSet.IntersectWith(a);

                        if (TmpSet.Count == 1) {
                            UnionCollisionSet.UnionWith(TmpSet);
                            nextBacktrack.UnionWith(TmpSet);
                        }
                            
                    }

                    foreach (Vector2Int v in toBacktrack)
                    {
                        if (!nextBacktrack.Contains(v))
                        {
                            HashSet<Vector2Int> a = new HashSet<Vector2Int>(Utility.getAllNeighbours_General(v, TypeGrid, width, height));
                            HashSet<Vector2Int> TmpSet = new HashSet<Vector2Int>(K_CollisionSet[kMaxStep - i - 1]);
                            TmpSet.IntersectWith(a);
                            TmpSet.Except(UnionCollisionSet);

                            Vector2Int e = TmpSet.ElementAt(pRNG_Alias.Next(0, TmpSet.Count));
                            UnionCollisionSet.Add(e);
                            nextBacktrack.Add(e);
                        }
                    }
                    toBacktrack = new HashSet<Vector2Int>(nextBacktrack);
                }
            }
        }

        return UnionCollisionSet;
    }

    /*
    public void renderAliasOnUI(RectTransform container, ITypeGrid typeGrid, StructuredAlias alias, bool attachMapMetrics)
    {
        GameObject AliasGO = Instantiate(AliasPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        container.parent.GetComponent<MapListManager>().addMapToDictionary(alias.AliasMap,AliasGO.GetInstanceID());

        AliasGO.transform.SetParent(container, false);
        Transform t = AliasGO.transform.Find("BorderMask/Content");
        RectTransform contentRect = t.GetComponent<RectTransform>();
        RectTransform prefabRect = typeGrid.TilePrefab.GetComponent<RectTransform>();
        initAliasGameObject(AliasGO);
        GeneratorUIManager.Instance.DisplayMap(alias.AliasMap, t, ParameterManager.Instance.GridType);

        GeneratorUIManager.Instance.ScaleToFitContainer(contentRect, new Rect(Vector2.zero, container.GetComponent<GridLayoutGroup>().cellSize));

        if (attachMapMetrics)
            AliasGO.GetComponentInChildren<HoverDisplayText>().textToDisplay = MapEvaluator.aggregateAliasDataMap(MapEvaluator.computeMetrics(alias.AliasMap, typeGrid, alias.start, alias.end), alias.similarityDistance);
        else
            AliasGO.GetComponentInChildren<HoverDisplayText>().gameObject.SetActive(false);
    }

    private void initAliasGameObject(GameObject AliasGO)
    {
        HoverDisplayText scriptHoverDisplay =  AliasGO.GetComponentInChildren<HoverDisplayText>();
        DragHandler dHand = AliasGO.GetComponentInChildren<DragHandler>();
        scriptHoverDisplay.DialogBoxInfo = GameObject.FindGameObjectWithTag("DialogBox");

        dHand.OriginalParent = dHand.ToMoveGameObj.transform.parent.parent.gameObject; 
        dHand.canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }
    */

    public void backToMapGeneratorHandler()
    {
        foreach (RectTransform rect in AliasDragAreas) { 
            GeneratorUIManager.Instance.deleteMapOnUI(rect.GetChild(0));
            rect.GetComponent<MapListManager>().dictionaryMap.Clear();
        }

        genMan.inAliasGenerator = false;
        K_CollisionSet = null;
        SimilarMapsQueue = null;

        foreach (var t in ToggleLinesContainer.GetComponentsInChildren<Toggle>())
            t.isOn = true;

        deleteBestWorstUILines();

        foreach (Transform child in GeneratorManager.Instance.Content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        GeneratorUIManager.Instance.gameObject.GetComponent<UIParametersValueChange>().refreshUIParams();
    }

    public void deleteBestWorstUILines()
    {
        Color defaultLineColor = GeneratorUIManager.Instance.LineUIPrefab.GetComponent<UnityEngine.UI.Extensions.UILineRenderer>().color;
        foreach (var lineGO in genMan.MapHolder.transform.Find("BorderMask/Content").GetComponentsInChildren<UnityEngine.UI.Extensions.UILineRenderer>())
            if (lineGO.color != defaultLineColor)
                Destroy(lineGO.gameObject);
    }

    public Dictionary<int, StructuredAlias>.ValueCollection generateAliasOnTheFly(){
        GenerateAndTestAliasMaps();
        Dictionary<int, StructuredAlias> dic = new Dictionary<int, StructuredAlias>();

        int i = UnityEngine.Random.Range(3, 7); //try to change

        while (i > 0)
        {
            dic.Add(i, new StructuredAlias(SimilarMapsQueue.Dequeue()));
            i--;
        }

        return dic.Values;

    }

    public Dictionary<int, StructuredAlias> GenerateNRandomAliasFromRealMap(StructuredAlias realMap, int N)
    {
        mainMap = realMap.AliasMap;
        gridType = ParameterManager.Instance.GridType;
        Vector2Int startMainMap = realMap.start;
        int width = realMap.AliasMap.GetLength(0);
        int height = realMap.AliasMap.GetLength(1);

        //Map initialization.
        int i = 0;

        Dictionary<int, StructuredAlias> AliasBatch = new Dictionary<int, StructuredAlias>();

        Vector2Int startAlias = realMap.start;
        Vector2Int endAlias = realMap.end;
        SimplePriorityQueue<TileObject[,]> tmpPQ = new SimplePriorityQueue<TileObject[,]>();

        while (i < N/*MAX_OPT_ALIAS*/)
        {
            //define here the width, height, start and end  of the chosen map
            TileObject[,] aliasMap = new TileObject[width, height];

            genMan.connectedGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);
            genMan.cellularAutomataGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);
            genMan.primGenerator.setBaseGeneratorParameters(gridType, width, height, startAlias, endAlias, i, false);

            switch (i % 3)
            {
                case 0:
                    aliasMap = genMan.connectedGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble());
                    break;
                case 1:
                    aliasMap = genMan.cellularAutomataGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble(), 1 + pRNG_Alias.Next(0,6), 5, i % 2 == 1, 0, 0);
                    break;
                case 2:
                    aliasMap = genMan.primGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble());
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "AliasManager: no map generator found.");
                    break;
            }

            foreach (var m in BaseAliasCollisionMask)
            {
                if (Utility.in_bounds_General(new Vector2Int(startMainMap.x + m.x, startMainMap.y + m.y), mainMap.GetLength(0), mainMap.GetLength(1)) &&
                    Utility.in_bounds_General(new Vector2Int(startAlias.x + m.x, startAlias.y + m.y), aliasMap.GetLength(0), aliasMap.GetLength(1)))
                    aliasMap[startAlias.x + m.x, startAlias.y + m.y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + m.x, startMainMap.y + m.y].type;
            }

            if (MapEvaluator.isEndReachable(aliasMap, gridType, startAlias, endAlias, ParameterManager.Instance.allowAutosolverForAlias).First() == endAlias)
            {
                //if the map has a path from start to end, add it
                float dst = MapEvaluator.BinaryMapSimilarity(mainMap, aliasMap, startMainMap, startAlias);
                /*
                int mapWCount = 0;
                int aliasWCount = 0;
                for (int h = 0; h < mainMap.GetLength(0); h++)
                {
                    for (int k = 0; k < mainMap.GetLength(1); k++)
                    {
                        if (mainMap[h, k].type == IGenerator.wallChar)
                            mapWCount++;
                        if (aliasMap[h, k].type == IGenerator.wallChar)
                            aliasWCount++;
                    }
                }

                dst = dst + Math.Abs(mapWCount - aliasWCount);
                */
                tmpPQ.Enqueue(aliasMap, dst);
                
                
                i++;
            }
            
        }

        i = 0;
        TileObject[,] tmpStrAlias;
        while (i < N)
        {
            if (ParameterManager.Instance.considerSimilar)
            {
                float dst = tmpPQ.GetPriority(tmpPQ.First());
                tmpStrAlias = tmpPQ.Dequeue();

                AliasBatch.Add(Guid.NewGuid().GetHashCode(), new StructuredAlias(tmpStrAlias, startAlias, endAlias, dst));

                i++;
            }

            if (ParameterManager.Instance.considerNovelty)
            {
                tmpStrAlias = tmpPQ.Last();
                float dst = tmpPQ.GetPriority(tmpStrAlias);
                tmpPQ.Remove(tmpStrAlias);

                AliasBatch.Add(Guid.NewGuid().GetHashCode(), new StructuredAlias(tmpStrAlias, startAlias, endAlias, dst));

                i++;
            }
            
        }

        return AliasBatch;
    }
}
