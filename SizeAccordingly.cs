using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeAccordingly : MonoBehaviour
{


    public float checkSphereRadius = 0.2f;
    public LayerMask checkLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Transform wallCheck01 = gameObject.transform.GetChild(0);


        bool wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);


        Debug.Log(wall1Touched);
    }


   
}
