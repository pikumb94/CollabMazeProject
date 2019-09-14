using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchManager : MonoBehaviour
{
    public Transform minutesHand;
    public Transform secondHand;
    void Start()
    {

    }

    void Update()
    {
        secondHand.Rotate(new Vector3(0f, 0f, 6f * Time.deltaTime));
        minutesHand.Rotate(new Vector3(0f, 0f,0.2f * Time.deltaTime));
    }
}
