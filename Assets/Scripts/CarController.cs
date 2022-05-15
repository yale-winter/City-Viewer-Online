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
    private float immuneToStopDur = 0.0f;//1.5f;
    private bool crossingIntersection = false;
    private bool waitingIntersection = false;


    public void SetUp(int setID, GameObject setCar, CarModel setModel, PathCreator setPC)
    {
        iD = setID;
        car = setCar;
        carModel = setModel;
        carPC = setPC;
        noStoppingBeforeTime = Time.time + immuneToStopDur;

        CarFrontCollider carFrontCol = car.transform.Find("InFrontDet").GetComponent<CarFrontCollider>();
        carFrontCol.SetUp(this);

    }
    public void ApproachIntersection(int intersectionID, string travelOK, float possibleWait)
    {

        if (Time.time > noStoppingBeforeTime && !crossingIntersection && !waitingIntersection)
        {

            float xDiff = car.transform.position.x - car.transform.GetChild(0).transform.position.x;
            float zDiff = car.transform.position.z - car.transform.GetChild(0).transform.position.z;
            string carDir = "";

            if (Math.Abs(zDiff) < Math.Abs(xDiff))
            {
                if (xDiff < 0)
                {
                    carDir = "east";
                }
                else
                {
                    carDir = "west";
                }
            }
            else
            {
                if (zDiff < 0)
                {
                    carDir = "north";
                }
                else
                {
                    carDir = "south";
                }

            }

           // if (intersectionID == 1)
                //Debug.Log("approach intersection (" + intersectionID + ") car (" + iD + ") carDir " + carDir + " travelOK " + travelOK);

            bool mustWait = true;
            if (travelOK == "NorthWest")
            {
                if (carDir == "north" || carDir == "west")
                {
                    mustWait = false;
                }
            }
            else
            {
                if (carDir == "south" || carDir == "east")
                {
                    mustWait = false;
                }
            }

            if (mustWait)
            {
                waitingIntersection = true;
                ChangeSpeed(0.0f);
                StartCoroutine(SlowDown(possibleWait));
            }
            else
            {
                StartCoroutine("CrossIntersection");
            }
            // Debug.Log("car approaching intersection: " + carDir + " travelOK " + travelOK);
        }
    }
    private IEnumerator CrossIntersection()
    {
        waitingIntersection = false;
        crossingIntersection = true;
        //car.transform.Find("CarBody1").gameObject.SetActive(false);
        ChangeSpeed(2.0f);
        yield return new WaitForSeconds(2.8f);
        ChangeSpeed(carModel.speed);

        //car.transform.Find("CarBody1").gameObject.SetActive(true);

        crossingIntersection = false;
    }
    private IEnumerator SlowDown(float waitAfter)
    {
        yield return new WaitForSeconds(waitAfter);
        StartCoroutine("CrossIntersection");
    }
    public void BlockedInFront()
    {
        if (!crossingIntersection && !waitingIntersection)
        {
            ChangeSpeed(0.0f);
            StopCoroutine("CheckUnblocked");
            StartCoroutine("CheckUnblocked");
        }
    }
    private IEnumerator CheckUnblocked()
    {
        yield return new WaitForSeconds(0.2f);
        ChangeSpeed(carModel.speed);
    }
    private IEnumerator changeSpeedCR;
    private bool changeSpeedCRRunning = false;

    private void ChangeSpeed(float targetSpeed)
    {
        //Debug.Log("changing car(" + carModel.iD + ") speed to: " + targetSpeed + " max speed is: " + carModel.speed);
        if (changeSpeedCRRunning)
        {
            StopCoroutine(changeSpeedCR);
        }
        changeSpeedCR = ChangeSpeedIE(targetSpeed);
        StartCoroutine(changeSpeedCR);
    }
    private float speedThresh = 0.5f;
    private float speedInc = 0.22f;
    private IEnumerator ChangeSpeedIE(float targetSpeed)
    {
        changeSpeedCRRunning = true;
        bool updated = true;
        while (updated)
        {
            updated = false;
            if (car.GetComponent<PathFollower>().speed < targetSpeed - speedThresh)
            {
                car.GetComponent<PathFollower>().speed += speedInc;
                updated = true;
            }
            else if (car.GetComponent<PathFollower>().speed > targetSpeed + speedThresh)
            {
                car.GetComponent<PathFollower>().speed -= speedInc * 2.0f;
                updated = true;
            }
            car.GetComponent<PathFollower>().speed = Mathf.Clamp(car.GetComponent<PathFollower>().speed, 0.0f, carModel.speed);
            yield return 0;
        }
        car.GetComponent<PathFollower>().speed = targetSpeed;
        changeSpeedCRRunning = false;
    }
}
