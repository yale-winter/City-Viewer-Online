using UnityEngine;

public class HeliPathCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("destroing heli path too close to sky scraper");
        Destroy(transform.parent.parent.gameObject);
    }
}
