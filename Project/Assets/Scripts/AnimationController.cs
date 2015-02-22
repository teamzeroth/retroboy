using UnityEngine;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour {

    public float speed = 10f;
    public float multiplier = 10f;

    [HideInInspector]
    public bool flipped = false;

    Animator anim;
    float deadVertical;
    float deadHorizontal = 1;

    /* Getters And Setters
     * ********************/

    public bool onAnyMoveButton {
        get{
            //if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
               //return true;

            if ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.3f) {
                deadVertical = Input.GetAxis("Vertical");
                deadHorizontal = Input.GetAxis("Horizontal");
                return true;
            }
           
            return false;
        }
    }

    public bool onShoot {
        get {
            return Input.GetButton("Fire1");
        }
    }

    /* ******************* */

    void Start(){
        anim = GetComponent<Animator>();
    }

    float lastMainAxis;

    void FixedUpdate() {
        float mainAxis = lastMainAxis;

        Vector2 moveVec = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (onAnyMoveButton && (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y)) > 0.05f) {
            if (Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y))
                mainAxis = 0.1665f + 0.1665f * moveVec.x;
            else 
                mainAxis = 0.8325f + 0.1665f * moveVec.y;
        }

        Move(moveVec);
        Animation(mainAxis, moveVec);

        if(onAnyMoveButton){
            if(Mathf.Abs(moveVec.x) >= Mathf.Abs(moveVec.y) * 0.42f){
                if(!flipped && moveVec.x < 0) Flip();
                if(flipped && moveVec.x > 0) Flip();
            }else{
                if (flipped) Flip();
            }
        }
    }

    public void Move(Vector2 moveVec) {
        moveVec = Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 1 ? moveVec.normalized : moveVec;

        if (onAnyMoveButton){
            float mainSpeed = Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y) ? Mathf.Abs(moveVec.x) : Mathf.Abs(moveVec.y);
            anim.speed = 0.5f + mainSpeed / 2;
        }else
            anim.speed = 1;

        Vector3 deltaMv;
        if (!onShoot)
            deltaMv = moveVec * speed;
        else
            deltaMv = Vector3.zero;

        rigidbody2D.velocity = deltaMv;
    }

    public void Animation(float mainAxis, Vector2 moveVec) {
        lastMainAxis = mainAxis;

        /// If the player just press any direction button and release fast the estate dont will change to Run Blend
        //bool tapCheck = (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y)) > 0.1f;
        anim.SetBool("OnMoving", onAnyMoveButton);
        anim.SetBool("OnShoot", onShoot);

        anim.SetFloat("MainAxis", mainAxis);

        if (!onShoot){
            anim.SetFloat("Horizontal", moveVec.x);
            anim.SetFloat("Vertical", moveVec.y);
        }else{
            anim.SetFloat("Horizontal", deadHorizontal);
            anim.SetFloat("Vertical", deadVertical);
        }
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}