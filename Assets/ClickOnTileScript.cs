using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickOnTileScript : MonoBehaviour
{
    void OnMouseDown()
    {/*
        GeneratorUIManager GUIM = GeneratorUIManager.Instance;
        Image TileImg = gameObject.GetComponent<Image>();

        if(TileImg.color == GUIM.startColor || TileImg.color == GUIM.endColor)
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "You cannot turn a start or end position into a wall tile.");
        else
        {
            if (TileImg.color == GUIM.roomColor)
                TileImg.color = GUIM.wallColor;
            else
                if (TileImg.color == GUIM.wallColor)
                    TileImg.color = GUIM.roomColor;
        }
        */

    }
    private void OnMouseEnter()
    {
        
    }

}
