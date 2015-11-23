using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class HidePlayer : MonoBehaviour {

	SortingOrder playerSorting;
	SpriteRenderer sr;
	int originalSort;

	void Awake () {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		playerSorting = player.GetComponent<SortingOrder> ();
		sr = player.GetComponent<SpriteRenderer> ();
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag ("NimFeet")) {
			Debug.Log("Enter to hide");

			playerSorting.Position = 0f;

			originalSort = sr.sortingOrder;
			sr.sortingOrder = 2;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.CompareTag ("NimFeet")) {
			Debug.Log("Exit");

			playerSorting.forcePosition = false;

			sr.sortingOrder = originalSort;
		}
	}
}
