using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public HelicopterView heliView;
    public HelicopterModel heliModel;

    public void SetUp(HelicopterModel setHM, HelicopterView setHV)
    {
        heliModel = setHM;
        heliView = setHV;
    }
}
