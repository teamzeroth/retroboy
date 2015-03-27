using UnityEngine;
using System.Collections.Generic;

using Helper;

public class AnimationController : MonoBehaviour {

    public float speed = 10f;
    public float multiplier = 10f;
    public float life = 10f;
    public GameObject ui = null;

    [HideInInspector]
    public bool flipped = false;

    Animator _anim;
    Vector2 deadMoveVec = new Vector2(1, 0);

    static private float REPEAT_FIRE_TIME = 0.5f;
    static private float DELIVERY_FIRE_TIME = 0.4f;

    float fireTime = 0;
    bool firstShoot = true; 

    #region Getters And Setters
    
    public bool CanMove {get {
        return !(OnDraw || OnCharge || OnShoot || fireTime > 0.3f || _anim.CurrentAnimState().StartsWith("shoot"));
    }}

    public bool ForcingMove {get{
        return (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.2f;
    }}

    public bool NormalState {get{
        return _anim.CurrentAnimState().StartsWith("idle") || _anim.CurrentAnimState().StartsWith("walk");
    }}

    public bool OnMoving { get { return CanMove && ForcingMove; } }

    public bool OnDraw { get { return Input.GetButtonDown("Fire1") && !_anim.CurrentAnimState().StartsWith("draw"); } }
    public bool OnCharge { get { return Input.GetButton("Fire1"); } }
    public bool OnShoot { get { return Input.GetButtonUp("Fire1"); } }

    #endregion

    void Start(){
        _anim = GetComponent<Animator>();
        if (ui == null) ui = GameObject.Find("Menu");
    }

    void FixedUpdate() {
        fireTime = fireTime > -0.1f ? fireTime - Time.deltaTime : -0.1f;
        if (NormalState) firstShoot = true;
    }

    void Update() {
        Vector2 moveVec = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        calcDeadMove();

        Move(moveVec);
        Animation(moveVec);

        if (ForcingMove) {
            if(Mathf.Abs(moveVec.x) >= Mathf.Abs(moveVec.y) * 0.42f){
                if(!flipped && moveVec.x < 0) Flip();
                if(flipped && moveVec.x > 0) Flip();
            }else{
                if (flipped) Flip();
            }
        }

        if (Input.GetButtonUp("Fire1") && fireTime <= 0) {
            if (firstShoot) {
                firstShoot = false;
                Invoke("shoot", 0.2f);
            }else{
                shoot();
            }
        }
    }

    public void shoot() {
        Vector2 moveVec = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        fireTime = REPEAT_FIRE_TIME;
        Vector2 dir = ForcingMove ? moveVec : deadMoveVec;

        GameObject shoot = Resources.Load<GameObject>("Shoots/Nim/shoot_1");
        Vector3 position = transform.position + (Vector3)(dir.normalized * 0.9f);

        shoot = (GameObject)Instantiate(shoot, position, Quaternion.identity);

        ShootMove move = shoot.GetComponent<ShootMove>();
        move.direction = dir;
    }

    void calcDeadMove() {
        if (ForcingMove)
            deadMoveVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void Move(Vector2 moveVec) {
        if (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 1) {
            moveVec.Normalize();
        }

        if (OnMoving) {
            float mainSpeed = Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y) ? Mathf.Abs(moveVec.x) : Mathf.Abs(moveVec.y);
            _anim.speed = 0.5f + mainSpeed / 2;
        }else{
            _anim.speed = 1;
        }

        Vector3 deltaMv = OnMoving && NormalState ? (Vector3)moveVec * speed : Vector3.zero;
        rigidbody2D.velocity = deltaMv;
    }

    public void Animation(Vector2 moveVec) {
        _anim.SetBool("OnMoving", OnMoving);
        _anim.SetBool("OnDraw", OnDraw);
        _anim.SetBool("OnCharge", OnCharge);
        _anim.SetBool("OnShoot", OnShoot && fireTime <= 0);
        //anim.SetBool("NormalState", NormalState);

        //anim.SetFloat("OnShootTime", onShootTime);

        if (ForcingMove) {
            _anim.SetFloat("Horizontal", moveVec.x);
            _anim.SetFloat("Vertical", moveVec.y);
        }else{
            _anim.SetFloat("Horizontal", deadMoveVec.x);
            _anim.SetFloat("Vertical", deadMoveVec.y);
        }
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void Hit(float damage)
    {
        this.life -= damage;
        if (this.life <= 0f)
        {
            this.life = 0f;
            Time.timeScale = 0f;
            ui.SetActive(true);
        }
        else
        {
            //Vector2 moveVec = new Vector2(
            //    Input.GetAxis("Horizontal"),
            //    Input.GetAxis("Vertical")
            //);
            //this.gameObject.GetComponent<Rigidbody2D>().AddForce(moveVec * -50, ForceMode2D.Impulse);
        }
        Camera.main.GetComponent<Director>().updateLife(this.life);
    }
}