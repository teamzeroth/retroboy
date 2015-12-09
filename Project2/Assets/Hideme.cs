using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class Hideme : MonoBehaviour {

	//http://answers.unity3d.com/questions/344295/colliders-in-children.html

	public GameObject tiles;
	public GameObject tiles2;
	public GameObject tiles3;
	public GameObject tiles4;

	void OnTriggerEnter2D(Collider2D other){

		if (other.CompareTag ("NimFeet")) {
			Debug.Log ("Hide me");

			// 50% Transparency.
			tiles.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 0.6f);
			tiles2.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 0.6f);
			tiles3.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 0.6f);
			tiles4.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 0.6f);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		
		if (other.CompareTag ("NimFeet")) {
			Debug.Log ("Hide me");
			
			// 50% Transparency.
			tiles.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 1f);
			tiles2.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 1f);
			tiles3.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 1f);
			tiles4.GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 1f);
		}
	}
}
