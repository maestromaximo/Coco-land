using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimator : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject car;
    public float offSet = -4f;
   private void AnimatorDisable() {

            gameObject.GetComponent<Animator>().enabled = false;

        gameObject.transform.parent = car.transform;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + offSet);
    }
}
