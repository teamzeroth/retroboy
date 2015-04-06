using UnityEngine;
using System.Collections;

public class MachineDog : Enemy {

    string state = null;
    float movementDirection = 0f;
    Vector3 localScale = Vector3.zero;

	// Use this for initialization
    protected override void Start () {
        base.Start();
        movementDirection = 1f;
        localScale = this.gameObject.transform.localScale;
	}
    		
    protected override void Movement()
    {
        if (seek)
        {
            body.velocity = new Vector2(movementDirection, 0);
            seek = false;
            movementDirection *= -1f;
            this.gameObject.transform.localScale = localScale;
            StartCoroutine("Wait", 1.5f);
            localScale.x *= -1;
        }
    }

    protected override void Attack(GameObject obj)
    {
        if (obj != null && destroy)
        {
            //GameObject shoot = Resources.Load<GameObject>("Shoots/Nim/shoot_1");
            //Vector3 position = transform.position + new Vector3(body.velocity.x, body.velocity.y, 0f);

            //shoot = (GameObject)Instantiate(shoot, position, Quaternion.identity);
            
            //ShootMove move = shoot.GetComponent<ShootMove>();
            //move.direction = body.velocity;
            //destroy = false;
            //StartCoroutine("Wait", 1f);
        }
    }
}
