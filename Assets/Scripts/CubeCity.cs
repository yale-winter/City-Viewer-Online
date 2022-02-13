using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class CubeCity : MonoBehaviour
{
    // attributes of cube city
    public Vector2Int numBlocksXZ = new Vector2Int(3, 4);
    public Vector2 blockSizeXZ = new Vector2(20.0F, 10.0F);
    private float stopLightDetectionSize = 2.0F;
    public float helicoptersAvgPerBlock = 1.0F;
    public float superSkyScrapersAvgPerBlock = 1.0F;
    public Color[] bColors = new Color[5];
    public Color[] sLColors = new Color[4];
    public Material bulbMat;
    public Material mat;
    public int maxBuildingsInBlock = 250;
    public float roadBuffer = 8.0F;
    public Material roadMat;

    // objects of cube city
    private Transform parentCubeCity;
    private Transform parentCars;
    private Transform parentHeliPaths;
    private Transform parentCarPaths;
    // street lights / intersections
    [SerializeField]
    private List<inCol> sLMatrix = new List<inCol>();
    public List<GameObject> streetLights = new List<GameObject>();
    [SerializeField]
    private List<StreetLightController> streetLightControllers = new List<StreetLightController>();
    public GameObject signStopLight;
    // cars
    public GameObject car;
    [SerializeField]
    public List<CarController> carControllers = new List<CarController>();
    public List<GameObject> superSkyScrapers = new List<GameObject>();

    // helicopters
    private Transform parentHelis;
    public GameObject heli;
    [SerializeField]
    public List<HelicopterController> heliControllers = new List<HelicopterController>();

    public GameObject[] heliPaths = new GameObject[3];
    private PathCreator usePath;
    private List<PathCreator> instancePathPrefabs = new List<PathCreator>();
    public GameObject pathDetCheckPrefab;

    void Start()
    {
        parentCubeCity = new GameObject("Parent Cube City").transform;
        parentCubeCity.parent = transform;
        parentCars = new GameObject("Parent Cars").transform;
        parentCars.parent = transform;
        parentCarPaths = new GameObject("Parent Car Paths").transform;
        parentCarPaths.parent = transform;
        parentHelis = new GameObject("Parent Helicopters").transform;
        parentHelis.parent = transform;
        parentHeliPaths = new GameObject("Parent Heli Paths").transform;
        parentHeliPaths.parent = transform;
        MakeCubeCity();
    }
    private void MakeCubeCity()
    {
        SpawnStopLights();
        Debug.Log("sLMatrix.Count " + sLMatrix.Count);
        SpawnCubeCity(new Vector2Int(0, 0));
        StartCoroutine("AddHelicoptersSoon");
    }
    // spawn all the Stop Lights aka Intersections
    private void SpawnStopLights()
    {
        Vector2Int onColRow = new Vector2Int(0, 0);
        Vector2 xZPush = new Vector2(0.0F, 0.0F);
        while (streetLights.Count < numBlocksXZ[0] * numBlocksXZ[1])
        {
            streetLights.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            int iI = streetLights.Count - 1;
            streetLights[iI].transform.parent = parentCubeCity;
            float instanceSize = stopLightDetectionSize;
            streetLights[iI].transform.localScale = new Vector3(instanceSize, instanceSize, instanceSize);
            streetLights[iI].transform.name = "Street Light (" + iI + ")";
            streetLights[iI].transform.position = new Vector3(xZPush[0], 0.0F, xZPush[1]);
            Rigidbody mRB = streetLights[iI].AddComponent<Rigidbody>();
            mRB.useGravity = false;
            mRB.isKinematic = true;
            Destroy(streetLights[iI].GetComponent<MeshRenderer>());
            IntersectionCollider instanceIC = streetLights[iI].AddComponent<IntersectionCollider>();

            // create the sign street light 
            GameObject instanceSSL = Instantiate(signStopLight);
            instanceSSL.transform.name = "Sign Street Light (" + iI + ")";
            instanceSSL.transform.parent = streetLights[iI].transform;
            instanceSSL.transform.localPosition = Vector3.zero;
            instanceSSL.transform.position = new Vector3(instanceSSL.transform.position.x, instanceSSL.transform.position.y + 1.4F, instanceSSL.transform.position.z);
            int thisIndex = streetLights.Count - 1;
            StreetLightModel instanceSLM = new StreetLightModel(thisIndex, xZPush[0], xZPush[1]);
            SignStopLightView instanceSSLV = instanceSSL.GetComponent<SignStopLightView>();
            StreetLightController instanceSLC = instanceSSL.AddComponent<StreetLightController>();
            instanceSLC.SetUp(instanceIC, instanceSSLV, instanceSLM, sLColors, bulbMat, this);
            streetLightControllers.Add(instanceSLC);
            // create StreetLight.cs adj list
            if (sLMatrix.Count <= onColRow[0])
            {
                inCol instanceInCol = new inCol();
                sLMatrix.Add(instanceInCol);
            }
            sLMatrix[onColRow[0]].inRow.Add(instanceSLC);

            onColRow[0]++;
            xZPush[0] += blockSizeXZ[0];
            if (onColRow[0] > numBlocksXZ[0] - 1)
            {
                onColRow[0] = 0;
                onColRow[1]++;
                xZPush[0] = 0.0F;
                xZPush[1] -= blockSizeXZ[1];
            }
        }
    }
    private void SpawnCubeCity(Vector2Int sIndxAdjList)
    {
        if (sIndxAdjList[1] < numBlocksXZ[1]) {
            //Debug.Log("looking to spawn city block at " + sIndxAdjList);
            // need to spawn cube city here? check the location to the right bottom
            Vector2Int botRightXZ = new Vector2Int(sIndxAdjList[0] + 1, sIndxAdjList[1] + 1);

            if (botRightXZ[0] < sLMatrix.Count) {
                if (botRightXZ[1] < sLMatrix[botRightXZ[0]].inRow.Count)
                {
                    bool clockWisePath = true;
                    if (sIndxAdjList[0] % 2 == 0)
                    {
                        clockWisePath = !clockWisePath;
                    }
                    if (sIndxAdjList[1] % 2 == 0)
                    {
                        clockWisePath = !clockWisePath;
                    }
                    //Debug.Log("found rect area to build block city starts at: " + sIndxAdjList);
                    // create city block's buildings - - - - - - - - - - -
                    GameObject instanceCityBlockGO = new GameObject("Parent City Block");
                    CityBlock instanceCB = instanceCityBlockGO.AddComponent<CityBlock>();
                    Vector2 instanceSetSize = new Vector2(blockSizeXZ.x - roadBuffer, blockSizeXZ.y - roadBuffer);
                    //Debug.Log("instanceSetSize: " + instanceSetSize);
                    instanceCB.SetUp(maxBuildingsInBlock, instanceSetSize, bColors, mat, this);
                    instanceCityBlockGO.transform.parent = parentCubeCity;
                    Vector3 setInstancePos = new Vector3(sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.xPos, 0.0F, sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.zPos);
                    setInstancePos += new Vector3(blockSizeXZ[0] / 2.0F, 0.0F, blockSizeXZ[1] / -2.0F);
                    instanceCityBlockGO.transform.position = setInstancePos;

                    // create road ring around the block - - - - - - - -
                    GameObject instanceRoad = new GameObject("straight road");
                    instanceRoad.transform.parent = instanceCityBlockGO.transform;
                    PathCreation.PathCreator newPath = instanceRoad.AddComponent<PathCreation.PathCreator>();

                    // GameObject instanceRoad = Instantiate(roadPrefab);
                    // instanceRoad.transform.parent = instanceCityBlockGO.transform;
                    // PathCreation.PathCreator newPath = instanceRoad.GetComponent<PathCreation.PathCreator>();
                    //newPath
                    List<Vector3> pathPoints = new List<Vector3>();
                    pathPoints.Add(Vector3.zero);
                    if (clockWisePath)
                    {
                        pathPoints.Add(new Vector3(blockSizeXZ.x, 0.0F, 0.0F));
                        pathPoints.Add(new Vector3(blockSizeXZ.x, 0.0F, blockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(0.0F, 0.0F, blockSizeXZ.y * -1.0F));
                    }
                    else
                    {
                        pathPoints.Add(new Vector3(0.0F, 0.0F, blockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(blockSizeXZ.x, 0.0F, blockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(blockSizeXZ.x, 0.0F, 0.0F));
                    }
                        pathPoints.Add(Vector3.zero);
                    for (int i = 0; i < pathPoints.Count; i++)
                    {
                        Vector3 instancePivotCenter = new Vector3(sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.xPos, 0.02F, sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.zPos);
                        pathPoints[i] += instancePivotCenter;
                    }
                    BezierPath newBPath = new BezierPath(pathPoints);
                    newPath.bezierPath = newBPath;

                    //fix path to be straight
                    newPath.bezierPath.AutoControlLength = 0.01F;

                    // road mesh
                    RoadMeshCreator instanceRoadMesh = instanceRoad.AddComponent<RoadMeshCreator>();
                    instanceRoadMesh.roadMaterial = roadMat;
                    instanceRoadMesh.undersideMaterial = roadMat;
                    instanceRoadMesh.roadWidth = 0.6F;

                    // create cars
                    GameObject instanceCar = Instantiate(car);
                    instanceCar.transform.parent = parentCars;
                    instanceCar.transform.name = "car " + carControllers.Count;

                    // car path
                    GameObject instanceCPC = new GameObject("CarPathCreat " + carControllers.Count);
                    instanceCPC.transform.parent = parentCarPaths;
                    PathCreator cPCScript = instanceCPC.AddComponent<PathCreator>();
                    BezierPath newCarPath = new BezierPath(pathPoints);
                    cPCScript.bezierPath = newCarPath;
                    cPCScript.bezierPath.AutoControlLength = 0.01F;



                    // car model
                    int instanceIndx = carControllers.Count;
                    CarModel instanceCarModel = new CarModel(instanceIndx, Random.Range(3.0F, 7.0F));

                    // car path follower
                    PathFollower instancePF = instanceCar.AddComponent<PathFollower>();
                    instancePF.pathCreator = cPCScript;
                    instancePF.speed = instanceCarModel.speed;

                    // car controller
                    CarController instanceCC = instanceCar.AddComponent<CarController>();
                    instanceCC.SetUp(instanceIndx, instanceCar, instanceCarModel, cPCScript);
                    carControllers.Add(instanceCC);
                }
            }

            sIndxAdjList[0]++;
            if (sIndxAdjList[0] >= numBlocksXZ[0])
            {
                sIndxAdjList[0] = 0;
                sIndxAdjList[1]++;
            }
            SpawnCubeCity(sIndxAdjList);
        }
    }
    private IEnumerator AddHelicoptersSoon()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.01F);
        if (superSkyScrapers.Count > 0)
        {
            AddHelicopters();
        }
    }

    private void AddHelicopters()
    {
        for (int i = 0; i < superSkyScrapers.Count; i++)
        {
            for (int i2 = 0; i2 < superSkyScrapers[i].transform.childCount; i2++)
            {
                    int rollPath = Random.Range(0, 2);
                    usePath = heliPaths[rollPath].GetComponent<PathCreator>();
                    PathCreator path = Instantiate(usePath, superSkyScrapers[i].transform.GetChild(i2).position, superSkyScrapers[i].transform.GetChild(i2).rotation);
                    path.bezierPath.GlobalNormalsAngle = 0.0F;
                    path.gameObject.transform.name = "heli path";
                    path.transform.parent = parentHeliPaths;
                    instancePathPrefabs.Add(path);

                    // path detection (should not get too near any super sky scrapers ordestroy the path)
                    
                    GameObject pathDetection = new GameObject("Path Detection");
                    
                    pathDetection.transform.parent = path.transform;
                    PathPlacer instancePathPlacer = pathDetection.AddComponent<PathPlacer>();
                    GameObject pathDetHolder = new GameObject("Path Det Holder");
                    pathDetHolder.AddComponent<PathDetDestroySoon>();
                    pathDetHolder.transform.parent = path.transform;
                    instancePathPlacer.pathCreator = path;
                    instancePathPlacer.prefab = pathDetCheckPrefab;
                    instancePathPlacer.holder = pathDetHolder;
            }
        }
        StartCoroutine(SpawnHelicoptersSoon());
    }
    private IEnumerator SpawnHelicoptersSoon()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.02F);
        if (parentHeliPaths.childCount > 0)
        {
            SpawnHelicopters();
        }
    }
    public void SpawnHelicopters()
    {
        int maxHelis = Mathf.RoundToInt(helicoptersAvgPerBlock * numBlocksXZ.x * numBlocksXZ.y);
        if (maxHelis > 200)
        {
            maxHelis = 200;
        }
        int tries = 0;
        
        List<Vector3> pathPoints = new List<Vector3>();
        pathPoints.Add(Vector3.zero);
        pathPoints.Add(Vector3.one);




        while (heliControllers.Count < maxHelis && tries < 300)
        {


            // create helis
            GameObject instanceHeli = Instantiate(heli);
            instanceHeli.transform.parent = parentHelis;
            instanceHeli.transform.name = "heli " + heliControllers.Count;

      
            // heli model
            int instanceIndx = heliControllers.Count;
            HelicopterModel instanceHeliModel = new HelicopterModel(instanceIndx, Random.Range(3.0F,10.0F), 0.0F);

            PathFollower instancePF = instanceHeli.AddComponent<PathFollower>();
            instancePF.speed = instanceHeliModel.speed;
            int pickPathInstance = heliControllers.Count;
            while (pickPathInstance >= parentHeliPaths.childCount)
            {
                pickPathInstance -= parentHeliPaths.childCount;
            }
            instancePF.pathCreator = parentHeliPaths.GetChild(pickPathInstance).GetComponent<PathCreator>();


            // heli controller
            HelicopterController instanceHC = instanceHeli.AddComponent<HelicopterController>();
            HelicopterView instanceHelicopterView = instanceHeli.AddComponent<HelicopterView>();

            instanceHC.SetUp(instanceHeliModel, instanceHelicopterView);
            heliControllers.Add(instanceHC);

            tries++;
        }
    }
}
