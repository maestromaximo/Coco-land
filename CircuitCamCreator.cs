using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class CircuitCamCreator : MonoBehaviour
{


    public float rayRange = 500f;
    public Camera cam;
    public LayerMask groundMask;


    public List<Vector3> rawRayPoints;
    public float delimeterRawDistance = 1f;

    public GameObject car;

    public GameObject marker;
    public GameObject rawRoad;
    public GameObject ground; //temporary for rotation work at need work
    public GameObject carSpawn;
    public GameObject city;

    public bool drawCircuit = false;
    private bool canPlaceSpawner = false;
    private bool isDrawingRoad = false;

    public Canvas adviceUi, mainUi;

    //TESTING START
    /*public Transform mark1, mark2, mark3, mark4;
    [Range(0f, 1f)]
    public float t = 0f;

    public GameObject finalMarker;*/


    public GameObject[] rawRoads;
    public List<GameObject> markerObjects;
    //TESTING END


    //tt3
    public float checkSphereRadius = 0.2f;
    public LayerMask checkLayer;

    //tt3


    void Start()
    {
        isDrawingRoad = true;
        rawRayPoints = new List<Vector3>();
        //OrientedPoint point = new OrientedPoint(Vector3.back, gameObject.transform.rotation);

        adviceUi.enabled = false;
        mainUi.enabled = true;
        //TEST



        //TEST END

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && cam.targetDisplay == 0 && isDrawingRoad)
        {
            ShootRay();
            //Debug.Log("shot");
        }
        if (Input.GetKeyDown(KeyCode.Tab) && isDrawingRoad)
        {
            PutRawRoads();
            canPlaceSpawner = true;
        }

        if (drawCircuit)
        {
            PrintDebugCircuit();
        }

        if (Input.GetButtonDown("Fire2") && isDrawingRoad)
        {
            carSpawn.transform.position = placeSpawner();
        }
        if (Input.GetKeyDown(KeyCode.Return) && isDrawingRoad)
        {
            RespawnCarBack();
            isDrawingRoad = false;
            city.SetActive(false);
            adviceUi.enabled = false;
        }

        


        //TESTING START

       /* Vector3[] guides = { mark1.position, mark2.position, mark3.position, mark4.position };
        Vector3 thePoint = GetPoint(guides, t);
        Vector3 thePointTangent = GetTangent(guides, t);

        finalMarker.transform.position = thePoint;
        Debug.DrawRay(thePoint, thePointTangent * 10f, Color.magenta);*/

        //TESTING END


    }//Update

    public void CityRun()
    {
        adviceUi.gameObject.SetActive(false);
        mainUi.gameObject.SetActive(false);
    }

    private void RespawnCarBack()
    {
        car.transform.position = carSpawn.transform.position + Vector3.up;
        carSpawn.GetComponent<MeshRenderer>().enabled = false;

        cam.targetDisplay = 1;
    }

    private Vector3 placeSpawner()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayRange, groundMask))
        {

            return hit.point;
        }

        return Vector3.zero;
    }

    void ShootRay()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayRange, groundMask))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.green);


            if (rawRayPoints.Count == 0)
            {
                rawRayPoints.Add(hit.point);
                GameObject markerObj = Instantiate(marker, hit.point, hit.transform.rotation);
                markerObjects.Add(markerObj);
                return;
            }

            Vector3 deltaVec = hit.point - rawRayPoints[rawRayPoints.Count - 1];
            float deltaDistance1 = Mathf.Abs(deltaVec.magnitude);

            if (deltaDistance1 >= delimeterRawDistance)
            {
                rawRayPoints.Add(hit.point);
                GameObject markerObj = Instantiate(marker, hit.point, hit.transform.rotation);
                markerObjects.Add(markerObj);
            }


        }//ray


    }//ShootRay

    public void PutRawRoads()
    {
        if (rawRayPoints.Count > 0)
        {
            int index = 0;
            rawRoads = new GameObject[rawRayPoints.Count];

            foreach (Vector3 point in rawRayPoints)
            {

                Quaternion endRotation;
                Vector3 tangentOfDirection;

                if (index + 1 < rawRayPoints.Count)
                {
                    tangentOfDirection = rawRayPoints[index] - rawRayPoints[index + 1];
                }
                else
                {
                    tangentOfDirection = -rawRayPoints[0] + rawRayPoints[rawRayPoints.Count - 1];
                }

                endRotation = Quaternion.LookRotation(tangentOfDirection, Vector3.up);

                GameObject road = Instantiate(rawRoad, new Vector3(point.x, point.y + 0.1f, point.z), endRotation);//rotation needs work

                rawRoads[index] = road;
                index++;

            }


            foreach (GameObject ball in markerObjects)
            {
                Destroy(ball);
            }



            //NOT SURE IT WILL WORK

            AdjustWalls3();
            CleanWalls();

            //NOT SURE IT WILL WORK
        }

    }//PutRawRoads

    private void CleanWalls()
    {
        


    }//CleanWalls

    IEnumerator CallAdjustWalls()
    {

        bool done = false;
        new Thread(() =>
        {

            // AdjustWalls();
            done = true;
        }).Start();

        while (!done)
        {
            yield return null;
        }

        Debug.Log("Completed");
    }

    private void AdjustWalls()
    {
        int index = 0;
        foreach (GameObject wall in rawRoads)
        {

            GameObject wall1 = wall.transform.GetChild(2).GetChild(0).gameObject;
            GameObject wall2 = wall.transform.GetChild(4).GetChild(0).gameObject;


            if (index < rawRoads.Length - 1)
            {
                bool wall1Touched = wall1.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);

                if (wall1Touched)
                {
                    bool while1 = wall1.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);//touched

                    while (while1 && wall1.transform.localScale.x >= 0.2f)//made a change
                    {
                        while1 = wall1.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                        wall1.GetComponent<MeshFilter>().mesh.RecalculateBounds();


                        wall1.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    bool while12 = wall1.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                    while (!while12 && wall1.transform.localScale.x <= 15f)
                    {
                        while12 = wall1.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                        wall1.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                        wall1.transform.localScale += new Vector3(0.2f, 0f, 0f);
                    }

                }//if1NotT

                //WALL2 LOGIC
                bool wall2Touched = wall2.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);

                if (wall2Touched)
                {

                    bool while2 = wall2.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                    while (while2 && wall2.transform.localScale.x >= 0.2f)
                    {
                        while2 = wall2.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                        wall2.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    bool while22 = wall2.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                    while (!while22 && wall2.transform.localScale.x <= 15f)
                    {
                        while22 = wall2.GetComponent<MeshFilter>().mesh.bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds);
                        wall2.transform.localScale += new Vector3(0.2f, 0f, 0f);
                    }

                }//if1NotT

                //WALL2 LOGIC


            }//not last if


            index++;
        }


    }

    private void AdjustWalls2()
    {
        int index = 0;
        foreach (GameObject wall in rawRoads)
        {

            GameObject wall1 = wall.transform.GetChild(2).GetChild(0).gameObject;
            GameObject wall2 = wall.transform.GetChild(4).GetChild(0).gameObject;


            if (index < rawRoads.Length - 1)
            {
                bool wall1Touched = wall1.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);

                if (wall1Touched)
                {
                    bool while1 = wall1.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);//touched

                    while (while1 && wall1.transform.localScale.x >= 0.2f)//made a change
                    {
                        while1 = wall1.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                        // wall1.GetComponent<BoxCollider>().RecalculateBounds();


                        wall1.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    bool while12 = wall1.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                    while (!while12 && wall1.transform.localScale.x <= 15f)
                    {
                        while12 = wall1.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(2).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                        // wall1.GetComponent<BoxCollider>().RecalculateBounds();
                        wall1.transform.localScale += new Vector3(0.2f, 0f, 0f);
                    }

                }//if1NotT

                //WALL2 LOGIC
                bool wall2Touched = wall2.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);

                if (wall2Touched)
                {

                    bool while2 = wall2.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                    while (while2 && wall2.transform.localScale.x >= 0.2f)
                    {
                        while2 = wall2.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                        wall2.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    bool while22 = wall2.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                    while (!while22 && wall2.transform.localScale.x <= 15f)
                    {
                        while22 = wall2.GetComponent<BoxCollider>().bounds.Intersects(rawRoads[index + 1].transform.GetChild(4).GetChild(0).gameObject.GetComponent<BoxCollider>().bounds);
                        wall2.transform.localScale += new Vector3(0.2f, 0f, 0f);

                    }

                }//if1NotT

                //WALL2 LOGIC


            }//not last if


            index++;
        }


    }//AdjustWalls2

    private void AdjustWalls3()
    {

        int index = 0;
        foreach (GameObject wall in rawRoads)
        {

            GameObject wall1 = wall.transform.GetChild(2).GetChild(0).gameObject;
            Transform wallCheck01 = wall1.transform.GetChild(0);

            GameObject wall2 = wall.transform.GetChild(4).GetChild(0).gameObject;
            Transform wallCheck02 = wall2.transform.GetChild(0);


            if (index < rawRoads.Length - 1)
            {
                bool wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);

                if (wall1Touched)
                {
                    //wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);

                    while (wall1Touched && wall1.transform.localScale.x >= 0.2f)//made a change
                    {
                        wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);
                        // wall1.GetComponent<BoxCollider>().RecalculateBounds();


                        wall1.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    //wall1Touched = wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);
                    while (!wall1Touched && wall1.transform.localScale.x <= 15f)
                    {
                        wall1Touched = wall1Touched = Physics.CheckSphere(wallCheck01.position, checkSphereRadius, checkLayer);
                        // wall1.GetComponent<BoxCollider>().RecalculateBounds();
                        wall1.transform.localScale += new Vector3(0.2f, 0f, 0f);
                    }

                }//if1NotT

                //WALL2 LOGIC
                bool wall2Touched = Physics.CheckSphere(wallCheck02.position, checkSphereRadius, checkLayer);

                if (wall2Touched)
                {

                    //wall2Touched = Physics.CheckSphere(wallCheck02.position, checkSphereRadius, checkLayer);

                    while (wall2Touched && wall2.transform.localScale.x >= 0.2f)
                    {
                        wall2Touched = Physics.CheckSphere(wallCheck02.position, checkSphereRadius, checkLayer);
                        wall2.transform.localScale -= new Vector3(0.2f, 0f, 0f);
                    }//whileReduce

                }//if1T
                else
                {
                    //wall2Touched = Physics.CheckSphere(wallCheck02.position, checkSphereRadius, checkLayer);
                    while (!wall2Touched && wall2.transform.localScale.x <= 15f)
                    {
                        wall2Touched = Physics.CheckSphere(wallCheck02.position, checkSphereRadius, checkLayer);
                        wall2.transform.localScale += new Vector3(0.2f, 0f, 0f);

                    }

                }//if1NotT

                //WALL2 LOGIC


            }//not last if


            index++;
        }





    }


    public void SwitchRenderCam()
    {
        if(cam.targetDisplay == 1)
        {
            cam.targetDisplay = 0;
            //Debug.Log("clicked");
        }

        mainUi.gameObject.SetActive(false);
        adviceUi.gameObject.SetActive(true);

        adviceUi.enabled = true;

    } 

    private void PrintDebugCircuit()
    {

        if (rawRayPoints.Count > 1)
        {
            for (int index = 0; index < rawRayPoints.Count; index++)
            {

                Debug.DrawLine(rawRayPoints[index], rawRayPoints[index + 1], Color.blue);


            }//for

        }//if
    }//PrintDebugCircuit


    /*Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;

        float t2 = t * t;

        return pts[0] * (omt2 * omt) +
               pts[1] * (3f * omt2 * t) +
               pts[2] * (3f * omt * t2) +
               pts[3] * (t2 * t);

    }

    Vector3 GetTangent(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
                pts[0] * (-omt2) +
                pts[1] * (3 * omt2 - 2 * omt) +
                pts[2] * (-3 * t2 + 2 * t) +
                pts[3] * (t2);
        return tangent.normalized;
    }

    Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }*/


}//class
