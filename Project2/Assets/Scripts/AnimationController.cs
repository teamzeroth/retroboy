using UnityEngine;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour {

    public float speed = 10f;
    public float multiplier = 10f;

    [HideInInspector]
    public bool flipped = false;

    Animator anim;

    Vector2 deadMoveVec;

    static private float REPEAT_FIRE_TIME = 0.3f; 
    float fireTime = 0;

    /* Getters And Setters
     * ********************/

    public string currentAnimState { get { return anim.GetCurrentAnimationClipState(0)[0].clip.name; } }
    public bool CanMove {
        get{
            if ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.2f)
                return true;

            return false;
        }
    }

    public bool OnMoving {
        get {
            if (onShoot || currentAnimState.StartsWith("shoot") || currentAnimState.StartsWith("draw"))
                return false;

            return CanMove;
        }

    }

    float onShootTime = 0;
    public bool onShoot { get { return Input.GetButton("Fire1"); } }

    /* ******************* */

    void Start(){
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        if (fireTime > 0) fireTime -= Time.deltaTime;
    }

    void Update() {
        Vector2 moveVec = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        calcDeadMove();

        Move(moveVec);
        Animation(moveVec);

        onShootTime = onShoot ? onShootTime + Time.deltaTime : 0;

        if (CanMove) {
            if(Mathf.Abs(moveVec.x) >= Mathf.Abs(moveVec.y) * 0.42f){
                if(!flipped && moveVec.x < 0) Flip();
                if(flipped && moveVec.x > 0) Flip();
            }else{
                if (flipped) Flip();
            }
        }

        if (Input.GetButtonUp("Fire1") && fireTime <= 0) {
            fireTime = REPEAT_FIRE_TIME;
            Vector2 dir = CanMove ? moveVec : deadMoveVec;

            GameObject shoot = Resources.Load<GameObject>("Shoots/Nim/shoot_1");
            Vector3 position = transform.position + (Vector3)(dir.normalized * 0.9f);

            shoot = (GameObject) Instantiate(shoot, position, Quaternion.identity);

            ShootMove move = shoot.GetComponent<ShootMove>();
            move.direction = dir;
        }
    }

    void calcDeadMove() {
        if (CanMove)
            deadMoveVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void Move(Vector2 moveVec) {
        if (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 1) {
            moveVec.Normalize();
        }

        //moveVec = Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 1 ? moveVec.normalized : moveVec;

        if (OnMoving) {
            float mainSpeed = Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y) ? Mathf.Abs(moveVec.x) : Mathf.Abs(moveVec.y);
            anim.speed = 0.5f + mainSpeed / 2;
        }else
            anim.speed = 1;

        Vector3 deltaMv = OnMoving ? (Vector3)moveVec * speed : Vector3.zero;
        rigidbody2D.velocity = deltaMv;
    }

    public void Animation(Vector2 moveVec) {
        anim.SetBool("OnMoving", OnMoving);
        anim.SetBool("OnShoot", onShoot);
        anim.SetFloat("OnShootTime", onShootTime);

        if (CanMove) {
            anim.SetFloat("Horizontal", moveVec.x);
            anim.SetFloat("Vertical", moveVec.y);
        }else{
            anim.SetFloat("Horizontal", deadMoveVec.x);
            anim.SetFloat("Vertical", deadMoveVec.y);
        }
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}