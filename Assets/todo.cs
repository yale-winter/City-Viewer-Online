using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class todo : MonoBehaviour
{
    // ***Steet light controller
    // ***allows passage = none, x, z
    // ***timer controls min max time to change light next
    // ***goes to yellow then red, then other side sets green
    // when trigger enter decide if stay same path, or switch to other path (from top left)
    // ****also if not allow passage: stop and wait for time duration until allow passage (get delay from controller)
    // mega skyscrapers can only make a few add trigger or rigidbody to collision detection
    // helicopters paths start as circle around mega skyscraper and add a few additional points (detection collision to verify OK) and then loops back
    // helicopter blade alternate showing different color texture add transparency
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
