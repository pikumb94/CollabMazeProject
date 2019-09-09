using System.Collections;
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


    private void Start()
    {
        Physics.Raycast(this.transform.Find("DeadWallContainer").position, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("Wall"));
        dst = hit.distance;
        trap = this.transform.Find("Trap").gameObject;
        smoothedTrapPos = trap.transform.position;
        initialTrapPos = trap.transform.position;
    }

    void Update()
    {
        //Debug.DrawRay(this.transform.Find("DeadWallContainer").position, transform.right * 1000, Color.white);
        //Debug.DrawRay(this.transform.Find("DeadWallContainer").position, transform.right * dst, Color.red);
        if(Physics.Raycast(this.transform.Find("DeadWallContainer").position, transform.right , out hit, dst, LayerMask.GetMask("Player")))
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
