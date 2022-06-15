using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBodyDetection : MonoBehaviour
{
    private CubeCity cubeCity;
    private void Awake()
    {
        cubeCity = GameObject.Find("CubeCity").GetComponent<CubeCity>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CarFrontDet"))
        {
            cubeCity.CarTrafficJam(other.transform.parent.name);
        }

    }
}
