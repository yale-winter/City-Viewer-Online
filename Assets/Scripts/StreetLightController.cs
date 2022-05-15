using System;
using System.Collections;
using UnityEngine;
[Serializable]
public class StreetLightController : MonoBehaviour
{
    public IntersectionCollider iC;
    public SignStopLightView sSLV;
    public StreetLightModel sLM;
    public Allow allowPassage = Allow.SouthEast;
    [Tooltip("delay to switch the lights")]
    private Vector2 delayMinMax = new Vector2(4.0F,10.0F);
    private float yellowLightDelay = 2.0F;
    private Color[] sLColors = new Color[4];
    public Material bulbMat;
    private CubeCity cubeCity;
    private float timeOfLastSwitch = float.PositiveInfinity;
    private float waitingDur = 0.0F;
    private int carsWaitingHere = 0;
    private bool flipAxis = false;

    public void SetUp(IntersectionCollider setIC, SignStopLightView setSSLV, StreetLightModel setSLM, Color[] setSLColors, Material setBulbMat, CubeCity setCubeCity, bool setFlipAxis)
    {
        iC = setIC;
        iC.myController = this;
        sSLV = setSSLV;
        sLM = setSLM;
        sLColors = setSLColors;
        bulbMat = setBulbMat;
        cubeCity = setCubeCity;
        flipAxis = setFlipAxis;
        if (flipAxis)
        {
            setSSLV.pivotLight.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        }


        allowPassage = Allow.SouthEast;
        float roll = UnityEngine.Random.Range(0, 2);
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
        float setWait = UnityEngine.Random.Range(delayMinMax.x, delayMinMax.y);
        waitingDur = setWait;
        timeOfLastSwitch = Time.time;
        StartCoroutine(WaitToSwitchLightsAgain(setWait));
    }
    public enum Allow { SouthEast, NorthWest };
    public IEnumerator SwitchLights()
    {
        Allow setA = Allow.SouthEast;
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
        if (allowAxis == Allow.SouthEast)
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
            string travelOK = "SouthEast";
            if (allowPassage == Allow.NorthWest)
            {
                travelOK = "NorthWest";
            }
            int carID = int.Parse(carName.Substring(4));
            float possibleWaitDur = waitingDur - (Time.time - timeOfLastSwitch) + yellowLightDelay + carsWaitingHere*1.2f;
            cubeCity.carControllers[carID].ApproachIntersection(sLM.iD, travelOK, possibleWaitDur);
        }
        carsWaitingHere++;
    }
}
