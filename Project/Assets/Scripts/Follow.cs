using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public float speed;
    public Transform target;
    private Vector3 heading, direction;
    private float distance;
    private bool seek;

	// Use this for initialization
	void Start () {
        seek = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (seek)
        {
            heading = target.position - this.gameObject.transform.position;
            distance = heading.magnitude;
            direction = heading / distance;

            this.gameObject.transform.position += direction * speed * Time.deltaTime;

            //if (heading.sqrMagnitude < 1)        
            //print(distance + " | " + direction);

            Debug.DrawLine(this.gameObject.transform.position, target.position);
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            Debug.LogWarning("Matei!");
            Object.Destroy(collision.gameObject);
            seek = false;
        }
    }
}
