using UnityEngine;

public class BuildingPlacementDet : MonoBehaviour
{
    float startTime = float.PositiveInfinity;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Substring(0,5) == "Super")
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        if (Time.time - 1.0F > startTime)
        {
            Destroy(transform.GetComponent<BoxCollider>());
            Destroy(transform.GetComponent<Rigidbody>());
            Destroy(this);
        }
    }
}
