using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AliasGeneratorManager : Singleton<AliasGeneratorManager>
{

    public GameObject[] AliasToggleAnimations;
    public RectTransform[] AliasDragAreas;

    protected AliasGeneratorManager() { }



    public void AliasGenUIToggleAnim()
    {
        foreach (GameObject g in AliasToggleAnimations)
        {
            GeneratorUIManager.Instance.toggleUIAnimator(g.GetComponent<Animator>());
        }
    }

}
