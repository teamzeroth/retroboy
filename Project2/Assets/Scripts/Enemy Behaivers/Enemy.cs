using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float life = 100f, damage = 2f, speed = 1f;
    public GameObject target = null;
    protected Vector3 heading = Vector3.zero, direction = Vector3.zero;
    protected float distance = 0f;
    protected bool seek = false, destroy = false;
    protected Rigidbody2D body = null;  

    protected virtual void Start()
    {
        seek = true;
        body = this.gameObject.GetComponent<Rigidbody2D>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (life <= 0)
        {
            GameObject.Destroy(this.gameObject);
            Camera.main.GetComponent<Director>().increaseScore();
        }
        else
        {
            UpdatePosition();
            Movement();
            Defense();
            Attack(target);
        }
    }

    protected void UpdatePosition()
    {
        heading = target.transform.position - this.gameObject.transform.position;
        distance = heading.magnitude;
        direction = heading / distance;	  
    }

    protected virtual void Movement()
    {
        if (seek)
        {
            body.velocity = new Vector2(direction.x * speed, direction.y * speed);
            if (Debug.isDebugBuild)
            {
                //Debug.Log("Direction: " + direction + " | Velocity: " + body.velocity);
                Debug.DrawLine(this.gameObject.transform.position, target.transform.position);
            }
        }
    }

    protected virtual void Defense() { }

    protected virtual void Attack(GameObject obj) 
    {
        if (obj != null && destroy)
        {
            if (Debug.isDebugBuild)
                Debug.Log(this.gameObject.name + " attacks " + obj.name + " for " + this.damage + " damage points");

            obj.GetComponent<AnimationController>().Hit(this.damage, this.direction);            
        }
    }

    IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        seek = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            destroy = true;
            target = collision.gameObject;
        }
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name.Contains("shoot"))
        {
            Vector3 h = trigger.transform.position - this.gameObject.transform.position;
            Vector2 d = h / h.magnitude;
            print(d);

            if (d.x < 0 || d.y < 0)
            {
                this.life -= trigger.gameObject.GetComponent<ShootMove>().damage;
                Object.Destroy(trigger.gameObject);
                if (Debug.isDebugBuild)
                    Debug.Log("Enemy Life: " + this.life);
                body.AddForce(new Vector2(direction.x * -1f, direction.y * -1f), ForceMode2D.Impulse);
                seek = false;
                this.gameObject.GetComponent<Animator>().enabled = false;
                StartCoroutine("Wait", 0.5f);
                this.gameObject.GetComponent<Animator>().enabled = true;
            }
            else
                print("meu tiro");
        }
        else if (trigger.gameObject.tag == "Player")
        {
            destroy = true;
            target = trigger.gameObject;
        }
    }    
}
