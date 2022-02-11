using System;
using System.Collections;
using UnityEngine;
[Serializable]
public class StreetLightController : MonoBehaviour
{
    public IntersectionCollider iC;
    public SignStopLightView sSLV;
    public StreetLightModel sLM;
    public Allow allowPassage = Allow.X;
    private Vector2 delayMinMax = new Vector2(4.0F,10.0F);
    private float yellowLightDelay = 2.0F;
    private Color[] sLColors = new Color[4];
    public Material bulbMat;
    private CubeCity cubeCity;
    private float timeOfLastSwitch = float.PositiveInfinity;
    private float waitingDur = 0.0F;



    public void SetUp(IntersectionCollider setIC, SignStopLightView setSSLV, StreetLightModel setSLM, Color[] setSLColors, Material setBulbMat, CubeCity setCubeCity)
    {
        iC = setIC;
        iC.myController = this;
        sSLV = setSSLV;
        sLM = setSLM;
        sLColors = setSLColors;
        bulbMat = setBulbMat;
        cubeCity = setCubeCity;

        allowPassage = Allow.X;
        float roll = UnityEngine.Random.Range(0, 2);
        if (roll == 0)
        {
            allowPassage = Allow.Z;
        }
        for (int i = 0; i < 3; i++)
        {
            SetLightBulbColor(Allow.X, i, 0);
            SetLightBulbColor(Allow.Z, i, 0);
        }
        if (allowPassage == Allow.X)
        {
            SetLightBulbColor(Allow.X, 2, 3);
            SetLightBulbColor(Allow.Z, 0, 1);
        }
        else
        {
            SetLightBulbColor(Allow.Z, 2, 3);
            SetLightBulbColor(Allow.X, 0, 1);
        }
        float setWait = UnityEngine.Random.Range(delayMinMax.x, delayMinMax.y);
        waitingDur = setWait;
        timeOfLastSwitch = Time.time;
        StartCoroutine(WaitToSwitchLightsAgain(setWait));
    }
    public enum Allow { X, Z };
    public IEnumerator SwitchLights()
    {
        Allow setA = Allow.X;
        if (allowPassage == Allow.X)
        {
            setA = Allow.Z;
        }
        if (setA == Allow.Z)
        {
            SetLightBulbColor(Allow.X, 2, 0);
            SetLightBulbColor(Allow.X, 1, 2);
            yield return new WaitForSeconds(yellowLightDelay);
            SetLightBulbColor(Allow.X, 1, 0);
            SetLightBulbColor(Allow.X, 0, 1);
            SetLightBulbColor(Allow.Z, 0, 0);
            SetLightBulbColor(Allow.Z, 2, 3);
        }
        else
        {
            SetLightBulbColor(Allow.Z, 2, 0);
            SetLightBulbColor(Allow.Z, 1, 2);
            yield return new WaitForSeconds(yellowLightDelay);
            SetLightBulbColor(Allow.Z, 1, 0);
            SetLightBulbColor(Allow.Z, 0, 1);
            SetLightBulbColor(Allow.X, 0, 0);
            SetLightBulbColor(Allow.X, 2, 3);
        }
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
        if (allowAxis == Allow.X)
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
        string travelOK = "x";
        if (allowPassage == Allow.Z)
        {
            travelOK = "z";
        }
        int carID = int.Parse(carName.Substring(4));
        float possibleWaitDur = waitingDur - (Time.time - timeOfLastSwitch) + yellowLightDelay + UnityEngine.Random.Range(-0.25F, 0.25F);
        cubeCity.carControllers[carID].ApproachIntersection(sLM.iD, travelOK, possibleWaitDur);
    }
}
