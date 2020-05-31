using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarReset : MonoBehaviour
{
    private Scrollbar s;

    public void Awake()
    {
        s= GetComponent<Scrollbar>();
    }

    public void ScrollBarResetHandler()
    {
        s.value = 0f;
    }
}
