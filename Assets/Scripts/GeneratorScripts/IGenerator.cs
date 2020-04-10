using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public abstract class IGenerator : MonoBehaviour
{
    [SerializeField]
    protected ITypeGrid TypeGrid=null;

    [SerializeField]
    protected int rows = 0;
    [SerializeField]
    protected int cols = 0;

    [SerializeField]
    protected char roomChar = '.';
    [SerializeField]
    protected char wallChar = '#';

    Dictionary<Pair<int, int>, TileObject> map = null;

    protected bool in_bounds(Pair<int, int> id)
    {
        return 0 <= id.First && id.First < rows && 0 <= id.Second && id.Second < cols;
    }

    protected bool passable(Pair<int, int> id)  {
        return map[id].type == wallChar;
    }

    //genericMapObject
    [Serializable]
    protected struct TileObject
    {
        public char type;
    }
}
