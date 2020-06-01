using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapListManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<int,StructuredAlias> dictionaryMap;

    private void Awake()
    {
        dictionaryMap = new Dictionary<int, StructuredAlias>();
    }

    public void addMapToDictionary(TileObject[,] newMap,Vector2Int start, Vector2Int end, float dst, int key)
    {
        dictionaryMap.Add(key, new StructuredAlias(newMap, start, end,dst));//we don't need the similarity metric
    }

    public KeyValuePair<int, StructuredAlias> removeMapFromDictionary(int key)
    {
        StructuredAlias t = dictionaryMap[key];
        dictionaryMap.Remove(key);
        return new KeyValuePair<int, StructuredAlias>(key, t);
    }

    public Dictionary<int, StructuredAlias>.ValueCollection getMapList()
    {
        return dictionaryMap.Values;
    }
}
