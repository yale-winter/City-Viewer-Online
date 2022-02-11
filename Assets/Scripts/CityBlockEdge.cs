using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockEdge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        GenerateCityBlock();
    }
    private Transform blockParent;
    private float theMinSize = 1.0F;
    private float useWidth = 1.0F;
    public Vector2Int buildingsCurMax = new Vector2Int(0, 20);
    private Vector2Int edgesCurMax = new Vector2Int(0, 4);
    private Vector2 buildingLengthMinMax = new Vector2(1.0F, 3.0F);
    public Vector2 blockSize = new Vector2(10.0F, 10.0F);
    public Material mat;
    public float noiseStrength = 0.25F;
    public Color[] cols = new Color[8];
    private Transform[] edgeGroupParent = new Transform[4];
    private void GenerateCityBlock()
    {
        blockSize = new Vector2(transform.localScale.x, transform.localScale.z);
        /*
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        */
        
        blockParent = new GameObject("Block Parent").transform;
        blockParent.parent = transform;
        //blockParent.localPosition = Vector3.zero;
        //blockParent.localScale = Vector3.one;

        float curX = transform.position.x - blockSize[0] * 0.5F;// + useWidth * 0.5F;
        float endX = transform.position.x + blockSize[0] * 0.5F;
        float startZ = transform.position.z - blockSize[1] * 0.5F;
        float endZ = transform.position.z + blockSize[1] * 0.5F;
        float curZ = startZ;

        while (edgesCurMax[0] < edgesCurMax[1] && buildingsCurMax[0] < buildingsCurMax[1])
        {
            edgeGroupParent[edgesCurMax[0]] = new GameObject("Edge Parent" + edgesCurMax[0]).transform;
            edgeGroupParent[edgesCurMax[0]].parent = blockParent;
            while (curZ < endZ - theMinSize && buildingsCurMax[0] < buildingsCurMax[1])
            {
                buildingsCurMax[0]++;
                Transform building = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                building.parent = edgeGroupParent[edgesCurMax[0]];//blockParent;

                Material instanceMat = new Material(mat);
                float colNoise = (Random.value - 0.5F) * noiseStrength;
                instanceMat.color = cols[Random.Range(0, cols.Length)] + new Color(colNoise, colNoise, colNoise);
                //instanceMat.color = cols[Random.Range(0, cols.Length)];


                building.GetComponent<MeshRenderer>().sharedMaterial = instanceMat;

                float length = Mathf.Min(Random.Range(buildingLengthMinMax.x, buildingLengthMinMax.y), endZ - curZ);

                // always end on building 
                if (curZ + length > endZ - 1.0F)
                {
                    length = endZ - curZ;
                }

                useWidth = Random.Range(buildingLengthMinMax.x, buildingLengthMinMax.y);
                float pushLeft = 0.0F;
                pushLeft = useWidth / 2.0F;
                building.transform.localScale = new Vector3(useWidth, Random.Range(1.0F, 3.0F), length);
                building.transform.position = new Vector3(curX + pushLeft, 0.0F, curZ + length * 0.5F);
                curZ += length;
            }
            curX += useWidth;
            curZ = startZ;
            edgesCurMax[0]++;

            // rot test
            //transform.Rotate(0.0F,90.0F,0.0F);

            if (edgesCurMax[0] % 2 == 0)
            {
                endZ = transform.position.z + blockSize[1] * 0.5F;
            }
            else
            {
                endZ = transform.position.z + blockSize[0] * 0.5F;
            }
            
        }
    }
}
