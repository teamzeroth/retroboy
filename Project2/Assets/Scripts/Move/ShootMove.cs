﻿using UnityEngine;
using System.Collections;

public class ShootMove : MonoBehaviour {

    public Vector2 direction;

    public float speed;
    public float damage;

    public bool isPlayerAlly;

    [HideInInspector]
    public bool flipped;

    void Start() {
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    void Update() {
        rigidbody2D.velocity = direction.normalized * speed;
    }

    public void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Enemy" && isPlayerAlly)
            trigger.gameObject.GetComponent<Enemy>().Hit(damage, direction);        
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        
        if (coll.gameObject.tag == "Player" && !isPlayerAlly)
            coll.gameObject.GetComponent<AnimationController>().Hit(damage, direction);

        Destroy(gameObject);
    }
  
    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }


    void OnBecameInvisible(){
        Destroy(gameObject);
    }
}
