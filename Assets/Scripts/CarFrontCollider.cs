using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFrontCollider : MonoBehaviour
{
    private CarController carController;
    private float lastTriggerTime = 0.0f;

    private void OnTriggerStay(Collider other)
    {
        int thisLayer = LayerMask.NameToLayer("CarMain2");
        if (other.gameObject.layer == thisLayer)
        {
            if (Time.time - 0.1f > lastTriggerTime)
            {
                //Debug.Log("blocked in front by layer" + other.gameObject.layer);
                //Debug.Log("looking for layer" + LayerMask.NameToLayer("CarMainDet"));
                lastTriggerTime = Time.time;
                //Debug.Log("car front detection collider triggered");
                //myController.CarApproaching(other.transform.parent.name);
                carController.BlockedInFront();
            }
        }
    }
    public void SetUp(CarController i)
    {
        carController = i;
    }
}
