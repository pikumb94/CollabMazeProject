using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerManager : MonoBehaviour
{
    public GameObject spherePrefab;
    public OrbitBoxPosition scriptObj=null;
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
    private bool isEquidistant = true;//PER ORA SI LASCIA EQUIDISTANTI E SI USA IL MOVIMENTO PER "CONFONDERE"
    [SerializeField] private bool isSameRotationDirection = true;
    [SerializeField] private bool isRandomFrequencies = false;
    [SerializeField] private bool isOrbitSpeedIncreasing = false;//NB: SE QUESTA è FALSA, LA ORBIT SPEED è COSTANTE E DI 0.5f
    [SerializeField] private float endingOrbitVelocity = 1f;
    [SerializeField] private float orbitVelocity = 0.5f;
    [SerializeField] private bool randomizeColours = false;
    /* RIATTIVA SE VUOI USARE ELLISSE
    [Range(0f, Mathf.PI)]
    public float delta; */

    private float time;

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
    public List<SphereInitialContext> sphereList;

    void Awake()
    {
        if (scriptObj != null)
            setInternalParametersFromSO(scriptObj);
    }

    void Start()
    {
        sphereList = new List<SphereInitialContext>();
        time = Time.time;
        

        if (isRandomFrequencies)
        {
            freqXOrbit = Random.Range(1, 11);
            freqYOrbit = Random.Range(1, 11);
            freqZOrbit = Random.Range(1, 11);
        }



        for (int i = 0; i < numObj; i++)
        {
            //if(isEquidistant)
            Debug.Log(this.transform.position);
            if (isSameRotationDirection)
                sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, new Vector3((2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i), Random.Range(0, 2) >= 1 ? true : true));
            else
                sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, new Vector3((2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i, (2 * Mathf.PI / numObj) * i), Random.Range(0, 2) >= 1 ? true : false));

            /*else

                if (isSameRotationDirection)
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, randomPointOnSphereToAngles(), Random.Range(0, 2) >= 1 ? true : true));
                else
                    sphereList.Add(new SphereInitialContext(Instantiate(spherePrefab, this.transform.position, Quaternion.identity) as GameObject, randomPointOnSphereToAngles(), Random.Range(0, 2) >= 1 ? true : false));

                */
            if (randomizeColours) {
                Color iObjColor;
                iObjColor = Color.HSVToRGB((1.0f / numObj) * i, 1, 1);
                iObjColor.a = sphereList[i].sphere.GetComponent<MeshRenderer>().material.color.a;
                sphereList[i].sphere.GetComponent<MeshRenderer>().material.color = iObjColor;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (isOrbitSpeedIncreasing) { 
            if (orbitVelocity <= 0.1f+ endingOrbitVelocity) { 
                orbitVelocity = 0.1f + endingOrbitVelocity*(float) 1 / (GameManager.instance.countdownSeconds) * Time.time;
            }
            

        }
        time += Time.deltaTime * orbitVelocity;
        StartCoroutine("Orbit");
    }

    private IEnumerator Orbit()
    {
        for (int i = 0; i < numObj; i++) {

            SphereInitialContext sphereIC = sphereList[i];
            int rotDirection = (sphereIC.isCounterclockwise ? 1 : -1);

            sphereIC.sphere.transform.SetPositionAndRotation( new Vector3(rotDirection * Mathf.Cos(freqXOrbit*time + sphereIC.initialPosition.x) * xSpread, rotDirection * Mathf.Sin(freqYOrbit* time + sphereIC.initialPosition.y) * ySpread, Mathf.Sin(freqZOrbit*time + sphereIC.initialPosition.z) * zSpread )+new Vector3(xPos,yPos,zPos)+ this.transform.position, Quaternion.identity);
            
            //PIANO XZ DI UN ELLISSE
            //sphereIC.sphere.transform.SetPositionAndRotation(new Vector3(rotDirection * Mathf.Cos( time + sphereIC.initialPosition.x)* Mathf.Sin(delta) * xSpread, Mathf.Cos(delta) * ySpread, Mathf.Sin(delta) * Mathf.Sin(time + sphereIC.initialPosition.z) * zSpread) + new Vector3(xPos, yPos, zPos), Quaternion.identity);
            

        }
        yield return null;

    }

    private Vector2 randomPointOnSphereToAngles()
    {
        Vector3 pos = Random.onUnitSphere;
        //return new Vector2(-Mathf.Atan(pos.x / pos.y), Mathf.Atan(pos.z / (Mathf.Sqrt(Mathf.Pow(pos.x , 2) + Mathf.Pow(pos.y , 2)))));
        return new Vector2(Mathf.Acos(pos.x), Mathf.Asin(pos.y));
    }

    public SpawnerManager(int numObj, float xSpread, float ySpread, float zSpread, float xPos, float yPos, float zPos, float freqXOrbit, float freqYOrbit, float freqZOrbit,
        bool isSameRotationDirection, bool isRandomFrequencies, bool isOrbitSpeedIncreasing, float endingOrbitVelocity = 1f, float orbitVelocity = 0.5f)
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
    }

    private void setInternalParametersFromSO(OrbitBoxPosition sO)
    {
        this.xSpread = sO.xSpread;
        this.ySpread = sO.ySpread;
        this.zSpread = sO.zSpread;
        this.xPos = sO.xPos;
        this.yPos = sO.yPos;
        this.zPos = sO.zPos;
        //this.freqXOrbit = sO.freqXOrbit;
        //this.freqYOrbit = sO.freqYOrbit;
        //this.freqZOrbit = sO.freqZOrbit;
        //this.numObj = sO.numObj;
        //this.isSameRotationDirection = sO.isSameRotationDirection;
        //this.isRandomFrequencies = sO.isRandomFrequencies;
        //this.isOrbitSpeedIncreasing = sO.isOrbitSpeedIncreasing;
        //this.endingOrbitVelocity = sO.endingOrbitVelocity;
        //this.orbitVelocity = sO.orbitVelocity;

}


}
