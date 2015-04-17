using UnityEngine;
using System.Collections;

public class StingerE : Enemy
{

    public float explosionRadius = 3f, explodeDelay = 1f;
    float movementDirection;
    Vector3 localScale;
    
    protected override void Start()
    {
        base.Start();
        movementDirection = 1f;
        localScale = this.gameObject.transform.localScale;
    }

    protected override void Movement()
    {
        if (seek)
        {
            body.velocity = Vector2.right * movementDirection;
            seek = false;
            movementDirection *= -1f;
            this.gameObject.transform.localScale = localScale;
            StartCoroutine("changeSeek", 1f);
            localScale.x *= -1;
        }
        else if (destroy)
            base.Movement();        
    }

    protected IEnumerator explode(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (distance <= explosionRadius)
            base.Attack(target);
        this.life = 0;
    }

    protected override void Attack(GameObject obj)
    {
        if (melee)
        {
            StopAllCoroutines();
            StartCoroutine("explode", explodeDelay);
            body.fixedAngle = true;
            melee = false;
            seek = false;
            destroy = false;
        }
    }
}
