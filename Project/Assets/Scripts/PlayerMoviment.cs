using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoviment : MonoBehaviour {

    public float speed = 10f;
    public float multiplier = 10f;

    Animator anim;
    Vector2 lastMoveVector;

    bool colliding = false;

	void Start(){
        anim = GetComponent<Animator>();
    }

    float ttw = 0;
    void FixedUpdate() {
        if (ttw > -.4f) {
            ttw -= Time.deltaTime;
            
            if (ttw <= 0 && anim.GetBool("OnSideStep")) 
                FinishSideStep();
            //else
            return;
        }

        Vector2 /*moveVector*/ mv = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (Input.GetButtonDown("Fire1")) {         /// Get Fire of gun
            
        } else if (Input.GetButtonDown("Jump")) {   /// Active side step
            lastMoveVector = mv;
            ttw = 0.7f;

            rigidbody2D.AddForce(mv.normalized * speed * multiplier);
            anim.SetBool("OnSideStep", true);

        } else {                                    /// Run normal walking
            mv = Mathf.Abs(mv.x) == 1 || Mathf.Abs(mv.y) == 1 ? mv.normalized : mv;

            Vector3 deltaMv = mv * speed;
            rigidbody2D.velocity = deltaMv;
        }
    }

    void Update() {
        if (ttw > -.4f) {
            //anim.SetFloat("Horizontal", 0);
            //anim.SetFloat("Vertical", 0);
            return;
        }
        Animation();
    }

    void FinishSideStep() {
        print(lastMoveVector);

        anim.SetBool("OnSideStep", false);
        anim.SetFloat("Horizontal", lastMoveVector.x);
        anim.SetFloat("Vertical", lastMoveVector.y);
    }

    void Animation() {

        var mainAxis = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical"));
        float vx = rigidbody2D.velocity.x / speed;
        float vy = rigidbody2D.velocity.y / speed;

        //print(vx + ", " + vy);

        float animSpeed = 0.9f * Mathf.Max(Mathf.Abs(vx), Mathf.Abs(vy)) + 0.1f;

        anim.SetFloat("Horizontal", vx < -0.01f || vx > 0.01f ? Input.GetAxis("Horizontal") : 0);
        anim.SetFloat("Vertical", vy < -0.01f || vy > 0.01f ? Input.GetAxis("Vertical") : 0);

        anim.SetBool("MainAxis", mainAxis);
        anim.speed = colliding ? animSpeed : 1;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        colliding = true;
    }

    void OnCollisionExit2D(Collision2D coll) {
        colliding = false;
    }

    //////////////////

    int getMainEixo(float h, float v) {
        float rad = Mathf.Atan2(v, h);

        var dir = Mathf.Round(rad / (Mathf.PI / 4));
		return 0;
    }
}
