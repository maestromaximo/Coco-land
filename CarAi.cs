using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAi : MonoBehaviour
{

    public NavMeshAgent car;
    public LayerMask groundLayer;
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, 10f, groundLayer)){
            car.SetDestination(player.transform.position);
        //}
        
    }
}
