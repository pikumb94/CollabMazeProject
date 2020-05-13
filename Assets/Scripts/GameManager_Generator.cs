using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Generator : GameManager
{

    protected override void Init()
    {
        base.Init();
        remainingSeconds = ParameterManager.Instance.countdownSecondsParam;
        penaltySeconds = ParameterManager.Instance.penaltySecondsParam;
        GameUIManager.instance.DisplayTimeFormatted();

    }
}
