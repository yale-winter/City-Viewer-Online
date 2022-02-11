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
    public List<EaserEase> eases = new List<EaserEase>();

    public void SetUp(int setID, GameObject setCar, CarModel setModel, PathCreator setPC, List<EaserEase> setEases) {
        iD = setID;
        car = setCar;
        carModel = setModel;
        carPC = setPC;
        eases = setEases;
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
                StartCoroutine(SlowDown(possibleWait));
            }
            Debug.Log("car approaching intersection: " + carDir + " travelOK " + travelOK);
        }
    }
    private IEnumerator SlowDown(float waitAfter)
    {
        float _t = 0.0F;
        float initSpeed = carModel.maxSpeed;
        float targetSpeed = 0.0F;
        float useSpeed = initSpeed;

        while (_t < 1.0F)
        {
            _t += Time.deltaTime * 1.2F;
            useSpeed = Easer.Ease(eases[0], initSpeed, targetSpeed, _t);
            if (_t > 1.0F)
            {
                _t = 1.0F;
                useSpeed = targetSpeed;
            }
            car.GetComponent<PathFollower>().speed = useSpeed;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(waitAfter);
        StartCoroutine(SpeedUp());
    }
    private IEnumerator SpeedUp()
    {
        float _t = 0.0F;
        float initSpeed = 0.0F;
        float targetSpeed = carModel.maxSpeed;
        float useSpeed = 0.0F;

        while (_t < 1.0F)
        {
            _t += Time.deltaTime;
            useSpeed = Easer.Ease(eases[1], initSpeed, targetSpeed, _t);
            if (_t > 1.0F)
            {
                _t = 1.0F;
                useSpeed = targetSpeed;
            }
            car.GetComponent<PathFollower>().speed = useSpeed;
            yield return new WaitForEndOfFrame();
        }
        car.GetComponent<PathFollower>().speed = carModel.maxSpeed;
    }
}
