using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTextSlider : MonoBehaviour
{
    RectTransform r;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    /*void Update()
    {
        //r.position = Input.mousePosition;
    }*/

    public void enableCursor(bool isActive) {
        if (isActive)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
