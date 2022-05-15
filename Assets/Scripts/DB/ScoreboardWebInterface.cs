using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class ScoreboardWebInterface : MonoBehaviour
{
    // psuedo-public password below: another hidden password is used on private website accessed only from the same site
    private string secretKey = "your public ish pass here"; // Edit this value and make sure it's the same as the one stored on the server
    public string addScoreURL = "online public .php with SQL to save city"; //be sure to add a ? to your url
    public string highscoreURL = "online public .php with SQL to read database";

    private int dataCols = 6;

    private Scores scores;
    public Scores Scores => scores;

    // Send the new score to the database
    public IEnumerator PostScores(string name, int score, int citySize, int helicopters, int scrapers, int cityColor)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = name + score + citySize + helicopters + scrapers + cityColor + secretKey;//Utility.Md5Sum(name + score + secretKey);

        string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&citySize=" + citySize + "&helicopters=" + helicopters + "&scrapers=" + scrapers + "&cityColor=" + cityColor + "&hash=" + hash;

        // Post the URL to the site and create a download object to get the result.
        Debug.Log("Submitting score");


        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done


        Debug.Log("Score submitted");
        Debug.Log("post_url: " + post_url);

        if (hs_post.error != null)
        {
            Debug.Log("There was an error posting the high score: " + hs_post.error);
        }
        else
        {
            GameObject.Find("CubeCity").GetComponent<CubeCity>().ReloadCity();
        }
        
    }

    // Get the scores from the database
    public IEnumerator GetScores(Action<int> returnCode)
    {
        Debug.Log("running get scores");


        /* (old)
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;
        */

        using (UnityWebRequest hs_get = UnityWebRequest.Get(highscoreURL))
        {

            yield return hs_get.SendWebRequest();

            if (hs_get.error != null)
            {
                Debug.Log("There was an error getting the high score: " + hs_get.error);
                returnCode(1);
            }
            else
            {
                if (scores != null)
                {
                    Destroy(scores.gameObject);
                }
                GameObject newLoadScores = new GameObject();
                scores = newLoadScores.AddComponent<Scores>();

                // split the results into an array
                Regex regex = new Regex(@"[\t\n]");
                string[] rawScores = regex.Split(hs_get.downloadHandler.text);

                int rawScoreIndex = 0;
                //Debug.Log("rawScores.Length " + rawScores.Length);
                for (int i = 0; i < Mathf.FloorToInt(rawScores.Length / dataCols); i++)
                {
                    scores.names[i] = rawScores[rawScoreIndex];
                    scores.scores[i] = int.Parse(rawScores[rawScoreIndex + 1]);
                    scores.citySize[i] = int.Parse(rawScores[rawScoreIndex + 2]);
                    scores.helicopters[i] = int.Parse(rawScores[rawScoreIndex + 3]);
                    scores.scrapers[i] = int.Parse(rawScores[rawScoreIndex + 4]);
                    scores.cityColor[i] = int.Parse(rawScores[rawScoreIndex + 5]);

                rawScoreIndex += dataCols;
                }
                returnCode(0);
            }
        }
    }
}