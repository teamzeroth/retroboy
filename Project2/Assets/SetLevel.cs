using UnityEngine;
using System.Collections;

public class SetLevel : MonoBehaviour {

	public int level;

	//Child colliders must have a rigidbody to distinguish from parent

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log(this.gameObject.tag);
		if (other.CompareTag ("NimFeet")) {
			Debug.Log("Nim enter");
			other.transform.parent.gameObject.GetComponent<CollisionLevel>().Level = level;
		}
	}
}
