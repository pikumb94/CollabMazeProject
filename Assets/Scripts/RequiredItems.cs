using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemsSequence
{
    public string[] sequence;
};

[CreateAssetMenu(fileName = "RequiredItems", menuName = "NewRequiredItems")]
public class RequiredItems : ScriptableObject
{
    public ItemsSequence[] sequences;
}
