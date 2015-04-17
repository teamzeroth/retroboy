using UnityEngine;
using System.Collections.Generic;

using Helper;
using DG.Tweening;
using System.Collections;

public class AnimationController : MonoBehaviour {

    static private float REPEAT_FIRE_TIME = 0.5f;
    static private float DELIVERY_FIRE_TIME = 0.4f;

    public float speed = 10f;
    public float multiplier = 10f;
    public float life = 10f;
    public GameObject ui = null;

    [HideInInspector]
    public bool flipped = false;

    Animator _anim;
    GameObject _chargeParticles;

    Vector2 deadMoveVec = new Vector2(1, 0);
    Vector2 fixedMoveVec = Vector2.zero;
    Vector2 currentMoveVec = Vector2.zero;

    float fireTime = 0;
    bool firstShoot = true;
    float frictionValue;

    #region Getters And Setters

    public bool OnHurt = false;

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

    public Vector2 DeadVector { set { deadMoveVec = value; } }

    #endregion

    #region MonoBehaviour Methods

    void Start(){
        _anim = GetComponent<Animator>();
        _chargeParticles = transform.Find("Charge Particles").gameObject;

        if (ui == null) ui = GameObject.Find("Menu");
    }

    void Update() {
        if (fixedMoveVec == Vector2.zero) {

            NormalUpdate();

            if (Input.GetKeyDown(KeyCode.Space)) {
                MakeFixedMove(Vector2.right / 2, 1, true, 1);
            }

        }else
            FixedMoveUpdate();
    }

    public void FixedUpdate() {
        if (fixedMoveVec != Vector2.zero) return;
        
        Move(currentMoveVec);
    }

    public void NormalUpdate() {
        currentMoveVec = new Vector2(
           Input.GetAxis("Horizontal"),
           Input.GetAxis("Vertical")
        );

        calcDeadMove();  
        checkVariations();

        Animation(currentMoveVec);
        CheckFlip(currentMoveVec);  

        if (Input.GetButtonUp("Fire1") && fireTime <= 0) {
            if (firstShoot) {
                firstShoot = false;
                Invoke("shoot", 0.2f);
            } else {
                shoot();
            }
        }
    }

    public void FixedMoveUpdate() {
        //_anim.speed = 0.5f;

        _anim.SetBool("OnHurt", OnHurt);
        _anim.SetBool("OnMoving", true);

        _anim.SetFloat("Horizontal", fixedMoveVec.x);
        _anim.SetFloat("Vertical", fixedMoveVec.y);

        rigidbody2D.velocity = (Vector3)fixedMoveVec;
        CheckFlip(fixedMoveVec);        
    }

    #endregion

    #region Messages

    public void Move(Vector2 moveVec) {
        if (moveVec.magnitude >= 1) {
            moveVec.Normalize();
        }

        Vector2 dir = (NormalState ? moveVec : Vector2.zero) * speed;
        Vector2 curDir = rigidbody2D.velocity;

        float value = Mathf.Abs(Mathf.DeltaAngle(
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, 
            Mathf.Atan2(curDir.y, curDir.x) * Mathf.Rad2Deg
        ));

        frictionValue = moveVec == Vector2.zero || curDir == Vector2.zero ? 0 : value / 180;

        if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical")) {
            rigidbody2D.velocity = Vector2.Lerp(curDir, dir, 1);
            return;
        }

        if (moveVec != curDir)
            rigidbody2D.velocity = Vector2.Lerp(curDir, dir, Mathf.Max(0.1f, 1 - frictionValue) * 20 * Time.deltaTime);
    }

    public void MakeFixedMove(Vector2 position, float duration, Color color) {
        MakeFixedMove(position, duration);

        SpriteRenderer spriteRenderer = (SpriteRenderer)renderer;
        spriteRenderer.DOColor(color, duration).SetEase(Ease.OutCubic);
    }

    public void MakeFixedMove(Vector2 position, float duration, bool hurt = false, float animSpeed = 1) {
        _anim.speed = animSpeed;
        
        OnHurt = hurt;

        fixedMoveVec = position;
        frictionValue = 0;
        
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(resetTween(duration));
    }

    IEnumerator resetTween(float waitTime){
        yield return new WaitForSeconds(waitTime);
        
        fixedMoveVec = Vector2.zero;
        rigidbody2D.velocity = Vector2.zero;
        _anim.speed = 1;
        OnHurt = false;

        GetComponent<Collider2D>().enabled = true;
    }

    public void Animation(Vector2 moveVec) {
        _anim.SetBool("OnMoving", OnMoving && (rigidbody2D.velocity != Vector2.zero || DrawState));
        _anim.SetBool("OnHurt", OnHurt);

        if (fixedMoveVec == Vector2.zero) {
            _anim.SetBool("OnDraw", OnDraw);
            _anim.SetBool("OnCharge", OnCharge);
            _anim.SetBool("OnShoot", OnShoot);
            _anim.SetFloat("Friction", frictionValue);
        }

        _chargeParticles.SetActive(OnCharge);

        //Vector2 pv = rigidbody2D.GetPointVelocity((Vector2) collider2D.bounds.center);

        

        if (NormalState && ForcingMove) {
            _anim.speed = 0.3f + Mathf.Clamp01(rigidbody2D.velocity.magnitude) * 0.7f;

            _anim.SetFloat("Horizontal", rigidbody2D.velocity.normalized.x);
            _anim.SetFloat("Vertical", rigidbody2D.velocity.normalized.y);
        }else if(ForcingMove){
            _anim.speed = 1;

            _anim.SetFloat("Horizontal", moveVec.normalized.x);
            _anim.SetFloat("Vertical", moveVec.normalized.y);
        }else{
            _anim.speed = 1;

            _anim.SetFloat("Horizontal", deadMoveVec.x);
            _anim.SetFloat("Vertical", deadMoveVec.y);
        }
    }

    public void CheckFlip(Vector2 moveVec) {
        if (ForcingMove) {
            if (Mathf.Abs(moveVec.x) >= Mathf.Abs(moveVec.y) * 0.42f) {
                if (!flipped && moveVec.x < 0) Flip();
                if (flipped && moveVec.x > 0) Flip();
            } else {
                if (flipped) Flip();
            }
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

    void OnTriggerEnter2D(Collider2D trigger){
        ShootMove s = trigger.gameObject.GetComponent<ShootMove>();
        if (trigger.gameObject.name.Contains("bullet") && !s.isAlly)
            Hit(s.damage, Vector2.zero);
    }

    #endregion

    #region Privates Methods

    void checkVariations() {
        fireTime = fireTime > -0.1f ? fireTime - Time.deltaTime : -0.1f;
        if (NormalState) firstShoot = true;
    }

    void shoot() {
        BroadcastMessage("HadShoot");

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

    #endregion
}