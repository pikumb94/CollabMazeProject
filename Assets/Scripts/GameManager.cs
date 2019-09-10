using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int countdownSeconds = 450;
    private TextMeshProUGUI timerText;
    private TextMeshProUGUI penaltyText;
    private int minutes, seconds;
    private bool isGameOver = false;
    public int penaltySeconds = 10;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);  //ESSENDO IL GAMEMANAGER DOVREBBE SOPRAVVIVERE MA ATTUALMENTE NON HO BISOGNO DI CONSERVARE DATI/STATI DEL GIOCO FRA LE SCENE
    }

    void Start()
    {

        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        penaltyText = GameObject.Find("PenaltyText").GetComponent<TextMeshProUGUI>();
        Debug.Log(timerText.name+" "+penaltyText.name);
        DisplayTimeFormatted();
        InvokeRepeating("DecreaseCounter", 1f, 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown("1")) {
           SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
        timerText.SetText(minutes.ToString() + ":" + String.Format("{0:00}", seconds));
    }

    public void ApplyPenalty()
    {
        if (countdownSeconds >= penaltySeconds)
            countdownSeconds = countdownSeconds - penaltySeconds;
        else
            countdownSeconds = 0;
        DisplayTimeFormatted();
        penaltyText.SetText("  -"+penaltySeconds);
        Invoke("CancelPenaltyText", 3);
    }

    private void CancelPenaltyText()
    {
        penaltyText.SetText("");
    }

}
