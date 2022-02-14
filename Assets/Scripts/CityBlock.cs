using System.Collections.Generic;
using UnityEngine;


public class CityBlock : MonoBehaviour
{
    void Start()
    {
        GenerateCityBlock();
    }
    private Transform blockParent;
    private float theMinSize = 1.0F;
    private float useWidth = 2.0F;
    private readonly float defaultUseWidth = 2.0F;
    public Vector2Int buildingsCurMax = new Vector2Int(0, 20);
    private Vector2 buildingLengthMinMax = new Vector2(1.0F, 6.0F);
    public Vector2 blockSize = new Vector2(10.0F,10.0F);
    public Material mat;
    [Tooltip("rotation noise for helicopter path start objects")]
    public float noiseStrength = 0.25F;
    public Color[] cols = new Color[5];
    public CubeCity cubeCity;
    [Tooltip("super skyscrapers here")]
    public List<GameObject> sSSHere = new List<GameObject>();
    private float superSkyWidth = 4.0F;
    [Tooltip("helicopter path start objects")]
    public List<Transform> heliPathRefGOs = new List<Transform>();

    public void SetUp(int maxBuildings, Vector2 instanceSize, Color[] setCols, Material setMat, CubeCity setCubeCity)
    {
        buildingsCurMax[1] = maxBuildings;
        blockSize = new Vector2(instanceSize[0], instanceSize[1]);
        cols = setCols;
        mat = setMat;
        cubeCity = setCubeCity;
    }
    private void GenerateCityBlock()
    {
        blockParent = new GameObject("Parent Buildings").transform;
        blockParent.parent = transform;

        float curX = transform.position.x - blockSize.x * 0.5F + useWidth * 0.5F;
        float endX = transform.position.x + blockSize.x * 0.5F;
        float startZ = transform.position.z - blockSize.y * 0.5F;
        float endZ = transform.position.z + blockSize.y * 0.5F;
        float curZ = startZ;
        while (curX < endX && buildingsCurMax[0] < buildingsCurMax[1])
        {
            while (curZ < endZ && buildingsCurMax[0] < buildingsCurMax[1])
            {
                buildingsCurMax[0]++;
                Transform building = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                building.parent = blockParent;
                
                Material instanceMat = new Material(mat);
                float colNoise = (Random.value - 0.5F) * noiseStrength;
                instanceMat.color = cols[Random.Range(0, cols.Length)] + new Color(colNoise, colNoise, colNoise);
                building.GetComponent<MeshRenderer>().sharedMaterial = instanceMat;
                float length = Mathf.Min(Random.Range(buildingLengthMinMax.x, buildingLengthMinMax.y), endZ - curZ);
                
                // always end on buildings
                if (curZ + length > endZ - 1.0F)
                {
                    length = endZ - curZ;
                }

                float buildingHeight = Random.Range(1.0F, 4.0F);
                float roll = Random.value;
                string typeStr = "Apartment Building";
                if (roll > 0.85F)
                {
                    typeStr = "Sky Scraper Building";
                    buildingHeight = Random.Range(5.0F, 10.0F);
                }

                int totalBlocks = Mathf.RoundToInt(cubeCity.cityBlockSizeXZ.x * cubeCity.cityBlockSizeXZ.y);

                if (sSSHere.Count < 1 && curX + superSkyWidth < endX && length >= buildingLengthMinMax.y*0.5F) {
                    if (cubeCity.superSkyScrapers.Count < Mathf.FloorToInt(cubeCity.superSkyScrapersAvgPerBlock * totalBlocks))
                    {
                        float roll2 = Random.value;
                        if (roll2 >= 0.9F)
                        {
                            typeStr = "Super Sky Scraper Building";
                            BoxCollider instanceBC = building.gameObject.GetComponent<BoxCollider>();
                            instanceBC.isTrigger = true;
                            sSSHere.Add(building.gameObject);
                            cubeCity.superSkyScrapers.Add(building.gameObject);
                            buildingHeight = Random.Range(16.0F, 22.0F);
                            useWidth = superSkyWidth;

                            for (int i = 0; i < 2; i++)
                            {
                                // objects for possible heli loops
                                GameObject instanceHeliLoopGO = GameObject.CreatePrimitive(PrimitiveType.Cube);

                                Destroy(instanceHeliLoopGO.transform.GetComponent<MeshRenderer>());

                                instanceHeliLoopGO.transform.parent = building;
                                instanceHeliLoopGO.transform.name = "possible heli loop";
                                float yPos = Random.Range(0.1F, 0.7F);
                                float moreXZ = Random.Range(5.1F, 5.3F);
                                instanceHeliLoopGO.transform.localPosition = new Vector3(0.0F, yPos, 0.0F);
                                instanceHeliLoopGO.transform.localScale = new Vector3(instanceHeliLoopGO.transform.localScale.x* moreXZ, instanceHeliLoopGO.transform.localScale.y*0.08F, instanceHeliLoopGO.transform.localScale.z* moreXZ);
                                float noiseScale = Random.Range(0.0F, 3.5F);
                                float rotateFullyHere = Random.Range(0.0F, 360.0F);
                                instanceHeliLoopGO.transform.localEulerAngles = new Vector3(Random.value* noiseScale - Random.value* noiseScale, rotateFullyHere, Random.value* noiseScale - Random.value* noiseScale);
                                

                                heliPathRefGOs.Add(instanceHeliLoopGO.transform);
                            }
                        }
                    }
                }

                building.transform.name = typeStr + buildingsCurMax[0];
                building.transform.localScale = new Vector3(useWidth, buildingHeight, length);
                building.transform.position = new Vector3(curX, buildingHeight/2.0F, curZ + length * 0.5F);
                curZ += length;
                // reset useWidth
                useWidth = defaultUseWidth;

                if (typeStr != "Super Sky Scraper Building")
                {
                    Rigidbody rB = building.gameObject.AddComponent<Rigidbody>();
                    rB.useGravity = false;
                    rB.isKinematic = true;
                    building.gameObject.AddComponent<BuildingPlacementDet>();
                }  

            }
            curX += useWidth;
            curZ = startZ;
        }
    }
}
