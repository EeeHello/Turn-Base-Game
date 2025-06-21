using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Test1 : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "onGround")
        {
            print("Enter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "onGround")
        {
            print("Still on Ground");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "onGround")
        {
            print("No longer on Ground");
        }
    }
}
