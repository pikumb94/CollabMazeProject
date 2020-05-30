using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Priority_Queue;

public class AliasGeneratorManager : Singleton<AliasGeneratorManager>
{
    static readonly int MAX_ALIAS = 500;
    static readonly int MAX_ALIASMASKS = 5;

    public int BatchAliasNumber = 5;
    static System.Random pRNG_Alias = new System.Random(0);

    public GameObject[] AliasToggleAnimations;
    public RectTransform[] AliasDragAreas;
    private HashSet<Vector2Int>[] K_CollisionSet;// = MapEvaluator.BuildKCollisionVec(mainMap, TypeGrid, startCell, Mathf.Max(kMinStep, kMaxStep)); //FIND RELATIVE TO THE START CELL 
    public GameObject AliasPrefab;

    private TileObject[,] mainMap;
    private ITypeGrid gridType;
    protected AliasGeneratorManager() { }

    private GeneratorManager genMan;

    private void Awake()
    {
        genMan = GeneratorManager.Instance;
    }

    public void AliasGenUIToggleAnim()
    {
        foreach (GameObject g in AliasToggleAnimations)
        {
            GeneratorUIManager.Instance.toggleUIAnimator(g.GetComponent<Animator>());
        }
    }

    public void CollabGameGeneration()
    {
        mainMap = ParameterManager.Instance.MapToPlay;
        gridType = ParameterManager.Instance.GridType;
        K_CollisionSet = MapEvaluator.BuildKCollisionVec(mainMap, gridType, ParameterManager.Instance.StartCell, Mathf.Max(ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.maxStepsSolution));
        Vector2Int startMainMap = ParameterManager.Instance.StartCell;
        int width = ParameterManager.Instance.MapToPlay.GetLength(0);
        int height = ParameterManager.Instance.MapToPlay.GetLength(1);

        
        HashSet<Vector2Int> BaseAliasCollisionMask = getMainMapKMaxMinCells(mainMap, gridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.minStepsSolution, startMainMap, 0f);
        HashSet<Vector2Int>[,] AliasCollisionMaskMatrix = new HashSet<Vector2Int>[ParameterManager.Instance.maxStepsSolution - ParameterManager.Instance.minStepsSolution, MAX_ALIASMASKS];

        if (ParameterManager.Instance.maxStepsSolution > ParameterManager.Instance.minStepsSolution) {
            for (int h =0 ; h < ParameterManager.Instance.maxStepsSolution - ParameterManager.Instance.minStepsSolution ; h++)
            {
                for (int k = 0; k < MAX_ALIASMASKS; k++)
                {
                    AliasCollisionMaskMatrix[h,k] = getMainMapKMaxMinCells(mainMap, gridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.minStepsSolution+h+1, startMainMap, ((k+1)*1.0f)/(MAX_ALIASMASKS)); ;
                }
            }
        }
        else
        {
            AliasCollisionMaskMatrix = null;
        }
            
        //Map initialization.
        int i = 0;
        int seed = 0;

        SimplePriorityQueue<TileObject[,]> SimilarMapsQueue = new SimplePriorityQueue<TileObject[,]>();

        while (i < MAX_ALIAS)
        {
            //define here the width, height, start and end  of the chosen map
            TileObject[,] aliasMap = new TileObject[width, height];
            Vector2Int startAlias = ParameterManager.Instance.StartCell;
            Vector2Int endAlias = ParameterManager.Instance.EndCell;

            switch (i%3)
            {
                case 0:
                    aliasMap = genMan.connectedGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder,(float) pRNG_Alias.NextDouble(), true, 0);
                    break;
                case 1:
                    aliasMap = genMan.cellularAutomataGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble(), 1+i%5, 5, i%2==1, 0, 0, true, 0);
                    break;
                case 2:
                    aliasMap = genMan.primGenerator.generateMapGeneral(ParameterManager.Instance.IsTrapsOnMapBorder, (float)pRNG_Alias.NextDouble(), true, 0);
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
                            if(AliasCollisionMaskMatrix[i % AliasCollisionMaskMatrix.GetLength(0), i % AliasCollisionMaskMatrix.GetLength(1)].Contains(new Vector2Int(x - startAlias.x, y - startAlias.y)))
                                aliasMap[x, y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + x, startMainMap.y + y].type;
                    }
                }
            }
            else
            {
                //only from zero to kMinStep
                for (int x = 0; x < aliasMap.GetLength(0); x++)
                {
                    for (int y = 0; y < aliasMap.GetLength(1); y++)
                    {
                        if (Utility.in_bounds_General(new Vector2Int(startMainMap.x + x, startMainMap.y + y), width, height) && BaseAliasCollisionMask.Contains(new Vector2Int(x - startAlias.x, y - startAlias.y)))
                            aliasMap[x, y].type = ParameterManager.Instance.MapToPlay[startMainMap.x + x, startMainMap.y + y].type;
                    }
                }
            }

            if(MapEvaluator.isEndReachable(aliasMap, gridType, startAlias, endAlias, ParameterManager.Instance.allowAutosolverForAlias).First() == endAlias)//if the map has a path from start to end, add it
                SimilarMapsQueue.Enqueue(aliasMap, MapEvaluator.BinaryMapSimilarity(mainMap, aliasMap, startMainMap, startAlias));

            i++;
        }

        i = 0;
        while (i< ParameterManager.Instance.aliasNum)
        {
            renderAliasOnUI(AliasDragAreas[0], ParameterManager.Instance.GridType, SimilarMapsQueue.Dequeue());
            i++;
        }

        i = 0;
        while (i < BatchAliasNumber)
        {
            renderAliasOnUI(AliasDragAreas[1], ParameterManager.Instance.GridType, SimilarMapsQueue.Dequeue());
            i++;
        }

        i = 0;
        while (i < BatchAliasNumber)
        {
            renderAliasOnUI(AliasDragAreas[2], ParameterManager.Instance.GridType, SimilarMapsQueue.Last());
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

        //Ensure same cells in minimum kMinStep steps
        while (i <= kMinStep)
        {
            foreach(Vector2Int p in K_CollisionSet[i])
            {
                Vector2Int x = p + startCell;
                if (Utility.in_bounds_General(x, width, height))
                    UnionCollisionSet.Add(x);
            }
            
            i++;
        }

        //Ensure one or more paths from kMinStep up to kMaxStep
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

    private void renderAliasOnUI(RectTransform container, ITypeGrid typeGrid,TileObject[,] Alias)
    {
        GameObject AliasGO = Instantiate(AliasPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        AliasGO.transform.SetParent(container, false);
        Transform t = AliasGO.transform.Find("BorderMask/Content");
        RectTransform contentRect = t.GetComponent<RectTransform>();
        RectTransform prefabRect = typeGrid.TilePrefab.GetComponent<RectTransform>();
        initAliasGameObject(AliasGO);
        GeneratorUIManager.Instance.DisplayMap(Alias, t, ParameterManager.Instance.GridType);

        GeneratorUIManager.Instance.ScaleToFitContainer(contentRect, new Rect(Vector2.zero, container.GetComponent<GridLayoutGroup>().cellSize));
    }

    private void initAliasGameObject(GameObject AliasGO)
    {
        DragHandler dHand = AliasGO.GetComponentInChildren<DragHandler>();
        AliasGO.GetComponentInChildren<HoverDisplayText>().DialogBoxInfo = GameObject.FindGameObjectWithTag("DialogBox");
        dHand.OriginalParent = dHand.ToMoveGameObj.transform.parent.gameObject;
        dHand.canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }
}
