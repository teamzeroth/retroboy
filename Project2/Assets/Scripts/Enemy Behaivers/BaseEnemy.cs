using UnityEngine;
using System.Collections;

using DG.Tweening;

public class BaseEnemy : MovableBehaviour {

    protected float TIME_IN_DAMAGE = 2;
    protected float DAMAGE_INFLUENCE = 1;

    public int life = 3;
    public int damage = 1;
    public float speed = 1f;

    public bool isColliderDamage = false;

    public Vector2 coinsChance = new Vector2(1, 3);

    public float rangeAtack = 1f;

    [HideInInspector]
    public MovableBehaviour target;
    [HideInInspector]
    public bool dead = false;

    protected Vector3 impulseForce;
    protected Tween impulseTween;

    protected Animator _anim;
    //protected Transform _renderer;
    //protected Rigidbody2D _rigidbody;

    //protected CollisionLevel _collisionLevel;
    protected CollisionLevel player;

    public void Awake() {
        base.Awake();
        _anim = GetComponent<Animator>();
    }

    public void Start() {
        impulseForce = Vector3.zero;
        impulseTween = null;
        target = null;
    }


    public void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeAtack);
#endif
    }


    public virtual void FindPlayer(MovableBehaviour player) { if (target != player) { target = player; } }

    public virtual void LostPlayer(MovableBehaviour player) { if (target == player) target = null; }

    public virtual void OnEnemyBehavior() { }

    public virtual void OnTakeDamage(ShootMove shoot, Collider2D coll) {
        if (impulseTween != null) impulseTween.Kill();

        impulseForce = ((Vector3)shoot.direction * shoot.damage * 8) + impulseForce * DAMAGE_INFLUENCE;
        impulseTween = DOTween.To(() => impulseForce, x => impulseForce = x, Vector3.zero, TIME_IN_DAMAGE).SetEase(Ease.OutCirc).OnComplete(() => impulseTween = null);

        life -= shoot.damage;
        if (life <= 0) {
            OnDestroyIt();
        }
    }

    public virtual void DropCoins() {
        if (coinsChance.x == 0 && coinsChance.y == 0) return;

        int chance = Random.Range((int)Mathf.Max(coinsChance.x, 1), (int)coinsChance.y);

        for (int i = 0; i < chance; i++) {
            Instantiate(
                Resources.Load<GameObject>("Coins/Coin"),
                transform.position + new Vector3(),
                Quaternion.identity
            );
        }
    }

    public void OnFinishAnimation() { OnFinishAnimationBehavior(); }
    public virtual void OnFinishAnimationBehavior() { OnDestroyIt(); }

    public virtual void OnDestroyIt() {
        DropCoins();
        Destroy(gameObject);
    }

    public virtual void OnDistanceWithPlayer(MovableBehaviour player, float distance) {
        if (renderer != null) {
            if (player.Level == Level && (distance <= rangeAtack)) {
                FindPlayer(player);
            } else
                LostPlayer(player);
        }
    }
}
