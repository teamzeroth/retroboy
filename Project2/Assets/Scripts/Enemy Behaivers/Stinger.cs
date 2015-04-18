using UnityEngine;
using System.Collections;

public class Stinger : Enemy
{

    public float rotateStep = -5f, maxRotation = 45f;
    int count;
    float movementDirection, z;
    Vector3 localScale;
    ShootMove bullet = null;
    Transform t;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
		speed = 10f;
		t = null;
        movementDirection = 1f;
        count = 0;
        z = 0f;
        localScale = Vector3.zero;
        localScale = this.gameObject.transform.localScale;
    }

    protected IEnumerator rotate(float delay)
    {
        yield return new WaitForSeconds(delay);
        z = this.gameObject.transform.rotation.eulerAngles.z;
        if (z > maxRotation)
        {
            rotateStep *= -1;
            count++;
        }
        this.gameObject.transform.Rotate(0, 0, rotateStep);
    }
    
    protected override void Movement()
    {
        Debug.DrawLine(this.gameObject.transform.position, target.transform.position - new Vector3(0f, 0.4f, 0f));
        
		print(body.velocity + " | " + seek + " | " +  direction);	
		body.AddForce(direction * speed, ForceMode2D.Impulse);
		if (distance < 0.2 && seek)
        {
			body.AddForce(Vector2.up * -1, ForceMode2D.Impulse);
//            seek = false;
//            movementDirection *= -1f;
//            this.gameObject.transform.localScale = localScale;
//            StartCoroutine("changeSeek", 0.5f);
//            localScale.x *= -1;
        }
//        else if (destroy)
//        {
//            if (this.transform.rotation.eulerAngles.z > maxRotation)
//            {
//                rotateStep *= -1f;
//                count++;
//            }
//
//            t = target.transform;
//            
//            Vector3 position = (target.transform.position - this.gameObject.transform.position - new Vector3(0f,0.4f,0f));
//            print(target.transform.position + " | " + position + " | " + position.magnitude + " | " + position / position.magnitude + " | " + (position / position.magnitude).magnitude);
//            this.gameObject.transform.RotateAround(t.position - new Vector3(0f,0.4f,0f), Vector3.forward,1 * rotateStep);
////            lookAt2D(t.position);            
//
//            if (count == 2)
//            {
//                StopAllCoroutines();
//                count = 0;
//                destroy = false;
//                seek = true;
//            }
//        }
    }

    protected override void Attack(GameObject obj)
    {
		if (false)
		//if (obj != null && destroy)
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
