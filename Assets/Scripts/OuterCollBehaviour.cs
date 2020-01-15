using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterCollBehaviour : MonoBehaviour
{
    MetalDoorBehaviour[] mDoors;
    // Start is called before the first frame update
    void Awake()
    {
        mDoors = transform.parent.GetComponentsInChildren<MetalDoorBehaviour>();

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player ESCE!");

            foreach (MetalDoorBehaviour m in mDoors)
            {
                m.hasTrap = true;
            }
        }
    }

}
