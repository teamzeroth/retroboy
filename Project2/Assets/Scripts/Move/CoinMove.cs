using UnityEngine;
using System.Collections;

using DG.Tweening;

public class CoinMove : MonoBehaviour {

    static public float TOTAL_TIME_OF_LIVE = 10f; //+ 3 

    public int quant = 10;

    [HideInInspector]
    public float currentValue = 0f;

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

        ground = new Vector2(_renderer.localPosition.x, _renderer.localPosition.y);
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, 1.5f);
    }

    void Update() {
        if (!disableMove) Move();
    }

    private void Move() {
        currentValue = Helper.EaseOutBounce(timer, 0, 0.5f, 2);
        timer += Time.deltaTime;
            
        Vector2 v = ground + new Vector2(0, 0.5f - currentValue);

        _renderer.localPosition = v;
        transform.position += (Vector3) velocity * (Time.deltaTime * (2 - timer) / 2);

        if (timer >= 2) disableMove = true;
    }
}
