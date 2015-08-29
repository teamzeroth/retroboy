#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections;

using DG.Tweening;

public class Player : MovableBehaviour {

    #region Attributes

    /// Publics 
    public float speed = 3;
    public float dashSpeed = 50;

    public Collider2D FeetCollider;
    public Collider2D BodyCollider;

    public Sprite[] ss_E;
    public Sprite[] ss_S;
    public Sprite[] ss_N;
    public Sprite[] ss_NE;
    public Sprite[] ss_SE;

    /// Childrens
    PlayerInput _input = new PlayerInput();
    PlayerSFX _sfx;
    Animator _anim;

    Transform _charge;

    /// Privates
    bool inSimulateMoviment; // Sinalized when the player are a simulated Moviment
    bool invulnerable = false; //Sinalized when the player can't receive hit

    Vector2 simulatedVector;
    Vector2 LastDirection;

    bool onChargeState = false;
    bool onShootState = false;
    bool onDashState = false;
    bool onHurtState = false;
    bool onDieState = false;

    /// Coroutines
    Coroutine watchDash;
    Coroutine watchShoot;

    #endregion


    #region Getters And Setters

    bool lockDirection = false;
    bool lockMoviment = false; /// Sinalized when player need is stopped 
    public bool LockMoviment {
        get {
            return lockMoviment;
        }
        set {
            lockMoviment = value;
        }
    }

    /// Return the dead direction of the player in the player input
    public Vector2 DeadDirection { get { return _input.deadDirection; } }

    /// Return true if the player is in some state that the Beta can be visible
    bool hideBeta;
    public bool BetaVisible {
        get {
            return hideBeta;
        }
        set {
            hideBeta = value;
        }
    }

    int life = 4;
    public int Life {
        get {
            return life;
        }
        set {
            life = value;
        }
    }

    public bool Dead {
        get { return onDieState; }
    }

    public bool Hurt {
        get { return onHurtState; }
    }

    [Range(0, 100)]
    public float actionPoints = 100;

    #endregion


    #region MonoBehaviour

    void Awake() {
        base.Awake();

        onChargeState = false;
        onShootState = false;
        onDashState = false;
        onHurtState = false;
        onDieState = false;

        _anim = GetComponent<Animator>();
        _sfx = GetComponent<PlayerSFX>();

        _charge = transform.Find("Charge");
    }

    public void UpdateMove(Vector2 deltaMovement) {
        if (LockMoviment) {
            Move(Vector2.zero);
            return;
        }

        if (inSimulateMoviment) {
            Move(simulatedVector);
            return;
        }

        Move(deltaMovement * speed);
    }

    public void Move(Vector3 move) {
        GetComponent<Rigidbody2D>().velocity = move;
    }

    public void UpdateAnimation(Vector2 deltaMovement) {
        Vector2 targetVector = inSimulateMoviment ?
            simulatedVector : _input.Direction;

        _anim.SetFloat("Horizontal", targetVector.x);
        _anim.SetFloat("Vertical", targetVector.y);

        checkFlip();

        if (_input.InCharging)
            LastDirection = targetVector;

        if (_input.InCharging && !onChargeState) {
            lockMoviment = true;
            onChargeState = true;

            _anim.SetTrigger("OnDraw");
            KillAndStartCoroutine(ref watchShoot, shootRoutine());
        }

        if (_input.InShooting && !onShootState) {
            _input.LockDirection = LastDirection;  // Lock Player Direction Look
            onShootState = true;

            _anim.SetBool("OnDraw", false);
            _anim.SetTrigger("OnShoot");
        }


        _anim.SetBool("OnHurt", onHurtState);
        _anim.SetBool("OnDie", onDieState);

        //Check if the player can use the dash
        _anim.SetBool("OnDash", _input.InDashing ? ActionDash() : false);

        _anim.SetBool("InMoving", _input.InMoving && !LockMoviment);
    }

    public void UpdateSound(Vector2 deltaMovement) {
        if (_input.InCharging)
            _sfx.Charge();
        else
            _sfx.UnCharge();
    }

    public void Update() {
        if (GameController.self.Pause || GameController.self.stopPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space)) OnGetHit(1, Vector2.right);

        Vector2 deltaMovement = _input.Update(this);

        UpdateAnimation(deltaMovement);
        UpdateMove(deltaMovement);
        UpdateSound(deltaMovement);

        if (!onChargeState) AccumulateActionPoints(Time.deltaTime * Game.PLAYER_ACTION_POINTS_BY_TIME);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos() {
        Vector3 p = transform.position - Vector3.one * 0.75f;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.red;
        style.border = new RectOffset(1, 1, 1, 1);

        Handles.Label(p, actionPoints.ToString(), style);
    }
#endif

    #endregion


    #region Messages

    public void UnlockInvulnerability() {
        invulnerable = false;
    }

    /// <summary>
    /// Make a player take a hit. This is the main OnGetHit method
    /// </summary>
    /// <param name="damage">The damage that the player will take</param>
    /// <param name="direction">The direction that the player will go to "fell" the hit</param>
    public void OnGetHit(int damage, Vector2 direction) {
        if (onHurtState) return;

        Life = Mathf.Max(Life - damage, 0);
        if (Life > 0)
            ActionHurt(direction * damage * 3);
        else {
            LockMoviment = true;
            onDieState = true;
        }
    }

    public void OnGetHit(BaseEnemy enemy, Collider2D other) {
        OnGetHit((int)enemy.damage, (FeetCollider.bounds.center - other.bounds.center).normalized);
    }

    public void OnGetHit(ShootMove shoot, Collider2D other) {
        OnGetHit((int)shoot.damage, shoot.direction);
    }

    public bool PrepareActionPoints(float points) {
        return (actionPoints - points) > 0;
    }

    public bool ConsumeActionPoints(float points) {
        if (actionPoints - points > 0) {
            actionPoints -= points;
            return true;
        }
        return false;
    }

    public void AccumulateActionPoints(float points) {
        if (actionPoints + points < Game.PLAYER_MAX_ACTION_POINTS)
            actionPoints += points;
        else
            actionPoints = Game.PLAYER_MAX_ACTION_POINTS;
    }

    #endregion


    #region Methods

    private void SimulateMove(Vector2 force, Vector2 final, float time, Color color, Ease ease = Ease.Linear) { }
    private void SimulateMove(Vector2 force, Vector2 final, float time, Ease ease = Ease.Linear) { }

    private void ActionHurt(Vector2 direction) {
        if (onHurtState) return;

        lockMoviment = false;

        onHurtState = true;
        invulnerable = true;

        inSimulateMoviment = true;
        simulatedVector = direction;

        DOTween.To(() => simulatedVector, x => simulatedVector = x, Vector2.zero, Game.PLAYER_HURT_TIME)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => {
                onHurtState = false;
                inSimulateMoviment = false;
                Invoke("UnlockInvulnerability", Game.PLAYER_INVULNERABILITY_TIME);
            });
    }

    public void OnAnimationFinish() {

    }

    /// <summary>
    /// Check if the player can use the dash in his current position and,
    /// if true, make a dash animation
    /// </summary>
    private bool ActionDash() {
        if (onDashState) return true;

        Vector2 normal = _input.Direction.normalized;
        float time = getDashTime(normal, dashSpeed, Game.TOTAL_TIME_PLAYER_DASH);

        if (time < 0.02f || !ConsumeActionPoints(50)) {
            _input.InDashing = false;
            return false;
        }

        lockMoviment = false;

        onDashState = true;
        inSimulateMoviment = true;
        simulatedVector = normal * dashSpeed;

        KillAndStartCoroutine(ref watchDash, dashRoutine());

        DOTween.To(() => simulatedVector, x => simulatedVector = x, Vector2.zero, time)
            .SetEase(Ease.InCirc)
            .OnComplete(() => {
                onDashState = false;
                inSimulateMoviment = false;
                _input.InDashing = false;
            });

        return true;
    }

    /// <summary>
    /// Active the right animation of charge by the time of charge
    /// </summary>
    public void enableAnimationCharge() {
        GameObject energy = _charge.GetChild(0).gameObject;

        if (_input.TimeInCharge < Game.PLAYER_TOTAL_CHARGE_TIME) {
            energy.SetActive(true);

        } else {
            if (energy.activeSelf) {
                _charge.GetChild(2).gameObject.SetActive(true);
                _charge.GetChild(2).GetComponent<SimpleAnimatior>().enabled = true;
            }

            _charge.GetChild(1).gameObject.SetActive(true);
            energy.SetActive(false);
        }


        Vector2 angle = Helper.GetDirectionVector((Direction)Helper.getGeoDirection(_input.Direction));
        _charge.localPosition = angle * (Game.PLAYER_DIST_SHOOT * Mathf.Max(Mathf.Abs(angle.x), Mathf.Abs(angle.y)));
    }

    /// <summary>
    /// Disable All animations of charge
    /// </summary>
    public void disableAnimationCharge() {
        _charge.GetChild(0).gameObject.SetActive(false);
        _charge.GetChild(1).gameObject.SetActive(false);
        _charge.GetChild(2).gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawn a shoot in a v direction
    /// </summary>
    /// <param name="v">the direction of the shoot</param>
    private void instantiateShoot(Vector3 v) {
        //Consume points of the shoot
        int damage = 0;

        if (_input.LastTimeInCharge >= Game.PLAYER_TOTAL_CHARGE_TIME) {
            if (!ConsumeActionPoints(25)) return;
            damage = 3;
        } else if (_input.LastTimeInCharge >= Game.PLAYER_TOTAL_CHARGE_TIME / 2) {
            if (!ConsumeActionPoints(25 / 2)) return;
            damage = 2;
        } else {
            if (!ConsumeActionPoints(25 / 4)) return;
            damage = 1;
        }

        Vector3 d = v.normalized;
        Vector3 spawn = transform.position + d * (Game.PLAYER_DIST_SHOOT * Mathf.Max(Mathf.Abs(d.x), Mathf.Abs(d.y)));
        spawn += (Vector3)Game.PLAYER_SHOOT_DIFERENCE;

        GameObject shootObject = Instantiate<GameObject>(
            Resources.Load<GameObject>("Shoots/Nim/Shoot")
        );

        shootObject.transform.position = spawn;

        // Setting the paramethes of the shoot
        ShootMove shoot = shootObject.GetComponent<ShootMove>();
        shoot.collisionLevel.Level = Level; /// TODO: Set it to shoot.Level = Level
        shoot.Direction = v;
        shoot.damage = damage;

        var x = Mathf.Clamp(_input.LastTimeInCharge, 0.5f, Game.PLAYER_TOTAL_CHARGE_TIME) / Game.PLAYER_TOTAL_CHARGE_TIME;
        shoot.Distance = 8 * x * x;

        _sfx.Shoot(_input.LastTimeInCharge);
    }

    #endregion

    #region CoRoutines

    IEnumerator shootRoutine() {
        // Waithing for the draw animation (or shoot)
        while (!_anim.CurrentAnimState().StartsWith("Nim-draw") &&
            !_anim.CurrentAnimState().StartsWith("Nim-shoot")) {
            yield return null;
        }

        // Active charge animation
        while (_anim.CurrentAnimState().StartsWith("Nim-draw")) {
            enableAnimationCharge();

            var x = Mathf.Min(_input.LastTimeInCharge, Game.PLAYER_TOTAL_CHARGE_TIME) / Game.PLAYER_TOTAL_CHARGE_TIME;
            PrepareActionPoints(x);
            yield return null;
        }

        disableAnimationCharge(); // Disable charge animation
        instantiateShoot(LastDirection); // Instatiate shoot

        while (_anim.CurrentAnimState().StartsWith("Nim-shoot")) {
            yield return null;
        }

        // Unlock movimatation

        lockMoviment = false;
        onShootState = false;
        onChargeState = false;

        _input.LockDirection = Vector2.zero;
    }

    IEnumerator dashRoutine() {
        Sprite[] array = null;

        float time = Time.time;
        float length = 4;

        switch (Helper.getGeoDirection(_input.Direction)) {
            case (int)Direction.E: array = ss_E; break;
            case (int)Direction.NE: array = ss_NE; break;
            case (int)Direction.N: array = ss_N; break;
            case (int)Direction.SE: array = ss_SE; break;
            case (int)Direction.S: array = ss_S; break;
        }

        length = array.Length - 1;

        while (_input.InDashing) {
            float i = (Time.time - time) / Game.TOTAL_TIME_PLAYER_DASH;
            i = -i * (i - 2);

            getDashClone(array[(int)UnityEngine.Random.Range(i * length, length)]);
            yield return new WaitForSeconds(Game.FRAMETIME_PLAYER_DASH);
        }

        while (time + Game.TOTAL_TIME_PLAYER_DASH + Game.EXTRA_DASH_TIME > Time.time) {
            getDashClone(renderer.sprite, 0.5f);
            yield return new WaitForSeconds(Game.FRAMETIME_PLAYER_DASH * 2.5f);
        }
    }

    #endregion

    #region PlayerUtils

    public void checkFlip() {
        Vector2 targetVector = inSimulateMoviment ?
           simulatedVector : _input.Direction;

        if (targetVector.x < 0 && Mathf.Abs(targetVector.x) >= Mathf.Abs(targetVector.y * 0.4f)) {
            if (!flipped) transform.Flip(ref flipped);
        } else {
            if (flipped) transform.Flip(ref flipped);
        }
    }

    /// <summary>
    /// Check the time of collision in some determined vector and velocity, return the clamp of this time and the gived time
    /// </summary>
    /// <param name="vector">The Motion Vector</param>
    /// <param name="velocity">The velocity by the time</param>
    /// <param name="time">The time to clamp</param>
    /// <returns></returns>
    private float getDashTime(Vector2 vector, float velocity, float time) {
        Vector2 sPoint = (Vector2)Feet.position;
        float radius = Feet.GetComponent<CircleCollider2D>().radius;

        int level = 1 << LayerMask.NameToLayer("Level");
        level |= 1 << LayerMask.NameToLayer("Wall");
        level |= 1 << LayerMask.NameToLayer("Floor");

        RaycastHit2D hit = Physics2D.Raycast(sPoint, vector, time * velocity, level);

        float r = hit.distance != 0 ? (hit.distance - radius) / velocity : time;
        return r;
    }

    /// <summary>
    /// Create a clone og the <arg>sprite</arg> in the same position of the player
    /// </summary>
    /// <param name="sprite">The sprite to create</param>
    /// <param name="alpha">The start alpha of the sprite</param>
    void getDashClone(Sprite sprite, float alpha = 1) {
        GameObject clone = new GameObject("Clone", typeof(SpriteRenderer));
        SpriteRenderer cloneSprite = (clone.GetComponent<Renderer>() as SpriteRenderer);

        clone.transform.position = transform.position;
        clone.transform.localScale = transform.localScale;

        cloneSprite.sprite = sprite;
        cloneSprite.sortingLayerID = renderer.sortingLayerID;
        cloneSprite.sortingOrder = renderer.sortingOrder - 4;
        cloneSprite.color = new Color(1, 1, 1, alpha);

        cloneSprite.DOColor(new Color(1, 1, 1, 0), Game.DASH_SHADOW_TIME).SetEase(Ease.InQuint).OnComplete(() => {
            Destroy(clone);
        });
    }

    void KillAndStartCoroutine(ref Coroutine routine, IEnumerator method) {
        if (routine != null) StopCoroutine(routine);
        StartCoroutine(method);
    }
    #endregion
}