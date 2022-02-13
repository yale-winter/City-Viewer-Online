using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
    public StreetLightController myController;
    private void OnTriggerEnter(Collider other)
    {
        myController.CarApproaching(other.transform.parent.name);
    }
}
