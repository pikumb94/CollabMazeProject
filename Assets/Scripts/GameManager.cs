using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public float countdownSeconds = 450.0f;
    private TextMeshProUGUI tmp;
    private int minutes, seconds;
    private bool isGameOver = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

        tmp = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        DisplayTimeFormatted();
        InvokeRepeating("DecreaseCounter", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DecreaseCounter (){
        countdownSeconds--;
        if (countdownSeconds >= 0)
            DisplayTimeFormatted();
        else
            isGameOver = true;
    }

    void DisplayTimeFormatted()
    {
        minutes = (int)countdownSeconds / 60;
        seconds = (int)countdownSeconds % 60;
        tmp.SetText(minutes.ToString() + ":" + String.Format("{0:00}", seconds));
    }
}
