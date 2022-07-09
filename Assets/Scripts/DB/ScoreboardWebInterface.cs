using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class ScoreboardWebInterface : MonoBehaviour
{
    // psuedo-public password below: another hidden password is used on private website accessed only from the same site
    string secretKey = "Gosh@_23872"; // Edit this value and make sure it's the same as the one stored on the server
    public string addScoreURL = "https://yalewinter.com/cityviewer/savecity.php?"; //be sure to add a ? to your url
    public string highscoreURL = "https://yalewinter.com/cityviewer/readcities.php";

    int dataCols = 8;

    Scores scores;
    public Scores Scores => scores;

    /// <summary>
    /// This connects to a server side php script that will add the name and score to a MySQL DB
    /// Supply it with a string representing the players name and the players score.
    /// can also hash and unhash info to confirm match (not used here)
    /// </summary>
    public IEnumerator PostScores(string name, int score, int citySize, int helicopters, int scrapers, int cityColor, int height, int cars)
    {

        string hash = name + score + citySize + helicopters + scrapers + height + cars + cityColor + secretKey;
        string post_url = addScoreURL + "name=" + UnityWebRequest.EscapeURL(name) + "&score=" + score + "&citySize=" + citySize + "&helicopters=" + helicopters + "&scrapers=" + scrapers + "&cityColor=" + cityColor + "&height=" + height + "&cars=" + cars + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        Debug.Log("Submitting score");
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        using (UnityWebRequest www = UnityWebRequest.Post(post_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                GameObject.Find("CubeCity").GetComponent<CubeCity>().ReloadCity();

            }
        }
    }
    /// <summary>
    /// Get 8 most recently saved cities information online 
    /// (external PHP / SQL)
    /// </summary>
    public IEnumerator GetScores(Action<int> returnCode)
    {
        Debug.Log("loading Cities");
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
                    scores.height[i] = int.Parse(rawScores[rawScoreIndex + 6]);
                    scores.cars[i] = int.Parse(rawScores[rawScoreIndex + 7]);

                    rawScoreIndex += dataCols;
                }
                returnCode(0);
            }
        }
    }
}