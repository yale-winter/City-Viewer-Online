using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFrontCollider : MonoBehaviour
{
    private CarController carController;
    private float lastTriggerTime = 0.0f;
    public void SetUp(CarController i)
    {
        carController = i;
    }
}
