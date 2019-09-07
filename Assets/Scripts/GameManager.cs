using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public float countdownMinutes = 450.0f;
    private TextMeshProUGUI tmp;
    private int minutes, seconds;

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
        minutes = (int) countdownMinutes / 60;
        seconds = (int) countdownMinutes % 60;

        tmp.SetText(minutes.ToString()+":"+seconds.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
