using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckMouseClick : MonoBehaviour
{
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    // Update is called once per frame
    void Update()
    {
        _raycastResults = Utility.GetEventSystemRaycastResults();
        Debug.Log(_raycastResults);
    }
}
