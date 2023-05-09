using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarDrive : MonoBehaviour
{

    Rigidbody rb;
    public Camera cam;

    public float maxSpeed = 100f;
    [Range(0f, 1f)]
    public float acceleration = 0.8f;


    private float currentSpeed = 0f;
    private float currentAccel = 0f;
    private float steerChange = 1f;

    private Vector3 force;
    public Vector3 camOffset = new Vector3(0f, 3f, -3f);
    public Vector3 rotationOffset;
    private Vector3 accelDirection;

    public GameObject carSpawn;

    public GameObject frontOfCar, backOfCar;
    public Transform groundCheck;
    public Transform roofCheck;
    public LayerMask groundMask;
    public GameObject frontLeftWheel, frontRightWheel, backLeftWheel, backRightWheel;

    private bool isOnGround = true;
    private bool isUpsideDown = false;
    private bool isFastStearing = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        
        /*cam.transform.position = gameObject.transform.localPosition + camOffset;
        cam.transform.parent = gameObject.transform;
        cam.transform.rotation = gameObject.transform.rotation;
        Vector3 eul = cam.transform.rotation.eulerAngles;*/


    }

    // Update is called once per frame
    void Update()
    {
        accelDirection = -backOfCar.transform.position + frontOfCar.transform.position;

        isOnGround = Physics.CheckSphere(groundCheck.position, 0.5f, groundMask);
        isUpsideDown = Physics.CheckSphere(roofCheck.position, 0.5f, groundMask);

        //CarGroundCheck();

        if (Input.GetKey(KeyCode.W) || Input.GetAxis("Fire1") != 0 )
        {
            Accelerate();
            
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            //PlayAccelerationSound();
            FindObjectOfType<AudioManager>().Play("Car Go");
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            FindObjectOfType<AudioManager>().StopPlaying("Car Go");
        }
        if (Input.GetKey(KeyCode.S) || Input.GetAxis("Fire2") != 0)
        {
            Break();

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeStearing();
        }

        if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
        {
            
            Vector2 steer = new Vector2(0f, Input.GetAxisRaw("Horizontal"));
            //Debug.Log(steer);
            Steer(steer);
            

        }
        if(Input.GetKey(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            HandBreak();
        }

        if (isUpsideDown || Input.GetKeyDown(KeyCode.X))
        {
            FlipCar();
        }//

        if(gameObject.transform.position.y <= -15f)
        {
            MoveToSpawn();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        //Debug.DrawRay(frontOfCar.transform.position, accelDirection, Color.blue);
    }//Update

    private void ChangeStearing()
    {
        if (isFastStearing)
        {
            isFastStearing = false;

            steerChange = 0.9f;
        }
        else
        {
            isFastStearing = true;

            steerChange = 0.38f;
        }
    }

    private void PlayAccelerationSound()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    

    private void MoveToSpawn()
    {
        gameObject.transform.position = carSpawn.transform.position + new Vector3(0f, 1f, 0f);
    }

    private void FlipCar()
    {
        gameObject.transform.position = gameObject.transform.position + Vector3.up;
        gameObject.transform.localRotation = Quaternion.LookRotation(gameObject.transform.forward, Vector3.up);
    }//FlipCar

    private void HandBreak()
    {
        if (isOnGround && rb.velocity.magnitude > 0)
        {
            rb.AddForce(-rb.velocity  * Time.deltaTime * 0.8f, ForceMode.Acceleration);
            frontRightWheel.transform.Rotate(new Vector3(20f * acceleration, 0f, 0f));
            frontLeftWheel.transform.Rotate(new Vector3(20f * acceleration, 0f, 0f));
        }
    }

    private void Break()
    {
        if (rb.velocity.magnitude >= -20 && isOnGround)
        {
            rb.AddForce(-accelDirection * Time.deltaTime * (400f * acceleration), ForceMode.Acceleration);
            frontRightWheel.transform.Rotate(new Vector3(20f * acceleration, 0f, 0f));
            frontLeftWheel.transform.Rotate(new Vector3(20f * acceleration, 0f, 0f));
            backLeftWheel.transform.Rotate(new Vector3(18f * acceleration, 0f, 0f));
            backRightWheel.transform.Rotate(new Vector3(18f * acceleration, 0f, 0f));
        }
    }

    private void Accelerate()
    {

        if (rb.velocity.magnitude <= maxSpeed && isOnGround)
        {
            rb.AddForce(accelDirection * Time.deltaTime * (500f * acceleration ), ForceMode.Acceleration);
            frontRightWheel.transform.Rotate(new Vector3(-20f * acceleration, 0f, 0f));
            frontLeftWheel.transform.Rotate(new Vector3(-20f * acceleration, 0f, 0f));
            backLeftWheel.transform.Rotate(new Vector3(-20f * acceleration, 0f, 0f));
            backRightWheel.transform.Rotate(new Vector3(-20f * acceleration, 0f, 0f));
        }
        else if(rb.velocity.magnitude >= maxSpeed - 1f)
        {
            rb.AddForce(accelDirection * Time.deltaTime * (10f * acceleration), ForceMode.Acceleration);
        }
    }

    private void Steer(Vector2 steer)
    {
        //rb.AddRelativeTorque(accelDirection * steer * 900f, ForceMode.VelocityChange);
        float turnCoefficient;
        if (isOnGround)
        {

          

                turnCoefficient = rb.velocity.magnitude / maxSpeed;
            
            
            
            gameObject.transform.Rotate(new Vector3(steer.x * turnCoefficient * steerChange, steer.y * turnCoefficient * steerChange, 0f));
            
        }
        else if (!isOnGround)
        {
            gameObject.transform.Rotate(new Vector3(steer.x/10f, steer.y/10f, 0f));
        }

/*        frontRightWheel.transform.Rotate(new Vector3(0f, 0f, steer.y));
        frontLeftWheel.transform.Rotate(new Vector3(0f, 0f, steer.y));*/
    }

    private void DisableAnimator()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
}
