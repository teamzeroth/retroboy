using UnityEngine;
using System.Collections;

public class SortingLayer : MonoBehaviour {
	
	void Awake(){

		foreach(Transform child in transform){
			child.gameObject.AddComponent<SortingStart>();
		}

	}

}
