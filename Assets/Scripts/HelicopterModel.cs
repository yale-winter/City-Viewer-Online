public class HelicopterModel
{
    public int ID => iD;
    int iD = -1;
    public float Speed => speed;

    float speed = 8.0F;
    
    public HelicopterModel(int setID, float setSpeed)
    {
        iD = setID;
        speed = setSpeed;

    }
}
