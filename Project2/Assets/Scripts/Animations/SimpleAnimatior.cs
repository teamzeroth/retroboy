using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class SimpleAnimatior : MonoBehaviour {

    public bool playOnce = false;
    public float sample;
    public Sprite[] animation;

    private float time;
    private int curr = -1;

    private SpriteRenderer _renderer;

    void Start(){
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        time += Time.deltaTime;
        curr = Mathf.FloorToInt(sample * time);

        if (curr >= animation.Length && playOnce) {
            SendMessageUpwards("OnFinishSimpleAnimation", SendMessageOptions.DontRequireReceiver);
            enabled = false;
            return;
        }

        curr %= animation.Length;

        _renderer.sprite = animation[curr];
    }
}
