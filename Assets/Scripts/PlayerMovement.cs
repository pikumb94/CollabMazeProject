using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string horizontalInputName, verticalInputName;
    [SerializeField] private float speed = 6f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;


    // Start is called before the first frame update
    protected void Awake()
    {
        controller = GetComponent<CharacterController>();
            
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!GameManager.instance.isGameOver && !GameManager.instance.isPause && !((GameManager_Generator)GameManager.instance).isAliasDisplayOn) {
            float horizInput = Input.GetAxis(horizontalInputName) * speed;
            float vertInput = Input.GetAxis(verticalInputName) * speed;

            Vector3 forwardMovement = transform.forward * vertInput;
            Vector3 rightMovement = transform.right * horizInput;

            controller.SimpleMove(forwardMovement + rightMovement);
        }

    }

    //CHIEDERE S3E VA BENE USARE CONTROLLER E ONTRIGGERENTER
    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "Trap") { 
            GameManager.instance.ApplyPenalty();
            Destroy(other.gameObject);
        }
        if (other.name.Contains("FinishLane"))
        {
            handleFinishLane();

        }
            
    }

    protected virtual void handleFinishLane()
    {
        GameManager.instance.YouWin();
    }
    
}
