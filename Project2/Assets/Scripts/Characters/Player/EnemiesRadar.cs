using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesRadar : MonoBehaviour {
	
	List<BaseEnemy> findedEnemys;
	Player _player;
	
	bool findPlayer = false;
	
	public void Start() {
		findedEnemys = new List<BaseEnemy>();
		_player = transform.parent.GetComponent<Player>();
	}
	public void Update() {
		for (int i = 0; i < findedEnemys.Count; i++) {
			if (!_player.OnDie) {
				findedEnemys[i].OnDistanceWithPlayer(
					transform,
					Vector2.Distance(transform.parent.position, findedEnemys[i].transform.position)
					);
			} else {
				findedEnemys[i].LostPlayer(_player.transform);
			}
		}
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