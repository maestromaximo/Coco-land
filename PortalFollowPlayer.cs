using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFollowPlayer : MonoBehaviour
{

    public GameObject player;
    public GameObject playerCam;
    public GameObject portal1;
    public GameObject portal2;

   // private Transform playerOTrans;
    private Vector3 oPosition;
    private Quaternion oRotation;

    public bool isPortal1 = true;

    // Start is called before the first frame update
    void Start()
    {
       // playerOTrans = player.transform;

        oPosition = player.transform.position;
        oRotation = player.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (!isPortal1)
        {
            Vector3 offSetPortal1 = player.transform.position - portal1.transform.position;

            gameObject.transform.position = portal2.transform.position + offSetPortal1;

            float angularDifferencePortals = Quaternion.Angle(portal1.transform.rotation, portal2.transform.rotation);

            Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferencePortals, Vector3.up);
            Vector3 newCamForward = portalRotationalDifference * playerCam.transform.forward;

            transform.rotation = Quaternion.LookRotation(newCamForward, Vector3.up);
        }
        else
        {

            Vector3 offSetPortal2 = player.transform.position - portal2.transform.position;

            gameObject.transform.position = portal1.transform.position + offSetPortal2;

            float angularDifferencePortals = Quaternion.Angle(portal1.transform.rotation, portal2.transform.rotation);

            Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferencePortals, Vector3.up);
            Vector3 newCamForward = portalRotationalDifference * playerCam.transform.forward;

            transform.rotation = Quaternion.LookRotation(newCamForward, Vector3.up);
        }


    }
}
