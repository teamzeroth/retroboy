using UnityEngine;
using System.Collections;

public class OrderRelatedPlayer : MonoBehaviour {

	Transform pt;
	SpriteRenderer pr;

	SpriteRenderer sr;

	CollisionLevel cl;
	CollisionLevel pcl;

	int sortingOrder = 1;

	GameObject player;

	void Awake(){
		cl = GetComponent<CollisionLevel>();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Start () {
		pt = player.transform;
		pr = player.GetComponent<SpriteRenderer> ();
		pcl = player.GetComponent<CollisionLevel> ();

		sr = GetComponent<SpriteRenderer> ();
	}
	
	void LateUpdate () {

		//sortingOrder = -(int)(transform.position.y * 1000);

		sr.sortingOrder = -((int)(Camera.main.WorldToScreenPoint (sr.bounds.min).y * 10) - (cl.Level * 1000));
		pr.sortingOrder = -((int)(Camera.main.WorldToScreenPoint (pr.bounds.min).y * 10) - (pcl.Level * 1000));

		/*if (cl) {
			Sortme();
		} else {
			CheckPosition();
		}*/

	}

	void Sortme(){
		if (cl.Level == pcl.Level) {
			
			//same level
			CheckPosition();
			
		} else if (cl.Level < pcl.Level) {
			
			//level down
			sr.sortingOrder = pr.sortingOrder - sortingOrder;
			
		} else {
			
			//level up
			sr.sortingOrder = pr.sortingOrder + sortingOrder;
			
		}
	}

	void CheckPosition(){

		if(pt.position.y > transform.position.y)
			sr.sortingOrder = pr.sortingOrder + sortingOrder;
		else
			sr.sortingOrder = pr.sortingOrder - sortingOrder;

	}


}
