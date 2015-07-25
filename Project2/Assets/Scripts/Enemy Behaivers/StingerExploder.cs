using UnityEngine;
using System.Collections;

public class StingerExploder : BaseEnemy {

    protected static float MIN_DISTANCE = 0.5f;
    protected Transform _explosion;
    protected StingerESFX _sfx;

    private Vector3 initialPos;
    private float randomTime;

    bool destroied = false;
    
    #region MonoBehaiver

    public void Start() {
        base.Start();

        _explosion = transform.Find("explosion");
        _sfx = GetComponent<StingerESFX>();

        initialPos = _renderer.localPosition;
        randomTime = Random.value * 3;
    }

    void Update() {
        if (destroied) return;

        float currDistance = target != null ? Vector2.Distance(target.position, transform.position) : rangeAtack;

        applySenoide();

        UpdateMove();
        UpdateAnimation(currDistance);

        _renderer.localRotation = Quaternion.Inverse(transform.localRotation);

        //if (currDistance <= 0.1f) OnDestroyIt();
    }

        void applySenoide() {
            Vector3 senoid = initialPos + new Vector3(0, 0.2f, 0) * Mathf.Sin((randomTime + Time.time) * 2);

            GetComponent<CircleCollider2D>().offset = senoid;

            _renderer.localPosition = senoid;
            _explosion.localPosition = senoid;
        }

        void UpdateMove(){
            Vector3 direction = Vector3.zero;

            if (target != null)direction = (target.position - transform.position).normalized * speed;
            Vector3 currentDirection = impulseForce != Vector3.zero ? (direction + impulseForce * 0.75f) / 2 : direction;

            GetComponent<Rigidbody2D>().MovePosition(transform.position + currentDirection * Time.deltaTime);
        }

        void UpdateAnimation(float currDistance) {
            if (target != null) {
                _anim.SetTrigger("Tracking");
                _anim.SetFloat("Distance", currDistance / rangeAtack);

                _sfx.Explosion(currDistance / rangeAtack);
            }
        }

    #endregion

    #region BaseEnemy

    public override void OnTakeDamage(ShootMove shoot, Collider2D coll){
        base.OnTakeDamage(shoot, coll);
        if(life > 0) _sfx.Hit();
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

        public void OnDestroyHimself() {
            destroied = true;

            //rigidbody2D.velocity = Vector2.zero;

            _renderer.gameObject.SetActive(false);
            _explosion.gameObject.SetActive(true);

            _sfx.Explosion(0.06f);
        }

    public void OnFinishSimpleAnimation() {
        StartCoroutine(WaitExplosionSound());
        //Destroy(gameObject);
    }

        IEnumerator WaitExplosionSound() {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

    public override void OnFinishAnimationBehavior() {
        if (Vector2.Distance(target.position, transform.position) <= MIN_DISTANCE)
            OnDestroyHimself();
    }
    
    #endregion
}
