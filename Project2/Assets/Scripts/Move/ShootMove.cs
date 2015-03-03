using UnityEngine;
using System.Collections;

public class ShootMove : MonoBehaviour {

    public Vector2 direction;

    public float speed;
    public float damage;

    public bool isAlly;

    void Update() {
        rigidbody2D.velocity = direction.normalized * speed;
        //rigidbody2D.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    void OnTriggerEnter2D(Collider2D other) {}
}
