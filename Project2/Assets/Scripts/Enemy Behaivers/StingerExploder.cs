using UnityEngine;
using System.Collections;

public class StingerExploder : BaseEnemy {

    public float distanceExplosion = 1.2f;

    protected Transform _explosion;
    protected StingerESFX _sfx;

    private Vector3 initialPos;
    private float randomTime;

    #region MonoBehaiver

    public void Awake() {
        base.Awake();

        _explosion = transform.Find("explosion");
        _sfx = GetComponent<StingerESFX>();
    }

    public void Start() {
        base.Start();

        initialPos = _renderer.localPosition;
        randomTime = Random.value * 3;
    }

    void Update() {
        if (dead) return;

        if (GameController.self.player.collisionLevel.Level != _collisionLevel.Level) target = null;
        float currDistance = target != null ? Vector2.Distance(target.position, transform.position) : rangeAtack;

        applySenoide();
        UpdateMove();
        UpdateAnimation(currDistance);

        //_renderer.localRotation = Quaternion.Inverse(transform.localRotation);

        //if (currDistance <= 0.1f) OnDestroyIt();
    }

    void applySenoide() {
        Vector3 senoid = initialPos + new Vector3(0, 0.2f, 0) * Mathf.Sin((randomTime + Time.time) * 2);

        GetComponent<CircleCollider2D>().offset = senoid;

        _renderer.localPosition = Helper.IgnoreZ(_renderer.localPosition, senoid);
        _explosion.localPosition = Helper.IgnoreZ(_explosion.localPosition, senoid);
    }

    void UpdateMove() {
        Vector3 direction = Vector3.zero;

        if (target != null) direction = (target.position - transform.position).normalized * speed;
        Vector3 currentDirection = impulseForce != Vector3.zero ? (direction + impulseForce * 0.75f) / 2 : direction;

        //GetComponent<Rigidbody2D>().MovePosition(transform.position + currentDirection * Time.deltaTime);
        //GetComponent<Rigidbody2D>().AddForce(direction);
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, currentDirection, Time.deltaTime);

    }

    void UpdateAnimation(float currDistance) {
        if (target != null) {
            _anim.SetTrigger("Tracking");
            _anim.SetFloat("Distance", currDistance / rangeAtack);

            _sfx.Explosion(currDistance / rangeAtack);
        }
    }

    public void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        if (!Application.isPlaying) Gizmos.DrawWireSphere(transform.position, distanceExplosion);
        else Gizmos.DrawWireSphere(transform.position + _renderer.localPosition, distanceExplosion);
#endif
    }

    #endregion

    #region BaseEnemy

    public override void OnTakeDamage(ShootMove shoot, Collider2D coll) {
        base.OnTakeDamage(shoot, coll);
        if (life > 0) _sfx.Hit();
    }

    public override void FindPlayer(Transform player) {
        if (target != player) {
            target = player;
            _sfx.Explosion(1);
        }
    }

    public override void LostPlayer(Transform player) { }

    public override void OnDestroyIt() {
        DropCoins();
        OnDestroyHimself();
    }

    public override void OnFinishAnimationBehavior() {
        if (Vector2.Distance(target.position, transform.position + _renderer.localPosition) <= distanceExplosion)
            OnDestroyHimself();
    }

    #endregion

    public void OnDestroyHimself() {
        dead = true;

        _rigidbody.velocity = Vector2.zero;

        _renderer.gameObject.SetActive(false);
        _explosion.gameObject.SetActive(true);

        _sfx.Explosion(0.06f);
    }

    public void OnFinishSimpleAnimation() {
        Destroy(gameObject);
    }

}
