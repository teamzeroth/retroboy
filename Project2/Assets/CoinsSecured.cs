using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

public class CoinsSecured : MonoBehaviour {

	private Animator _anim;
    private Transform countText, child;
    
    void Start()
    {
        countText = transform.Find("Amount");
        child = transform.GetChild(0);
    }

    void Update()
    {
        child.localPosition = (transform.parent.localScale.x < 0)? new Vector3(-0.33f,0.33f,0) : new Vector3(0.33f,0.33f,0);
        child.localScale = transform.parent.localScale;
    }

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
			countText.GetComponent<TextMesh>().text = delta.ToString();
		});
	}
}
