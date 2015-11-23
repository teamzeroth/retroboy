using UnityEngine;
using System.Collections;
using Fungus;

public class TriggerDialog : MonoBehaviour {

	public Flowchart flow;
	public string fungusMessage;

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("PlayerCollider")) {
			flow.SendFungusMessage(fungusMessage);

			//Inactive this cutscene
			GetComponent<Collider2D>().enabled = false;
		}
	}
}