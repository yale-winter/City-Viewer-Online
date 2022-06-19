using System;
public class CarModel
{
    public int ID => iD;

    int iD = -1;

    public float Speed => speed;

    float speed = 6.0F;

    public CarModel(int setIndex, float setSpeed)
    {
        iD = setIndex;
        speed = setSpeed;
    }
}
