using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TemSorting : MonoBehaviour {

	SpriteRenderer  tr;
	float layerZindex = 42;

	Transform actualLayer;
	public Transform[] layers;
	public int levelLayer = -1;

	public float layerCorrection = 0f;

	public Transform tilemap;
	public float alturaPersonagem = 0.9f;

	void Awake () {
		tr = GetComponent<SpriteRenderer> ();
		Sort ();
	}

	void OnEnable () {
		tr = GetComponent<SpriteRenderer> ();
		Sort ();
	}
	
	void LateUpdate () {

		if (levelLayer >= 0) {
			transform.SetParent (layers [levelLayer]);
		}

		//ON GAMEPLAY
		//if (!this.gameObject.isStatic) {
			//Sort ();
		//}

		//sortingOrder = y * map.MapRenderParameter.Width + x;


		//float y = (transform.position.y - tilemap.position.y);
		//float x = (transform.position.x - tilemap.position.x);
		//float sortingOrder = (y * 40 + x);
		//float sortingOrder = (transform.position.y * 40*2 + transform.position.x);
		//float sortingOrder = transform.position.y * 0.04f + transform.position.x;
		float sortingOrder = transform.position.y * 2f + transform.position.x;

		//transform.position = new Vector3 (transform.position.x, transform.position.y, -sortingOrder/1000);
		//transform.position = new Vector3 (transform.position.x, transform.position.y, ((sortingOrder/1000) + alturaPersonagem));
		//transform.position = new Vector3(transform.position.x, transform.position.y, -((float)sortingOrder)/1000 + alturaPersonagem);
		//transform.position = new Vector3(transform.position.x, transform.position.y, sortingOrder/100 * 4 + alturaPersonagem);

		transform.position = new Vector3(transform.position.x, transform.position.y, sortingOrder/100 + alturaPersonagem);
	}

	void Sort () {
		//tr.sortingOrder = (int) Camera.main.WorldToScreenPoint(tr.bounds.min).y * -1;

		//Debug.Log ("SORTED");

		if(tr)
			tr.sortingOrder = 0;

		float newz; 

		if (tr) {
			newz = Camera.main.WorldToScreenPoint (tr.bounds.min).y;
		} else {
			//Floor tiles (Mesh Renderer)
			newz = Camera.main.WorldToScreenPoint (transform.position).y;
		}

		if (transform.parent) {

			switch (transform.parent.name) {

				case "layer-1":
					newz -= layerZindex;
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

			transform.position = new Vector3 (transform.position.x, transform.position.y, newz + layerCorrection);
		}
	}
}
