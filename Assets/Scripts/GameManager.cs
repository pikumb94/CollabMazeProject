﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector] public int remainingSeconds;
    public int countdownSeconds = 300;

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isWin = false;
    public int penaltySeconds = 10;


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
        Init();
        InvokeRepeating("DecreaseCounter", 1f, 1f);
    }

    void Update()
    {

        if (Input.GetKeyDown("1")) {
           SceneManager.LoadScene(0);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Init();
    }

    void Init()
    {
        isGameOver = false;
        isWin = false;
        GameUIManager.instance.HideGameOverPanel();
        GameUIManager.instance.HideYouWinPanel();
        remainingSeconds = countdownSeconds;
        GameUIManager.instance.DisplayTimeFormatted();
    }

    public void DecreaseCounter(){
        if (!isWin)
        {
            remainingSeconds--;
            if (remainingSeconds >= 0)
            {
                GameUIManager.instance.DisplayTimeFormatted();
            }
            else
                GameIsOver();
        }
        
    }



    public void ApplyPenalty()
    {
        if (remainingSeconds >= penaltySeconds)
            remainingSeconds = remainingSeconds - penaltySeconds;
        else
            remainingSeconds = 0;

        if (remainingSeconds <= 0)
            GameIsOver();
        GameUIManager.instance.DisplayPenaltyText(penaltySeconds);
        GameUIManager.instance.DisplayTimeFormatted();
        
    }

    void GameIsOver()
    {
        isGameOver = true;
        GameUIManager.instance.DisplayGameOverPanel();
    }

    public void YouWin()
    {
        isWin = true;
        GameUIManager.instance.DisplayYouWinPanel();
    }

}
