using System.Collections;
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
    [HideInInspector] public bool isPause = false;
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
        if (Input.GetKeyDown("2"))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown("3"))
        {
            SceneManager.LoadScene(2);
        }
        if (Input.GetKeyDown("4"))
        {
            SceneManager.LoadScene(3);
        }
        if (Input.GetKeyDown("5"))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown("6"))
        {
            SceneManager.LoadScene(5);
        }
        if (Input.GetKeyDown("7"))
        {
            SceneManager.LoadScene(6);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                GameUIManager.instance.HideDescriptionPanel();
                isPause = false;
            }
            else
            {
                if(!isWin && !isGameOver)
                {
                    isPause = true;
                    GameUIManager.instance.DisplayDescriptionPanel();
                }
            }
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
        isPause = false;

        GameUIManager.instance.HideGameOverPanel();
        GameUIManager.instance.HideYouWinPanel();
        GameUIManager.instance.HideDescriptionPanel();
        remainingSeconds = countdownSeconds;
        GameUIManager.instance.DisplayTimeFormatted();
    }

    public void DecreaseCounter(){
        if (!isWin && !isPause)
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
        GameUIManager.instance.DisplayDescriptionPanel();
    }

    public void YouWin()
    {
        isWin = true;
        GameUIManager.instance.DisplayYouWinPanel();
        GameUIManager.instance.DisplayDescriptionPanel();
    }



}
