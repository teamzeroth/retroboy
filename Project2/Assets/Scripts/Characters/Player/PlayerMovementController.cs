using UnityEngine;
using System.Collections;
using System;

using DG.Tweening;

public class PlayerMovementController : MonoBehaviour {

    private class ControllerStates{

        public Vector2 deltaDirection;
        public Vector2 deadDirection = new Vector2(1, 0);

        public Vector2 Direction;

        public bool InMoving;
        public bool OnCharging;
        public bool OnShooting;

        public float TimeInCharge;
        public float TimeLastShoot;
        public float LastTimeInCharge;


        public Vector3 Update() {
            deltaDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (deltaDirection.magnitude > 1)
                deltaDirection.Normalize();

            Direction = deltaDirection;

            if (deltaDirection.magnitude > 0.1f){
                deadDirection = deltaDirection;
            } else {
                Direction = deadDirection;
            }

            InMoving = deltaDirection != Vector2.zero;

            if (OnCharging && !Input.GetButton("Fire1")) {
                OnShooting = true;
                OnCharging = false;
            } else {
                OnCharging = Input.GetButton("Fire1");
                OnShooting = Input.GetButtonUp("Fire1");
            }

            TimeInCharge = OnCharging ? TimeInCharge + Time.deltaTime : 0;
            if (OnCharging) LastTimeInCharge = TimeInCharge;

            return deltaDirection;
        }

        public void registreTime() {
            LastTimeInCharge = 0;
        }

    }

    public float speed = 3;
    
    [HideInInspector] public bool flipped = false;
    [HideInInspector] public Vector2 fixedMove = Vector2.zero;

    private bool OnHurt = false;

    private bool waitToMove = false;
    private bool waitShootFinish = false;
    private bool waitToNewShoot = false;

    private ControllerStates controller = new ControllerStates();
    private Coroutine watchShoot;

    private Animator _anim;
    private Rigidbody2D _rigidbody;
    private PlayerSFXController _sfx;

    #region Getters and Setters
    
    public Vector2 DeadDirection {
        get { return controller.deadDirection; }
    }

    public bool BetaVisible {
        get { return _anim.CurrentAnimState().StartsWith("Nim-idle") || _anim.CurrentAnimState().StartsWith("Nim-run"); }
    }

    #endregion

    public void Start() {
        _anim = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sfx = GetComponent<PlayerSFXController>();
    }

    public void Update() {
        if (GameController.self.Pause) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            OnHurt = true;
            startFixedMove(Vector2.one, Vector2.zero, 1);
        }

        UpdateMove();
        UpdateAnimation();
        UpdateSound();
    }

    
    public void UpdateAnimation() {
        if(!(waitShootFinish && !waitToMove)){
            _anim.SetFloat("Horizontal", controller.Direction.x);
            _anim.SetFloat("Vertical", controller.Direction.y);

            checkFlip();
        }

        //Checa o estado de carregar e atirar
        if (controller.OnCharging && !waitShootFinish) {
            _anim.SetTrigger("OnDraw");

            if (watchShoot != null) StopCoroutine(watchShoot);
            watchShoot = StartCoroutine(WaitShootAnimationFinish());
        }

        if (controller.OnShooting && !waitToNewShoot) {
            waitToNewShoot = true;
            StartCoroutine(WaitShootAnimationStart());

            _anim.SetTrigger("OnShoot");
            _anim.SetBool("OnDraw", false);
        }

        //Checa o estado de se machucar
        if (OnHurt) {
            waitToMove = false;

            _anim.SetBool("OnShoot", false);
            _anim.SetBool("OnDraw", false);
        }

        _anim.SetBool("OnHurt", OnHurt);

        //Checa se o jogador move
        _anim.SetBool("InMoving", controller.InMoving && !waitToMove);
    }

    public void UpdateSound() {
        if (controller.OnCharging && _anim.CurrentAnimState().StartsWith("Nim-draw")) 
            _sfx.Charge();
        else if (_anim.CurrentAnimState().StartsWith("Nim-shoot")) //TODO: Controlar o tempo entre cada tiro
            _sfx.Shoot(controller.LastTimeInCharge);
    }

    public void UpdateMove() {
        Vector2 deltaMovement = controller.Update();

        if (fixedMove == Vector2.zero){
            if (controller.OnCharging || controller.OnShooting || waitToMove)
                deltaMovement = Vector2.zero;
            else {
                deltaMovement *= speed;
            }
        }else{
            deltaMovement = fixedMove;
        }

        Move(deltaMovement);
    }

    public void checkFlip() {
        if (controller.Direction.x < 0 && Mathf.Abs(controller.Direction.x) >= Mathf.Abs(controller.Direction.y * 0.4f)) {
            if (!flipped)
                transform.Flip(ref flipped);
        
        } else {
            if (flipped) 
                transform.Flip(ref flipped);
        }
    }

    public void Move(Vector3 deltaMovement){
        rigidbody2D.velocity = deltaMovement;
    }

    Tween lastFixedTween;
    public void startFixedMove(Vector2 start, Vector2 to, float time, Ease ease = Ease.Linear){
        if (fixedMove != Vector2.zero)
            lastFixedTween.Kill();

        waitToMove = true;
        fixedMove = fixedMove + start;

        lastFixedTween = DOTween.To(() => fixedMove, x => fixedMove = x, to, time).SetEase(ease).OnComplete(() => {
            OnHurt = false;
            waitToMove = false;
            fixedMove = Vector2.zero;
        });
    }

    IEnumerator WaitShootAnimationFinish() {
        waitShootFinish = true;
        waitToMove = true;

        while (!_anim.CurrentAnimState().EndsWith("last") && waitShootFinish)
            yield return null;


        yield return new WaitForSeconds(0.35f);
        waitShootFinish = false;
        waitToNewShoot = false;
        yield return new WaitForSeconds(0.2f);
        waitToMove = false;
        watchShoot = null;        
    }

    IEnumerator WaitShootAnimationStart() {
        while (!_anim.CurrentAnimState().StartsWith("Nim-shoot"))
            yield return null;

        instaceShoot(new Vector2(_anim.GetFloat("Horizontal"), _anim.GetFloat("Vertical")));
    }


    public void OnGetHit() {

    }

    private void instaceShoot(Vector3 v){
        GameObject shootGO = (GameObject) Instantiate(
            Resources.Load<GameObject>("Shoots/Nim/shoot_1"),
            transform.position + v.normalized * 0.3f,
            Quaternion.identity
        );

        ShootMove shoot = shootGO.GetComponent<ShootMove>();
        shoot.direction = v;
        shoot.damage = controller.LastTimeInCharge >= 1.5f ? controller.LastTimeInCharge >= 3f ? 3 : 2 : 1;
    }
}
