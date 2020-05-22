using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIParametersValueChange : MonoBehaviour
{
    GeneratorManager genM;

    [Header("UI input components")]
    public TMP_InputField UIWidth;
    public TMP_InputField UIHeight;
    public TMP_InputField UIStartX;
    public TMP_InputField UIStartY;
    public TMP_InputField UIEndX;
    public TMP_InputField UIEndY;
    public TMP_InputField UISeed;

    public Toggle UIUseSeed;

    [Header("UI Connected input components")]
    public TMP_InputField UIObstacleCountC;

    [Header("UI Cellular Automata input components")]
    public TMP_InputField UIObstacleCountCA;
    public TMP_InputField UIIterationsNumber;
    public TMP_InputField UIObstacleThreshold;
        public Toggle UIborderIsObstacle;
    [Header("UI Prim's input components")]
    public TMP_InputField UIObstacleToRemove;

    [Header("UI Generators panels")]
    public GameObject[] GeneratorParametersPanels;
    public ScrollRect scrollUIParameterPanel;

    private void Start()
    {
        genM = GeneratorManager.Instance;
    }

    public void generalUICallBackText(TMP_InputField UIIF)
    {
        
        if (UIIF == UIWidth)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.width=Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.width = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.width = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR,"GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }

        }
        else if(UIIF== UIHeight)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.height = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.height = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.height = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF == UIStartX)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.startPos.x = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.startPos.x = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.startPos.x = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF == UIStartY)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.startPos.y = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.startPos.y = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.startPos.y = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF == UIEndX)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.endPos.x = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.endPos.x = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.endPos.x = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF == UIEndY)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.endPos.y = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.endPos.y = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.endPos.y = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF == UISeed)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.seed = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.seed = Int32.Parse(UIIF.text);
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.seed = Int32.Parse(UIIF.text);
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (UIIF==UIObstacleCountC)
        {
            genM.connectedGenerator.obstaclePercent = float.Parse(UIObstacleCountC.text);
        }
        else if (UIIF == UIObstacleToRemove)
        {
            genM.primGenerator.obstacleToRemovePercent = float.Parse(UIObstacleToRemove.text);
        }
        else if (UIIF == UIObstacleCountCA)
        {
            genM.cellularAutomataGenerator.obstaclePercent = float.Parse(UIObstacleCountCA.text);
        }
        else if (UIIF == UIObstacleThreshold)
        {
            genM.cellularAutomataGenerator.thresholdWall = Int32.Parse(UIObstacleThreshold.text);
        }
        else if (UIIF == UIIterationsNumber)
        {
            genM.cellularAutomataGenerator.iterationsNumber = Int32.Parse(UIIterationsNumber.text);
        }
        else
        {
            ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "INPUT FIELD TO REFRESH NOT FOUND!");
        }

    }

    public void onGeneratorAlgChange(TMP_Dropdown d)
    {
        GeneratorParametersPanels[(int)genM.activeGenerator].SetActive(false);
        genM.activeGenerator = (GeneratorManager.GeneratorEnum)Enum.ToObject(typeof(GeneratorManager.GeneratorEnum), d.value);
        GeneratorParametersPanels[(int)genM.activeGenerator].SetActive(true);

        scrollUIParameterPanel.content = GeneratorParametersPanels[(int)genM.activeGenerator].GetComponent<RectTransform>();
        //update all params on UI
        GeneratorUIManager.Instance.deleteMapOnUI(GeneratorManager.Instance.Content.transform);
        GeneratorUIManager.Instance.hideUIMapInfo(GeneratorManager.Instance.MapHolder.transform);
        refreshUIParams();

    }

    private void setNavigationOnParamPanel(TMP_Dropdown d)
    {
        Selectable s = d.gameObject.GetComponent<Selectable>();
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        
        switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    customNav.selectOnDown = UIObstacleCountC.gameObject.GetComponent<Selectable>();
                    s.navigation = customNav;
            break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    customNav.selectOnDown = UIObstacleCountCA.gameObject.GetComponent<Selectable>();
                    s.navigation = customNav;
                break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    customNav.selectOnDown = UIObstacleToRemove.gameObject.GetComponent<Selectable>();
                    s.navigation = customNav;
                break;
            default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
            break;
        }

    }

    public void generalUICallBackValue(Slider s)
    {

    }

    public void generalUICallBackIsOn(Toggle t)
    {
        if (t == UIUseSeed)
        {
            switch (genM.activeGenerator)
            {
                case GeneratorManager.GeneratorEnum.CONNECTED:
                    genM.connectedGenerator.useRandomSeed = t.isOn;
                    break;
                case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                    genM.cellularAutomataGenerator.useRandomSeed = t.isOn;
                    break;
                case GeneratorManager.GeneratorEnum.PRIM:
                    genM.primGenerator.useRandomSeed = t.isOn;
                    break;
                default:
                    ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                    break;
            }
        }
        else if (t==UIborderIsObstacle)
        {
            genM.cellularAutomataGenerator.borderIsObstacle = t.isOn;
        }
        else{
            ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "INPUT FIELD TO REFRESH NOT FOUND!");
        }
    }

    public void refreshUIParams()
    {
        UIWidth.text = genM.GeneratorsVect[(int) genM.activeGenerator].width.ToString();
        UIHeight.text = genM.GeneratorsVect[(int)genM.activeGenerator].height.ToString();
        UIStartX.text = genM.GeneratorsVect[(int)genM.activeGenerator].startPos.x.ToString();
        UIStartY.text = genM.GeneratorsVect[(int)genM.activeGenerator].startPos.y.ToString();
        UIEndX.text = genM.GeneratorsVect[(int)genM.activeGenerator].endPos.x.ToString();
        UIEndY.text = genM.GeneratorsVect[(int)genM.activeGenerator].endPos.y.ToString();
        UISeed.text = genM.GeneratorsVect[(int)genM.activeGenerator].seed.ToString();
        UIUseSeed.isOn = genM.GeneratorsVect[(int)genM.activeGenerator].useRandomSeed;

        switch (genM.activeGenerator)
        {
            case GeneratorManager.GeneratorEnum.CONNECTED:
                UIObstacleCountC.text = string.Format("{0:N2}", genM.connectedGenerator.obstaclePercent.ToString());
                break;
            case GeneratorManager.GeneratorEnum.CELLULAR_AUTOMATA:
                UIObstacleCountCA.text = string.Format("{0:N2}", genM.cellularAutomataGenerator.obstaclePercent.ToString());
                UIObstacleThreshold.text = genM.cellularAutomataGenerator.thresholdWall.ToString();
                UIIterationsNumber.text = genM.cellularAutomataGenerator.iterationsNumber.ToString();
                UIborderIsObstacle.isOn = genM.cellularAutomataGenerator.borderIsObstacle;
                break;
            case GeneratorManager.GeneratorEnum.PRIM:
                UIObstacleToRemove.text = string.Format("{0:N2}", genM.primGenerator.obstacleToRemovePercent.ToString());
                break;
            default:
                ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "GeneratorEnum PARAMETER NOT FOUND!");
                break;
        }
    }

    public void toggleUIAutosolver(Animator a)
    {
        bool isOn = a.GetBool("isOn");
        isOn = !isOn;
        a.SetBool("isOn", isOn);
        GeneratorManager.Instance.isAutosolverOn = isOn;
    }
}
