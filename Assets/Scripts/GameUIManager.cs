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
    public TextMeshProUGUI requiredItemsText;
    public TextMeshProUGUI collectedItemsText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        GameObject g = GameObject.Find("/RequiredItems");
        if (g != null)
        {
            string s= "";
            RequiredItemsBehaviour rIB = g.GetComponent<RequiredItemsBehaviour>();
            foreach(string[] seq in rIB.GetSequences())
            {
                for(int i=0; i< seq.Length; i++)
                {
                    if(i==seq.Length-1)
                        s += printColored(seq[i]) + ".";
                    else
                        s += printColored(seq[i]) + ",";
                }
                /*
                foreach(string str in seq)
                {
                    s += str + ",";
                }*/
                s += "\n\n";
            }

            requiredItemsText.text = s;
        }
        
    }

    public void DisplayCollectedItem(String s)
    {
        collectedItemsText.text += printColored(s) + "\n";
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

    private string printColored(string s)
    {
        string res = s;
        if (s.Contains("Cube"))
        {
            res = "<#00ffff>" + s + "</color>";
        }
        else if (s.Contains("Sphere"))
        {
            res = "<#00ff00>" + s + "</color>";
        }
        else if (s.Contains("Prism"))
        {
            res = "<#0000ff>" + s + "</color>";
        }
        else
        {
            Debug.Log("Default color assigned");
        }

        return res;

    }
}
