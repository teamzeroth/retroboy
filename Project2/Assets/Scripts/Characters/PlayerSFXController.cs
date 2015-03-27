using UnityEngine;
using System.Collections;

using FMOD.Studio;

public class PlayerSFXController : MonoBehaviour {

    public FMODAsset shoot;

    FMOD_CustonEmitter shootEmitter;
    
    AnimationController _animControl;

    float timer = 0;

    void Awake() {
        GameObject gameObject = (GameObject) Instantiate(new GameObject("SFX"));
        gameObject.transform.parent = transform;
        gameObject.transform.position = Vector3.zero;

        shootEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        shootEmitter.Init(shoot);

        _animControl = GetComponent<AnimationController>();
    }

    void Update() {
        ShootSFX();
    }

    private bool shooted = false;

    void ShootSFX() {
        if (_animControl.OnCharge) {
            if (timer == 0) {
                shootEmitter.Stop();
                shootEmitter.SetParameter("shoot", .0f);
                shootEmitter.Play();
            }

            timer += Time.deltaTime;
        }

        if (_animControl.OnShoot){// && !shooted) {
            if (timer > 0) {
                var value = Mathf.Min(3, Mathf.CeilToInt(timer));
                shootEmitter.SetParameter("shoot", value);
            }

            timer = 0;
            shooted = true;
        }
    }
}
