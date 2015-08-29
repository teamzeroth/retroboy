﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

public class CoinsSecured : MonoBehaviour {

	private Animator _anim;

	private int _coinsCollected;
	public int CoinsCollected {
		get {
			return _coinsCollected;
		}
		set {
			GetComponent<Animator>().SetTrigger("Collected");
			changeCoinsCollected(_coinsCollected, value);
			_coinsCollected = value;
		}
	}

	Tween coinsCollectedToween;
	private void changeCoinsCollected(int before, int after) {
		if (coinsCollectedToween != null) {
			coinsCollectedToween.Kill();
		}
		
		int delta = before;
		coinsCollectedToween = DOTween.To(() => delta, x => delta = x, after, 0.5f).OnUpdate(() => {
			
			Transform countText = transform.Find("Amount");
			countText.GetComponent<Text>().text = delta.ToString();
		});
	}
}