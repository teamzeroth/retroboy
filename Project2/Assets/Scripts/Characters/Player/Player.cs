using UnityEngine;
using System.Collections;
using System;

using DG.Tweening;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public static Vector2 PLAYER_SHOOT_DIFERENCE = new Vector2(0, 0.03f);

    private class ControllerStates {

        public Transform _player;

        public Vector2 deltaDirection = new Vector2(1, 0);
        public Vector2 deadDirection = new Vector2(1, 0);

        public Vector2 Direction;

        public bool InMoving;
        public bool InDash;

        public bool OnCharging;
        public bool OnShooting;
        public bool OnSimulateMove;

        public float TimeInCharge;
        //public float TimeLastShoot;
        public float LastTimeInCharge;
        public Vector3 Update(Player player) {
            setupDirection(player);
            setupShoot(player);

            return deltaDirection;
        }

        public void setupDirection(Player player) {
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

                deltaDirection = pos - _player.transform.position;
                deltaDirection.Normalize();

                deadDirection = deltaDirection;
            }

            Direction = deadDirection;

            InMoving = deltaDirection != Vector2.zero;
            if (!OnCharging) {
                if (player.watchDash == null) InDash = InDash || Input.GetButtonDown("Dash");
            }
        }

        public void setupShoot(Player player) {
            OnShooting = false;
            OnCharging = false;

            if (OnCharging && !Input.GetButton("Action")) {
                OnShooting = true;
                OnCharging = false;
            } else {
                OnCharging = Input.GetButton("Action");
                OnShooting = Input.GetButtonUp("Action");
            }

            TimeInCharge = OnCharging ? TimeInCharge + Time.deltaTime : 0;
            if (OnCharging) LastTimeInCharge = TimeInCharge;
        }

        public void registreTime() {
            LastTimeInCharge = 0;
        }

    }

    public float speed = 3;
    public float dashSpeed = 50;

    public Collider2D FeetCollider;
    public Collider2D BodyCollider;
    public Sprite[] ss_E;
    public Sprite[] ss_S;
    public Sprite[] ss_N;
    public Sprite[] ss_NE;
    public Sprite[] ss_SE;
    public GameObject shootPrefab;
    [HideInInspector]
    public bool flipped = false;
    [HideInInspector]
    public Vector2 fixedMove = Vector2.zero;
    [HideInInspector]
    public CollisionLevel collisionLevel;

    private bool waitToMove = false;
    private bool waitShootFinish = false;
    private bool waitToNewShoot = false;

    private Tween lastFixedTween;

    private ControllerStates controller = new ControllerStates();

    private Coroutine watchShoot = null;
    private Coroutine watchDash = null;

    private Animator _anim;
    private SpriteRenderer _sprite;
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
            bool colorTest = ((SpriteRenderer)GetComponent<Renderer>()).color != Color.clear;
            bool animationTest = _anim.CurrentAnimState().StartsWith("Nim-idle") || _anim.CurrentAnimState().StartsWith("Nim-run");

            return !OnHurt && !controller.OnSimulateMove && colorTest && animationTest;
        }
    }

    private int _life;
    public int Life {
        get { return _life; }

        set {
            _life = value;
            UiController.self.Life = value;
        }
    }

    #endregion

    #region MonoBehaviour
    public void Start() {
        controller._player = transform;
        _anim = GetComponent<Animator>();
        _sprite = (SpriteRenderer)GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sfx = GetComponent<PlayerSFX>();

        collisionLevel = GetComponent<CollisionLevel>();
        _life = UiController.self.Life; /*To Do: Pog*/
    }

    public void Update() {
        if (GameController.self.Pause || GameController.self.stopPlayer) return;


        UpdateMove();
        UpdateAnimation();
        UpdateSound();

    }

    public void UpdateAnimation() {

        if (!(waitShootFinish && !waitToMove)) {

            Vector2 targetVector = fixedMove != Vector2.zero ? fixedMove : controller.Direction;
            _anim.SetFloat("Horizontal", targetVector.x);
            _anim.SetFloat("Vertical", targetVector.y);
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
        _anim.SetBool("OnDash", controller.InDash);

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
        Vector2 deltaMovement = controller.Update(this);

        if (watchDash == null && controller.InDash && !waitToMove) {
            Vector2 normal = controller.Direction.normalized;
            float time = getDashTime(normal, dashSpeed, Game.TOTAL_TIME_DASH);

            if (time > 0.011f) {
                StartFixedMove((normal * dashSpeed), (normal * dashSpeed) / 2, time, Ease.InExpo);
                watchDash = StartCoroutine(Clone());
            } else {
                controller.InDash = false;
            }
        }

        if (fixedMove == Vector2.zero) {
            if (controller.OnCharging || controller.OnShooting || waitToMove)
                deltaMovement = Vector2.zero;
            else {
                deltaMovement *= speed;
            }
        } else {
            deltaMovement = fixedMove;
        }

        //_sfx.Footstep(deltaMovement != Vector2.zero);
        Move(deltaMovement);
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Coin")
            getCoin(coll.gameObject.GetComponent<CoinMove>());

        /*if (coll.gameObject.layer == LayerMask.NameToLayer("Level") && controller.InDash) {
            print("OK GO");

            //StopCoroutine(watchDash);
            CancelFixedMove();
        }*/

    }

    #endregion


    #region Messages

    public void Move(Vector3 deltaMovement) {
        GetComponent<Rigidbody2D>().velocity = deltaMovement;
    }

    public void Move2(Vector3 deltaMovement) {
        Vector3 move = transform.position + deltaMovement * Time.deltaTime;
        GetComponent<Rigidbody2D>().MovePosition(move);
    }

    public void DisableColliders(bool disable = false) {
        FeetCollider.enabled = disable;
        BodyCollider.enabled = disable;
    }

    public void StartFixedMove(Vector2 start, Vector2 to, float time, Color color, Ease ease = Ease.Linear) {
        StartFixedMove(start, to, time, ease);
        waitToMove = false;
        ((SpriteRenderer)GetComponent<Renderer>()).DOColor(color, time);
        //((SpriteRenderer)transform.Find("Shadow").GetComponent<Renderer>()).DOColor(color, time);
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

            DisableColliders(true);
            controller.InDash = false;
            controller.OnSimulateMove = false;

            waitShootFinish = false;
            waitToNewShoot = false;
            waitToMove = false;
        });
    }

    public void CancelFixedMove() {
        if (lastFixedTween == null) return;
        lastFixedTween.Kill(true);
    }

    public void OnGetHit(int damage, Vector2 direction, Collider2D other) {
        _sfx.Hurt();

        StopAllCoroutines();

        Life -= damage;

        if (Life > 0) {
            OnHurt = true;
            StartFixedMove(direction.normalized * damage * 4, Vector2.zero, Game.TIME_PLAYER_DAMAGE);
        } else {
            DisableColliders();

            OnDie = true;
            waitToMove = true;

            _anim.SetBool("OnHurt", false);
            _anim.SetTrigger("OnDie");
        }
    }

    public void OnGetHit(BaseEnemy enemy, Collider2D other) {
        if (afterOnHurt) return;

        Vector2 d = (FeetCollider.bounds.center - other.bounds.center).normalized;
        OnGetHit((int)enemy.damage, d, other);
    }

    /*public void OnGetHit(Enemy enemy, Collider2D other) {
        if (afterOnHurt) return;

        Vector2 d = (collider2D.bounds.center - other.bounds.center).normalized;
        OnGetHit((int) enemy.damage, d, other);
    }*/

    public void OnGetHit(ShootMove shoot, Collider2D other) {
        if (afterOnHurt) return;

        Vector2 d = shoot.direction;
        OnGetHit((int)shoot.damage, d, other);
    }

    public void OnAnimationFinish() {
        if (OnDie) GameController.self.CanRestartTheGame = true;
    }

    #endregion


    #region Private Methods

    public void checkFlip() {
        Vector2 targetVector = fixedMove != Vector2.zero ? fixedMove : controller.Direction;

        if (targetVector.x < 0 && Mathf.Abs(targetVector.x) >= Mathf.Abs(targetVector.y * 0.4f)) {
            if (!flipped)
                transform.Flip(ref flipped);

        } else {
            if (flipped)
                transform.Flip(ref flipped);
        }
    }

    private void instaceShoot(Vector3 v) {
        Vector3 d = v.normalized;
        Vector3 spawn = transform.position + d * (Game.PLAYER_DIST_SHOOT * Mathf.Max(Mathf.Abs(d.x), Mathf.Abs(d.y)));
        spawn += (Vector3)PLAYER_SHOOT_DIFERENCE;
        /*
        GameObject shootGO = (GameObject)Instantiate(
            Resources.Load<GameObject>("Shoots/Nim/shoot_1"),
            spawn, Quaternion.identity
        );*/
        GameObject shootGO = (GameObject)Instantiate(
            shootPrefab,
            spawn, Quaternion.identity
        );
        ShootMove shoot = shootGO.GetComponent<ShootMove>();
        shoot.collisionLevel.Level = collisionLevel.Level;
        shoot.damage = controller.LastTimeInCharge >= 1.5f ? controller.LastTimeInCharge >= 3f ? 3 : 2 : 1;
        var x = Mathf.Clamp(controller.LastTimeInCharge, 1, 3);
        shoot.Direction = v;
        shoot.Distance = 8 * (x / 3);
        _sfx.Shoot(controller.LastTimeInCharge);
    }

    private void getCoin(CoinMove coin) {
        UiController.self.Coins += coin.quant;
        Destroy(coin.gameObject);
    }

    private float getDashTime(Vector2 vector, float velocity, float time) {
        Vector2 sPoint = (Vector2)FeetCollider.bounds.center;// + vector * (transform.collider2D as CircleCollider2D).radius;
        float radius = (FeetCollider as CircleCollider2D).radius * 2.5f;

        RaycastHit2D hit = Physics2D.Raycast(sPoint, vector, time * velocity, 1 << LayerMask.NameToLayer("Level"));

        float r = hit.distance != 0 ? (hit.distance - radius) / velocity : time;
        //print(r);
        return r;//time * (hit.distance / time * velocity);
    }

    /*public void OnDrawGizmos() {
        Vector2 vector = controller.Direction.normalized;
        float time = Game.TOTAL_TIME_DASH;
        float velocity = dashSpeed;

        Vector2 sPoint = (Vector2) transform.collider2D.bounds.center;
        float radius = (transform.collider2D as CircleCollider2D).radius * 2.5f;

        RaycastHit2D hit = Physics2D.Raycast(sPoint, vector, time * velocity, 1 << LayerMask.NameToLayer("Level"));

        float r = hit.distance != 0 ? (hit.distance - radius) : time * velocity;

        print(r);

        Gizmos.DrawLine(sPoint, sPoint + vector * time * velocity);
        Gizmos.DrawWireSphere(sPoint + vector * r, 0.1f);
    }*/

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

    IEnumerator Clone() {
        Sprite[] array = null;

        float time = Time.time;
        float length = 4;

        switch (Helper.getGeoDirection(controller.Direction)) {
            case (int)Direction.E: array = ss_E; break;
            case (int)Direction.NE: array = ss_NE; break;
            case (int)Direction.N: array = ss_N; break;
            case (int)Direction.SE: array = ss_SE; break;
            case (int)Direction.S: array = ss_S; break;
        }

        length = array.Length - 1;

        while (/* time + Game.TOTAL_TIME_DASH > Time.time && */ controller.InDash) {
            float i = (Time.time - time) / Game.TOTAL_TIME_DASH;
            i = -i * (i - 2);

            cloneMethod(array[(int)UnityEngine.Random.Range(i * length, length)]);
            yield return new WaitForSeconds(Game.FRAMETIME_PLAYER_DASH);
        }

        while (time + Game.TOTAL_TIME_DASH + Game.EXTRA_DASH_TIME > Time.time) {
            cloneMethod(_sprite.sprite, 0.5f);
            yield return new WaitForSeconds(Game.FRAMETIME_PLAYER_DASH * 2.5f);
        }

        yield return new WaitForSeconds(Game.TIME_TO_NEW_DASH);

        watchDash = null;
    }

    void cloneMethod(Sprite sprite, float alpha = 1) {
        GameObject clone = new GameObject("Clone", typeof(SpriteRenderer));
        SpriteRenderer cloneSprite = (clone.GetComponent<Renderer>() as SpriteRenderer);

        clone.transform.position = transform.position;
        clone.transform.localScale = transform.localScale;

        cloneSprite.sprite = sprite;
        cloneSprite.sortingLayerID = _sprite.sortingLayerID;
        cloneSprite.sortingOrder = _sprite.sortingOrder - 4;
        cloneSprite.color = new Color(1, 1, 1, alpha);

        cloneSprite.DOColor(new Color(1, 1, 1, 0), Game.DASH_SHADOW_TIME).SetEase(Ease.InQuint).OnComplete(() => {
            Destroy(clone);
        });
    }

    #endregion

}
