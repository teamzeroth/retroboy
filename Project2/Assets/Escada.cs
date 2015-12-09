using UnityEngine;
using System.Collections;

public class Escada : MonoBehaviour {

	bool startUp = false;
	bool startDown = false;

	Transform player;
	Player pi;
	//CollisionLevel cl;

	public float vel = 0.1f;

	void Awake(){
		pi = GameObject.FindObjectOfType<Player> ();
		//cl = pi.gameObject.GetComponent<CollisionLevel> ();
	}

	void OnTriggerEnter2D(Collider2D col){
		Debug.Log("Escada:"+this.gameObject.tag);
		if (col.CompareTag ("NimFeet")) {

			if(col.transform.position.y < transform.position.y){
				Debug.Log("Subindo");
				startUp = true;
			}else{
				Debug.Log("Descendo");
				startDown = true;
			}

			player = col.transform.parent;
			pi.climbing = true;
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.CompareTag ("NimFeet")) {

			if(startUp){
				pi.GoLeft();
			}else{
				pi.GoRight();
			}

			startUp = false;
			startDown = false;
			pi.climbing = false;
		}
	}

	void Update(){
		if (player && startUp && pi.Moving()) {
			pi.InputDirection(Vector2.zero);
			player.Translate(0f, vel * Time.deltaTime, 0f);
		}

		if (player && startDown && pi.Moving()) {
			pi.InputDirection(Vector2.zero);
			player.Translate(0f, -vel * Time.deltaTime, 0f);
		}
	}

	void LateUpdate(){

		if (player && (startUp || startDown) && pi.Moving()) {
			player.position = new Vector3(transform.position.x, player.position.y, player.position.z);
		}

	}
}
