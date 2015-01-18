using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoviment : MonoBehaviour {

    Animator anim;
    bool colliding = false;
    
    public float speed = 10f;

	void Start(){
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        Vector2 /*moveVector*/ mv = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector3 deltaMv = mv * speed;
        rigidbody2D.velocity = deltaMv;
        Animation();
    }

    void Update() {
        Animation();
    }

    void Animation() {

        var mainAxis = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical"));
        float vx = rigidbody2D.velocity.x / speed;
        float vy = rigidbody2D.velocity.y / speed;


        print(vx + ", " + vy);

        float animSpeed = 0.9f * Mathf.Max(Mathf.Abs(vx), Mathf.Abs(vy)) + 0.1f;

        anim.SetFloat("Horizontal", vx < -0.01f || vx > 0.01f ? Input.GetAxis("Horizontal") : 0);
        anim.SetFloat("Vertical", vy != 0.0f ? Input.GetAxis("Vertical") : 0);

        anim.SetBool("MainAxis", mainAxis);
        anim.speed = colliding ? animSpeed : 1;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        print("Enter");
        colliding = true;
    }

    void OnCollisionExit2D(Collision2D coll) {
        print("Out");
        colliding = false;
    }
}
