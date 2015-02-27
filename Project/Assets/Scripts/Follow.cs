using UnityEngine;
using System.Collections;

public class Follow : Enemy {

    private bool seek;

	// Use this for initialization
	void Start () {
        seek = true;
        target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (seek)
        {
            UpdatePosition();
            Moviment();
        }
	}
    
    void Moviment()
    {
        this.gameObject.transform.position += direction * speed * Time.deltaTime;
        Debug.DrawLine(this.gameObject.transform.position, target.position);
    }

    void Attack(GameObject target)
    {
        print(target.gameObject.name);
        if (target.gameObject.tag == "Player")
        {
            Debug.LogWarning("Matei!");
            //Object.Destroy(GameObject.Find("Beta (SideKick)"));
            Object.Destroy(target.gameObject);
            seek = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Attack(collision.gameObject);       
    }
}
