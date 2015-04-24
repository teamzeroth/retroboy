using UnityEngine;
using System.Collections;

public class ShootMove : MonoBehaviour {

    public Vector2 direction;

    public float speed;
    public float damage;

    public bool isAlly;

    [HideInInspector]
    public bool flipped;

    void Start() {
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    void Update() {
        rigidbody2D.velocity = direction.normalized * speed;
        //rigidbody2D.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    void OnTriggerEnter2D(Collider2D other) {}

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }


    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
