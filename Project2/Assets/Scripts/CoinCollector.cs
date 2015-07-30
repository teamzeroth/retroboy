using UnityEngine;
using System.Collections;

public class CoinCollector : MonoBehaviour {
	
	public void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log("Collected");
		if (coll.gameObject.tag == "Coin")
			getCoin(coll.gameObject.GetComponent<CoinMove>());
	}
	private void getCoin(CoinMove coin) {
		UiController.self.Coins += coin.quant;
		Destroy(coin.gameObject);
	}
}