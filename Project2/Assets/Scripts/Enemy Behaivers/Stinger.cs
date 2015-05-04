using UnityEngine;
using System.Collections;

public class Stinger : Enemy
{
	public float amplitude = 0.2f, frequency = 0.85f;
	int count;
    float movementDirection, z;
    Vector3 localScale;
    ShootMove bullet = null;
    Transform t;

    // Use this for initialization
    /*protected override*/void Start()
    {
        base.Start();
		this.speed = 2f;
        movementDirection = 1f;
        count = 0;
        z = 0f;
        localScale = Vector3.zero;
        localScale = this.gameObject.transform.localScale;
    }

//    protected IEnumerator rotate(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        z = this.gameObject.transform.rotation.eulerAngles.z;
//        if (z > maxRotation)
//        {
//            rotateStep *= -1;
//            count++;
//        }
//        this.gameObject.transform.Rotate(0, 0, rotateStep);
//    }
    
    protected override void Movement()
    {
		Debug.Log("D: " + distance);
		if (target.rigidbody2D.velocity.sqrMagnitude < 1f && distance < 1f)
		{
			rigidbody2D.velocity = Vector2.zero;
			Debug.DrawLine(transform.position, transform.position+transform.up, Color.cyan);
			//rigidbody2D.MovePosition((Vector2) transform.position + Vector2.up * Mathf.Sin(Time.deltaTime)*5f);
			transform.position += amplitude*(Mathf.Sin(2*Mathf.PI*frequency*Time.time) - Mathf.Sin(2*Mathf.PI*frequency*(Time.time - Time.deltaTime)))*transform.up;
		}
		else
			base.Movement();
	}

    protected override void Attack(GameObject obj)
    {
		if (obj != null)
        {
            z = this.gameObject.transform.rotation.eulerAngles.z;
            bullet = prefab.Spawn();
            Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * -Vector3.right;
            bullet.transform.position = position;
            bullet.direction = Quaternion.Euler(0, 0, z) * Vector2.right * -movementDirection;

            destroy = false;
            StartCoroutine("changeDestroy", shootDelay);
        }
    }
}
