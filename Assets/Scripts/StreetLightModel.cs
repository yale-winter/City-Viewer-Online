using System.Numerics;
using System;
[Serializable]
public class StreetLightModel
{
    public int iD = -1;
    public float xPos = -1;
    public float zPos = -1;
    public StreetLightModel(int setIndex, float posX, float posY)
    {
        iD = setIndex;
        xPos = posX;
        zPos = posY;
    }
}

