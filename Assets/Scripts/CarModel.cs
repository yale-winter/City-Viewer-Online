using System.Collections;
using System.Collections.Generic;
using System;
[Serializable]
public class CarModel
{
    public int iD = -1;
    public float maxSpeed = 6.0F;
    public CarModel(int setIndex)
    {
        iD = setIndex;
    }
}
