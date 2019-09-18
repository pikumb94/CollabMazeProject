using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerManager : MonoBehaviour
{
    public GameObject spherePrefab;
    [SerializeField] private float xSpread=1;
    [SerializeField] private float ySpread=1;
    [SerializeField] private float zSpread=1;
    [SerializeField] private float xPos = 0;
    [SerializeField] private float yPos = 0;
    [SerializeField] private float zPos = 0;
    [SerializeField] private float freqXOrbit = 1f;
    [SerializeField] private float freqYOrbit = 1f;
    [SerializeField] private float freqZOrbit = 1f;
    [SerializeField] private int numObj = 1;
    [SerializeField] private bool isEquidistant = false;
    [SerializeField] private bool isSameRotationDirection = true;

    [Range(0f, Mathf.PI)]
    public float delta; 

    public class SphereInitialContext
    {
        public GameObject sphere;
        public Vector3 initialPosition;    //long e lat
        public bool isCounterclockwise;

        public SphereInitialContext(GameObject sphere, Vector3 initialPosition, bool isCounterclockwise)
        {
            this.sphere = sphere;
            this.initialPosition = initialPosition;
            this.isCounterclockwise = isCounterclockwise;
        }
    }

    private float time;
    [SerializeField] private float orbitVelocity= 0.5f;

    public List<SphereInitialContext> sphereList;

    void Start()
    {
        sphereList = new List<SphereInitialContext>();
        time = Time.time;
        for (int i = 0; i < numObj; i++)
        {
            if(isEquidistant)

                if(isSameRotationDirection)
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, new Vector3((2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i), Random.Range(0, 2)>=1 ? true: true));
                else
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, new Vector3((2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i), Random.Range(0, 2) >= 1 ? true : false));

            else

                if (isSameRotationDirection)
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, randomPointOnSphereToAngles(), Random.Range(0, 2) >= 1 ? true : true));
                else
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, randomPointOnSphereToAngles(), Random.Range(0, 2) >= 1 ? true : false));


            Debug.Log(sphereList[i].isCounterclockwise);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * orbitVelocity;
        StartCoroutine("Orbit");
    }

    private IEnumerator Orbit()
    {
        for (int i = 0; i < numObj; i++) {

            SphereInitialContext sphereIC = sphereList[i];
            int rotDirection = (sphereIC.isCounterclockwise ? 1 : -1);
            Debug.Log(sphereIC.initialPosition);

            //sphereIC.sphere.transform.SetPositionAndRotation( new Vector3(Mathf.Cos(rotDirection * time + sphereIC.initialPosition.x) * xSpread, Mathf.Sin(time + sphereIC.initialPosition.y) * ySpread, Mathf.Sin(rotDirection * time + sphereIC.initialPosition.z) * zSpread )+new Vector3(xPos,yPos,zPos), Quaternion.identity);
            sphereIC.sphere.transform.SetPositionAndRotation(new Vector3(Mathf.Cos(rotDirection * time + sphereIC.initialPosition.x)* Mathf.Sin(delta) * xSpread, Mathf.Cos(delta) * ySpread, Mathf.Sin(delta) * Mathf.Sin(time + sphereIC.initialPosition.z) * zSpread) + new Vector3(xPos, yPos, zPos), Quaternion.identity);

        }
        yield return null;

    }

    private Vector2 randomPointOnSphereToAngles()
    {
        Vector3 pos = Random.onUnitSphere;
        Debug.Log("Coordinate sfera: "+pos.x +" "+ pos.y+" " + pos.z);
        //return new Vector2(-Mathf.Atan(pos.x / pos.y), Mathf.Atan(pos.z / (Mathf.Sqrt(Mathf.Pow(pos.x , 2) + Mathf.Pow(pos.y , 2)))));
        return new Vector2(Mathf.Acos(pos.x), Mathf.Asin(pos.y));
    }

    public SpawnerManager(int numObj, float xSpread, float ySpread, float zSpread, float xPos, float yPos, float zPos, float freqXOrbit, float freqYOrbit, float freqZOrbit, bool isRandomSpawning)
    {
        this.xSpread = xSpread;
        this.ySpread = ySpread;
        this.zSpread = zSpread;
        this.xPos = xPos;
        this.yPos = yPos;
        this.zPos = zPos;
        this.freqXOrbit = freqXOrbit;
        this.freqYOrbit = freqYOrbit;
        this.freqZOrbit = freqZOrbit;
        this.numObj = numObj;
        this.isEquidistant = isRandomSpawning;
    }




}
