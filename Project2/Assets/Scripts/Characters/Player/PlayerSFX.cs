using UnityEngine;
using System.Collections;

using FMOD.Studio;

public class PlayerSFX : MonoBehaviour {

    public FMODAsset shoot;
    public FMODAsset footstep;
    public FMODAsset hurt;

    private Coroutine watchCharge;
    private bool inCharge = false;
    private bool inShoot = false;
    private bool inWalking = false;
    private bool haveHurt = false;

    private FMOD_CustonEmitter shootEmitter;
    private FMOD_CustonEmitter footstepEmitter;
    private FMOD_CustonEmitter hurtEmitter;

    private Player _player;
    
    void Awake() {
        GameObject gameObject = (GameObject) Instantiate(new GameObject("SFX"), Vector3.zero, Quaternion.identity);
        gameObject.transform.parent = transform;

        shootEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        shootEmitter.Init(shoot);

        footstepEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        footstepEmitter.Init(footstep);

        hurtEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        hurtEmitter.Init(hurt);

        _player = GetComponent<Player>();
    }

    public void Charge() {
        if (inCharge) return;

        inCharge = true;

        shootEmitter.SetParameter("Shoot", 0f);
        shootEmitter.SetParameter("Reset", 1f);

        if (watchCharge != null) StopCoroutine(watchCharge);
        watchCharge = StartCoroutine(WaitPlayerConfirm());
    }

    public void UnCharge() {
        if (!inCharge) return;

        inCharge = false;

        shootEmitter.SetParameter("Reset", 0f);
    }

    public void Shoot(float forceTime) {
        inShoot = true;
        shootEmitter.SetParameter("Shoot", forceTime >= 1.5f ? forceTime >= 3f ? 3 : 2 : 1);

        playShoot();
    }
    
    public void Footstep(bool walking, int surface = 1) {
        if (_player.OnHurt) return;

        if (walking && inWalking != walking) {
            inWalking = true;

            footstepEmitter.Play();
            footstepEmitter.SetParameter("walk", (float) surface);
        }

        if (!walking && inWalking != walking) {
            inWalking = false;

            footstepEmitter.SetParameter("walk", 0);
            footstepEmitter.Stop();
        }
    }


    public void Hurt() {
        if (!haveHurt) {
            haveHurt = true;
            hurtEmitter.SetParameter("hurt", 1f);
        }

        //hurtEmitter.Release();
        hurtEmitter.Play(0.5f);
    }
    
    void playShoot() { if (shootEmitter.HasStoped()) shootEmitter.Play(); }

    IEnumerator WaitPlayerConfirm() {
        yield return new WaitForSeconds(0.1f);
        if (inCharge && !inShoot) playShoot();

        watchCharge = null;
    }
}
