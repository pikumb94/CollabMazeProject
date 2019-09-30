using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawnerManager : MonoBehaviour
{
    [HideInInspector] public  enum Direction { Left, Center, Right };
    public Direction nextDir;
    private float time;
    public ScriptableObject[] scrObjList;
    public GameObject sphereSpawner;
    private GameObject[] spawners;

    void Awake()
    {
        //for(int i=0; i<scrObjList.Length; i++)
            //spawners[i] = Instantiate(sphereSpawner, this.transform.position, Quaternion.identity) as GameObject;

    }

    void Update()
    {
        
    }
}
