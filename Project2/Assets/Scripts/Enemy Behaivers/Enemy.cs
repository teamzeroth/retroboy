using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f, attackDelay = 0.25f;
    public GameObject target = null;
    public ShootMove prefab = null;

    protected Vector3 heading = Vector3.zero, direction = Vector3.zero, futureDirection = Vector3.zero, futureHeading = Vector3.zero;
    protected float distance = 0f;
    protected bool seek = false, destroy = false;

    public void Start()
    {
        destroy = false;
        prefab.CreatePool();

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

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

    protected virtual void Movement()
    {
        //Debug.Log("V: " + rigidbody2D.velocity + " | Vm: " + (double)rigidbody2D.velocity.magnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm: " + (double)target.rigidbody2D.velocity.magnitude + " | D: " + distance);
        
        if (distance > 1.5f)
            //Movimento de follow funcional (não se antecipa tanto a voce)
            //rigidbody2D.velocity = (futureDirection * (speed + distance/1.5f)) + (Vector3)target.rigidbody2D.velocity.normalized;
            
            rigidbody2D.velocity = futureDirection + (Vector3)target.rigidbody2D.velocity.normalized * (speed * target.rigidbody2D.velocity.magnitude * distance) / 1.1f;
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
