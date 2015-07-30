﻿using UnityEngine;
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

    public Vector2 Direction {
        set {
            if (_feet == null) _feet = transform.GetComponent<SortingMoveableLayer>().positionPoint;
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

    #region MonoBehaviour

    void Awake() {
        collisionLevel = GetComponent<CollisionLevel>();

        _anim = GetComponent<Animator>();

        if (transform.Find("Particles"))
            _particles = transform.Find("Particles").GetComponent<ParticleSystem>();
        if (transform.Find("Collision Particles"))
            _collisionParticles = transform.Find("Collision Particles").GetComponent<ParticleSystem>();
    }

    void Start() {
        //createFeetCollider();

        destroied = false;
        Direction = direction;
    }

    void createFeetCollider() {
        if (transform.GetComponent<SortingMoveableLayer>() == null) return;

        _feet = transform.GetComponent<SortingMoveableLayer>().positionPoint;

        CircleCollider2D feetCollider = _feet.gameObject.AddComponent<CircleCollider2D>();
        CollisionListener listener = _feet.gameObject.AddComponent<CollisionListener>();

        feetCollider.radius = Mathf.Min(GetComponent<Collider2D>().bounds.extents.x, GetComponent<Collider2D>().bounds.extents.y);
        feetCollider.isTrigger = GetComponent<Collider2D>().isTrigger;

        listener.Layer = LayerMask.NameToLayer("Level");
        listener.Father = gameObject;
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

        CollisionLevel collision = trigger.GetComponent<CollisionLevel>();

        if (isPlayerAlly) print(trigger.name + " " + trigger.tag + " " + isPlayerAlly);

        if (collision != null && (collisionLevel == null || collision.Level == collisionLevel.Level)) {
            if (trigger.tag == "Enemy" && isPlayerAlly) {
                if (trigger.GetComponent<Enemy>() != null) {
                    trigger.GetComponent<Enemy>().Hit(damage, direction);
                    DestroyMove();
                }
            }
            if (trigger.GetComponent<BaseEnemy>() != null) {
                trigger.GetComponent<BaseEnemy>().OnTakeDamage(this, trigger);
                DestroyMove();
            }
            // if the shoot collides with the wall ,it's destroyed.
            if (trigger.gameObject.tag == "Wall") {
                DestroyMove();
            }
            if (trigger.tag == "Player" && !isPlayerAlly) {
                //Player player = trigger.GetComponent<Player>();
                //if (collisionLevel.Level == player.collisionLevel.Level) {
				GameController.self.player.OnGetHit(this, trigger);
                DestroyMove();
                //}
            }
        }
    }
    /*if (trigger.gameObject.layer == LayerMask.NameToLayer("Level")) {
        DestroyMove();
    }*/

    /*public void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player" && !isPlayerAlly) {
            coll.gameObject.GetComponent<AnimationController>().Hit(damage, direction);
            DestroyMove();
        }

        if (coll.gameObject.layer == LayerMask.NameToLayer("Level")) {
            DestroyMove();
        }   
    }*/

    #endregion

    #region Messages
    /*
    public void OnCollisionListener(Collider2D trigger) {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("Level")) {
            DestroyMove();
        }
    }*/

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
