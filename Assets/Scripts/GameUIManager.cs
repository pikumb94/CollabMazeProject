using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject youWinPanel;
    public GameObject descriptionPanel;
    public static GameUIManager instance = null;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI penaltyText;
    // Start is called before the first frame update

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

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void DisplayTimeFormatted()
    {
        int minutes = GameManager.instance.remainingSeconds / 60;
        int seconds = GameManager.instance.remainingSeconds % 60;
        timerText.SetText(minutes.ToString() + ":" + String.Format("{0:00}", seconds));
    }

    public void DisplayPenaltyText(int penaltySeconds)
    {
        penaltyText.SetText("  -" + penaltySeconds);
        Invoke("CancelPenaltyText", 3);
    }

    public void CancelPenaltyText()
    {
        penaltyText.SetText("");
    }

    public void DisplayGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void DisplayYouWinPanel()
    {
        youWinPanel.SetActive(true);
    }

    public void HideYouWinPanel()
    {
        youWinPanel.SetActive(false);
    }

    public void DisplayDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
    }

    public void ExitButtonPressed()
    {
        Application.Quit();
    }

    public void ResumeButtonPressed()
    {
        HideDescriptionPanel();
        GameManager.instance.isPause = false;
    }
}
