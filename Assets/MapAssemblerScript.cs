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
                switch (pMan.MapToPlay[i,j].type)
                {
                    case IGenerator.roomChar:
                        Instantiate(pMan.GridType.InGameTilePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        break;
                    case IGenerator.wallChar:
                        Instantiate(pMan.GridType.InGameObstaclePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        break;
                    case IGenerator.startChar:
                        Instantiate(pMan.GridType.InGameTilePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, 0, pMan.GridType.inGameOffsetY * j), Quaternion.identity);
                        GameManager.instance.Player.transform.position = new Vector3(2.5f+ pMan.GridType.inGameOffsetX * i, 1,2.5f +pMan.GridType.inGameOffsetY * j);
                        break;
                    case IGenerator.endChar:
                        //Instantiate(pMan.GridType.InGameTilePrefab, new Vector3(pMan.GridType.inGameOffsetX * i, pMan.GridType.inGameOffsetY * j, 0), Quaternion.identity);
                        break;
                    default:
                        ErrorManager.ManageError(ErrorManager.Error.HARD_ERROR, "Map character not identified");
                        break;
                }

            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
