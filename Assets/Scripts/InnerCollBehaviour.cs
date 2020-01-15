using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerCollBehaviour : MonoBehaviour
{
    MetalDoorBehaviour[] mDoors;
    // Start is called before the first frame update
    void Awake()
    {
        mDoors = transform.parent.GetComponentsInChildren<MetalDoorBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player ENTRA!");
            foreach (MetalDoorBehaviour m in mDoors)
            {
                m.hasTrap = false;
            }

            StartCoroutine("IncreasingPenalty");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player ESCE!");
            StopCoroutine("IncreasingPenalty");
        }
    }

    IEnumerator IncreasingPenalty()
    {
        while (true)
        {
            GameManager.instance.ApplyPenalty();
            yield return new WaitForSeconds(1);
        }

    }
}
