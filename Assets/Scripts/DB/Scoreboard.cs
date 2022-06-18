using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Scoreboard : MonoBehaviour
{
    ScoreboardWebInterface scoreboardWebInterface;
    private Scores scores;
    string highScoreLocation;
    string saveStatePath;
    bool scoresSubmitted = false;
    GUI gUI;

    private void Awake()
    {
        gUI = transform.GetComponent<GUI>();
        GameObject theScores = new GameObject();
        scoreboardWebInterface = theScores.AddComponent<ScoreboardWebInterface>();
        highScoreLocation = scoreboardWebInterface.highscoreURL;
    }
    private IEnumerator LoadScores()
    {
        // temp
        yield return new WaitForSeconds(1.0F);


        // This initializes the default scores from Scores.cs
        GameObject newScores = new GameObject();
        scores = newScores.AddComponent<Scores>();

        // Fetch the global high score list from database
        int returnCode = -1;
        yield return scoreboardWebInterface.GetScores(status => returnCode = status);

        if (returnCode == 0)
        {
            highScoreLocation = scoreboardWebInterface.highscoreURL;
            int readMany = Math.Min(scoreboardWebInterface.Scores.names.Length, 8);
            for (int i = 0; i < readMany; i++)
            {
                scores.names[i] = scoreboardWebInterface.Scores.names[i];
                scores.scores[i] = scoreboardWebInterface.Scores.scores[i];
                scores.citySize[i] = scoreboardWebInterface.Scores.citySize[i];
                scores.helicopters[i] = scoreboardWebInterface.Scores.helicopters[i];
                scores.scrapers[i] = scoreboardWebInterface.Scores.scrapers[i];
                scores.cityColor[i] = scoreboardWebInterface.Scores.cityColor[i];
            }
        }
        else if (returnCode == 1)
        {
            // An error occurred attempting to get the high scores, so fall back to the local high score list
            highScoreLocation = scoreboardWebInterface.highscoreURL;
            saveStatePath = Path.Combine(Application.persistentDataPath, "scoreboard.sgm");

            // Read the high scores from the local JSON file, if it exists
            if (File.Exists(saveStatePath))
            {
                String fileContents = File.ReadAllText(saveStatePath);
                JsonUtility.FromJsonOverwrite(fileContents, scores);
            }
        }
        /*
        for (int i = 0; i < scores.scores.Length; i++)
        {
            Debug.Log("scores.scores[i].ToString() " + scores.scores[i]);
            //scoreTexts[i].text = scores.scores[i].ToString();
        }
        for (int i = 0; i < scores.names.Length; i++)
        {
           Debug.Log("scores.names[i].ToString() " + scores.names[i]);
            //nameTexts[i].text = scores.names[i].ToString();
        }*/
        gUI.DBLoaded(scores);
    }
    private bool scoresLoaded = false;
    public void Start()
    {

        
        if (!scoresLoaded)
        {
            scoresLoaded = true;
            StartCoroutine("LoadScores");

        }
    }
    public void SubmitCity(string name, int citySize, int helicopters, int scrapers, int cityColor)
    {
        if (!scoresSubmitted)
        {
            StartCoroutine(scoreboardWebInterface.PostScores(name, 0, citySize, helicopters, scrapers, cityColor));
            scoresSubmitted = true;
        }
    }
}
