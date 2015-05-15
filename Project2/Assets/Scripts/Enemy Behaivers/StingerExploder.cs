using UnityEngine;
using System.Collections;

public class StingerExploder : BaseEnemy {

    private Vector3 initialPos;
    private float randomTime;

    #region MonoBehaiver

    public void Start() {
        base.Start();

        initialPos = _renderer.localPosition;
        randomTime = Random.value * 3;
    }

    void Update() {
        _renderer.localPosition = initialPos + new Vector3(0,0.2f,0) * Mathf.Sin((randomTime + Time.time) * 2);

        UpdateMove();
        UpdateAnimation();

        _renderer.localRotation = Quaternion.Inverse(transform.localRotation);
    }

        void UpdateMove(){
            //rigidbody2D.velocity = (target.position - transform.position).normalized * speed;transform.position + (target.position - transform.position).normalized * speed * Time.deltaTime
            Vector3 direction = Vector3.zero;

            if (target != null) direction = (target.position - transform.position).normalized * speed;
            Vector3 currentDirection = (direction + impulseForce) / 2;

            print(impulseForce);

            rigidbody2D.MovePosition(transform.position + currentDirection * Time.deltaTime);
        }

        void UpdateAnimation() {
            var currDistance = target != null ? Vector2.Distance(target.position, transform.position) : 1;

            if(target != null) _anim.SetTrigger("Tracking");
            _anim.SetFloat("Distance", currDistance / rangeAtack);
        }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeAtack);
    }

    #endregion

    #region BaseEnemy

    public override void LostPlayer(Transform player) { }

    //public override void OnDestroy() { }
    
    #endregion
}
