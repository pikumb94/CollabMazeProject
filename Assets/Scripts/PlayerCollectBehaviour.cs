using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectBehaviour : PlayerMovement
{
    private List<string> collectedItems;

    RequiredItemsBehaviour rIB;

    private new void Awake()
    {
        base.Awake();
        collectedItems = new List<string>();


        GameObject g = GameObject.Find("/RequiredItems");
        if (g != null)
        {
            rIB = g.GetComponent<RequiredItemsBehaviour>();
        }
        else
        {
            Debug.LogError("Required Items to complete the level are missing!");
        }
    }

    private new void Update()
    {
        base.Update();
    }

    private new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.tag == "Collectibles")
        {
            if (other.name.Contains("Cube"))
            {
                collectedItems.Add("Cube");
                GameUIManager.instance.DisplayCollectedItem("Cube");
            }
            else if (other.name.Contains("Sphere"))
            {
                collectedItems.Add("Sphere");
                GameUIManager.instance.DisplayCollectedItem("Sphere");
            }
            else if (other.name.Contains("Prism"))
            {
                collectedItems.Add("Prism");
                GameUIManager.instance.DisplayCollectedItem("Prism");
            }
            else
            {
                Debug.Log("ERROR!");
            }
            Destroy(other.gameObject);
            
            printList();
        }
        
    }

    private void printList()
    {
        int n = 0;
        foreach(string s in collectedItems)
        {
            Debug.Log(s);
            n++;
        }
        Debug.Log(n);
    }

    protected override void  handleFinishLane()
    {
        string[][] v = rIB.GetSequences();
        bool res = true;
        foreach(string[] a in v)
        {
            res &= validateAssertion(a);
        }

        if (res)
        {
            GameManager.instance.YouWin();
        }
        else
        {
            GameManager.instance.GameIsOver();
        }
    }

    private bool validateAssertion(string[] itemsSequence)
    {
        bool result = false;
        int j = 0;

        for (int i = 0; i < collectedItems.Count; i++)
        {
            if(collectedItems[i] == itemsSequence[j])
            {
                                    
                j++;

                if (j == itemsSequence.Length)
                {
                    result = true;
                    break;
                }

            }
            else
            {
                j = 0;
            }
        }

        return result;
    }
}
