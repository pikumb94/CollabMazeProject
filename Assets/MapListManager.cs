using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapListManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<int,TileObject[,]> dictionaryMap;

    private void Awake()
    {
        dictionaryMap = new Dictionary<int, TileObject[,]>();
    }

    public void addMapToDictionary(TileObject[,] newMap, int key)
    {
        dictionaryMap.Add(key, newMap);
    }

    public KeyValuePair<int, TileObject[,]> removeMapFromDictionary(int key)
    {
        TileObject[,] t = dictionaryMap[key];
        dictionaryMap.Remove(key);
        return new KeyValuePair<int, TileObject[,]>(key, t);
    }

    public Dictionary<int, TileObject[,]>.ValueCollection getMapList()
    {
        return dictionaryMap.Values;
    }
}
