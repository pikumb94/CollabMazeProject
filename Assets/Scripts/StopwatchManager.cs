using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchManager : MonoBehaviour
{
    public Transform minutesHand;
    public Transform secondHand;
    public bool isCountDown = false;
    public bool isSymmetrical = false;
    void Start()
    {
        if (isCountDown)
        {
            secondHand.Rotate(new Vector3(0f, 0f, 6f * (GameManager.instance.countdownSeconds%60)));
            minutesHand.Rotate(new Vector3(0f, 0f, 0.2f * GameManager.instance.countdownSeconds));
        }

    }

    void Update()
    {
        if (isCountDown)
        {
            if (isSymmetrical) {
                secondHand.Rotate(new Vector3(0f, 0f, 6f * Time.deltaTime));
                minutesHand.Rotate(new Vector3(0f, 0f, 0.2f * Time.deltaTime));
            } else {
                secondHand.Rotate(new Vector3(0f, 0f, -6f * Time.deltaTime));
                minutesHand.Rotate(new Vector3(0f, 0f, -0.2f * Time.deltaTime));
            }
            
        }
        else
        {

            if (isSymmetrical)
            {
                secondHand.Rotate(new Vector3(0f, 0f, -6f * Time.deltaTime));
                minutesHand.Rotate(new Vector3(0f, 0f, -0.2f * Time.deltaTime));
            }
            else
            {
                secondHand.Rotate(new Vector3(0f, 0f, 6f * Time.deltaTime));
                minutesHand.Rotate(new Vector3(0f, 0f, 0.2f * Time.deltaTime));
            }
        }
        
    }
}
