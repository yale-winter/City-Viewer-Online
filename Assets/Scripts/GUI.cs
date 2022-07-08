using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{

    bool showingLoadMenu = false;
    public GameObject loadCityMenu;

    public GameObject mainCanvas;
    public Transform layoutCityButs;
    public GameObject loadCityBut;
    public List<GameObject> loadCityButs = new List<GameObject>();
    CubeCity cubeCity;
    Scores scores;
    bool showingCreateMenu = false;
    public GameObject createCityExpandBut;
    public GameObject createCityMenu;
    public GameObject pauseDarken;


    public GameObject colorPickerP;
    public FlexibleColorPicker fcp;
    public Image customColorSwatch;
    float nudgeAmount = 0.1F;
    /// <summary>
    /// slider 0 = size, 1 = helis, 2 = scrapers
    /// </summary>
    public List<Slider> sliders = new List<Slider>();



    public InputField inputFieldName;
    public List<string> namesPre = new List<string>();
    public List<string> namesMid = new List<string>();
    public List<string> namesSuf = new List<string>();

    Scoreboard scoreboard;

    public Text curCityNameExtra;
    public Text aboveCityText;

    public Text[] advText = new Text[3];

    public void DBLoaded(Scores data)
    {
        mainCanvas.SetActive(true);
        GameObject scoresGO = new GameObject();
        scores = scoresGO.AddComponent<Scores>();
        scores = data;
        CreateLoadButtons();
        cubeCity.CreateCubeCity(data);
    }

    void Awake()
    {
        loadCityMenu.SetActive(false);
        showingLoadMenu = false;
        mainCanvas.SetActive(false);
        curCityNameExtra.text = "";
        aboveCityText.text = "";
        scoreboard = transform.GetComponent<Scoreboard>();
        cubeCity = transform.GetComponent<CubeCity>();
        colorPickerP.SetActive(false);
        createCityMenu.SetActive(false);
        pauseDarken.SetActive(false);
        Time.timeScale = 1.0F;
    }
    void CreateLoadButtons()
    {
        for (int i = 0; i < loadCityButs.Count; i++)
        {
            if (loadCityButs[i] != null)
            {
                Destroy(loadCityButs[i]);
            }
        }
        loadCityButs.Clear();
        for (int i = 0; i < 8; i++)
        {
            GameObject newBut = Instantiate(loadCityBut);
            newBut.transform.name = "load city button";
            newBut.transform.SetParent(layoutCityButs);
            newBut.transform.localScale = Vector3.one;
            string iStr = scores.names[i];
            LoadCityBut script = newBut.GetComponent<LoadCityBut>();
            script.SetText(iStr);
            script.SetID(i);
            if (i == cubeCity.loadIndex)
                script.SetColor(Color.green);
        }
        curCityNameExtra.text = scores.names[cubeCity.loadIndex];
        aboveCityText.text = "Now Viewing:";
    }

    public void ToggleCreateCityMenu()
    {
        showingCreateMenu = !showingCreateMenu;
        if (showingCreateMenu)
        {
            RandomName();
            createCityMenu.SetActive(true);
            Time.timeScale = 0.0f;
            pauseDarken.SetActive(true);
            Color32 randColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            customColorSwatch.color = randColor;
            fcp.color = randColor;
            for (int i = 0; i < sliders.Count; i++)
            {
                float roll = Random.Range(0.0f, 1.0f);
                sliders[i].value = roll;
            }

        }
        else
        {
            createCityMenu.SetActive(false);
            colorPickerP.SetActive(false);
            pauseDarken.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
    public void NudgeSlider(int thisSlider)
    {
        bool positive = true;
        if (thisSlider < 0)
        {
            positive = false;
            thisSlider = Mathf.Abs(thisSlider);
        }
        // adjust for index number
        thisSlider--;
        
        if (thisSlider == 0)
        {
            if (positive)
            {
                sliders[thisSlider].value += 1.0f/5.0f;
            }
            else
            {
                sliders[thisSlider].value -= 1.0f/5.0f;
            }
        }
        else
        {
            
            if (positive)
            {
                sliders[thisSlider].value += nudgeAmount;
            }
            else
            {
                sliders[thisSlider].value -= nudgeAmount;
            }
        }
        sliders[thisSlider].value = Mathf.Clamp(sliders[thisSlider].value, 0.0f, 1.0f);
    }
    public void SelectColorPressed()
    {
        customColorSwatch.color = fcp.color;
        colorPickerP.SetActive(false);
    }
    public void ColorSwatchPressed()
    {
        colorPickerP.SetActive(true);
    }

    public void RandomName()
    {
        int roll = Random.Range(0, namesPre.Count - 1);
        int roll2 = Random.Range(0, namesMid.Count - 1);
        int roll3 = Random.Range(0, namesSuf.Count - 1);
        string newStr = namesPre[roll] + namesMid[roll2] + namesSuf[roll3];
        Debug.Log("newStr " + newStr);
        inputFieldName.text = newStr;
    }

    public void SaveAndCreateNewCity()
    {
        // apply color if not pressed apply
        if (colorPickerP.activeSelf)
        {
            SelectColorPressed();
        }
        Color32 intColor = customColorSwatch.color;
        string padR = PadZeros((intColor.r).ToString(), 3);
        string padG = PadZeros((intColor.g).ToString(), 3);
        string padB = PadZeros((intColor.b).ToString(), 3);
        string colorStr = padR + padG + padB;
        Debug.Log("colorStr " + colorStr);
        int colorInt = int.Parse(colorStr);
        Debug.Log("colorInt " + colorInt);
        scoreboard.SubmitCity(inputFieldName.text, (int)(sliders[0].value * 100.0f), (int)(sliders[1].value * 100.0f), (int)(sliders[2].value * 100.0f), colorInt);
    }
    static string PadZeros(string str, int setChars)
    {
        while (str.Length < setChars)
        {
            str = "0" + str;
        }
        return str;
    }
    void Update()
    {
        if (showingCreateMenu)
        {
            if (Input.GetMouseButtonUp(0))
            {
               sliders[0].value = Mathf.RoundToInt(sliders[0].value * 5.0f) / 5.0f;

            }
            advText[0].text = XHelpers.sizeFromLoadSettings(Mathf.RoundToInt(sliders[0].value * 100.0f)).ToString() + " blocks";
            advText[1].text = XHelpers.maxHeliFromLoadSettings(Mathf.RoundToInt(sliders[1].value * 100.0f)).ToString();
            advText[2].text = XHelpers.scrapFromLoadSettings(Mathf.RoundToInt(sliders[2].value * 100.0f)).ToString() + "% rate";
        }
    }
    public void OpenLoadCityMenuPressed()
    {
        showingLoadMenu = !showingLoadMenu;
        if (showingLoadMenu)
        {
            loadCityMenu.SetActive(true);
        }
        else
        {
            loadCityMenu.SetActive(false);
        }
    }

}
