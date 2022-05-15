using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
    public StreetLightController myController;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("CarFrontDet"))
        {
            myController.CarApproaching(other.transform.parent.name);
        }
        
    }
}
