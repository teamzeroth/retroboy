using UnityEngine;
using System.Collections;

public class EnemiesRadar : MonoBehaviour {

    Player _player;

    public void Start() {
        _player = transform.parent.GetComponent<Player>();
    }

    public void OnTriggerStay2D(Collider2D other) {
        if (
            other.gameObject.layer != LayerMask.NameToLayer("Enemies") &&
            other.gameObject.layer != LayerMask.NameToLayer("IgnoreEnemies")
        ) return;

        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy == null) return;

        if (!_player.OnDie) {
            enemy.OnDistanceWithPlayer(
                transform,
                Vector3.Distance(transform.parent.position, other.transform.position)
            );
        } else {
            enemy.LostPlayer(_player.transform);
        }
    }
}
