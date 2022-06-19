using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadCityBut : MonoBehaviour
{
    int iD = -1;
    
    public void SetID(int i) {
        iD = i;
    }
    public void SetColor(Color i)
    {
        transform.GetComponent<Image>().color = i;
    }
    public void SetText(string i)
    {
        transform.GetChild(0).GetComponent<Text>().text = i;
    }
    public void ButtonPressed()
    {
        //Debug.Log("button pressed id: " + iD);
        // set persistant save ibjc info
        GameObject.Find("Persist").GetComponent<Persist>().forceLoad = iD;
        GameObject.Find("CubeCity").GetComponent<CubeCity>().ReloadCity();
    }
}
