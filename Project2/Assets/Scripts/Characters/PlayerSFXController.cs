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

    void Update() {
        ShootSFX();
    }

    private bool shooted = false;
    private bool initShoot = false;

    void ShootSFX() {

        if (_animControl.OnCharge && shootEmitter.HasStoped()) shootEmitter.Play();

        if (Input.GetButtonDown("Fire1")) {
            shootEmitter.TimelinePosition = 0000001;
            shootEmitter.SetParameter("shoot", 0.1f);   
        }

        if (_animControl.OnCharge)
            timer += Time.deltaTime;

        if (shooted) {
            var value = timer >= 1f ? timer >= 2f ? 3 : 2 : 1;

            shootEmitter.SetParameter("shoot", value);
            shootEmitter.Play();
            Camera.main.BroadcastMessage("Quake", value);

            timer = 0;
            shooted = false;
        }
    }

    #region getMessages

    public void HadShoot() {
        shooted = true;
    }

    #endregion
}
