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

    public bool crossingIntersection = false;
    public bool waitingIntersection = false;
    public bool lerpingPos = false;
    public bool pathControlled = true;
    public float lSpeed = 1.0f;
    public string[] posPassS = new string[2];
    bool leavingTown = false;



    public GameObject inFrontDet;


    /// <summary>
    /// set between 0 - 1
    /// </summary>
    public float[] moveDir = new float[]{
        1.0f,0.0f
    };



    Quaternion normalCarQ = Quaternion.identity;
    float spawnTime = 0.0f;
    public void SetUp(int setID, GameObject setCar, CarModel setModel, string instantDirection)
    {

        spawnTime = Time.time;

        normalCarQ.eulerAngles = new Vector3(0.0f, 90.0f, -90.0f);

        iD = setID;
        car = setCar;
        carModel = setModel;

        CarFrontCollider carFrontCol = car.transform.Find("InFrontDet").GetComponent<CarFrontCollider>();
        carFrontCol.SetUp(this);
        inFrontDet = carFrontCol.gameObject;

        lSpeed = carModel.speed;

        InstantDirection(instantDirection);

    }
    public void ApproachIntersection(int intersectionID, string travelOK, float possibleWait, string[] setPosPassS, Vector3 interPos)
    {
        if (spawnTime + 1.0f < Time.time)
        {
            if (!crossingIntersection && !waitingIntersection)
            {

                posPassS = setPosPassS;
                float xDiff = car.transform.position.x - car.transform.GetChild(0).transform.position.x;
                float zDiff = car.transform.position.z - car.transform.GetChild(0).transform.position.z;
                string carDir = "";

                if (Math.Abs(zDiff) < Math.Abs(xDiff))
                {
                    if (xDiff < 0)
                    {
                        carDir = "East";
                    }
                    else
                    {
                        carDir = "West";
                    }
                }
                else
                {
                    if (zDiff < 0)
                    {
                        carDir = "North";
                    }
                    else
                    {
                        carDir = "South";
                    }

                }
                if (iD == 0)
                {
                    Debug.Log("approach intersection (" + intersectionID + ") car (" + iD + ") carDir " + carDir + " travelOK " + travelOK + " intersec id " + intersectionID + " inter pos " + interPos + " posPassS[0] " + posPassS[0] + " posPassS[1] " + posPassS[1]);
                }
                bool mustWait = true;
                if (travelOK == carDir)
                {

                    mustWait = false;

                }

                if (!mustWait)
                {
                    possibleWait = 0.0f;
                }



                //possibleWait = 0.0f;
                StartCoroutine(CrossIntersection(possibleWait, interPos));
                //}
                // Debug.Log("car approaching intersection: " + carDir + " travelOK " + travelOK);
            }
        }
    }
    public static string FlipDir(string thisDir)
    {
        if (thisDir == "North")
        {
            thisDir = "South";
        }
        else if (thisDir == "South")
        {
            thisDir = "North";
        }
        else if (thisDir == "East")
        {
            thisDir = "West";
        }
        else if (thisDir == "West")
        {
            thisDir = "East";
        }
        return thisDir;
    }
    private IEnumerator CrossIntersection(float waitTime, Vector3 interPos)
    {

        inFrontDet.SetActive(false);
        ChangeSpeed(0.0f);
        yield return new WaitForSeconds(waitTime * 0.5f);
        waitingIntersection = true;
        yield return new WaitForSeconds(waitTime * 0.5f);
        waitingIntersection = false;
        crossingIntersection = true;
        ChangeSpeed(carModel.speed);

        int rollForDir = UnityEngine.Random.Range(0, 2);
        // decide direction (opposite of passage dir)
        //Debug.Log("car(" + iD + ") posPassS[rollForDir] " + posPassS[rollForDir]);

        float[] newDir = new float[2];
        float[] setDir = getDirFromCDir(posPassS[rollForDir]);

        bool turning = false;
        bool turnLeft = true;
        if (moveDir[0] != setDir[0] || moveDir[1] != setDir[1])
        {
            turning = true;

            if (moveDir[0] == 1 && setDir[1] == -1)
            {
                turnLeft = false;
            }
            else if (moveDir[0] == -1 && setDir[1] == 1)
            {
                turnLeft = false;
            }
            else if (moveDir[1] == 1 && setDir[0] == 1)
            {
                turnLeft = false;
            }
            else if (moveDir[1] == -1 && setDir[0] == -1)
            {
                turnLeft = false;
            }
            
        }
        if (turning)
        {

            LerpMoveFromTo(car.transform.position, XHelpers.midIntersectionPos(setDir, interPos));
          
            
            ChangeSpeed(carModel.speed/ 4.0f);
            yield return new WaitForSeconds(4.25f / carModel.speed);
            if (turnLeft)
            {
                LerpRotFromTo(new Vector3(90.0f, 0.0f, 0.0f));
            }
            else
            {
                LerpRotFromTo(new Vector3(-90.0f, 0.0f, 0.0f));
            }

            while (lerpingPos)
            {
                yield return 0;
            }

            //yield return new WaitForSeconds(3.5f / carModel.speed);



            moveDir[0] = setDir[0];
            moveDir[1] = setDir[1];


            yield return new WaitForSeconds(3.0f / carModel.speed);

            ChangeSpeed(carModel.speed);

            LerpMoveFromTo(car.transform.position, XHelpers.exitIntersectionPos(setDir, interPos));

            while (lerpingPos)
            {
                yield return 0;
            }
        }
        else
        {
            LerpMoveFromTo(car.transform.position, XHelpers.exitIntersectionPos(setDir, interPos));
            while (lerpingPos)
            {
                yield return 0;
            }

        }
        //yield return new WaitForSeconds(1.0f / carModel.speed);

        //yield return new WaitForSeconds(2.0f / carModel.speed);
        //car.transform.Find("CarBody1").gameObject.SetActive(true);

        crossingIntersection = false;
        inFrontDet.SetActive(true);

    }

    private IEnumerator SlowDown(float waitAfter)
    {
        yield return new WaitForSeconds(waitAfter);
        StartCoroutine("CrossIntersection");
    }

    private static float[] getDirFromCDir(string cDir)
    {
        float[] dir = new float[2];
        if (cDir == "North")
        {
            dir[0] = 0.0f;
            dir[1] = 1.0f;
        }
        else if (cDir == "South")
        {
            dir[0] = 0.0f;
            dir[1] = -1.0f;
        }
        else if (cDir == "East")
        {
            dir[0] = 1.0f;
            dir[1] = 0.0f;
        }
        else if (cDir == "West")
        {
            dir[0] = -1.0f;
            dir[1] = 0.0f;
        }

        return dir;
    }
    bool runCUB = false;
    private IEnumerator CheckUnblocked()
    {
        runCUB = true;
        yield return new WaitForSeconds(0.3f);
        inFrontDet.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        ChangeSpeed(carModel.speed);
        //waitingIntersection = false;
        runCUB = false;

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
    private float speedInc = 2.0f;
    private IEnumerator ChangeSpeedIE(float targetSpeed)
    {
        // Debug.Log("change speed to: " + targetSpeed);
        changeSpeedCRRunning = true;
        bool updated = true;
        float newSpeed = lSpeed;
        while (updated)
        {
            updated = false;
            if (newSpeed < targetSpeed - speedThresh)
            {
                newSpeed += speedInc * Time.deltaTime;
                updated = true;
            }
            else if (newSpeed > targetSpeed + speedThresh)
            {
                newSpeed -= speedInc * Time.deltaTime * 10.0f;//2.0f;
                updated = true;
            }
            newSpeed = Mathf.Clamp(newSpeed, 0.0f, carModel.speed);
            lSpeed = newSpeed;
            yield return 0;
        }
        lSpeed = targetSpeed;
        changeSpeedCRRunning = false;
    }
    Vector3 exitStartPos;
    Quaternion exitStartQ;



    IEnumerator moveCR;
    void LerpMoveFromTo(Vector3 from, Vector3 to)
    {

        if (moveCR != null)
        {
            StopCoroutine(moveCR);
        }
        float dist = Vector3.Distance(from, to);
        moveCR = LerpMoveFromToCR(dist, from, to);
        StartCoroutine(moveCR);
    }
    IEnumerator rotCR;
    void LerpRotFromTo(Vector3 rotPush)
    {

        if (rotCR != null)
        {
            StopCoroutine(rotCR);
        }
        float dist = 1.0f;
        rotCR = LerpRotFromToCR(rotPush);
        StartCoroutine(rotCR);
    }
    public bool lerpingRot = false;
    private IEnumerator LerpRotFromToCR(Vector3 rotPush)
    {
        Quaternion ogQ = car.transform.localRotation;
        lerpingRot = true;
        float p = 0.0f;
        float damp = 0.1f;
        while (p < 1.0f)
        {
            float incrPush = Time.deltaTime * carModel.speed * damp * 1.8f * (lSpeed / carModel.speed);
            if (p < 0.3f)
            {
                damp += Time.deltaTime;
                if (damp > 1.0f)
                {
                    damp = 1.0f;
                }
            }
            else if (p > 0.7f)
            {
                damp -= Time.deltaTime;
                if (damp < 0.1f)
                {
                    damp = 0.1f;
                }
            }
            p += incrPush;
            if (p > 1.0f)
            {
                p = 1.0f;
            }
            car.transform.Rotate(new Vector3(rotPush.x * incrPush, rotPush.y, rotPush.z));
            yield return 0;
        }
        car.transform.localRotation = ogQ;
        car.transform.Rotate(new Vector3(rotPush.x, rotPush.y, rotPush.z));
        lerpingRot = false;
    }
    private IEnumerator LerpMoveFromToCR(float dist, Vector3 from, Vector3 to)
    {
        lerpingPos = true;
        float dur = 1.0f / (dist / carModel.speed);
        float p = 0.0f;
        while (p < 1.0f)
        {
            p += Time.deltaTime * dur * (lSpeed / carModel.speed);
            if (p > 1.0f)
            {
                p = 1.0f;
            }
            car.transform.position = Vector3.Lerp(from, to, p);
            yield return 0;
        }

        lerpingPos = false;
    }
    public void PossiblyMove()
    {
        if (!pathControlled && !lerpingPos && !waitingIntersection)
        {
            car.transform.position = new Vector3(car.transform.position.x + lSpeed * Time.deltaTime * moveDir[0], car.transform.position.y, car.transform.position.z + lSpeed * Time.deltaTime * moveDir[1]);
        }
    }


    public void InstantDirection(string dir)
    {
        car.transform.localRotation = normalCarQ;
        if (dir == "North")
        {
            car.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
            moveDir[0] = 0.0f;
            moveDir[1] = 1.0f;
        }
        else if (dir == "South")
        {
            car.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            moveDir[0] = 0.0f;
            moveDir[1] = -1.0f;
        }
        else if (dir == "East")
        {

            moveDir[0] = 1.0f;
            moveDir[1] = 0.0f;
        }
        else if (dir == "West")
        {
            car.transform.Rotate(new Vector3(180.0f, 0.0f, 0.0f));
            moveDir[0] = -1.0f;
            moveDir[1] = 0.0f;
        }
    }
    public void TrafficJam()
    {
        ChangeSpeed(0.0f);
        //lSpeed = 0.0f;
        //waitingIntersection = true;
        inFrontDet.SetActive(false);
        if (runCUB)
        {
            StopCoroutine(CheckUnblocked());
        }
        StartCoroutine(CheckUnblocked());
    }
}
