using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
    public StreetLightController myController;
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("0 street light trigger enter " + other.transform.parent.name);
        myController.CarApproaching(other.transform.parent.name);
    }
}
