using UnityEngine;

public class CarFrontCollider : MonoBehaviour
{
    CarController carController;
    public void SetUp(CarController i)
    {
        carController = i;
    }
}
