using UnityEngine;
using System.Collections;

public class StingerShooter : BaseEnemy {

    protected float DISTANCE_TO_TARGET = 2f;
    protected float MAX_VELOCITY = 2f;

    protected StingerSSFX _sfx;

    public Vector2 chanceToShoot;
    public int shootTimes = 3;

    private Vector3 initialPos;
    private Vector2 intercept;

    private Vector2 velocity;
    private Vector2 lastDirection;

    private float randomTime;
    private float timeToShoot;
    private float currTime;

    [HideInInspector] public bool OnShooting = false;
    [HideInInspector] public bool OnLostPlayer = false;
    [HideInInspector] private bool AfterShoot = false;

    private Coroutine watchTarget;


    #region MonoBehaiver

    public void Start() {
        base.Start();

        intercept = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        velocity = lastDirection = Vector2.zero;

        initialPos = _renderer.localPosition;
        randomTime = Random.value * 3;

        _sfx = GetComponent<StingerSSFX>();
    }

    void LateUpdate() {
        _sfx.Voo();

        if (!OnShooting) {
            currTime += Time.deltaTime;
            applySenoide();
        }

        UpdateAnimation();
    }

    void FixedUpdate() {
        if (!OnShooting) UpdateMove();
        else UpdateMoveSleep();
    }   

        void applySenoide() {
            Vector3 senoid = initialPos + new Vector3(0, 0.1f, 0) * Mathf.Sin((randomTime + currTime) * 4);
            GetComponent<CircleCollider2D>().center = senoid;
            _renderer.localPosition = senoid;
        }

        Vector2 toIntercept;
        void UpdateMove() {
            if (target == null || target.rigidbody2D == null) {
                rigidbody2D.velocity = (Vector2)impulseForce * 0.5f;
                return;
            }

            /* Controlhe de angulo do Stinger Shooter */ {
                if (target.rigidbody2D.velocity != Vector2.zero) 
                    toIntercept = target.GetComponent<PlayerMovementController>().DeadDirection.normalized;

                float currAngle = Mathf.LerpAngle(
                    (360 + Mathf.Atan2(intercept.y, intercept.x) * Mathf.Rad2Deg) % 360,
                    (360 + Mathf.Atan2(toIntercept.y, toIntercept.x) * Mathf.Rad2Deg) % 360,
                    Time.deltaTime 
                );

                intercept.Set(Mathf.Cos(currAngle * Mathf.Deg2Rad), Mathf.Sin(currAngle * Mathf.Deg2Rad));
            }

            Vector2 direction = intercept * DISTANCE_TO_TARGET;

            // Limite de velocidade
            var magnitude = direction.magnitude;
            if (magnitude > MAX_VELOCITY) {
                direction *= (MAX_VELOCITY / magnitude);
            }

            if (impulseForce != Vector3.zero) direction += (Vector2)impulseForce * 0.5f;

            velocity = (direction - lastDirection);
            velocity = velocity.magnitude > 0.01f ? velocity.normalized : Vector2.zero;
            lastDirection = direction;

            direction = Vector2.Lerp(transform.position, (Vector2)target.position + direction, Time.deltaTime * speed);
            
            rigidbody2D.MovePosition(direction);
            //rigidbody2D.velocity = (((Vector2)target.position + direction) - (Vector2) transform.position).normalized * speed;
        }

        void UpdateMoveSleep() {
            velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime);
            rigidbody2D.velocity = velocity + (Vector2)impulseForce * 0.5f;

            //Vector2 direction = Vector2.Lerp(transform.position, (Vector2)target.position + velocity, Time.deltaTime * speed);            
        }

        void UpdateAnimation() {
            Vector2 direction = target != null ?
                (Vector2)(target.position - transform.position).normalized :
                intercept * -1;

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
        if (OnShooting) AfterShoot = true;
    }

    #endregion


    #region Private Methods

    void getTimetoShootAgain() {
        timeToShoot = Random.Range(chanceToShoot[0], chanceToShoot[1]);
    }

    protected void shoot() {
        _sfx.Laser();

        _anim.SetTrigger("Shoot");

        Vector3 direction = new Vector3(_anim.GetFloat("Horizontal"), _anim.GetFloat("Vertical"));

        /*Spawn a shoot */{
            GameObject shootGO = (GameObject)Instantiate(
                Resources.Load<GameObject>("Shoots/EnemySimple"),
                transform.Find("shootspawn").position + direction.normalized * 0.25f,
                Quaternion.identity
            );

            ShootMove shoot = shootGO.GetComponent<ShootMove>();
            shoot.direction = direction;
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
