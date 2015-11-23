using UnityEngine;
using System.Collections;

public class TemSortingLayer : MonoBehaviour {

	SpriteRenderer  tr;
	public float layerZindex = 40;

	void Awake () {
		tr = GetComponent<SpriteRenderer> ();

		transform.position = Vector3.zero;

		foreach(Transform child in transform){
			Sort (child.gameObject);
		}
	}

	void Start () {

		foreach(Transform child in transform){
			Sort (child.gameObject);
		}
	}

	void Sort (GameObject go) {
		//tr.sortingOrder = (int) Camera.main.WorldToScreenPoint(tr.bounds.min).y * -1;

		tr = go.GetComponent<SpriteRenderer> ();
		tr.sortingOrder = 0;

		float newz = Camera.main.WorldToScreenPoint(tr.bounds.min).y + 200f;

		switch (transform.name) {
			case "layer-1":
				newz -= layerZindex * 1;
				break;
			case "layer0":
				newz -= layerZindex * 2;
				break;
			case "layer1-floor":
				newz -= layerZindex * 3;
				break;
			case "layer2":
				newz -= layerZindex * 4;
				break;
			case "layer3":
				newz -= layerZindex * 5;
				break;
			case "layer4-floor":
				newz -= layerZindex * 6;
				break;
			case "layer5":
				newz -= layerZindex * 7;
				break;
			case "layer6":
				newz -= layerZindex * 8;
				break;
			case "layer7-floor":
				newz -= layerZindex * 9;
				break;
			case "layer8":
				newz -= layerZindex * 10;
				break;
		}

		go.transform.position = new Vector3 (go.transform.position.x, go.transform.position.y, newz);
	}
}
