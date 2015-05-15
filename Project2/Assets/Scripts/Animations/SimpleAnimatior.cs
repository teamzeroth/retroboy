using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer)]
public class SimpleAnimatior : MonoBehaviour {

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

        curr = 0

        _renderer.sprite = animation[];
    }
}
