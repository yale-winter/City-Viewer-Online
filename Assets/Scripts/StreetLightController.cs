using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StreetLightController : MonoBehaviour
{
    public IntersectionCollider iC;
    public SignStopLightView sSLV;
    public StreetLightModel sLM;
    public Allow allowPassage = Allow.None;
    public Allow[] posPassAllowed = new Allow[2];
    [Tooltip("delay to switch the lights")]
    private Vector2 delayMinMax = new Vector2(4.0F,10.0F);
    private float yellowLightDelay = 2.0F;
    private Color[] sLColors = new Color[4];
    public Material bulbMat;
    private CubeCity cubeCity;
    private float timeOfLastSwitch = float.PositiveInfinity;
    private float waitingDur = 0.0F;
    private int carsWaitingHere = 0;
    private int flipAxis = 0;
    string[] posPassAllowedS = new string[2];
    public List<string> blocked = new List<string>();
    public bool freewayEntrance = false;

    public void SetUp(IntersectionCollider setIC, SignStopLightView setSSLV, StreetLightModel setSLM, Color[] setSLColors, Material setBulbMat, CubeCity setCubeCity, bool[] flip, List<string> setBlocked)
    {
        for (int i = 0; i < setBlocked.Count; i++)
        {
            blocked.Add(setBlocked[i]);
        }
        iC = setIC;
        iC.myController = this;
        sSLV = setSSLV;
        sLM = setSLM;
        sLColors = setSLColors;
        bulbMat = setBulbMat;
        cubeCity = setCubeCity;


        if (sLM.iD == 17)
        {
            freewayEntrance = true;
        }
        

        posPassAllowed[0] = Allow.East;
        posPassAllowed[1] = Allow.North;

        if (flip[0])
        {
          //  Debug.Log("flip 0");
            posPassAllowed[0] = Allow.West;
            setSSLV.xLights[0].transform.parent.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        }
        
        if (flip[1])
        {
            //Debug.Log("flip 1");
            posPassAllowed[1] = Allow.South;
            setSSLV.zLights[0].transform.parent.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        // removed blocked from options
        if (isBlocked(posPassAllowed[0].ToString()))
        {
            posPassAllowed[0] = posPassAllowed[1];
        }
        else if (isBlocked(posPassAllowed[1].ToString()))
        {
            posPassAllowed[1] = posPassAllowed[0];

        }

        posPassAllowedS = new string[2];
        posPassAllowedS[0] = posPassAllowed[0].ToString();
        posPassAllowedS[1] = posPassAllowed[1].ToString();



        /*
        if (flipAxis)
        {
            setSSLV.pivotLight.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        }
        */


        allowPassage = Allow.North;
        float roll = UnityEngine.Random.Range(0, 2);
        /*
        if (roll == 0)
        {
            allowPassage = Allow.NorthWest;
        }
        for (int i = 0; i < 3; i++)
        {
            SetLightBulbColor(Allow.SouthEast, i, 0);
            SetLightBulbColor(Allow.NorthWest, i, 0);
        }
        if (allowPassage == Allow.NorthWest)
        {
            SetLightBulbColor(Allow.SouthEast, 2, 3);
            SetLightBulbColor(Allow.NorthWest, 0, 1);
        }
        else
        {
            SetLightBulbColor(Allow.NorthWest, 2, 3);
            SetLightBulbColor(Allow.SouthEast, 0, 1);
        }
        */




        float setWait = UnityEngine.Random.Range(delayMinMax.x, delayMinMax.y);
        waitingDur = setWait;
        timeOfLastSwitch = Time.time;
        StartCoroutine(WaitToSwitchLightsAgain(setWait));
    }
    bool isBlocked(string desc)
    {
        for (int i = 0; i < blocked.Count; i++) {
            if (desc == blocked[i])
            {
                return true;
            }
        }
        return false;
    }
    public enum Allow { None, North, East, South, West };
    public IEnumerator SwitchLights()
    {
        yield return 0;
        Allow setA = Allow.South;
        /*
        
        if (allowPassage == Allow.SouthEast)
        {
            setA = Allow.NorthWest;
        }
        if (setA == Allow.SouthEast)
        {
            SetLightBulbColor(Allow.SouthEast, 2, 0);
            SetLightBulbColor(Allow.SouthEast, 1, 2);
            yield return new WaitForSeconds(yellowLightDelay);
            SetLightBulbColor(Allow.SouthEast, 1, 0);
            SetLightBulbColor(Allow.SouthEast, 0, 1);
            SetLightBulbColor(Allow.NorthWest, 0, 0);
            SetLightBulbColor(Allow.NorthWest, 2, 3);
        }
        else
        {
            SetLightBulbColor(Allow.NorthWest, 2, 0);
            SetLightBulbColor(Allow.NorthWest, 1, 2);
            yield return new WaitForSeconds(yellowLightDelay);
            SetLightBulbColor(Allow.NorthWest, 1, 0);
            SetLightBulbColor(Allow.NorthWest, 0, 1);
            SetLightBulbColor(Allow.SouthEast, 0, 0);
            SetLightBulbColor(Allow.SouthEast, 2, 3);
        }
        */

        carsWaitingHere = 0;
        allowPassage = setA;
        float setWait = UnityEngine.Random.Range(delayMinMax.x, delayMinMax.y);
        waitingDur = setWait;
        timeOfLastSwitch = Time.time;
        StartCoroutine(WaitToSwitchLightsAgain(setWait));
    }
    private IEnumerator WaitToSwitchLightsAgain(float thisWait)
    {
        yield return new WaitForSeconds(thisWait);
        StartCoroutine("SwitchLights");
    }
    private void SetLightBulbColor(Allow allowAxis, int setBulb, int setColor)
    {
        Material instanceMat = new Material(bulbMat);
        instanceMat.color = sLColors[setColor];
        if (allowAxis == Allow.West || allowAxis == Allow.East)
        {
            sSLV.xLights[setBulb].transform.GetComponent<MeshRenderer>().material = instanceMat;
        }
        else
        {
            sSLV.zLights[setBulb].transform.GetComponent<MeshRenderer>().material = instanceMat;
        }
    }
    public void CarApproaching(string carName)
    {
        if (carName.Substring(0, 3) == "car")
        {
            string travelOK = allowPassage.ToString();
            int carID = int.Parse(carName.Substring(4));
            float possibleWaitDur = waitingDur - (Time.time - timeOfLastSwitch) + yellowLightDelay + carsWaitingHere*1.2f;

            cubeCity.carControllers[carID].ApproachIntersection(sLM.iD, travelOK, possibleWaitDur, posPassAllowedS, new Vector3(transform.position.x,0.05f,transform.position.z), freewayEntrance);
        }
        carsWaitingHere++;
    }
}
