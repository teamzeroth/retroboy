using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour {
    
    public float life = 3f;
    public float damage = 2f;
    public float speed = 1f;

    public float rangeAtack = 1f;

    [HideInInspector]
    public Transform target;

    protected Animator _anim;
    protected Transform _renderer;

    public void Start() {
        _anim = GetComponent<Animator>();
        _renderer = transform.Find("renderer");
    }

    public virtual void FindPlayer(Transform player) { target = player; }

    public virtual void LostPlayer(Transform player) { target = null; }

    public virtual void OnEnemyBehavior() { }

    public virtual void OnTakeDamage(ShootMove shoot, Collision2D coll) { }

    public virtual void OnDistanceWithPlayer(Transform player, float distance) {
        if (distance <= rangeAtack) FindPlayer(player);
        else LostPlayer(player);
    }
}
