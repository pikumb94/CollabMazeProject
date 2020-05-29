using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class AliasGeneratorManager : Singleton<AliasGeneratorManager>
{

    public GameObject[] AliasToggleAnimations;
    public RectTransform[] AliasDragAreas;
    private HashSet<Vector2Int>[] K_CollisionSet;// = MapEvaluator.BuildKCollisionVec(mainMap, TypeGrid, startCell, Mathf.Max(kMinStep, kMaxStep)); //FIND RELATIVE TO THE START CELL 
    public GameObject AliasPrefab;
    protected AliasGeneratorManager() { }



    public void AliasGenUIToggleAnim()
    {
        foreach (GameObject g in AliasToggleAnimations)
        {
            GeneratorUIManager.Instance.toggleUIAnimator(g.GetComponent<Animator>());
        }
    }

    public void CollabGameGeneration()
    {

        K_CollisionSet = MapEvaluator.BuildKCollisionVec(ParameterManager.Instance.MapToPlay, ParameterManager.Instance.GridType, ParameterManager.Instance.StartCell, Mathf.Max(ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.maxStepsSolution));

        int width = ParameterManager.Instance.MapToPlay.GetLength(0);
        int height = ParameterManager.Instance.MapToPlay.GetLength(1);
        TileObject[,] aliasMap = new TileObject[width, height];
        //Map initialization.


        HashSet<Vector2Int> AliasCollisionSet = CopyMainMapSurroundingCells(ParameterManager.Instance.MapToPlay, aliasMap, ParameterManager.Instance.GridType, ParameterManager.Instance.minStepsSolution, ParameterManager.Instance.maxStepsSolution, ParameterManager.Instance.StartCell);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!AliasCollisionSet.Contains(new Vector2Int(x, y)))
                    aliasMap[x, y].type = IGenerator.endChar;
                else
                    aliasMap[x, y].type = ParameterManager.Instance.MapToPlay[x, y].type;
            }
        }

        renderAliasOnUI(AliasDragAreas[0], ParameterManager.Instance.GridType, aliasMap);
    }

    //Needs K_CollisionSet to be relative to the (0,0) cell!
    private HashSet<Vector2Int> CopyMainMapSurroundingCells(TileObject[,] mainMap, TileObject[,] aliasMap, ITypeGrid TypeGrid, int kMinStep, int kMaxStep, Vector2Int startCell)
    {
        HashSet<Vector2Int> UnionCollisionSet = new HashSet<Vector2Int>();
        if (K_CollisionSet == null || K_CollisionSet.Length == 0)
            ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR,"No collision set for the main map has been built.");
        int i = 0;

        //Ensure same cells in minimum kMinStep steps
        while (i <= kMinStep)
        {
            foreach(Vector2Int p in K_CollisionSet[i])
            {
                Vector2Int x = p + startCell;
                if (Utility.in_bounds_General(x, mainMap.GetLength(0), mainMap.GetLength(1)))
                    UnionCollisionSet.Add(x);
            }
            
            i++;
        }

        //Ensure one or more paths from kMinStep up to kMaxStep
        if (kMaxStep > kMinStep)
        {
            HashSet<Vector2Int> toBacktrack= new HashSet<Vector2Int>();
            foreach (Vector2Int p in K_CollisionSet[kMaxStep])
            {
                Vector2Int x = p + startCell;
                if (Utility.in_bounds_General(x, aliasMap.GetLength(0), aliasMap.GetLength(1)))
                    toBacktrack.Add(x);
            }

            if (toBacktrack.Count > 0)
            {
                float percBacktrack = .5f;
                int toRemove = (int)((1- percBacktrack)* (float)toBacktrack.Count);
                System.Random pseudoRandom = new System.Random(0);
                
                //remove from toBacktrack in order to have percbacktrack elements in the set
                for (i = 0; i < toRemove; i++)
                {
                    toBacktrack.Remove(toBacktrack.ElementAt(pseudoRandom.Next(0, toBacktrack.Count)));
                }

                UnionCollisionSet.UnionWith(toBacktrack);

                //backtrack and reconstruct the paths
                for(i=0; i<kMaxStep-kMinStep; i++)
                {
                    foreach(Vector2Int v in toBacktrack)
                    {
                        HashSet<Vector2Int> a = new HashSet<Vector2Int>(Utility.getAllNeighbours_General(v, TypeGrid, aliasMap.GetLength(0), aliasMap.GetLength(1)));
                        HashSet<Vector2Int> TmpSet = new HashSet<Vector2Int>(K_CollisionSet[kMaxStep - i - 1]);
                        TmpSet.IntersectWith(a);

                        if (TmpSet.Count == 1)
                            UnionCollisionSet.UnionWith(TmpSet);
                    }

                    foreach (Vector2Int v in toBacktrack)
                    {

                        HashSet<Vector2Int> a = new HashSet<Vector2Int>(Utility.getAllNeighbours_General(v, TypeGrid, aliasMap.GetLength(0), aliasMap.GetLength(1)));
                        HashSet<Vector2Int> TmpSet = new HashSet<Vector2Int>(K_CollisionSet[kMaxStep - i - 1]);
                        TmpSet.IntersectWith(a);
                        TmpSet.Except(UnionCollisionSet);

                        if(TmpSet.Count>0)
                            UnionCollisionSet.Add(TmpSet.ElementAt(pseudoRandom.Next(0, TmpSet.Count)));
                    }
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
