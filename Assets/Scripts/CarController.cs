using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CarController : MonoBehaviour
{
    public int iD;
    public GameObject car;
    public CarModel carModel;
    public PathCreator carPC;
    private float noStoppingBeforeTime = float.PositiveInfinity;
    private float immuneToStopDur = 1.5F;


    public void SetUp(int setID, GameObject setCar, CarModel setModel, PathCreator setPC) {
        iD = setID;
        car = setCar;
        carModel = setModel;
        carPC = setPC;
        noStoppingBeforeTime = Time.time + immuneToStopDur;
    }
    public void ApproachIntersection(int intersectionID, string travelOK, float possibleWait)
    {
        if (Time.time > noStoppingBeforeTime)
        {
            float xDiff = Mathf.Abs(car.transform.position.x - car.transform.GetChild(0).transform.position.x);
            float zDiff = Mathf.Abs(car.transform.position.z - car.transform.GetChild(0).transform.position.z);
            string carDir = "x";
            if (zDiff < xDiff)
            {
                carDir = "z";
            }
            if (carDir != travelOK)
            {
                car.GetComponent<PathFollower>().speed = 0.0F;
                StartCoroutine(SlowDown(possibleWait));
            }
            Debug.Log("car approaching intersection: " + carDir + " travelOK " + travelOK);
        }
    }
    private IEnumerator SlowDown(float waitAfter)
    {
        yield return new WaitForSeconds(waitAfter);
        car.GetComponent<PathFollower>().speed = carModel.maxSpeed;
    }
}
