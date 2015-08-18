using UnityEngine;
using System.Collections;

public class StingerESFX : MonoBehaviour {

    public FMODAsset explosion;
    public FMODAsset hit;

    private FMOD_CustonEmitter explosionEmitter;
    private FMOD_CustonEmitter hitEmitter;

    private StingerExploder _enemy;

    void Awake() {
        GameObject gameObject = new GameObject("SFX");
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition = Vector3.zero;

        explosionEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        explosionEmitter.Init(explosion);

        hitEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        hitEmitter.Init(hit);

        _enemy = GetComponent<StingerExploder>();
    }

    public void Hit() {
        hitEmitter.Play();
    }

    bool inAtraction = false;
    public void Explosion(float distance) {
        explosionEmitter.SetParameter("Attack", 1 - Mathf.Clamp(distance, -0.16f, 1) + 0.16f);

        if (!inAtraction) {
            inAtraction = true;
            explosionEmitter.Play();
        }
    }

}
