using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAssemblerScript : MonoBehaviour
{
    ParameterManager pMan;
    // Start is called before the first frame update
    private void Awake()
    {
        pMan = ParameterManager.Instance;
    }
    void Start()
    {

        for (int i = 0; i < pMan.MapToPlay.GetLength(0); i++)
        {
            for (int j = 0; j < pMan.MapToPlay.GetLength(1); j++)
            {
                switch (pMan.MapToPlay[i, j].type)
                {
                    case IGenerator.roomChar:
                        Instantiate(pMan.GridType.InGameTilePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        break;
                    case IGenerator.wallChar:
                        Instantiate(pMan.GridType.InGameObstaclePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        break;
                    case IGenerator.startChar:
                        Instantiate(pMan.GridType.InGameTilePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        GameManager.instance.Player.transform.position = new Vector3(pMan.GridType.inGameOffsetX * i, 1, pMan.GridType.inGameOffsetY * j);
                        break;
                    case IGenerator.endChar:
                        Instantiate(pMan.GridType.InGameEndPrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        break;
                    default:
                        ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "Map character not identified");
                        break;
                }

            }
        }

        AssembleMapBorder();

    }

    private void AssembleMapBorder()
    {
        float width = pMan.GridType.inGameOffsetX * pMan.MapToPlay.GetLength(0);
        float height = 3f;
        GameObject bottomPlane = Utility.CreatePlane(width,height);
        bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.color = Color.black;

        bottomPlane.transform.position = new Vector3(-pMan.GridType.inGameOffsetX * .5f, 0, pMan.GridType.inGameOffsetY * ( - .5f));
        GameObject topPlane = Instantiate(bottomPlane, new Vector3(-pMan.GridType.inGameOffsetX * .5f, 0, pMan.GridType.inGameOffsetY * (pMan.MapToPlay.GetLength(1) - .5f)), Quaternion.identity);
        bottomPlane.transform.localScale = new Vector3(1, 1, -1);

        width = pMan.GridType.inGameOffsetY * pMan.MapToPlay.GetLength(1);
        height = 3f;
        GameObject leftPlane = Utility.CreatePlane(width, height);
        leftPlane.GetComponent<MeshRenderer>().sharedMaterial.color = Color.black;

        leftPlane.transform.position = topPlane.transform.position;
        leftPlane.transform.Rotate(0, 90, 0);
        leftPlane.transform.localScale = new Vector3(1, 1, -1);

        GameObject rightPlane = Instantiate(leftPlane, new Vector3(pMan.GridType.inGameOffsetX * (pMan.MapToPlay.GetLength(0) - .5f) ,0,- pMan.GridType.inGameOffsetY * .5f ), Quaternion.identity);
        rightPlane.transform.Rotate(0, -90, 0);
        rightPlane.transform.localScale = new Vector3(1, 1, -1);

        bottomPlane.AddComponent<BoxCollider>();
        leftPlane.AddComponent<BoxCollider>();
        topPlane.AddComponent<BoxCollider>();
        rightPlane.AddComponent<BoxCollider>();
    }

    
}
