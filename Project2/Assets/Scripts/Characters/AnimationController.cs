using UnityEngine;
using System.Collections.Generic;

using Helper;
using DG.Tweening;

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
        return _anim.CurrentAnimState().StartsWith("idle") || _anim.CurrentAnimState().StartsWith("walk") || _anim.CurrentAnimState().StartsWith("friction");
    }}

    public bool DrawState {get{
        return _anim.CurrentAnimState().StartsWith("draw");
    }}

    public bool OnMoving { get { return CanMove && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")); } }

    public bool OnDraw { get { return Input.GetButtonDown("Fire1") && !DrawState; } }
    public bool OnCharge { get { return Input.GetButton("Fire1"); } }
    public bool OnShoot { get { return Input.GetButtonUp("Fire1") && fireTime <= 0; } }

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
//        BroadcastMessage("HadShoot");

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

    private float frictionValue;

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

        Vector2 dir = (NormalState ? moveVec : Vector2.zero) * speed;
        Vector2 curDir = rigidbody2D.velocity;

        float value = Mathf.Abs(
            (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360 -
            (Mathf.Atan2(curDir.y, curDir.x) * Mathf.Rad2Deg) % 360
        );

        frictionValue = moveVec == Vector2.zero ? 0 : value / 180;
        
        //print(value + " : " + frictionValue);
        rigidbody2D.velocity = Vector2.Lerp(curDir, dir, Mathf.Max(0.1f, 1 - frictionValue) * 20 * Time.deltaTime);
    }

    public void Animation(Vector2 moveVec) {
        _anim.SetBool("OnMoving", OnMoving);
        _anim.SetBool("OnDraw", OnDraw);
        _anim.SetBool("OnCharge", OnCharge);
        _anim.SetBool("OnShoot", OnShoot);

        _anim.SetFloat("Friction", frictionValue);
        
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

    public void Hit(float damage, Vector2 direction){

        this.life -= damage;
        if (Debug.isDebugBuild)
            Debug.Log("Player Life: " + this.life);

        if (this.life <= 0f){
            this.life = 0f;
            Time.timeScale = 0f;
            ui.SetActive(true);

        }else
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -2, ForceMode2D.Impulse);
        
        Camera.main.GetComponent<Director>().updateLife(this.life);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        //print(trigger.gameObject.name);
        ShootMove s = trigger.gameObject.GetComponent<ShootMove>();
        if (trigger.gameObject.name.Contains("bullet") && !s.isAlly)
            Hit(s.damage, Vector2.zero);
    }   
}