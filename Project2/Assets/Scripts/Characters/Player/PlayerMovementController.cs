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
            OnCharging = Input.GetButton("Fire1");
            OnShooting = Input.GetButtonUp("Fire1");

            TimeInCharge = OnCharging ? TimeInCharge + Time.deltaTime : 0;
            if (OnCharging) LastTimeInCharge = TimeInCharge;

            return deltaDirection;
        }

    }

    public float speed = 3;
    
    [HideInInspector] public bool flipped = false;
    [HideInInspector] public Vector2 fixedMove = Vector2.zero;

    private bool OnHurt = false;

    private bool waitToMove = false;
    private bool waitShootFinish = false;

    private ControllerStates controller = new ControllerStates();
    private Coroutine watchShoot;

    private Animator _anim;
    private Rigidbody2D _rigidbody;
    private PlayerSFXController _sfx;


    public Vector2 DeadDirection {
        get { return controller.deadDirection; }
    }


    public void Start() {
        _anim = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sfx = GetComponent<PlayerSFXController>();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            OnHurt = true;
            startFixedMove(Vector2.one, Vector2.zero, 1);
        }

        UpdateMove();
        UpdateAnimation();
        UpdateSound();
    }

    
    public void UpdateAnimation() {
        _anim.SetFloat("Horizontal", controller.Direction.x);
        _anim.SetFloat("Vertical", controller.Direction.y);

        
        //Checa o estado de carregar e atirar
        if ((controller.OnCharging) && !waitShootFinish) _anim.SetTrigger("OnDraw");
        if (controller.OnShooting && !waitShootFinish) {
            _anim.SetTrigger("OnShoot");
            _anim.SetBool("OnDraw", false);

            if (watchShoot != null) StopCoroutine(watchShoot);
            watchShoot = StartCoroutine(WaitShootAnimation());
        }

        //Checa o estado de se machucar
        if (OnHurt) {
            waitShootFinish = false;

            _anim.SetBool("OnShoot", false);
            _anim.SetBool("OnDraw", false);
        }

        _anim.SetBool("OnHurt", OnHurt);

        //Checa se o jogador move
        _anim.SetBool("InMoving", controller.InMoving && !waitToMove);

        checkFlip();
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

        fixedMove = fixedMove + start;

        lastFixedTween = DOTween.To(() => fixedMove, x => fixedMove = x, to, time).SetEase(ease).OnComplete(() => {
            OnHurt = false;
            fixedMove = Vector2.zero;
        });
    }

    IEnumerator WaitShootAnimation() {
        waitShootFinish = true;
        waitToMove = true;

        while (!_anim.CurrentAnimState().EndsWith("last") && waitShootFinish)
            yield return null;

        waitShootFinish = false;
        yield return new WaitForSeconds(0.35f);
        waitToMove = false;
        watchShoot = null;        
    }
}
