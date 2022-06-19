using System;
public class StreetLightModel
{
    public int ID => iD;
    int iD = -1;
    public float XPos => xPos;
    float xPos = -1;
    public float ZPos => zPos;
    float zPos = -1;
    public StreetLightModel(int setIndex, float posX, float posY)
    {
        iD = setIndex;
        xPos = posX;
        zPos = posY;
    }
}

