using UnityEngine;
using System.Collections;

public class MachineDog : Enemy {

    public float rotateStep = 5f, maxRotation = 45f, rotationDelay = 0.25f;
    int count;
    float movementDirection, z;
    Vector3 localScale;
    ShootMove bullet = null;

	// Use this for initialization
    protected override void Start () {
        base.Start();
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
        this.gameObject.transform.Rotate(0,0,rotateStep);
    }

    protected override void Movement()
    {
        if (seek)
        {
            body.velocity = Vector2.right * movementDirection;
            seek = false;
            movementDirection *= -1f;
            this.gameObject.transform.localScale = localScale;
            StartCoroutine("changeSeek",1f);
            localScale.x *= -1;
        }
        else if (destroy)
        {
            if (count == 2)
            {
                StopAllCoroutines();
                count = 0;
                destroy = false;
                seek = true;
            }
            else
                StartCoroutine("rotate", rotationDelay);
        }
    }

    protected override void Attack(GameObject obj)
    {
        if (obj != null && destroy)
        {
            z = this.gameObject.transform.rotation.eulerAngles.z;
            bullet = prefab.Spawn();
            Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * -Vector3.right;
            bullet.transform.position = position;
            print(z);
            bullet.direction = Quaternion.Euler(0, 0, z) * Vector2.right * -movementDirection;           

            destroy = false;
            StartCoroutine("changeDestroy", shootDelay);            
        }
    }
}
