using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensivity;
    private float xAxisClamp;

    private void Awake()
    {
        xAxisClamp = 0.0f;
    }

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        //Debug.Log(Input.GetAxis(mouseXInputName)+ "  " + Input.GetAxis(mouseYInputName));
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensivity;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensivity;
        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp <- 90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0;
            ClampXAxisRotationToValue(90.0f);
        }

        camera.transform.Rotate(-transform.right * mouseY);
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = camera.transform.eulerAngles;
        eulerRotation.x = value;
        camera.transform.eulerAngles = eulerRotation;
    }
}
