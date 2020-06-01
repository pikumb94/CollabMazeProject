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
    public int countdownSeconds;

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isWin = false;
    [HideInInspector] public bool isPause = false;
    public int penaltySeconds = 10;
    public GameObject Player;

    protected GameManager() { }
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
        GameUIManager.instance.DisplayTimeFormatted();
        InvokeRepeating("DecreaseCounter", 1f, 1f);
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    protected virtual void  Update()
    {
        if (Input.GetKeyDown("0"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Init();
        GameUIManager.instance.DisplayTimeFormatted();

    }

    protected virtual void Init()
    {
        isGameOver = false;
        isWin = false;
        isPause = false;

        GameUIManager.instance.HideGameOverPanel();
        GameUIManager.instance.HideYouWinPanel();
        GameUIManager.instance.HideDescriptionPanel();
        remainingSeconds = countdownSeconds;
 
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

    public void GameIsOver()
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
