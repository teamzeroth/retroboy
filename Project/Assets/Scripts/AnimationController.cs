using UnityEngine;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour {

    public float speed = 10f;
    public float multiplier = 10f;

    Animator anim;

    /* Getters And Setters
     * ********************/

    public bool onAnyMoveButton {
        get{
            //if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
               //return true;

            if ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.1f)
                return true;
            
            return false;
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
        FlipAnimation(moveVec);
    }

    public void Move(Vector2 moveVec) {
        moveVec = Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 1 ? moveVec.normalized : moveVec;

        if (onAnyMoveButton){
            float mainSpeed = Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y) ? Mathf.Abs(moveVec.x) : Mathf.Abs(moveVec.y);
            anim.speed = 0.5f + mainSpeed / 2;
        }else
            anim.speed = 1;

        Vector3 deltaMv = moveVec * speed;
        rigidbody2D.velocity = deltaMv;
    }

    public void Animation(float mainAxis, Vector2 moveVec) {
        lastMainAxis = mainAxis;

        /// If the player just press any direction button and release fast the estate dont will change to Run Blend
        //bool tapCheck = (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y)) > 0.1f;
        anim.SetBool("OnMoving", onAnyMoveButton);

        anim.SetFloat("MainAxis", mainAxis);
        anim.SetFloat("Horizontal", moveVec.x);
        anim.SetFloat("Vertical", moveVec.y);
    }

    public void FlipAnimation(Vector2 moveVec) {
        if (onAnyMoveButton)
            if(Mathf.Abs(moveVec.x) >= Mathf.Abs(moveVec.y) && moveVec.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
    }
}