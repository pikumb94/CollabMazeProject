using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITypeGrid : MonoBehaviour
{
    [SerializeField]
    protected Pair<int, int>[] directions;

    protected ITypeGrid(Pair<int, int>[] dir)
    {
        directions = dir;
    }

    public abstract Pair<int, int>[] getNeighbours(Pair<int, int> location);
}
