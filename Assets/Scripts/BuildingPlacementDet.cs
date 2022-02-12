using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementDet : MonoBehaviour
{
    private float startTime = float.PositiveInfinity;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Substring(0,5) == "Super")
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        startTime = Time.time;
    }
    private void Update()
    {
        if (Time.time - 1.0F > startTime)
        {
            Destroy(transform.GetComponent<BoxCollider>());
            Destroy(transform.GetComponent<Rigidbody>());
            Destroy(this);
        }
    }
}
