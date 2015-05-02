using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f, shootDelay = 0.25f;
    public GameObject target = null;
    public ShootMove prefab = null;

    protected Vector3 heading = Vector3.zero, direction = Vector3.zero, futureDirection = Vector3.zero, futureHeading = Vector3.zero;
    protected float distance = 0f;
    protected bool seek = false, destroy = false, melee = false;

    protected virtual void Start()
    {
        seek = true;
        prefab.CreatePool();
    }

    void Update()
    {
		Debug.DrawLine(transform.position, target.transform.position, Color.red);
		Debug.DrawLine(transform.position, transform.position + direction, Color.blue);
		Debug.DrawLine(transform.position, target.transform.position + (Vector3)target.rigidbody2D.velocity, Color.green);
		Debug.DrawLine(transform.position, transform.position + futureDirection, Color.yellow);

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
            Attack(null);
        }
    }

	protected void updateFuturePosition()
	{
		futureHeading = (target.transform.position + (Vector3)target.rigidbody2D.velocity.normalized) - transform.position;
		//futureHeading = (target.transform.position + (Vector3)target.rigidbody2D.velocity) - transform.position;
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
//		Debug.Log("Atan2: " + (double)Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
		rigidbody2D.MoveRotation(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
	}

    protected virtual void Movement()
    {

	    //rigidbody2D.velocity = new Vector2(direction.x * speed, direction.y * speed);
        rigidbody2D.velocity = (Vector2)futureDirection * speed / 3f;
		if (target.rigidbody2D.velocity.sqrMagnitude > 0.2f)
//		rigidbody2D.velocity += ((Vector2)futureDirection + target.rigidbody2D.velocity.normalized).normalized;
			rigidbody2D.velocity = (futureDirection * speed) + ((Vector3)target.rigidbody2D.velocity.normalized * distance);
		else if (rigidbody2D.velocity.sqrMagnitude < 0.2f)
			rigidbody2D.velocity = ((futureDirection * speed) + (Vector3)rigidbody2D.velocity) * distance / 3f;
    }

    protected virtual void Defense() { }

    protected virtual void Attack(GameObject obj) 
    {
		if (obj != null)
		{
	        if (Debug.isDebugBuild)
	            Debug.Log(name + " attacks " + obj.name + " for " + this.damage + " damage points");

	        obj.GetComponent<AnimationController>().Hit(this.damage, this.direction);
		}
    }

    protected IEnumerator changeSeek(float delay)
    {
        yield return new WaitForSeconds(delay);
        seek = !seek;
    }

    protected IEnumerator changeDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        destroy = !destroy;
    }

    protected IEnumerator changeAnimator(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().enabled = !GetComponent<Animator>().enabled;
    }

//    void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.tag == "Player")
//        {
//            destroy = true;
//            melee = true;
//            target = collision.gameObject;
//        }
//    }

    public void onTriggerExternal(GameObject sensor, Collider2D trigger)
    {
//        //print(sensor.name + " | " + trigger.name + " | " + sensor.gameObject.GetComponent<Collider2D>().collider2D + " | " + trigger.collider2D);
//        if (sensor == this.gameObject && trigger.gameObject.name.Contains("shoot"))
//        //Colizão com object tiro (Hit)
//        {
//            this.life -= trigger.gameObject.GetComponent<ShootMove>().damage;
//            Object.Destroy(trigger.gameObject);
//            if (Debug.isDebugBuild)
//                Debug.Log("Enemy Life: " + this.life);
//            rigidbody2D.AddForce(new Vector2(direction.x * -1f, direction.y * -1f), ForceMode2D.Impulse);
//            seek = false;
//            GetComponent<Animator>().enabled = false;
//            StartCoroutine("changeSeek", 0.5f);
//            StartCoroutine("changeAnimator", 0.5f);
//        }
//        else if (sensor.name.Contains("Detection") && trigger.gameObject.tag == "Player")
//        //Colizão com o object DetectionRange (Found)
//        {
//            destroy = true;
//            seek = false;
//            StopAllCoroutines();
//            rigidbody2D.velocity = Vector2.zero;
//            target = trigger.gameObject;
//        }
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
		if (trigger.gameObject.tag == "Player")
		{
			destroy = true;
			melee = true;
			Attack(target);
		}
    }
    
    void OnTriggerExit2D(Collider2D trigger)
    {
//        if (trigger.tag == "Player")
//        {
//            destroy = false;
//            seek = true;
//        }
    }
}
