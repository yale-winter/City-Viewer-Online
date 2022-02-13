public class HelicopterModel
{
    public int iD = -1;
    public float speed = 8.0F;
    public float toggleBladesTime = 100.0F;
    
    public HelicopterModel(int setID, float setSpeed, float setToggleTime)
    {
        iD = setID;
        speed = setSpeed;
        toggleBladesTime = setToggleTime;
    }
}
