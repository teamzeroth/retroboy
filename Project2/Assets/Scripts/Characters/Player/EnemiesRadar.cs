using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemiesRadar : MonoBehaviour {

    List<BaseEnemy> findedEnemys;
    Player _player;
    CircleCollider2D _collider;
    float _baseRadius;
    IEnumerator _revertRadius;
    Tween radiusTween;

    bool findPlayer = false;

    public void Start() {
        findedEnemys = new List<BaseEnemy>();
        _player = transform.parent.GetComponent<Player>();
        _collider = transform.GetComponent<CircleCollider2D>();
        _baseRadius = _collider.radius;
    }

    private void setEnemyDistance() {
        if (_player.Dead) return;

        foreach (BaseEnemy enemy in findedEnemys) {
            float dist = Vector2.Distance(_player.transform.position, enemy.transform.position);
            enemy.OnDistanceWithPlayer(_player, dist);
        }
    }

    public void Update() {
        setEnemyDistance();

        if (findedEnemys.Count == 0) {
            SoundTrackController.self.SetBackgroundSound(3);
            return;
        }


        bool findedPlayer = findedEnemys.Exists(enemy => enemy.target != null);

        if (findedPlayer && !findPlayer) {
            findPlayer = findedPlayer;
            SoundTrackController.self.SetBackgroundSound(2);

            return;
        }

        if (!findedPlayer && findPlayer) {
            findPlayer = findedPlayer;
            SoundTrackController.self.SetBackgroundSound(3);
            return;
        }

        SoundTrackController.self.SetBackgroundSound(1);
    }

    IEnumerator defaultRadius(float inc, float time) {
        
        float delta = _collider.radius;
        radiusTween = DOTween.To(() => delta, x => delta = x, _collider.radius + inc, 1f).OnUpdate(() => {
            _collider.radius = delta;
        });

        yield return new WaitForSeconds(time);

        delta = _collider.radius;
        radiusTween = DOTween.To(() => delta, x => delta = x, _baseRadius, 1f).OnUpdate(() => {
            _collider.radius = delta;
        });

        StopCoroutine(_revertRadius);            
    }

    public void increaseRadius(float inc, float duration) {
        _revertRadius = defaultRadius(inc, duration);
        StartCoroutine(_revertRadius);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();

        if (enemy == null) return;

        if (findedEnemys.IndexOf(enemy) == -1) {
            findedEnemys.Add(enemy);
            ListenDestroy listenDestroy = enemy.gameObject.AddComponent<ListenDestroy>();
            listenDestroy.radar = this;
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null) findedEnemys.Remove(enemy);
    }

    public void OnDestroyEnemy(BaseEnemy enemy) {
        findedEnemys.Remove(enemy);
    }
}