using System;
[Serializable]
public class CarModel
{
    public int iD = -1;
    public float speed = 6.0F;
    public CarModel(int setIndex, float setSpeed)
    {
        iD = setIndex;
        speed = setSpeed;
    }
}
