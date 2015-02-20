using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

    public enum Behaviours {Attack, Follow, Defend, Idle};
    public float life = 100f, damage = 2f, speed = 1f, minDistance = 3f, maxDistance = 5f;
    public Behaviours behaviour = Behaviours.Idle;
    [HideInInspector]
    public Transform target = null;
    private Vector3 heading = Vector3.zero, direction = Vector3.zero;
    private float distance = 0f;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        heading = target.position - this.gameObject.transform.position;
        distance = heading.magnitude;
        direction = heading / distance;

        print("Distance: " + distance);

	    switch (behaviour)
        {
            case Behaviours.Follow:
                                    this.gameObject.transform.position += direction * speed * Time.deltaTime;
                                    Debug.DrawLine(this.gameObject.transform.position, target.position);
                                    break;
            case Behaviours.Attack:
                                    if (distance > maxDistance)
                                    {
                                        this.gameObject.transform.position += direction * speed * Time.deltaTime;
                                        Debug.DrawLine(this.gameObject.transform.position, target.position);
                                        Debug.LogWarning("Charge!!!");
                                    }
                                    break;
            case Behaviours.Defend:
                                    if (distance < minDistance)
                                    {
                                        this.gameObject.transform.position -= direction * speed * Time.deltaTime;
                                        Debug.DrawLine(this.gameObject.transform.position, target.position);
                                        Debug.LogWarning("Afraid!");
                                    }
                                    break;
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            Debug.LogWarning("Matei!!");
            Object.Destroy(collision.gameObject);
            behaviour = Behaviours.Idle;
        }
    }
}
