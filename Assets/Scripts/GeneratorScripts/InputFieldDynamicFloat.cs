using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
[RequireComponent(typeof(TMP_InputField))]
public class InputFieldDynamicFloat : MonoBehaviour
{

    private TMP_InputField ValueText;
    GeneratorManager genM;

    private void Start()
    {
        ValueText = GetComponent<TMP_InputField>();
        genM = GeneratorManager.Instance;
    }

    public void OnSliderValueChanged(float value)
    {
        
        genM.connectedGenerator.width = (int) value;
    }
    

}
