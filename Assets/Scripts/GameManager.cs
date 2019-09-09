using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int countdownSeconds = 450;
    private TextMeshProUGUI tmp;
    private int minutes, seconds;
    private bool isGameOver = false;
    public int timePenalty = 10;

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
        minutes = countdownSeconds / 60;
        seconds = countdownSeconds % 60;
        tmp.SetText(minutes.ToString() + ":" + String.Format("{0:00}", seconds));
    }

    private void ApplyPenalty()
    {
        if (countdownSeconds >= timePenalty)
            countdownSeconds = countdownSeconds - timePenalty;
        else
            countdownSeconds = 0;
    }
}
