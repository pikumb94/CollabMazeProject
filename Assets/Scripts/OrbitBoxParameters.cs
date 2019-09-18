using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrbitBox", menuName = "NewOrbitBox")]
public class OrbitBoxParameters : ScriptableObject
{
    public float xOffset = 1;
    public float yOffset = 1;
    public float zOffset = 1;
    public float xPos = 0;
    public float yPos = 0;
    public float zPos = 0;
    public float freqXOrbit = 1f;
    public float freqYOrbit = 1f;
    public float freqZOrbit = 1f;
    public int numObj = 1;
    public bool isRandomSpawning = false;
}
