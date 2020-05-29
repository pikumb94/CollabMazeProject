using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeMainMap : MonoBehaviour
{
    void FitDisplayPanelContent()
    {
        GeneratorUIManager.Instance.ScaleToFitContainer(transform.Find("MapHolder/BorderMask/Content").GetComponent<RectTransform>());
    }
}
