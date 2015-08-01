using UnityEngine;
using System.Collections;

using DG.Tweening;

public class ShootMove : MonoBehaviour {

    public float distance = 0;
    public float speed;
    public int damage;

    public bool isPlayerAlly;
    public bool hasCollider = false;
    public bool hasLostForce = false;

    [HideInInspector]
    public Vector2 direction;
    [HideInInspector]
    public CollisionLevel collisionLevel;
    [HideInInspector]
    public bool flipped;
    [HideInInspector]
    public bool destroied;

    private Vector2 startPosition;
    private Transform _feet;
    private ParticleSystem _particles;
    private ParticleSystem _collisionParticles;
    private Animator _anim;

    #region Getters And Setters

    public Vector2 Direction {
        set {
            if (_feet == null) _feet = transform.GetComponent<SortingOrder>().positionPoint;
            Vector3 local = _feet.position;

            direction = value.normalized;
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            _feet.position = local;
        }
    }

    public float Distance {
        set {
            distance = value;
            startPosition = transform.position;
        }
    }

    #endregion

    #region Core

    void Awake() {
        collisionLevel = GetComponent<CollisionLevel>();

        _anim = GetComponent<Animator>();

        if (transform.Find("Particles"))
            _particles = transform.Find("Particles").GetComponent<ParticleSystem>();

        if (transform.Find("Collision Particles"))
            _collisionParticles = transform.Find("Collision Particles").GetComponent<ParticleSystem>();
    }

    void Start() {
        destroied = false;
        Direction = direction;
    }

    void Update() {
        if (!destroied)
            GetComponent<Rigidbody2D>().velocity = direction * speed;

        if (distance != 0 && Vector2.Distance(startPosition, transform.position) >= distance) {
            DestroyMove(false);
        }
    }

    void LateUpdate() {
        if (_particles == null) return;
        _particles.SetParticlesVelocity(direction.normalized * speed * 0.5f);
    }

    public void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.tag == "Enemy" && isPlayerAlly) {
            CollisionLevel collision = trigger.GetComponent<CollisionLevel>();

            if (collision == null) return;
            if (collision.Level != collisionLevel.Level) return;

            trigger.GetComponent<BaseEnemy>().OnTakeDamage(this, trigger);
            DestroyMove();

        } else if (trigger.tag == "PlayerCollider" && !isPlayerAlly) {
            Player player = GameController.self.player;

            if (player.collisionLevel.Level != collisionLevel.Level) return;

            player.OnGetHit(this, trigger);
            DestroyMove();
        }
    }

    public void OnTriggerListener(Collider2D trigger) {
        if (trigger.gameObject.tag == "Wall") {
            DestroyMove();
        }
    }

    #endregion

    #region Messages

    public void OnFinishDestroyAnimation() {
        destroied = true;
        Destroy(gameObject);
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void DestroyMove(bool collided = true) {
        if (collided) {

            if (_particles != null) _particles.gameObject.SetActive(false);
            if (_collisionParticles != null) setCollisionPartiles();

            if (_anim != null && hasCollider) {
                _anim.SetTrigger("Collided");
            } else {
                OnFinishDestroyAnimation();
            }

            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            destroied = true;

        } else {

            if (_anim != null && hasLostForce) {
                _anim.SetTrigger("LostForce");

                DOTween.To(() => direction, x => direction = x, Vector2.zero, .5f);
            } else {

                DOTween.To(() => direction, x => direction = x, Vector2.zero, .5f).OnComplete(() => {
                    OnFinishDestroyAnimation();
                });
            }
        }
    }

    #endregion

    #region private Methods

    private void setCollisionPartiles() {
        if (_collisionParticles == null) return;

        _collisionParticles.transform.parent = transform.parent;
        _collisionParticles.gameObject.SetActive(true);

        int count = Random.Range(3, 8);
        _collisionParticles.Emit(count);
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[count];
        _collisionParticles.GetParticles(p);
        Vector2 v = direction.normalized * -speed * 0.5f;

        for (int i = 0; i < count; i++)
            p[i].velocity = Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.forward) * v;

        _collisionParticles.SetParticles(p, count);
        Destroy(_collisionParticles.gameObject, 0.5f);
    }

    #endregion
}
