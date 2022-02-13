using UnityEngine;

public class PathDetDestroySoon : MonoBehaviour
{
    private float startTime = Mathf.Infinity;
    void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        if (Time.time - 0.01F > startTime)
        {
            Destroy(gameObject);
        }
    }
}
