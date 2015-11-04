using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f, attackDelay = 0.25f, pointDistance = 1f, seekDistance = 2f;
    public GameObject target = null;
    public ShootMove prefab = null;

    protected int nearestPointIndex = -1;
    protected Vector3 heading = Vector3.zero, direction = Vector3.zero, futureDirection = Vector3.zero, futureHeading = Vector3.zero, destinationHeading = Vector3.zero, destinationDirection;
    protected float distance = 0f;
    protected bool seek = false, destroy = false, pointSet = false, attacking = false;
	protected Vector3[] points;

    public void Start() {
        destroy = false;
        seek = true;
        points = new Vector3[4];
        prefab.CreatePool();

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

        updatePoints();
    }

	void Update()
    {
        Debug.DrawLine(transform.position, target.transform.position, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + heading, Color.red);
        Debug.DrawLine(transform.position, transform.position + direction, Color.magenta);
        Debug.DrawLine(transform.position, transform.position + destinationDirection, Color.blue);
        Debug.DrawLine(transform.position, transform.position + destinationHeading, Color.cyan);
        
        //Debug.DrawLine(transform.position, target.transform.position + (Vector3)target.rigidbody2D.velocity, Color.green);
        //Debug.DrawLine(transform.position, transform.position + futureDirection, Color.yellow);

		if (life <= 0)
        {
            GameObject.Destroy(this.gameObject);
//            Camera.main.GetComponent<Director>().increaseScore();
            StopAllCoroutines();
        }
        else
        {
			updatePosition();
            if (nearestPointIndex == -1)
                nearestPoint();
            updatePointPosition();
			Movement();
            Defense();
            Attack(target);
        }
    }

    protected void updatePoints()
    {
        Vector3 t = new Vector3(-1, 1, 0);
        points[0] = target.transform.position + (Vector3)Vector2.one * pointDistance;
        points[1] = target.transform.position + t * pointDistance;
        points[2] = target.transform.position - (Vector3)Vector2.one * pointDistance;
        points[3] = target.transform.position - t * pointDistance;
    }

	protected void updatePointPosition()
	{
        destinationHeading = points[nearestPointIndex] - transform.position;
        destinationDirection = destinationHeading.normalized;
	}
    
    protected void updatePosition()
    {
        heading = target.transform.position - transform.position;
        distance = heading.magnitude;
        direction = heading.normalized;
		GetComponent<Animator> ().SetFloat ("Vertical", direction.y);
		GetComponent<Animator> ().SetFloat ("Horizontal", -direction.x);
    }

	void FixedUpdate()
	{
//        if (seek)
		  //  rigidbody2D.MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
	}

	void nearestPoint()
	{
		float minDistance = 100f, d;
		for (int i = 0; i < points.GetLength(0); i++) {
			d = (transform.position - points [i]).sqrMagnitude;
			if (d < minDistance)
			{
				minDistance = d;
				nearestPointIndex = i;
			}
		}
	}

    protected virtual void Movement()
    {
        //Debug.Log("V: " + rigidbody2D.velocity + " | Vm: " + (double)rigidbody2D.velocity.magnitude + " | TV: " + target.rigidbody2D.velocity + " | TVm: " + (double)target.rigidbody2D.velocity.sqrMagnitude + " | TD: " + distance + " | Dest: " + destinationHeading + " | DestM: " + destinationHeading.sqrMagnitude);
        
        if (distance > 5f)
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        else if (pointSet) 
		{
            if (destinationHeading.magnitude > 0.2f)
//                                                                    Desacelera quanto mais perto do ponto se estiver
                GetComponent<Rigidbody2D>().velocity = destinationDirection * speed * destinationHeading.magnitude / pointDistance;
            else if (heading.magnitude > seekDistance) // se aproximar
                pointSet = false;
            else // se parado
                StartCoroutine(turnAttack(attackDelay));
        }
		else // se afastar
        {
            updatePoints();
            nearestPointIndex = -1;
            pointSet = true;
            destroy = false;
		}
    }

    protected virtual void Defense() { }

    protected virtual void Attack(GameObject obj)
    {
		if (destroy)
		{
            destroy = false;
            //StopAllCoroutines();
            obj.GetComponent<AnimationController>().Hit(this.damage, this.direction);

            if (Debug.isDebugBuild)
	            Debug.Log(name + " attacks " + obj.name + " for " + this.damage + " damage points");
		}
    }
    
    protected IEnumerator turnAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        destroy = true;
        StopCoroutine("turnAttack");
    }

    protected IEnumerator turnAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().enabled = true;
        StopCoroutine("turnAnimation");
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
            //Debug.Log("Enemy Life: " + life);
        GetComponent<Rigidbody2D>().AddForce(direction * -2f, ForceMode2D.Impulse);
        destroy = false;
        StartCoroutine(turnAttack(attackDelay));
        GetComponent<Animator>().SetBool("Shooting", false);
        GetComponent<Animator>().enabled = false;
        StartCoroutine(turnAnimation(attackDelay));
    }
}
