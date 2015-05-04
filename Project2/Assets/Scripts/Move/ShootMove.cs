using UnityEngine;
using System.Collections;

using DG.Tweening;

public class ShootMove : MonoBehaviour {

    public Vector2 direction;

    public float speed;
    public float damage;

    public bool isPlayerAlly;

    [HideInInspector]
    public bool flipped;
    [HideInInspector]
    public bool destroied;

    private ParticleSystem _particles;
    private ParticleSystem _collisionParticles;
    private Animator _anim;

    void Start() {
        destroied = false;
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        if(transform.Find("Particles"))
            _particles = transform.Find("Particles").particleSystem;
        
        if(transform.Find("Collision Particles"))
            _collisionParticles = transform.Find("Collision Particles").particleSystem;

        _anim = GetComponent<Animator>();
    }

    void Update() {
        if (!destroied)
            rigidbody2D.velocity = direction.normalized * speed;
    }

    void LateUpdate() {
        if (_particles == null) return;
        setParticlesVelocity(direction.normalized * speed * 0.5f);
    }

    public void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Enemy" && isPlayerAlly)
            trigger.gameObject.GetComponent<Enemy>().Hit(damage, direction);        
    }

    private void setParticlesVelocity(Vector2 velocity){
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[_particles.particleCount + 1];
        int l = _particles.GetParticles(p);

        for (int i = 0; i < l; i++)
            p[i].velocity = velocity * (1 + 0.3f * Random.value);

        _particles.SetParticles(p, l);
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        
        if (coll.gameObject.tag == "Player" && !isPlayerAlly)
            coll.gameObject.GetComponent<AnimationController>().Hit(damage, direction);

        DestroyMove(-coll.contacts[0].normal);
    }

    public void DestroyMove(Vector2 contactDirection) {
        if (_particles != null) {
//            _collisionParticles.transform.parent = transform.parent;
            //transform.DetachChildren();
            setCollisionPartiles(contactDirection);
            _particles.gameObject.SetActive(false);
            
            DOTween.To(x => setParticlesVelocity(direction.normalized * speed * x), 0.5f, 0, 0.5f).SetEase(Ease.OutQuint);
            
//            Destroy(_collisionParticles.gameObject, 0.5f);
        }

        destroied = true;
        rigidbody2D.velocity = Vector3.zero;
//        _anim.SetTrigger("Collided");
    }

    private void setCollisionPartiles(Vector2 contactDirection) {
        if (_collisionParticles == null) return;
        _collisionParticles.transform.parent = transform.parent;
        _collisionParticles.gameObject.SetActive(true);

        int count = Random.Range(4, 6);
        _collisionParticles.Emit(count);
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[count];
        _collisionParticles.GetParticles(p);

        Vector2 v = contactDirection * -speed * 0.5f;

        for (int i = 0; i < count; i++) {
            p[i].velocity = Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward) * v;
            print(p[i].velocity);
        }

        _collisionParticles.SetParticles(p, count);
    }

    public void OnFinishDestroyAnimation() {
        Destroy(gameObject);
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnBecameInvisible(){
        // Destroy(gameObject);
    }
}
