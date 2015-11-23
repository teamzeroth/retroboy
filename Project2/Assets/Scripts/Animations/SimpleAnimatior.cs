using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SimpleAnimatior : MonoBehaviour {

#if UNITY_EDITOR
    public string test;
#endif

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
        if (animation.Length == 0) return;

        timer += Time.deltaTime;
        curr = Mathf.FloorToInt(sample * timer);

        if (curr >= animation.Length && playOnce && Application.isPlaying) {
            _renderer.sprite = null;
            SendMessageUpwards("OnFinishSimpleAnimation", SendMessageOptions.DontRequireReceiver);

            enabled = false;
            return;
        }

        curr %= animation.Length;

        _renderer.sprite = animation[curr];
    }

    public void OnEnable() {
        timer = 0;
        curr = -1;
    }


#if UNITY_EDITOR
    void OnRenderObject() {
        if (animation.Length == 0) return;

        if (Application.isEditor && !Application.isPlaying) {
            timer += Time.fixedDeltaTime;
            curr = Mathf.FloorToInt(sample * timer);
            curr %= animation.Length;
            _renderer.sprite = animation[curr];
        }
    }
#endif
}
