using UnityEngine;
using System.Collections;

public class Teletransport : MonoBehaviour {

	public Teletransport where;
	Collider2D collider;

	public int order;
	public bool active = true;

	void Awake(){
		collider = GetComponent<Collider2D> ();


	}

	void OnTriggerEnter2D(Collider2D other){

		if (other.CompareTag ("NimFeet") && active) {

			where.active = false;

			Transform player = other.transform.parent;
			player.position = where.transform.position;
			player.GetComponent<SpriteRenderer>().sortingOrder = where.order;
			player.GetComponent<CollisionLevel>().Level = where.order;
		}

	}

	void OnTriggerExit2D(Collider2D other){

		if (other.CompareTag ("NimFeet")) {
			active = true;
		}

	}
}
