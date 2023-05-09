using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCar : MonoBehaviour
{

    public Transform respawnLocation;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
       if(other.transform.tag == "Car")
        {
            Respawn(other.gameObject);
        } 
    }

    public void Respawn(GameObject car)
    {
        car.transform.position = respawnLocation.position + new Vector3(0f,0.4f,0f);
    }
}
