using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
    public StreetLightController myController;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CarFrontDet"))
        {
            myController.CarApproaching(other.transform.parent.name);
        }
        
    }
}
