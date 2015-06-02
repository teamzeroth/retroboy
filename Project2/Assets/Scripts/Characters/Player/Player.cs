using UnityEngine;
using System.Collections;
using System;

using DG.Tweening;

public class Player : MonoBehaviour {

    private class ControllerStates{

        public Transform player;

        public Vector2 deltaDirection;
        public Vector2 deadDirection = new Vector2(1, 0);

        public Vector2 Direction;

        public bool InMoving;
        public bool OnCharging;
        public bool OnShooting;
        public bool OnSimulateMove;

        public float TimeInCharge;
        public float TimeLastShoot;
        public float LastTimeInCharge;

        public Vector3 Update() {
            if (!Input.GetKey(KeyCode.Mouse0)) {
                deltaDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                if (deltaDirection.magnitude > 1)
                    deltaDirection.Normalize();

                if (deltaDirection.magnitude > 0.1f) {
                    deadDirection = deltaDirection;
                }
            } else {
                Camera camera = Camera.main;
                Vector3 pos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));

                deltaDirection = pos - player.transform.position;
                deltaDirection.Normalize();

                deadDirection = deltaDirection;
            }
            

            Direction = deadDirection;

            InMoving = deltaDirection != Vector2.zero;

            OnShooting = false;
            OnCharging = false;

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

    private bool waitToMove = false;
    private bool waitShootFinish = false;
    private bool waitToNewShoot = false;

    private Tween lastFixedTween;

    private ControllerStates controller = new ControllerStates();

    private Coroutine watchShoot;

    private Animator _anim;
    private Rigidbody2D _rigidbody;
    private PlayerSFX _sfx;

    #region Getters and Setters

    [HideInInspector]
    public bool OnDie;

    private bool _onHurt = false;
    private bool afterOnHurt = false;
    public bool OnHurt {
        get { return _onHurt; }

        set {
            if (_onHurt && !value) {
                Invoke("SetAfterOnHurt", 1f);
                _onHurt = value;
            } else {
                _onHurt = afterOnHurt = value;
            }
        }
    }

        public void SetAfterOnHurt() {
            afterOnHurt = _onHurt;
        }

    public Vector2 DeadDirection {
        get { return controller.deadDirection; }
        set { controller.deadDirection = value; }
    }

    public bool BetaVisible {
        get { 
            bool colorTest = ((SpriteRenderer) renderer).color != Color.clear;
            bool animationTest = _anim.CurrentAnimState().StartsWith("Nim-idle") || _anim.CurrentAnimState().StartsWith("Nim-run");

            return !OnHurt && !controller.OnSimulateMove && colorTest && animationTest; 
        }
    }

    private int _life;
    public int Life {
        get {return _life; }

        set {
            _life = value;
            UiController.self.Life = value; 
        }
    }

    #endregion


    #region MonoBehaviour

    public void Start() {
        controller.player = transform;

        _anim = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sfx = GetComponent<PlayerSFX>();

        _life = UiController.self.Life; /*To Do: Pog*/
    }

    public void Update() {
        if (GameController.self.Pause) return;

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
            if (controller.OnCharging && !waitShootFinish && !waitToNewShoot) {
                _anim.SetTrigger("OnDraw");

                if (watchShoot != null) StopCoroutine(watchShoot);
                watchShoot = StartCoroutine(WaitShootAnimationFinish());
            }

            if (controller.OnShooting && !waitToNewShoot) {
                StartCoroutine(WaitShootAnimationStart());

                _anim.SetTrigger("OnShoot");
                _anim.SetBool("OnDraw", false);
            }

            //Checa o estado de se machucar
            if (OnHurt) {
                waitToMove = true;

                _anim.SetBool("OnShoot", false);
                _anim.SetBool("OnDraw", false);
            }

            _anim.SetBool("OnHurt", OnHurt);

            //Checa se o jogador move
            _anim.SetBool("InMoving", (controller.InMoving || controller.OnSimulateMove) && !waitToMove);
        }

        public void UpdateSound() {
            if (controller.OnCharging && _anim.CurrentAnimState().StartsWith("Nim-draw"))
                _sfx.Charge();
            else
                _sfx.UnCharge();
                
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

        _sfx.Footstep(deltaMovement != Vector2.zero);
        Move(deltaMovement);
    }


    public void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Coin")
            getCoin(coll.gameObject.GetComponent<CoinMove>());
    }

    #endregion


    #region Messages

    public void Move(Vector3 deltaMovement){
        rigidbody2D.velocity = deltaMovement;
    }

    public void DisableCollider() {
        collider2D.enabled = false;
    }

    public void StartFixedMove(Vector2 start, Vector2 to, float time, Color color, Ease ease = Ease.Linear) {
        StartFixedMove(start, to, time, ease);
        waitToMove = false;

        ((SpriteRenderer)renderer).DOColor(color, time);
    }

    public void StartFixedMove(Vector2 start, Vector2 to, float time, Ease ease = Ease.OutCirc) {
        if (fixedMove != Vector2.zero)
            lastFixedTween.Kill();

        waitToMove = true;
        fixedMove = fixedMove + start;
        controller.OnSimulateMove = true;

        lastFixedTween = DOTween.To(() => fixedMove, x => fixedMove = x, to, time).SetEase(ease).OnComplete(() => {
            OnHurt = false;
            fixedMove = Vector2.zero;

            collider2D.enabled = true;
            controller.OnSimulateMove = false;
            
            waitShootFinish = false;
            waitToNewShoot = false;
            waitToMove = false;
        });
    }

    public void OnGetHit(int damage, Vector2 direction, Collider2D other) {
        _sfx.Hurt();

        StopAllCoroutines();

        Life -= damage;

        if (Life > 0) {
            OnHurt = true;
            StartFixedMove(direction.normalized * damage * 4, Vector2.zero, Game.TIME_PLAYER_DAMAGE);
        } else {
            DisableCollider();

            OnDie = true;
            waitToMove = true;

            _anim.SetBool("OnHurt", false);
            _anim.SetTrigger("OnDie");
        }
    }

        public void OnGetHit(BaseEnemy enemy, Collider2D other) {
            if (afterOnHurt) return;

            Vector2 d = (collider2D.bounds.center - other.bounds.center).normalized;
            OnGetHit((int) enemy.damage, d, other);
        }

        /*public void OnGetHit(Enemy enemy, Collider2D other) {
            if (afterOnHurt) return;

            Vector2 d = (collider2D.bounds.center - other.bounds.center).normalized;
            OnGetHit((int) enemy.damage, d, other);
        }*/

        public void OnGetHit(ShootMove shoot , Collider2D other) {
            if (afterOnHurt) return;

            Vector2 d = shoot.direction;
            OnGetHit((int) shoot.damage, d, other);
        }

    public void OnAnimationFinish(){
        if(OnDie) GameController.self.CanRestartTheGame = true;
    }

    #endregion


    #region Private Methods

    public void checkFlip() {
        if (controller.Direction.x < 0 && Mathf.Abs(controller.Direction.x) >= Mathf.Abs(controller.Direction.y * 0.4f)) {
            if (!flipped)
                transform.Flip(ref flipped);

        } else {
            if (flipped)
                transform.Flip(ref flipped);
        }
    }

    private void instaceShoot(Vector3 v){
        GameObject shootGO = (GameObject) Instantiate(
            Resources.Load<GameObject>("Shoots/Nim/shoot_1"),
            transform.position + v.normalized * 0.3f,
            Quaternion.identity
        );

        ShootMove shoot = shootGO.GetComponent<ShootMove>();
        shoot.Direction = v;
        shoot.damage = controller.LastTimeInCharge >= 1.5f ? controller.LastTimeInCharge >= 3f ? 3 : 2 : 1;

        //SFX: Shoot
        _sfx.Shoot(controller.LastTimeInCharge);
    }

    private void getCoin(CoinMove coin) {
        UiController.self.Coins += coin.quant;
        Destroy(coin.gameObject);
    }

    #endregion


    #region CoRotinnes

    IEnumerator WaitShootAnimationFinish() {
        waitShootFinish = true;
        waitToMove = true;

        while (waitShootFinish)
            yield return null;

        yield return new WaitForSeconds(0.5f);
        waitToMove = false;
        watchShoot = null;
    }

    IEnumerator WaitShootAnimationStart() {
        waitToNewShoot = true;

        while (!_anim.CurrentAnimState().StartsWith("Nim-shoot"))
            yield return null;

        instaceShoot(new Vector2(_anim.GetFloat("Horizontal"), _anim.GetFloat("Vertical")));
        waitShootFinish = false;

        yield return new WaitForSeconds(0.35f);
        waitToNewShoot = false;
    }

    /*IEnumerator CancelShoot() {
        yield return new WaitForSeconds(0.1f);    

        waitShootFinish = false;
        waitToNewShoot = false;
        waitToMove = false;
        watchShoot = null;
    }*/

    #endregion

}
