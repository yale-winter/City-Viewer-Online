using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class PageTool : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GoFullscreen();
 
    public static void ActivateFullscreen()
    {
        GoFullscreen();
    }
#else
    public static void ActivateFullscreen()
    { }
#endif
}