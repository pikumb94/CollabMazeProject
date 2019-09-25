using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public new GameObject camera;
    private Transform _tr;
    private Transform _trCamera;

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensivity;
    private float xAxisClamp;

    private void Awake()
    {
        _trCamera = camera.GetComponent<Transform>();
        _tr = GetComponent<Transform>();
    }

    void Update()
    {
        if (!GameManager.instance.isGameOver) {
            float mouseX = Input.GetAxis(mouseXInputName) * mouseSensivity;
            float mouseY = Input.GetAxis(mouseYInputName) * mouseSensivity;
            xAxisClamp += mouseY;

            if (xAxisClamp > 90.0f)
            {
                xAxisClamp = 90.0f;
                mouseY = 0;
                ClampXAxisRotationToValue(270.0f);
            }
            else if (xAxisClamp < -90.0f)
            {
                xAxisClamp = -90.0f;
                mouseY = 0;
                ClampXAxisRotationToValue(90.0f);
            }

            _trCamera.Rotate(Vector3.left * mouseY);
            _tr.Rotate(Vector3.up * mouseX);
        }
        
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = _trCamera.transform.eulerAngles;
        eulerRotation.x = value;
        _trCamera.transform.eulerAngles = eulerRotation;
    }
}
