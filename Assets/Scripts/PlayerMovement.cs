﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string horizontalInputName, verticalInputName;
    [SerializeField] private float speed = 6f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameOver) {
            float horizInput = Input.GetAxis(horizontalInputName) * speed;
            float vertInput = Input.GetAxis(verticalInputName) * speed;

            Vector3 forwardMovement = transform.forward * vertInput;
            Vector3 rightMovement = transform.right * horizInput;

            controller.SimpleMove(forwardMovement + rightMovement);
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        GameManager.instance.ApplyPenalty();
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }

}
