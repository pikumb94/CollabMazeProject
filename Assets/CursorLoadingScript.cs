using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ChangeTextSlider : MonoBehaviour
{
    TMP_InputField iF;
    // Start is called before the first frame update
    void Start()
    {
        iF = GetComponent<TMP_InputField>();
    }

    public void textUpdate(float value) {

        iF.text = string.Format("{0:N2}", value);
    }
}
