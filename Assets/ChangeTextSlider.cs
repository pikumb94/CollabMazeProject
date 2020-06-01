using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ChangeTextSlider : MonoBehaviour
{
    private TMP_InputField r;

    private void Start()
    {
        r = GetComponent<TMP_InputField>();
    }

    public void InputFieldUpdate(float value) {
        r.text = value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
    }
}
