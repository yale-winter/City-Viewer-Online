using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XHelpers
{
    public static Vector3 exitIntersectionPos(float[] moveDir, Vector3 interPos)
    {
        float distToEscapeInter = 1.5f;
        Vector3 exitPos = new Vector3(interPos.x + moveDir[0] * distToEscapeInter, 0.05f, interPos.z + moveDir[1] * distToEscapeInter);
        return exitPos;
    }

    public static Vector3 midIntersectionPos(float[] moveDir, Vector3 interPos)
    {
        float distToEscapeInter = 0.2f;
        Vector3 exitPos = new Vector3(interPos.x + moveDir[0] * distToEscapeInter, 0.05f, interPos.z + moveDir[1] * distToEscapeInter);
        return exitPos;
    }
    public static Vector2Int sizeFromLoadSettings(int i)
    {
        float sizeMult = i / 100.0f;
        Vector2Int size = new Vector2Int((int)Mathf.Round(4.0f + 4.0f * sizeMult), (int)Mathf.Round(4.0f + 8.0f * sizeMult));
        if (size.x % 2 != 0)
        {
            size.x--;
        }
        if (size.y % 2 != 0)
        {
            size.y--;
        }
        return size;
    }
    public static float heliFromLoadSettings(int i)
    {
        return i / 100.0f * 0.5f;

    }
    public static float maxHeliFromLoadSettings(int i)
    {
        int r = Mathf.RoundToInt(i / 100.0f * 20.0f);

        return r;

    }
    public static float scrapFromLoadSettings(int i)
    {
        return Mathf.Max(0.01f, i / 100.0f);

    }
    public static float heightFromLoadSettings(int i)
    {
        return Mathf.Max(0.01f, i / 100.0f);

    }
    public static float carsFromLoadSettings(int i)
    {
        return Mathf.Max(0.01f, i / 100.0f);

    }
    public static string PadZeros(string str, int setChars)
    {
        while (str.Length < setChars)
        {
            str = "0" + str;
        }
        return str;
    }
}
