using UnityEngine;
using System.Collections;

using DG.Tweening;

public class ShootMove : MonoBehaviour {

    public Vector2 direction;

    public float speed;
    public int damage;

    public bool isPlayerAlly;

    [HideInInspector]
    public bool flipped;
    [HideInInspector]
    public bool destroied;

    private ParticleSystem _particles;
    private ParticleSystem _collisionParticles;
    private Animator _anim;

    #region MonoBehaviour

    void Start() {
        destroied = false;
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        if(transform.Find("Particles")) _particles = transform.Find("Particles").particleSystem;
        if(transform.Find("Collision Particles")) _collisionParticles = transform.Find("Collision Particles").particleSystem;

        _anim = GetComponent<Animator>();
    }

    void Update() {
        if (!destroied)
            rigidbody2D.velocity = direction.normalized * speed;
    }

    void LateUpdate() {
        if (_particles == null) return;
            _particles.SetParticlesVelocity(direction.normalized * speed * 0.5f);
    }

    public void OnTriggerEnter2D(Collider2D trigger){
        if (trigger.tag == "Enemy" && isPlayerAlly) {

            if (trigger.GetComponent<Enemy>() != null) {
                trigger.GetComponent<Enemy>().Hit(damage, direction);
                DestroyMove();
            }

            if (trigger.GetComponent<BaseEnemy>() != null) {
                trigger.GetComponent<BaseEnemy>().OnTakeDamage(this, trigger);
                DestroyMove();
            }

        }

        if (trigger.tag == "Player" && !isPlayerAlly) {
			print("Collide with " + trigger.gameObject.name);
            //trigger.GetComponent<PlayerMovementController>().OnGetHit();
            DestroyMove();
        }

        if (trigger.gameObject.layer == LayerMask.NameToLayer("Level")) {
            DestroyMove();
        }
    }   

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

    public void OnFinishDestroyAnimation() {
        Destroy(gameObject);
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void DestroyMove() {
		if (!isPlayerAlly || _anim == null)
			OnFinishDestroyAnimation();
		else
			_anim.SetTrigger("Collided");

        if (_particles != null) {
            setCollisionPartiles();
            _particles.gameObject.SetActive(false);
        }

        destroied = true;
        rigidbody2D.velocity = Vector3.zero;
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
