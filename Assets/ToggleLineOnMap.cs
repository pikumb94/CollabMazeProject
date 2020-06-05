using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ToggleLineOnMap : MonoBehaviour
{
    Toggle m_Toggle;
    public GameObject LineContainer;
    private GameObject m_Line;

    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });

    }

    public void refreshLineReference()
    {
        UILineRenderer[] lines = LineContainer.GetComponentsInChildren<UILineRenderer>();
        if(lines != null)
        {
            foreach (var l in lines)
                if (compareRGB(l.color, transform.Find("Background/Checkmark").GetComponent<Image>().color))
                {
                    m_Line = l.gameObject;
                    break;
                }
        }
                
    }

    public void refreshLineReference(GameObject lineGO)
    {
        UILineRenderer  lR= lineGO.GetComponent<UILineRenderer>();
        if (lR != null)
        {
              if (compareRGB(lR.color, transform.Find("Background/Checkmark").GetComponent<Image>().color))
            {
                m_Line = lR.gameObject;
            }
        }

    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
        if (m_Line != null) { 
            m_Line.SetActive(m_Toggle.isOn);
            if (m_Toggle.isOn)
                m_Line.transform.SetAsLastSibling();
        }
    }
    
    private bool compareRGB(Color c1, Color c2)
    {
        Color t1 = c1;
        t1.a = 1;

        Color t2 = c2;
        t2.a = 1;

        return (t1 == t2);
    }
}
