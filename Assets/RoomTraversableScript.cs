using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTraversableScript : MonoBehaviour
{
    public GameObject[] TraversableObjects;

    public void setTraversableObjectsState(bool b)
    {
        foreach (GameObject g in TraversableObjects)
            if (b)
                g.SetActive(true);
            else
                g.SetActive(false);
    }
}
