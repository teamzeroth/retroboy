using UnityEngine;
using System.Collections;

public class StingerE : Enemy
{

	public float explosionRadius = 3f, explodeDelay = 1f, amplitude = 0.2f, frequency = 0.85f;
    float movementDirection;
    Vector3 localScale;
    
    protected override void Start()
    {
        base.Start();
        movementDirection = 1f;
        localScale = transform.localScale;
    }

    protected override void Movement()
    {
		if (distance < 1f)
		{
			print ("Movement < 1");
			rigidbody2D.velocity = Vector2.zero;
			Debug.DrawLine(transform.position, transform.position+transform.up, Color.cyan);
			//rigidbody2D.MovePosition((Vector2) transform.position + Vector2.up * Mathf.Sin(Time.deltaTime)*5f);
			transform.position += amplitude*(Mathf.Sin(2*Mathf.PI*frequency*Time.time) - Mathf.Sin(2*Mathf.PI*frequency*(Time.time - Time.deltaTime)))*transform.up;
			melee = true;
			Attack(target);
		}	
		else
        	base.Movement();        
    }

    protected IEnumerator explode(float delay)
    {
        yield return new WaitForSeconds(delay);
		print ("explode");
        if (distance <= explosionRadius)
            base.Attack(target);
        life = 0;
    }

    protected override void Attack(GameObject obj)
    {
        if (melee)
        {
			print ("Attack");
            StopAllCoroutines();
            StartCoroutine(explode(explodeDelay));
            rigidbody2D.fixedAngle = true;
            melee = false;
            seek = false;
            destroy = false;
        }
    }
}
