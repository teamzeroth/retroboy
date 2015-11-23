using UnityEngine;
using System.Collections;

public class SetSentryGunAngle : MonoBehaviour {
    public Transform objectiveDirection;
    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        int direction = Helper.getGeoDirection(objectiveDirection.transform.position,false);
        animator.SetInteger("Direction", (int)direction);
	}
}
