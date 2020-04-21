using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MergingTextureScript : MonoBehaviour
{
    public Image TopImage;
    public Image BottomImage;

    // Start is called before the first frame update
    void Start()
    {
        //Texture2D resImg = Utility.AlphaBlend((Texture2D)BottomImage.mainTexture, (Texture2D) TopImage.mainTexture);
        //GetComponent<Image>().sprite = Sprite.Create(resImg, new Rect(0, 0, 64, 64),Vector2.zero);
    }


}
