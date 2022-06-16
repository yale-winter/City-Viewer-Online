using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBodyDetection : MonoBehaviour
{
    CubeCity cubeCity;
    void Awake()
    {
        cubeCity = GameObject.Find("CubeCity").GetComponent<CubeCity>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CarFrontDet"))
        {
            cubeCity.CarTrafficJam(other.transform.parent.name);
        }

    }
}
