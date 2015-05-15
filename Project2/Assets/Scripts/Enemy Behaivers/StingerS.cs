//using UnityEngine;
//using System.Collections;
//
//public class StingerS : BaseEnemy {
//
//	public float amplitude = 0.2f, frequency = 0.85f, bulletSpeed = 2f, maxWalkTime = 0.1f, rotationDelay = 0.1f, rotateStep = 10f, pointDistance = 1f;
//	public int numberofShoots = 3;
//	
//	float walkTime = 0f;
//	int nearestPointIndex;
//	bool startedRotateAndShoot = false;
//	Vector3 _center = Vector3.zero, destinationHeading, destinationDirection;
//	Vector3[] points;
//	ShootMove bullet = null;
//
//	protected void updatePoints()
//	{
//		Vector3 t = new Vector3(-1, 1, 0);
//		points[0] = target.transform.position + (Vector3)Vector2.one * pointDistance;
//		points[1] = target.transform.position + t * pointDistance;
//		points[2] = target.transform.position - (Vector3)Vector2.one * pointDistance;
//		points[3] = target.transform.position - t * pointDistance;
//	}
//	
//	protected void updatePointPosition()
//	{
//		destinationHeading = points[nearestPointIndex] - transform.position;
//		destinationDirection = destinationHeading.normalized;
//	}
//
//	void nearestPoint()
//	{
//		float minDistance = 100f, d;
//		for (int i = 0; i < points.GetLength(0); i++) {
//			d = (transform.position - points [i]).sqrMagnitude;
//			if (d < minDistance)
//			{
//				minDistance = d;
//				nearestPointIndex = i;
//			}
//		}
//	}
//
//	protected IEnumerator rotateAndShoot(float delay)
//	{
//		print("Tiro comum");
//		
//		for (int i = 0; i < numberofShoots; i++)
//		{
//			// Logica do Meio "de totoro", acho que não ficou muito legal...
//			//int signal = (Mathf.Min(direction.x, direction.y) == direction.y) ? (int)(direction.y / Mathf.Abs(direction.y)) : (int)(direction.x / Mathf.Abs(direction.x));
//			//transform.Rotate(0, 0, rotateStep * signal);
//			//yield return new WaitForSeconds(delay);
//			//z = transform.rotation.eulerAngles.z;
//			
//			//Rotacionando o inimigo
//			//rigidbody2D.MoveRotation(rotateStep * (i - numberofShoots / 2));
//			//yield return new WaitForSeconds(delay);
//			//z = transform.rotation.eulerAngles.z;
//			
//			//Rotacionando somente o tiro
//			yield return new WaitForSeconds(delay);
//			//Debug.Log("Time" + i + ": " + (double)Time.time + " | t: " + (double)(i - numberofShoots / 2) + " | z: " + (double)z);
//			bullet = prefab.Spawn();
//			//Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * Vector3.right / 3f;
//			bullet.transform.position = transform.position + direction.normalized * 0.21f;
//			bullet.direction = direction;
//			bullet.damage = damage;
//			bullet.speed = bulletSpeed;
//		}
//		
//		walkTime = 0f;
//		seek = true;
//		StopCoroutine("rotateAndShoot");
//		startedRotateAndShoot = false;
//	}
//
//	protected override void Movement()
//	{
//		_center = GetComponentsInChildren<Transform>()[1].position;
//		//Debug.Log("V: " + rigidbody2D.velocity + " | Vm(2): " + (double)rigidbody2D.velocity.sqrMagnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm(2): " + (double)target.rigidbody2D.velocity.sqrMagnitude + " | D: " + distance);
//		
//		//if (target.rigidbody2D.velocity.sqrMagnitude < 3f && distance <= seekDistance)
//		if (destinationHeading.sqrMagnitude <= 1f && distance <= seekDistance)
//		{
//			if (target.rigidbody2D.velocity.sqrMagnitude < 1f)
//				walkTime = 0f;
//			else if (walkTime == 0f)
//				walkTime = Time.time;
//			
//			rigidbody2D.velocity = Vector2.zero; // Tem que ser zero pra o efeito ficar certo, mas causa problemas ao aplicar uma força (i.e. quando o inimigo toma um tiro)
//			if (seek)
//			{
//				transform.position += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;
//				if (!attacking)
//				{
//					StartCoroutine(turnAttack(attackDelay));
//					attacking = true;
//				}
//			}
//		}
//		else
//			base.Movement();
//	}
//
//	protected void Attack(GameObject obj)
//	{
//		if (destroy)
//		{
//			destroy = false;
//			//Debug.Log("MaxWalkingTime: " + (double)maxWalkTime + " | Walking Time: " + (double)walkTime + " | Time-walktime: " + (double)(Time.time - walkTime) + " | startrotate: " + startedRotateAndShoot + " | attacking: " + attacking);
//			
//			
//			if (Time.time - walkTime > maxWalkTime && walkTime != 0 && !startedRotateAndShoot)
//			{
//				//print("Tiro incomum");
//				GetComponent<Animator> ().SetBool("Shooting",true);
//				seek = false;
//				attacking = false;
//				startedRotateAndShoot = true;
//				walkTime = 0f;
//				StartCoroutine(rotateAndShoot(attackDelay / speed));
//			}
//			else if (!startedRotateAndShoot && attacking)
//			{
//				//print("Tiro comum");
//				GetComponent<Animator> ().SetBool("Shooting",true);
//				seek = true;
//				attacking = false;
//				bullet = prefab.Spawn();
//				Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * Vector3.right / 3f;
//				bullet.transform.position = transform.position + direction.normalized * 0.21f;
//				bullet.direction = direction;
//				bullet.damage = damage;
//				bullet.speed = bulletSpeed;
//			}
//		}
//	}
//}
