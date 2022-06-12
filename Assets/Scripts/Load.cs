using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    void Start()
    {
            StartCoroutine(LoadMain());
    }

    IEnumerator LoadMain()
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CityViewerOnline");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
