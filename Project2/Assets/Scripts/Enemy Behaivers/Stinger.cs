 using UnityEngine;
using System.Collections;

public class Stinger : Enemy
{
	public float amplitude = 0.2f, frequency = 0.85f, maxWalkTime = 0.1f, rotationDelay = 0.1f, rotateStep = 5f, maxRotation = 45f;
	float z = 0f, walkTime = 0f;
    ShootMove bullet = null;

    protected override void Movement()
    {
        //Debug.Log("V: " + rigidbody2D.velocity + " | Vm(2): " + (double)rigidbody2D.velocity.sqrMagnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm(2): " + (double)target.rigidbody2D.velocity.sqrMagnitude + " | D: " + distance);
        
        //if (target.rigidbody2D.velocity.sqrMagnitude < 3f && distance <= seekDistance)
        if (destinationHeading.sqrMagnitude <= 1f && distance <= seekDistance)
		{
            if (target.rigidbody2D.velocity.sqrMagnitude < 1f)
                walkTime = 0f;
            else if (walkTime == 0f)
                walkTime = Time.time;

			rigidbody2D.velocity = Vector2.zero; // Tem que ser zero pra o efeito ficar certo, mas causa problemas ao aplicar uma força (i.e. quando o inimigo toma um tiro)
            //Debug.DrawLine(transform.position, transform.position + transform.up, Color.cyan);
            if (seek)
                transform.position += amplitude*(Mathf.Sin(2*Mathf.PI*frequency*Time.time) - Mathf.Sin(2*Mathf.PI*frequency*(Time.time - Time.deltaTime)))*transform.up;
            StartCoroutine(turnAttack(attackDelay));
		}
		else
			base.Movement();
	}

    protected IEnumerator rotateAndShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Comecei");
        z = transform.rotation.eulerAngles.z;
        if (z > maxRotation)
            rotateStep *= -1;
        transform.Rotate(0, 0, rotateStep);
        bullet = prefab.Spawn();
        Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * Vector3.right / 3f;
        bullet.transform.position = position;
        bullet.direction = Quaternion.Euler(0, 0, z) * Vector2.right;
        bullet.damage = damage;
    }

    protected override void Attack(GameObject obj)
    {
        if (destroy)
        {
            destroy = false;
            StopAllCoroutines();
            z = this.gameObject.transform.rotation.eulerAngles.z;

            //Debug.Log("MaxWalkingTime: " + (double)maxWalkTime + " | Walking Time: " + (double)walkTime + " | Time-walktime: " + (double)(Time.time - walkTime));

            if (Time.time - walkTime > maxWalkTime && walkTime != 0)
            {
                seek = false;
                StartCoroutine(rotateAndShoot(rotationDelay));
            }
            else
            {
                seek = true;
                StopAllCoroutines();
                bullet = prefab.Spawn();
                Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * Vector3.right / 3f;
                bullet.transform.position = position;
                bullet.direction = Quaternion.Euler(0, 0, z) * Vector2.right;
                bullet.damage = damage;
            }
        }
    }
}
