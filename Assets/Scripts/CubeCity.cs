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
    private float stopLightDetectionSize = 4.0F;
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
    // street lights / intersections
    [SerializeField]
    private List<inCol> sLMatrix = new List<inCol>();
    public List<GameObject> streetLights = new List<GameObject>();
    [SerializeField]
    private List<StreetLightController> streetLightControllers = new List<StreetLightController>();
    public GameObject signStopLight;
    // cars
    public GameObject car;
    public GameObject camCar;
    [SerializeField]
    public List<CarController> carControllers = new List<CarController>();
    public List<EaserEase> eases = new List<EaserEase>();



    void Start()
    {
        parentCubeCity = new GameObject("Parent Cube City").transform;
        parentCubeCity.parent = transform;
        parentCars = new GameObject("Parent Cars").transform;
        parentCars.parent = transform;
        MakeCubeCity();
    }
    private void MakeCubeCity()
    {
        SpawnStopLights();
        Debug.Log("sLMatrix.Count " + sLMatrix.Count);
        SpawnCubeCity(new Vector2Int(0, 0));
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
            instanceSSL.transform.position = new Vector3(instanceSSL.transform.position.x, instanceSSL.transform.position.y + 1.3F, instanceSSL.transform.position.z);
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
                    instanceCB.SetUp(maxBuildingsInBlock, instanceSetSize, bColors, mat);
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
                    instanceCPC.transform.parent = parentCars;
                    PathCreator cPCScript = instanceCPC.AddComponent<PathCreator>();
                    BezierPath newCarPath = new BezierPath(pathPoints);
                    cPCScript.bezierPath = newCarPath;
                    cPCScript.bezierPath.AutoControlLength = 0.01F;

                    // car path follower
                    PathFollower instancePF = instanceCar.AddComponent<PathFollower>();
                    instancePF.pathCreator = cPCScript;

                    // car model
                    int instanceIndx = carControllers.Count;
                    CarModel instanceCarModel = new CarModel(instanceIndx);

                    // car controller
                    CarController instanceCC = instanceCar.AddComponent<CarController>();
                    instanceCC.SetUp(instanceIndx, instanceCar, instanceCarModel, cPCScript, eases);
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
}
