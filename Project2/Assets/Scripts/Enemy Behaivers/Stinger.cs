 using UnityEngine;
using System.Collections;

public class Stinger : Enemy
{
	public float amplitude = 0.2f, frequency = 0.85f;
	float z = 0f;
    ShootMove bullet = null;

    protected override void Movement()
    {
        //Debug.Log("V: " + rigidbody2D.velocity + " | Vm(2): " + (double)rigidbody2D.velocity.sqrMagnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm(2): " + (double)target.rigidbody2D.velocity.sqrMagnitude + " | D: " + distance);

        if (target.rigidbody2D.velocity.sqrMagnitude < 3f && distance <= 1.5f)
		{
			rigidbody2D.velocity = Vector2.zero; // Tem que ser zero pra o efeito ficar certo, mas causa problemas ao aplicar uma força (i.e. quando o inimigo toma um tiro)
            //Debug.DrawLine(transform.position, transform.position + transform.up, Color.cyan);
			transform.position += amplitude*(Mathf.Sin(2*Mathf.PI*frequency*Time.time) - Mathf.Sin(2*Mathf.PI*frequency*(Time.time - Time.deltaTime)))*transform.up;
            StartCoroutine(turnAttack(attackDelay));
		}
		else
			base.Movement();
	}

    protected override void Attack(GameObject obj)
    {
        if (false)
        {
            destroy = false;
            StopAllCoroutines();
            z = this.gameObject.transform.rotation.eulerAngles.z;
            bullet = prefab.Spawn();
            Vector3 position = transform.position + Quaternion.Euler(0, 0, z) * Vector3.right/3f;
            bullet.transform.position = position;
            bullet.direction = Quaternion.Euler(0, 0, z) * Vector2.right;
            bullet.damage = damage;
        }
    }
}
