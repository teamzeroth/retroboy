
using UnityEngine;
using System.Collections;

using DG.Tweening;

public class CoinMove : MovableBehaviour {

    const float TIME_OF_FALLING = 1;

    static public float TOTAL_TIME_OF_LIVE = 10f; //+ 3 

	Animator _anim;

    public int quant = 10;
    public Transform feet;
    public float fallingDuration;
    public float extraX;
    public float movingDuration;

    float startY;
    float curY;

    void Start() {
		_anim = GetComponent<Animator>();
        Move();
    }

    private void Move() {
        /*startY = transform.position.y;
        Vector3 toPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        DG.Tweening.Core.DOGetter<float> getter = () => curY;
        DG.Tweening.Core.DOSetter<float> setter = x => {
            curY = x;
        };


        Tweener t = DOTween.To(getter, x => {
            curY = x;
            toPosition * t.ElapsedPercentage;
            transform.position = new Vector3();
        }, Feet.localPosition, 1);*/

        Vector3 endValue = feet.position + new Vector3(Random.value, Random.value, 0);
        transform.DOMove(endValue, fallingDuration, false).SetEase(Ease.OutBounce);
        //transform.DOMove (transform.position + transform.right *(extraX * (Random.value-0.5f)),movingDuration,false).SetEase(Ease.Linear);
    }

	public void secureCoin()
	{
		GetComponent<BoxCollider2D>().enabled = false;
		_anim.SetBool("get",true);
	}
}