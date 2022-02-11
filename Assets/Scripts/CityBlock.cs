using System.Collections;
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
    public Vector2Int buildingsCurMax = new Vector2Int(0, 20);
    private Vector2 buildingLengthMinMax = new Vector2(1.0F, 6.0F);
    public Vector2 blockSize = new Vector2(10.0F,10.0F);
    public Material mat;
    public float noiseStrength = 0.25F;
    public Color[] cols = new Color[5];

    public void SetUp(int maxBuildings, Vector2 instanceSize, Color[] setCols, Material setMat)
    {
        buildingsCurMax[1] = maxBuildings;
        blockSize = new Vector2(instanceSize[0], instanceSize[1]);
        cols = setCols;
        mat = setMat;
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
                string typeStr = "Apartment Building ";
                if (roll > 0.85F)
                {
                    typeStr = "Sky Scraper Building ";
                    buildingHeight = Random.Range(5.0F, 10.0F);
                }
                building.transform.name = typeStr + buildingsCurMax[0];
                building.transform.localScale = new Vector3(useWidth, buildingHeight, length);
                building.transform.position = new Vector3(curX, buildingHeight/2.0F, curZ + length * 0.5F);
                curZ += length;


                Vector2 textureSize = new Vector2(512.0F,512.0F);
                instanceMat.mainTextureScale = new Vector2(transform.localScale.x / textureSize.x, transform.localScale.y / textureSize.y);

            }
            curX += useWidth;
            curZ = startZ;
        }
    }
}
