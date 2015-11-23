using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour {

    Player _player;

    void Start() {
        _player = transform.parent.GetComponent<Player>();
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy"){
            var baseEnemy = other.GetComponent<BaseEnemy>();

            if(other.GetComponent<BaseEnemy>() != null && other.GetComponent<BaseEnemy>().isColliderDamage) 
                _player.OnGetHit(other.GetComponent<BaseEnemy>(), other);
        }
    }
}
