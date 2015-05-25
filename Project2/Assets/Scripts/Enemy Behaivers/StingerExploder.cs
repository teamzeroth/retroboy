using UnityEngine;
using System.Collections;

public class StingerExploder : BaseEnemy {

    protected static float MIN_DISTANCE = 0.5f;
    protected Transform _explosion;

    private Vector3 initialPos;
    private float randomTime;

    bool destroied = false;
    
    #region MonoBehaiver

    public void Start() {
        base.Start();

        _explosion = transform.Find("explosion");

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

            GetComponent<CircleCollider2D>().center = senoid;

            _renderer.localPosition = senoid;
            _explosion.localPosition = senoid;
        }

        void UpdateMove(){
            Vector3 direction = Vector3.zero;

            if (target != null) direction = (target.position - transform.position).normalized * speed;
            Vector3 currentDirection = impulseForce != Vector3.zero ? (direction + impulseForce) / 2 : direction;

            rigidbody2D.MovePosition(transform.position + currentDirection * Time.deltaTime);
        }

        void UpdateAnimation(float currDistance) {
            if(target != null) _anim.SetTrigger("Tracking");
            _anim.SetFloat("Distance", currDistance / rangeAtack);
        }

    #endregion

    #region BaseEnemy

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
        }

    public void OnFinishSimpleAnimation() {
        Destroy(gameObject);
    }

    public override void OnFinishAnimationBehavior() {
        if (Vector2.Distance(target.position, transform.position) <= MIN_DISTANCE)
            OnDestroyHimself();
    }
    
    #endregion
}
