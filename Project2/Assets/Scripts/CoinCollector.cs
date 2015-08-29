using UnityEngine;
using System.Collections;

public class CoinCollector : MonoBehaviour {

	private CoinsSecured _collector;

	void start() {
		_collector = transform.parent.gameObject.GetComponent<CoinsSecured>();
	}

    public void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Coin")
            getCoin(coll.gameObject.GetComponent<CoinMove>());
    }

    private void getCoin(CoinMove coin) {
        UiController.self.Coins += coin.quant;
		_collector.CoinsCollected += coin.quant;
		//UiController.self.moveCoinsCollectedUI();
		coin.secureCoin();
    }
}