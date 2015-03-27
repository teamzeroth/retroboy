using UnityEngine;
using System.Collections;

using FMOD.Studio;

public class PlayerSFXController : MonoBehaviour {

    public FMODAsset shoot;

    FMOD_CustonEmitter shootEmitter;
    
    AnimationController _animControl;

    float timer = 0.0f;
    float wait = 0.2f;

    void Awake() {
        GameObject gameObject = (GameObject) Instantiate(new GameObject("SFX"), Vector3.zero, Quaternion.identity);
        gameObject.transform.parent = transform;

        shootEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        shootEmitter.Init(shoot);

        _animControl = GetComponent<AnimationController>();
    }

    /*void FixedUpdate() {
        shooted = _animControl.OnShoot;
    }*/

    void Update() {
        ShootSFX();
    }

    private bool shooted = false;
    private bool initShoot = false;

    void ShootSFX() {

        if (shootEmitter.HasStoped()) shootEmitter.Play();

        if (_animControl.OnCharge) {
            if (timer == 0) {
                shootEmitter.SetParameter("shoot", 0.0f);
            }
            timer += Time.deltaTime;
        }

        if (shooted) {
            if (timer > 0) {
                var value = timer >= 1f ? timer >= 2f ? 3 : 2 : 1;

                shootEmitter.SetParameter("shoot", value);
                Camera.main.BroadcastMessage("Quake", value);
            }

            timer = 0;
            shooted = false;
        }

        if (_animControl.NormalState) shootEmitter.Stop();
    }

    #region getMessages

    public void HadShoot() {
        shooted = true;
    }

    #endregion
}
