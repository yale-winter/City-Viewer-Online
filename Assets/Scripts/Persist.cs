using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour
{
    public int forceLoad = -1;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
