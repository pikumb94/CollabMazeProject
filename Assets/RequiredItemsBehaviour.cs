using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequiredItemsBehaviour : MonoBehaviour
{
    public RequiredItems rI;

    public string[][] GetSequences()
    {
        string[][] sequences = new string[rI.sequences.Length][];
        for(int i=0; i< rI.sequences.Length; i++)
        {
            
            sequences[i] = new string[rI.sequences[i].sequence.Length];
            for (int j = 0; j < rI.sequences[i].sequence.Length; j++)
                sequences[i][j] = rI.sequences[i].sequence[j];
        }

        return sequences;
    }
}
