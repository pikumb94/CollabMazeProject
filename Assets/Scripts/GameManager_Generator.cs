using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Generator : GameManager
{
    [HideInInspector] public bool isAliasDisplayOn = false;

    protected GameManager_Generator() { }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }

    protected override void Init()
    {
        base.Init();
        remainingSeconds = ParameterManager.Instance.countdownSecondsParam;
        penaltySeconds = ParameterManager.Instance.penaltySecondsParam;
        GameUIManager.instance.DisplayTimeFormatted();

    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //toggleDisplay
            GameUIManager.instance.ToggleAliasPanel();
            isAliasDisplayOn = !isAliasDisplayOn;
        }
    }
}
