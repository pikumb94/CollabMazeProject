﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    private RaycastHit hit;
    private float dst;
    public float trapVelocity = 8f;
    private bool isTrapActive = false;
    private GameObject trap;
    private Vector3 smoothedTrapPos;
    private Vector3 initialTrapPos;

    [SerializeField] private Transform positionDeadWallContainer;
    [SerializeField] private GameObject trapGameObj;


    void Awake()
    {
        Physics.Raycast(positionDeadWallContainer.position, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("Wall"));
        dst = hit.distance;
        trap = trapGameObj;
        smoothedTrapPos = trap.transform.position;
        initialTrapPos = trap.transform.position;
    }

    void Update()
    {
        
        if(Physics.Raycast( positionDeadWallContainer.position, transform.right , out hit, dst, LayerMask.GetMask("Player")))
        {
            Debug.Log("Raycast: Player Hit!");
            isTrapActive = true;
        }

        if (isTrapActive && trap !=null)
        {
            smoothedTrapPos = Vector3.Lerp(trap.transform.position, (initialTrapPos + transform.right * (2 + dst)), trapVelocity * Time.deltaTime);
            trap.transform.position = smoothedTrapPos;

            if (((trap.transform.position) - (initialTrapPos + transform.right * (2 + dst))).sqrMagnitude <= 0.1)
                Destroy(trap);
        }

    }
}
