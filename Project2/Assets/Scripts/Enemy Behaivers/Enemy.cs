using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f, attackDelay = 0.25f, pointDistance = 1f;
    public GameObject target = null;
    public ShootMove prefab = null;

    protected Vector3 heading = Vector3.zero, direction = Vector3.zero, futureDirection = Vector3.zero, futureHeading = Vector3.zero;
    protected float distance = 0f;
    protected bool seek = false, destroy = false;
	protected Vector2[] points;

    public void Start()
    {
        destroy = false;
		points = new Vector2[4];
        prefab.CreatePool();

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");
    }

	void UpdatePoints()
	{
		Vector2 t = {-1,1};
		points [0] = target.transform.position + Vector2.one;
		points [1] = target.transform.position + t;
		points [2] = target.transform.position - Vector2.one;
		points [3] = target.transform.position - t;
	}

    void Update()
    {
        //Debug.DrawLine(transform.position, target.transform.position, Color.red);
        //Debug.DrawLine(transform.position, transform.position + direction, Color.blue);
        //Debug.DrawLine(transform.position, target.transform.position + (Vector3)target.rigidbody2D.velocity, Color.green);
        //Debug.DrawLine(transform.position, transform.position + futureDirection, Color.yellow);

		if (life <= 0)
        {
            GameObject.Destroy(this.gameObject);
            Camera.main.GetComponent<Director>().increaseScore();
            StopAllCoroutines();
        }
        else
        {
			updatePosition();
			updateFuturePosition();
			Movement();
            Defense();
            Attack(target);
        }
    }

	protected void updateTargetPosition(Vector2 p)
	{
	}

	protected void updateFuturePosition()
	{
		futureHeading = (target.transform.position + (Vector3)target.rigidbody2D.velocity.normalized) - transform.position;
		futureDirection = futureHeading.normalized;
	}

    protected void updatePosition()
    {
        heading = target.transform.position - transform.position;
        distance = heading.magnitude;
        direction = heading.normalized;
    }

	void FixedUpdate()
	{
		rigidbody2D.MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
	}

	Vector2 nearestPoint()
	{
		float minDistance = 100f, d;
		int j = 0;
		for (int i = 0; i < points.GetLength; i++) {
			d = transform.position - points [i].sqrMagnitude;
			if (d < minDistance)
			{
				minDistance = d;
				j = i;
			}
		}
		return points [j];
	}

    protected virtual void Movement()
    {
        Debug.Log("V: " + rigidbody2D.velocity + " | Vm: " + (double)rigidbody2D.velocity.magnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm: " + (double)target.rigidbody2D.velocity.magnitude + " | D: " + distance);
        
		if (distance < 3f) // se aproximar
		{
			updateFuturePosition(nearestPoint());
			if (futureDirection.magnitude > 0.2f) // se parado
				rigidbody2D.velocity = futureDirection * speed;
		}
		else // se afastar
		{
			updateFuturePosition(nearestPoint());
			// Delay para procurar o inimigo dnovo
		}


            //Movimento de follow funcional (não se antecipa tanto a voce)
            //rigidbody2D.velocity = (futureDirection * (speed + distance/1.5f)) + (Vector3)target.rigidbody2D.velocity.normalized;
//			rigidbody2D.velocity = futureDirection + (Vector3)target.rigidbody2D.velocity.normalized * (speed * target.rigidbody2D.velocity.magnitude * distance) / 1.1f;
//			rigidbody2D.velocity = futureDirection + (Vector3)target.rigidbody2D.velocity.normalized * (speed * target.rigidbody2D.velocity.magnitude * futureHeading.magnitude) / 1.1f;
//			rigidbody2D.velocity = futureDirection + (Vector3)target.rigidbody2D.velocity * ((speed + futureHeading.magnitude) * distance / 1.1f);
			//Cerca o player com uma rotação em circulo ao redor do player (cerca a "saida" do player)
			//Debug.DrawLine(transform.position, target.rigidbody2D.velocity.normalized + (Vector2)transform.up * futureHeading.magnitude, Color.magenta);
    }

    protected virtual void Defense() { }

    protected virtual void Attack(GameObject obj)
    {
		if (destroy)
		{
            destroy = false;
            StopAllCoroutines();
            obj.GetComponent<AnimationController>().Hit(this.damage, this.direction);

            if (Debug.isDebugBuild)
	            Debug.Log(name + " attacks " + obj.name + " for " + this.damage + " damage points");
		}
    }
    
    protected IEnumerator turnAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        destroy = true;
    }

    protected IEnumerator turnAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
		if (trigger.gameObject.tag == "Player")
		    StartCoroutine(turnAttack(attackDelay));
    }

    public void Hit(float damage, Vector2 direction)
    {
        StopAllCoroutines(); 
        life -= damage;
        if (Debug.isDebugBuild)
            Debug.Log("Enemy Life: " + life);
        rigidbody2D.AddForce(direction * -2f, ForceMode2D.Impulse);
        GetComponent<Animator>().enabled = false;
        destroy = false;
        StartCoroutine(turnAnimation(0.5f));
        StartCoroutine(turnAttack(attackDelay));
    }
}
