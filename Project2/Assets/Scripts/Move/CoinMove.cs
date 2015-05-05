using UnityEngine;
using System.Collections;

using DG.Tweening;

public class CoinMove : MonoBehaviour {

    [HideInInspector]
    public float currentValue;

    Vector2 ground;
    Vector2 velocity;

    float timer;
    bool completeBounce;
    bool disableMove;

    Transform _renderer;

    void Awake() {
        _renderer = transform.Find("Renderer");

        currentValue = 0f;
        timer = 0;

        disableMove = false;
        completeBounce = false;

        ground = new Vector2(_renderer.localPosition.x, _renderer.localPosition.y);
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.value;

        print(velocity);
    }

    void Update() {

        //currentValue = Helper.EaseOutBounce(0, 0, 2,  );
        /*DOTween.To(() => currentValue, x => currentValue = x, 2f, 2f).SetEase(Ease.OutBounce).OnComplete(() => {
            completeBounce = true;
            timer = 1;
        });*/

        if (!disableMove) Move();
    }

    private void Move() {
        //if (!completeBounce) {
            currentValue = Helper.EaseOutBounce(timer, 0, 0.5f, 2);
            timer += Time.deltaTime;
            
            Vector2 v = ground + new Vector2(0, 0.5f - currentValue);

            _renderer.localPosition = v;
            transform.position += (Vector3) velocity * (Time.deltaTime * (2 - timer) / 2);

            print(transform.position);

            if (timer >= 2) disableMove = true;

        /*} else if (!completeBounce && velocity != Vector2.zero){
            timer -= Time.deltaTime;

            if(timer < 0){
                disableMove = true; 
                return;
            }

            transform.position += (Vector3)velocity * (Time.deltaTime * timer);
        }*/
    }
}
