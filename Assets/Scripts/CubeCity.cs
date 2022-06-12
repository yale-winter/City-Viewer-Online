using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeCity : MonoBehaviour
{
    public List<GameObject> allCityBlocks = new List<GameObject>();
    public GameObject freewayPath;
    Vector3 freewayPos = new Vector3(-5.5f, 0.0f, -10.3f);
    public GameObject freewayPath2;
    Vector3 freewayPos2 = new Vector3(101.0f, 0.0f, -18.39f);
    /// <summary>
    /// number of city blocks to use in X and Z
    /// </summary>
    public Vector2Int numCityBlocksXZ = new Vector2Int(3, 4);
    /// <summary>
    /// size of each city blocks in X and Z
    /// </summary>
    public Vector2 cityBlockSizeXZ = new Vector2(20.0f, 10.0f);

    /// <summary>
    /// average number of helicopters to spawn per city block (numCityBlocksXZ.x * numCityBlocksXZ.y)
    /// </summary>
    public float helicoptersAvgPerBlock = 1.0f;
    /// <summary>
    /// average number of super skyscrapers to spawn per city block (numCityBlocksXZ.x * numCityBlocksXZ.y)
    /// </summary>
    public float superSkyScrapersAvgPerBlock = 1.0f;
    public Color[] sLColors = new Color[4];
    public Material bulbMat;
    public Material mat;
    private int maxBuildingsInBlock = 250;

    /// <summary>
    /// distance to give space for roads inbetween city blocks
    /// </summary>
    private float roadBuffer = 2.0f;
    public Material roadMat;

    // objects of cube city
    public Transform cameraTarget;
    public Transform camP;
    private Vector2Int camViewCurMax = new Vector2Int(0, 3);
    public List<Vector3> cameraSettings = new List<Vector3>();
    private Transform parentCubeCity;
    private Transform parentCars;
    private Transform parentHeliPaths;
    private Transform parentCarPaths;

    private float stopLightDetectionSize = 1.25f;

    /// <summary>
    /// adjancency matrix of street lights / intersections
    /// </summary>
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

    private List<PathCreator> instancePathPrefabs = new List<PathCreator>();
    public GameObject pathDetCheckPrefab;
    public GameObject displayText;

    private Persist persist;
    Vector3 viewPos1;
    Vector3 viewPos2;

    public GameObject cityBuilding;

    private int carsTotal = 0;

    private GameObject cameraMovingTarget;


    void Awake()
    {

        if (GameObject.Find("Persist") == null)
        {
            GameObject persistGO = new GameObject("Persist");
            persist = persistGO.AddComponent<Persist>();
        }
        else
        {
            persist = GameObject.Find("Persist").GetComponent<Persist>();
        }
        if (persist.forceLoad > 0)
        {
            loadIndex = persist.forceLoad;
            persist.forceLoad = -1;
        }
        else
        {
            loadIndex = 0;
        }

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
        cameraTarget = new GameObject("Camera Target").transform;
        cameraTarget.position = new Vector3(0.0f, 30.0f, 0.0f);
        camP.position = cameraTarget.position;
        cameraMovingTarget = new GameObject("Camera Moving Target");
        cameraMovingTarget.transform.parent = transform;
    }
    public int loadIndex = 0;
    public void CreateCubeCity(Scores data, bool loadOnline = true)
    {
        //dev
        //loadOnline = false;

        if (loadOnline)
        {
            // import data from most recent save
            numCityBlocksXZ = new Vector2Int(3, 3);
            float sizeMult = data.citySize[loadIndex] / 100.0f;
            Vector2Int addToSize = new Vector2Int((int)Mathf.Round(3.0f * sizeMult), (int)Mathf.Round(5.0f * sizeMult));
            numCityBlocksXZ += addToSize;
            if (numCityBlocksXZ.x % 2 != 0)
            {
                numCityBlocksXZ.x++;
            }
            if (numCityBlocksXZ.y % 2 != 0)
            {
                numCityBlocksXZ.y++;
            }
            Debug.Log("loaded num city blocks: " + numCityBlocksXZ + " size mult: " + sizeMult);

            float heliMult = data.helicopters[loadIndex] / 100.0f;
            helicoptersAvgPerBlock = heliMult * 0.5f;

            float scrapersMult = data.scrapers[loadIndex] / 100.0f;
            superSkyScrapersAvgPerBlock = Mathf.Max(0.01f, scrapersMult);

            string loadColorStr = PadZeros(data.cityColor[loadIndex].ToString(), 9);
            //loadColorStr = "000000000";
            //loadColorStr = "255255255";
            Debug.Log("loadColorStr : " + loadColorStr);
            mat.color = new Color32(byte.Parse(loadColorStr.Substring(0, 3)), byte.Parse(loadColorStr.Substring(3, 3)), byte.Parse(loadColorStr.Substring(6, 3)), 255);
            /*
            roadMat.color = new Color32(byte.Parse(loadColorStr.Substring(0, 3)), byte.Parse(loadColorStr.Substring(3, 3)), byte.Parse(loadColorStr.Substring(6, 3)), 255);
            float brightness = (roadMat.color.r + roadMat.color.g + roadMat.color.b) / 3.0f;
            if (brightness < 0.5)
            {
                //Debug.Log("brighter roads");
                roadMat.color += new Color32(40, 40, 40, 0);
            }
            else
            {
                //Debug.Log("darker roads");
                roadMat.color -= new Color32(40, 40, 40, 0);
            }
            */
            // end import data
            freewayPos2 = new Vector3((numCityBlocksXZ.x - 1) * cityBlockSizeXZ.x, freewayPos2.y, freewayPos2.z);
        }

        viewPos1 = new Vector3((((float)numCityBlocksXZ.x - 1.0f) * cityBlockSizeXZ.x) * 0.5F, 30.0F, ((float)numCityBlocksXZ.y * cityBlockSizeXZ.y) * -0.9f);
        viewPos2 = new Vector3((((float)numCityBlocksXZ.x - 1.0f) * cityBlockSizeXZ.x) * 0.7F, 25.0F, ((float)numCityBlocksXZ.y * cityBlockSizeXZ.y) * -0.9f);


        cameraTarget.position = viewPos1;
        camP.position = cameraTarget.position;
        CreateFreeways();
        SpawnStopLights();

        SpawnCubeCity(new Vector2Int(0, 0));
        StartCoroutine("AddHelicoptersSoon");
    }
    // spawn all the Stop Lights aka Intersections
    private void SpawnStopLights()
    {
        Vector2Int onColRow = new Vector2Int(0, 0);
        Vector2 xZPush = new Vector2(0.0f, 0.0f);
        while (streetLights.Count < numCityBlocksXZ[0] * numCityBlocksXZ[1])
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
            int layerInfo = LayerMask.NameToLayer("IntersectionDet");
            streetLights[iI].layer = layerInfo;
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
            bool[] flipLightAxis = new bool[2];
            // if blocked because on edge redirect for the car to turn and to use other direction
            List<string> blocked = new List<string>();
            if (onColRow[0] == 0)
            {
                blocked.Add("West");
            }
            if (onColRow[0] == numCityBlocksXZ[0] - 1)
            {
                blocked.Add("East");
            }
            if (onColRow[1] == 0)
            {
                blocked.Add("North");
            }
            if (onColRow[1] == numCityBlocksXZ[1] - 1)
            {
                blocked.Add("South");
            }

            /*
            if (numCityBlocksXZ.x % 2 == 0)
            {
                //Debug.Log("even number of stop lights in row");
                // then need to alternate starting
                isEvenCheck += onColRow[1];
            }
            */

            if (onColRow[0] % 2 != 0)
            {
                flipLightAxis[1] = true;
            }

            if (onColRow[1] % 2 != 0)
            {
                flipLightAxis[0] = true;
            }


            bool isCorner = false;
            if (onColRow[0] == numCityBlocksXZ[0] - 1 || onColRow[0] == 0)
            {
                if (onColRow[1] == numCityBlocksXZ[1] - 1 || onColRow[1] == 0)
                {
                    isCorner = true;
                    Debug.Log("found corner " + onColRow[0] + " " + onColRow[1]);
                }
            }

            int freewayEntranceID = numCityBlocksXZ[0] * 3 - 1;
            instanceSLC.SetUp(instanceIC, instanceSSLV, instanceSLM, sLColors, bulbMat, this, flipLightAxis, blocked, isCorner, freewayEntranceID);
            streetLightControllers.Add(instanceSLC);


            // create StreetLight.cs adj list
            if (sLMatrix.Count <= onColRow[0])
            {
                inCol instanceInCol = new inCol();
                sLMatrix.Add(instanceInCol);
            }
            sLMatrix[onColRow[0]].inRow.Add(instanceSLC);

            onColRow[0]++;
            xZPush[0] += cityBlockSizeXZ[0];
            if (onColRow[0] > numCityBlocksXZ[0] - 1)
            {
                onColRow[0] = 0;
                onColRow[1]++;
                xZPush[0] = 0.0F;
                xZPush[1] -= cityBlockSizeXZ[1];
            }
        }
    }
    GameObject instanceFreeway;
    GameObject instanceFreeway2;
    BezierPath freewayBP;
    BezierPath freewayBP2;
    void CreateFreeways()
    {
        // enter city freeway
        instanceFreeway = Instantiate(freewayPath);
        instanceFreeway.transform.name = "freeway";
        instanceFreeway.transform.position = freewayPos;
        instanceFreeway.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        // road mesh
        RoadMeshCreator instanceRoadMesh = instanceFreeway.AddComponent<RoadMeshCreator>();
        instanceRoadMesh.roadMaterial = roadMat;
        instanceRoadMesh.undersideMaterial = roadMat;
        instanceRoadMesh.roadWidth = 0.6f;

        freewayBP = new BezierPath(Vector3.zero);
        freewayBP = instanceFreeway.GetComponent<PathCreator>().bezierPath;


        // exit city freeway
        instanceFreeway2 = Instantiate(freewayPath2);
        instanceFreeway2.transform.name = "freeway2";
        instanceFreeway2.transform.position = freewayPos2;
        instanceFreeway2.transform.localEulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
        // road mesh
        RoadMeshCreator instanceRoadMesh2 = instanceFreeway2.AddComponent<RoadMeshCreator>();
        instanceRoadMesh2.roadMaterial = roadMat;
        instanceRoadMesh2.undersideMaterial = roadMat;
        instanceRoadMesh2.roadWidth = 0.6f;

        freewayBP2 = new BezierPath(Vector3.zero);
        freewayBP2 = instanceFreeway2.GetComponent<PathCreator>().bezierPath;

    }
    private void SpawnCubeCity(Vector2Int sIndxAdjList)
    {
        if (sIndxAdjList[1] < numCityBlocksXZ[1])
        {
            //Debug.Log("looking to spawn city block at " + sIndxAdjList);
            // need to spawn cube city here? check the location to the right bottom
            Vector2Int botRightXZ = new Vector2Int(sIndxAdjList[0] + 1, sIndxAdjList[1] + 1);

            if (botRightXZ[0] < sLMatrix.Count)
            {
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

                    //Debug.Log("instanceSetSize: " + instanceSetSize);
                    


                    Vector2 instanceSetSize = new Vector2(cityBlockSizeXZ.x - roadBuffer, cityBlockSizeXZ.y - roadBuffer);
                    instanceCB.SetUp(maxBuildingsInBlock, instanceSetSize, mat, this);


                    allCityBlocks.Add(instanceCityBlockGO);
                    //Debug.Log("instanceSetSize: " + instanceSetSize);
                    instanceCityBlockGO.transform.parent = parentCubeCity;
                    Vector3 setInstancePos = new Vector3(sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.xPos, 0.0F, sLMatrix[sIndxAdjList[0]].inRow[sIndxAdjList[1]].sLM.zPos);
                    setInstancePos += new Vector3(cityBlockSizeXZ[0] / 2.0F, 0.0F, cityBlockSizeXZ[1] / -2.0F);
                    instanceCityBlockGO.transform.position = setInstancePos;

                    // create road ring around the block - - - - - - - -






                    GameObject instanceRoad = new GameObject("straight road");
                    instanceRoad.transform.parent = instanceCityBlockGO.transform;
                    PathCreation.PathCreator newPath = instanceRoad.AddComponent<PathCreation.PathCreator>();



                    //newPath
                    List<Vector3> pathPoints = new List<Vector3>();
                    pathPoints.Add(Vector3.zero);
                    if (clockWisePath)
                    {
                        pathPoints.Add(new Vector3(cityBlockSizeXZ.x, 0.0F, 0.0F));
                        pathPoints.Add(new Vector3(cityBlockSizeXZ.x, 0.0F, cityBlockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(0.0F, 0.0F, cityBlockSizeXZ.y * -1.0F));
                    }
                    else
                    {
                        pathPoints.Add(new Vector3(0.0F, 0.0F, cityBlockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(cityBlockSizeXZ.x, 0.0F, cityBlockSizeXZ.y * -1.0F));
                        pathPoints.Add(new Vector3(cityBlockSizeXZ.x, 0.0F, 0.0F));
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
                    newPath.bezierPath.AutoControlLength = 0.01f;

                    // road mesh
                    RoadMeshCreator instanceRoadMesh = instanceRoad.AddComponent<RoadMeshCreator>();
                    instanceRoadMesh.roadMaterial = roadMat;
                    instanceRoadMesh.undersideMaterial = roadMat;
                    instanceRoadMesh.roadWidth = 0.6f;



                    // car path
                    GameObject instanceCPC = new GameObject("CarPathCreat " + carControllers.Count);
                    instanceCPC.transform.parent = parentCarPaths;
                    instanceCPC.transform.position = freewayPos;
                    instanceCPC.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                    PathCreator cPCScript = instanceCPC.AddComponent<PathCreator>();
                    cPCScript.bezierPath = freewayBP;


                    GameObject instanceCPC2 = new GameObject("Car2PathCreat " + carControllers.Count);
                    instanceCPC2.transform.parent = parentCarPaths;
                    instanceCPC2.transform.position = freewayPos2;
                    instanceCPC2.transform.localEulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
                    PathCreator cPCScript2 = instanceCPC2.AddComponent<PathCreator>();
                    cPCScript2.bezierPath = freewayBP2;


                    cPCScript.bezierPath = freewayBP;
                    //cPCScript.bezierPath.AutoControlLength = 0.01f;

                    // end car path stuff
                    int createThisManyCarsBlock = Random.Range(0, 2);
                    /*
                    if (sIndxAdjList[0] == 0 && sIndxAdjList[1] == 0)
                    {
                        createThisManyCarsBlock = 1;
                    }
                    */
                    if (carsTotal >= 12)
                    {
                        createThisManyCarsBlock = 0;
                    }
                    //dev
                    createThisManyCarsBlock = 2;

                    // Debug.Log("create this many cars block: " + createThisManyCarsBlock);
                    for (int i = 0; i < createThisManyCarsBlock; i++)
                    {
                        // create cars
                        GameObject instanceCar = Instantiate(car);
                        instanceCar.transform.parent = parentCars;
                        instanceCar.transform.name = "car " + carControllers.Count;

                        // car model
                        int instanceIndx = carControllers.Count;
                        CarModel instanceCarModel = new CarModel(instanceIndx, 6.0f);

                        // car path follower

                        PathFollower instancePF = instanceCar.AddComponent<PathFollower>();
                        instancePF.pathCreator = cPCScript;
                        instancePF.speed = instanceCarModel.speed;
                        instancePF.carID = instanceIndx;
                        instancePF.onFreeway = true;


                        // ?
                        instancePF.enabled = false;
                        instanceCar.transform.eulerAngles = new Vector3(0.0f, 90.0f, -90.0f);


                        // car controller
                        CarController instanceCC = instanceCar.AddComponent<CarController>();
                        // ?
                        instanceCC.pathControlled = false;

                        int rollSL = Random.Range(0, streetLights.Count);
                        Vector3 thisPos = new Vector3(streetLightControllers[rollSL].sLM.xPos, 0.05f, streetLightControllers[rollSL].sLM.zPos);
                        //thisPos += new Vector3(2.0f, 0.0f, 0.0f);
                        instanceCar.transform.position = thisPos;

                        Vector3 enterFwyPos = freewayPos2 + new Vector3(-1.46f, 0.0f, -0.51f);


                        instanceCC.SetUp(instanceIndx, instanceCar, instanceCarModel, cPCScript, cPCScript2, enterFwyPos, streetLightControllers[rollSL].posPassAllowed[0].ToString());//enterFwyPos
                        carControllers.Add(instanceCC);
                        carsTotal++;
                    }
                }
            }

            sIndxAdjList[0]++;
            if (sIndxAdjList[0] >= numCityBlocksXZ[0])
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
        //yield return new WaitForSeconds(0.01F);
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
                PathCreator usePath = heliPaths[rollPath].GetComponent<PathCreator>();
                PathCreator path = Instantiate(usePath, superSkyScrapers[i].transform.GetChild(i2).position, superSkyScrapers[i].transform.GetChild(i2).rotation);

                path.bezierPath.GlobalNormalsAngle = 0.0F;
                path.gameObject.transform.name = "heli path";
                path.transform.parent = parentHeliPaths;
                instancePathPrefabs.Add(path);

                // path detection (should not get too near any super sky scrapers or destroy the path)

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
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForEndOfFrame();
        if (parentHeliPaths.childCount > 0)
        {
            SpawnHelicopters();
        }
    }
    public void SpawnHelicopters()
    {
        int maxHelis = Mathf.RoundToInt(helicoptersAvgPerBlock * numCityBlocksXZ.x * numCityBlocksXZ.y);
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
            HelicopterModel instanceHeliModel = new HelicopterModel(instanceIndx, Random.Range(2.5f, 4.5f));

            PathFollower instancePF = instanceHeli.AddComponent<PathFollower>();
            instancePF.speed = instanceHeliModel.speed;
            instancePF.type = "helicopter";
            int pickPathInstance = Random.Range(0, parentHeliPaths.childCount);
            // Debug.Log("pick path instance: " + pickPathInstance);
            instancePF.pathCreator = parentHeliPaths.GetChild(pickPathInstance).GetComponent<PathCreator>();

            // heli controller
            HelicopterController instanceHC = instanceHeli.AddComponent<HelicopterController>();
            HelicopterView instanceHelicopterView = instanceHeli.AddComponent<HelicopterView>();

            instanceHC.SetUp(instanceHeliModel, instanceHelicopterView);
            heliControllers.Add(instanceHC);

            tries++;
        }
    }
    public void SwitchViewPressed()
    {


        string instanceStr = "";
        camViewCurMax.x++;
        // skip heli view if no helis
        if (camViewCurMax.x == 1 && parentHelis.childCount == 0)
        {
            camViewCurMax.x++;
        }
        // skip car view if no cars
        if (camViewCurMax.x == 2 && parentCars.childCount == 0)
        {
            camViewCurMax.x++;
        }
        if (camViewCurMax.x > camViewCurMax.y)
        {
            camViewCurMax.x = 0;
        }
        if (camViewCurMax.x == 0)
        {
            instanceStr = "City View";
            cameraTarget.parent = null;
            cameraTarget.position = viewPos1;
            cameraTarget.localRotation = Quaternion.identity;
            camP.parent = null;
            camP.position = cameraTarget.position;
            camP.localRotation = Quaternion.identity;
            camP.GetChild(0).localPosition = new Vector3(0.0F, cameraSettings[0].x, cameraSettings[0].y);
            camP.GetChild(0).localEulerAngles = new Vector3(cameraSettings[0].z, 0.0F, 0.0F);
        }
        else if (camViewCurMax.x == 1)
        {



            int chooseHeli = Random.Range(0, parentHelis.childCount - 1);
            cameraTarget.parent = parentHelis.GetChild(chooseHeli);
            cameraTarget.localPosition = Vector3.zero;
            cameraTarget.localRotation = Quaternion.identity;
            camP.parent = cameraTarget;
            camP.localPosition = Vector3.zero;
            camP.localRotation = Quaternion.identity;
            camP.localEulerAngles = new Vector3(0.0F, 0.0F, 90.0F);
            camP.GetChild(0).localPosition = new Vector3(0.0F, cameraSettings[1].x, cameraSettings[1].y);
            camP.GetChild(0).localEulerAngles = new Vector3(cameraSettings[1].z, 0.0F, 0.0F);

            // new
            cameraTarget.transform.eulerAngles = new Vector3(0.0f, -90.0f, -90.0f);
            cameraTarget.parent = null;

            cameraMovingTarget.transform.parent = parentHelis.GetChild(chooseHeli);
            cameraMovingTarget.transform.localPosition = Vector3.zero;


            int showRealNumHeli = chooseHeli + 1;
            instanceStr = "Heli (" + showRealNumHeli + ") View";
        }
        else if (camViewCurMax.x == 2)
        {
            int chooseCar = Random.Range(0, parentCars.childCount - 1);
            int showRealNumCar = chooseCar + 1;
            instanceStr = "Car (" + showRealNumCar + ") View";
            cameraTarget.parent = parentCars.GetChild(chooseCar);
            cameraTarget.localPosition = Vector3.zero;
            cameraTarget.localRotation = Quaternion.identity;
            camP.parent = cameraTarget;
            camP.localPosition = Vector3.zero;
            camP.localRotation = Quaternion.identity;
            camP.localEulerAngles = new Vector3(0.0F, 0.0F, 90.0F);
            camP.GetChild(0).localPosition = new Vector3(0.0F, cameraSettings[2].x, cameraSettings[2].y);
            camP.GetChild(0).localEulerAngles = new Vector3(cameraSettings[2].z, 0.0F, 0.0F);
        }
        else if (camViewCurMax.x == 3)
        {
            instanceStr = "City View 2";
            cameraTarget.parent = null;
            cameraTarget.position = viewPos2;
            cameraTarget.localRotation = Quaternion.identity;
            camP.parent = null;
            camP.position = cameraTarget.position;
            camP.localRotation = Quaternion.identity;
            camP.localEulerAngles = new Vector3(-15.0f, -30.0f, 0.0f);
            camP.GetChild(0).localPosition = new Vector3(0.0F, cameraSettings[0].x, cameraSettings[0].y);
            camP.GetChild(0).localEulerAngles = new Vector3(cameraSettings[0].z, 0.0F, 0.0F);
        }

        displayText.GetComponent<Text>().text = instanceStr;
    }
    private bool startedRestart = false;
    public void ReloadCity()
    {
        if (startedRestart == false)
        {
            startedRestart = true;
            StartCoroutine(ReloadCityIE());
        }
    }
    public IEnumerator ReloadCityIE()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Load");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    private static string PadZeros(string str, int setChars)
    {
        while (str.Length < setChars)
        {
            str = "0" + str;
        }
        return str;
    }
    public void CarExitFreeway(int carID)
    {
        carControllers[carID].ExitFreeway();
        //Debug.Log("car exit freeway " + carID);
    }
    private void Update()
    {
        if (camViewCurMax.x == 1)
        {
            cameraTarget.transform.position = cameraMovingTarget.transform.position;
        }
        for (int i = 0; i < carControllers.Count; i++)
        {
            carControllers[i].PossiblyMove();
        }
    }
}
