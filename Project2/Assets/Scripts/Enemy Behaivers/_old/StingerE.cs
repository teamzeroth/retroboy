﻿using UnityEngine;
using System.Collections;

public class StingerE : Enemy
{
    public float explosionRadius = 3f, amplitude = 0.2f, frequency = 0.85f;
    
    protected override void Movement()
    {
        //Debug.Log("V: " + rigidbody2D.velocity + " | Vm: " + (double)rigidbody2D.velocity.magnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm: " + (double)target.rigidbody2D.velocity.magnitude + " | D: " + distance);

        if (distance <= 1f)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.position += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;
            if (!attacking)
            {
                StartCoroutine(turnAttack(attackDelay));
                attacking = true;
            }
        }
        else
            base.Movement();       
    }

    protected override void Attack(GameObject obj)
    {
        if (destroy)
        {
            destroy = false;
            StopAllCoroutines();
            if (distance <= explosionRadius)
                base.Attack(target);
            life = 0;
        }
    }
}
