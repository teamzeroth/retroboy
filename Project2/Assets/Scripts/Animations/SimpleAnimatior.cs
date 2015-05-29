using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
[ExecuteInEditMode]
public class SimpleAnimatior : MonoBehaviour {

    public string test;

    public bool playOnce = false;
    public float sample;
    public Sprite[] animation;

    private float timer;
    private int curr = -1;

    private SpriteRenderer _renderer;

    public int Frame {
        get { return curr; }
    }

    public float NormalizeTime {
        get { return (curr + 1) / sample; }
    }

    void Start() {
        timer = 0;
        curr = -1;
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        timer += Time.deltaTime;
        curr = Mathf.FloorToInt(sample * timer);

        if (curr >= animation.Length && playOnce) {
            SendMessageUpwards("OnFinishSimpleAnimation", SendMessageOptions.DontRequireReceiver);
            enabled = false;
            return;
        }

        curr %= animation.Length;

        _renderer.sprite = animation[curr];
    }

    void OnRenderObject() {
        if (Application.isEditor && !Application.isPlaying) {
            timer += Time.fixedDeltaTime;
            curr = Mathf.FloorToInt(sample * timer);
            curr %= animation.Length;
            _renderer.sprite = animation[curr];
        }
    }


}
