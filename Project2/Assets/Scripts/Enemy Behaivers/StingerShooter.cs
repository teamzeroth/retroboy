using UnityEngine;
using System.Collections;

public class StingerShooter : BaseEnemy {

    protected float DISTANCE_TO_TARGET = 4f;
    protected float MAX_VELOCITY = 2f;
    protected float START_DISTANCE_OF_SHOOT = 0.6f;
    protected float DISTANCE_OF_SMOKE = -0.5f;
    protected float BROKEN = 0.4f; /*Relative damege to look broken */

    protected StingerSSFX _sfx;
    protected Transform _shootspawn;
    protected Transform _smoke;

    public Vector2 chanceToShoot;
    public int shootTimes = 3;

    private Vector3 initPos;
    private Vector2 intercept;

    private Vector2 velocity;
    private Vector2 lastDirection;

    private float randomTime;
    private float timeToShoot;
    private float currTime;

    private int initLife;

    [HideInInspector]
    public bool OnShooting = false;
    [HideInInspector]
    public bool OnLostPlayer = false;
    [HideInInspector]
    public bool OnDie = false;
    [HideInInspector]
    private bool AfterShoot = false;

    private Coroutine watchTarget;

    #region MonoBehaiver

    public void Start() {
        base.Start();

        intercept = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        velocity = lastDirection = Vector2.zero;

        initPos = _renderer.localPosition;
        initLife = life;
        randomTime = Random.value * 3;

        _sfx = GetComponent<StingerSSFX>();
        _shootspawn = transform.Find("shootspawn");
        _smoke = transform.Find("smoke");
    }

    void LateUpdate() {
        //_sfx.Voo();

        if (!OnShooting) {
            currTime += Time.deltaTime;
            applySenoide();
        }

        //_FixedUpdate();
        UpdateAnimation();
    }

    public void OnDrawGizmosSelected() {
#if UNITY_EDITOR
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        float[,] vectors = new float[8, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } };

        for (int i = 0; i < 8; i++) {
            Vector3 a = new Vector3(vectors[i, 0], vectors[i, 1], 0);
            Vector3 b = new Vector3(vectors[(i + 1) % 8, 0], vectors[(i + 1) % 8, 1], 0);

            a.Normalize();
            b.Normalize();

            a = transform.Find("shootspawn").position + a * (0.6f * Mathf.Max(Mathf.Abs(a.x), Mathf.Abs(a.y)));
            b = transform.Find("shootspawn").position + b * (0.6f * Mathf.Max(Mathf.Abs(b.x), Mathf.Abs(b.y)));

            Gizmos.DrawLine(a, b);
        }
#endif
    }

    void FixedUpdate() {
        if (OnDie) return;

        if (!OnShooting) UpdateMove();
        else UpdateMoveSleep();
    }

    void applySenoide() {
        Vector3 senoid = initPos + new Vector3(0, 0.1f, 0) * Mathf.Sin((randomTime + currTime) * 4);

        GetComponent<CircleCollider2D>().offset = senoid;

        _renderer.localPosition = Helper.IgnoreZ(_renderer.localPosition, senoid);
    }

    Vector2 toIntercept;
    void UpdateMove() {
        if (target == null || target.GetComponent<Rigidbody2D>() == null) {
            GetComponent<Rigidbody2D>().velocity = (Vector2)impulseForce * 0.3f;
            return;
        }

        /* Controlhe de angulo do Stinger Shooter */
        {
            if (target.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
                toIntercept = target.GetComponent<Player>().DeadDirection.normalized;

            float currAngle = Mathf.LerpAngle(
                (360 + Mathf.Atan2(intercept.y, intercept.x) * Mathf.Rad2Deg) % 360,
                (360 + Mathf.Atan2(toIntercept.y, toIntercept.x) * Mathf.Rad2Deg) % 360,
                Time.deltaTime
            );

            intercept.Set(Mathf.Cos(currAngle * Mathf.Deg2Rad), Mathf.Sin(currAngle * Mathf.Deg2Rad));
        }

        Vector2 direction = intercept * DISTANCE_TO_TARGET;

        // Limite de velocidade
        //direction = Vector2.ClampMagnitude(direction, MAX_VELOCITY);

        if (impulseForce != Vector3.zero) direction += (Vector2)impulseForce * 0.3f;

        velocity = (direction - lastDirection);
        velocity = velocity.magnitude > 0.01f ? velocity.normalized : Vector2.zero;
        lastDirection = direction;

        direction = Vector2.Lerp(transform.position, (Vector2)target.position + direction, Time.deltaTime / 2);


        GetComponent<Rigidbody2D>().MovePosition(direction);
    }

    void UpdateMoveSleep() {
        velocity = Vector2.ClampMagnitude(velocity, MAX_VELOCITY);

        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = velocity + (Vector2)impulseForce * 0.5f;
    }

    void UpdateAnimation() {
        Vector2 direction = target != null ?
            (Vector2)(target.position - transform.position).normalized :
            intercept * -1;

        if (life <= initLife * BROKEN) {
            float time = _smoke.GetComponent<SimpleAnimatior>().NormalizeTime;

            _smoke.localPosition = (Vector3)direction * DISTANCE_OF_SMOKE + Vector3.up * 1 * time;
            _smoke.GetComponent<SortingOrder>().Position = transform.Find("feet").position.y + direction.y * DISTANCE_OF_SMOKE;
        }

        _anim.SetFloat("Horizontal", direction.x);
        _anim.SetFloat("Vertical", direction.y);
    }

    #endregion


    #region BaseEnemy

    public override void FindPlayer(Transform player) {
        if (target == player) return;

        getTimetoShootAgain();
        watchTarget = StartCoroutine(StopAndShoot());

        target = player;
    }

    public override void LostPlayer(Transform player) {
        if (target == player)
            OnLostPlayer = true;
    }

    public override void OnFinishAnimationBehavior() {
        if (OnDie) Destroy(gameObject);
        if (OnShooting) AfterShoot = true;
    }

    public override void OnTakeDamage(ShootMove shoot, Collider2D coll) {
        base.OnTakeDamage(shoot, coll);

        if (life > 0 && life <= initLife * BROKEN) {
            _smoke.gameObject.SetActive(true);
        }
    }

    public override void OnDestroyIt() {
        OnDie = true;

        GetComponent<Rigidbody2D>().velocity = -Vector2.up / 2;

        _anim.SetTrigger("Die");
        _smoke.gameObject.SetActive(false);

        _sfx.Explosion();

        DropCoins();
    }

    #endregion


    #region Private Methods

    void getTimetoShootAgain() {
        timeToShoot = Random.Range(chanceToShoot[0], chanceToShoot[1]);
    }

    protected void shoot() {
        _sfx.Laser();

        _anim.SetTrigger("Shoot");

        Vector3 d = new Vector3(_anim.GetFloat("Horizontal"), _anim.GetFloat("Vertical"));
        Vector3 spawn = _shootspawn.position + d * (START_DISTANCE_OF_SHOOT * Mathf.Max(Mathf.Abs(d.x), Mathf.Abs(d.y)));

        /*Spawn a shoot */
        {
            GameObject shootGO = (GameObject)Instantiate(
                Resources.Load<GameObject>("Shoots/Enemies/StingerShooter"),
                spawn, Quaternion.identity
            );

            ShootMove shoot = shootGO.GetComponent<ShootMove>();
            shoot.Direction = d;
            shoot.collisionLevel.Level = _collisionLevel.Level;
        }
    }

    #endregion


    #region CoRoutines

    IEnumerator StopAndShoot() {

        bool fistTime = true;
        Coroutine self = null;

        while ((fistTime || self == watchTarget) && !OnLostPlayer) {
            yield return new WaitForSeconds(timeToShoot);

            OnShooting = true;
            AfterShoot = true;

            var i = 0;

            yield return new WaitForSeconds(0.5f);

            while (i < shootTimes) {

                if (AfterShoot) {
                    i++;
                    AfterShoot = false;
                    shoot();
                }

                yield return null;
            }


            yield return new WaitForSeconds(0.5f);

            OnShooting = false;
            getTimetoShootAgain();

            fistTime = false;
            self = watchTarget;
        }

        intercept = (Vector2)(target.position - transform.position).normalized * -1;

        OnLostPlayer = false;
        watchTarget = null;
        target = null;
    }

    #endregion

}
