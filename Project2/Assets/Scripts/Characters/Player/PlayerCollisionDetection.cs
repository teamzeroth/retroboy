using UnityEngine;
using System.Collections;

public class PlayerCollisionDetection : MonoBehaviour {


    PlayerMovementController _player;

    void Start() {
        _player = transform.parent.GetComponent<PlayerMovementController>();
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy"){

            var baseEnemy = other.GetComponent<BaseEnemy>();

            if(other.GetComponent<BaseEnemy>() != null && other.GetComponent<BaseEnemy>().isColliderDamage) 
                _player.OnGetHit(other.GetComponent<BaseEnemy>(), other);

            /*if(other.GetComponent<Enemy>())
                *  _player.OnGetHit(other.GetComponent<Enemy>(), other);
                */

        }
    }
}
