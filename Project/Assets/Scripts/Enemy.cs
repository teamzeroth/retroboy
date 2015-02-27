using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f;
    public Transform target = null;
    protected Vector3 heading = Vector3.zero, direction = Vector3.zero;
    protected float distance = 0f;

    protected void UpdatePosition()
    {
        heading = target.position - this.gameObject.transform.position;
        distance = heading.magnitude;
        direction = heading / distance;	  
    }

    protected virtual void Defense() { }

    protected virtual void Attack() { }

    protected virtual void Movement() { }
}
