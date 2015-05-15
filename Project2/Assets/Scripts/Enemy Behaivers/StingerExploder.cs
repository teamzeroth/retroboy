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
        //Senoide
        _renderer.localPosition = initialPos + new Vector3(0,0.2f,0) * Mathf.Sin((randomTime + Time.time) * 2);

        if (target != null) UpdateMove();
        UpdateAnimation();
    }

        void UpdateMove(){
            rigidbody2D.velocity = (target.position - transform.position).normalized * speed;
            _renderer.localRotation = Quaternion.Inverse(transform.localRotation);
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
    
    #endregion
}
