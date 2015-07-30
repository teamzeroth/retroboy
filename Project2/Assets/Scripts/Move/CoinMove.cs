
using UnityEngine;
using System.Collections;

using DG.Tweening;

public class CoinMove : MonoBehaviour {
	
	static public float TOTAL_TIME_OF_LIVE = 10f; //+ 3 
	
	public int quant = 10;
	public Transform feet;
	public float fallingDuration;
	public float extraX;
	public float movingDuration;
	void Start() {
		Move ();
	}
	
	private void Move() {
		transform.DOMove (feet.position , fallingDuration, false).SetEase (Ease.OutBounce);
		//transform.DOMove (transform.position + transform.right *(extraX * (Random.value-0.5f)),movingDuration,false).SetEase(Ease.Linear);
	}
	
}