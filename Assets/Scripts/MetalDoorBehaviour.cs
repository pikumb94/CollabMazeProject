using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDoorBehaviour : MonoBehaviour
{
    bool isOpening=false;   //because is closing by default
    // Start is called before the first frame update
    Transform[] doorTrs;
    Vector3 initialUpDoor;
    Vector3 initialDownDoor;
    public float deltaY =1.5f;
    float vel1;
    float vel2;
    float vel3;
    float vel4;
    public float smoothTime = 1f;
    float currUp;
    float currDown;
    public bool hasTrap = false;
    public GameObject trapPrefab;
    GameObject trapGObject;
    bool moveTrap = false;

    void Start()
    {
        doorTrs = GetComponentsInChildren<Transform>();
        initialUpDoor = doorTrs[1].transform.position;
        initialDownDoor = doorTrs[2].transform.position;

        //foreach (Transform t in doorTrs)
        //    Debug.Log(t.name);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isOpening == true) {
            currUp = Mathf.SmoothDamp(doorTrs[1].transform.position.y, initialUpDoor.y + deltaY, ref vel1, smoothTime);
            currDown = Mathf.SmoothDamp(doorTrs[2].transform.position.y, initialDownDoor.y - deltaY, ref  vel2, smoothTime);
        }
        else
        {
            currUp = Mathf.SmoothDamp(doorTrs[1].transform.position.y, initialUpDoor.y, ref vel3, smoothTime);
            currDown = Mathf.SmoothDamp(doorTrs[2].transform.position.y, initialDownDoor.y, ref vel4, smoothTime);
        }
        doorTrs[1].transform.position = new Vector3 (doorTrs[1].transform.position.x, currUp, doorTrs[1].transform.position.z);
        doorTrs[2].transform.position = new Vector3 (doorTrs[2].transform.position.x, currDown, doorTrs[2].transform.position.z);

        if (moveTrap)
        {
            trapGObject.transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player")
        {
            isOpening = true;
            if (hasTrap)
                Invoke("ShootTrap", .5f);
            /*if ()
                Debug.Log("Dietro");
            else
                Debug.Log("Avanti");*/
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isOpening = false;
    }

    void ShootTrap()
    {
        if(Physics.Raycast(transform.position, transform.forward, 1f))
            trapGObject = Instantiate(trapPrefab,transform.position+(- transform.forward),Quaternion.identity);
        else
            trapGObject = Instantiate(trapPrefab, transform.position + transform.forward, Quaternion.Euler(0f,180f,0f));
        moveTrap = true;
    }
}
