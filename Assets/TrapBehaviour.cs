using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    private RaycastHit hit;
    private float dst;

    private void Start()
    {
        Debug.Log(Physics.Raycast(this.transform.Find("DeadWallContainer").position, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("Wall")));
        
        dst = hit.distance;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(this.transform.Find("DeadWallContainer").position, transform.right * 1000, Color.white);
        Debug.DrawRay(this.transform.Find("DeadWallContainer").position, transform.right * dst, Color.red);
        if(Physics.Raycast(this.transform.Find("DeadWallContainer").position, transform.right , out hit, dst, LayerMask.GetMask("Player")))
        {
            Debug.Log("Player Hit!");
        }
       

    }
}
