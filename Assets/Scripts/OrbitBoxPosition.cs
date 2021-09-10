using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrbitBox", menuName = "NewOrbitBox")]
public class OrbitBoxPosition : ScriptableObject
{
    public float xSpread = 1;
    public float ySpread = 1;
    public float zSpread = 1;
    public float xPos = 0;
    public float yPos = 0;
    public float zPos = 0;
}
