using UnityEngine;
using System.Collections;

using DG.Tweening;

public class BaseEnemy : MonoBehaviour {

    protected float TIME_IN_DAMAGE = 2;

    public float life = 3f;
    public float damage = 2f;
    public float speed = 1f;

    public Vector2 coinsChange = new Vector2(1, 3);

    public float rangeAtack = 1f;

    [HideInInspector]
    public Transform target;

    protected Vector3 impulseForce;
    protected Tween impulseTween;

    protected Animator _anim;
    protected Transform _renderer;

    public void Start() {
        impulseForce = Vector3.zero;
        impulseTween = null;
        target = null;

        _anim = GetComponent<Animator>();
        _renderer = transform.Find("renderer");
    }

    public void Update() {
        calcImpulseForce();
    }

    private void calcImpulseForce(){

    }

    public virtual void FindPlayer(Transform player) { target = player; }

    public virtual void LostPlayer(Transform player) { target = null; }

    public virtual void OnEnemyBehavior() { }

    public virtual void OnTakeDamage(ShootMove shoot, Collider2D coll) {
        if (impulseTween != null) impulseTween.Kill();

        impulseForce = ((Vector3)shoot.direction * shoot.damage * 8) + impulseForce;
        impulseTween = DOTween.To(() => impulseForce, x => impulseForce = x, Vector3.zero, TIME_IN_DAMAGE).SetEase(Ease.OutCirc).OnComplete(() => impulseTween = null);
    
        life -= shoot.damage;
        if (life <= 0) {
            OnDestroyIt();
        }
    }

    public virtual void OnDestroyIt() {
        int chance = Random.Range((int) Mathf.Max(coinsChange.x, 1), (int) coinsChange.y);
        
        for (int i = 0; i < chance; i++) {
            Instantiate(
                Resources.Load<GameObject>("Coins/Coin"),
                transform.position + new Vector3(),
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    public virtual void OnDistanceWithPlayer(Transform player, float distance) {
        if (distance <= rangeAtack) FindPlayer(player);
        else LostPlayer(player);
    }
}
