﻿using UnityEngine;
using System.Collections;

using FMOD.Studio;

public class PlayerSFXController : MonoBehaviour {

    public FMODAsset shoot;

    private Coroutine watchCharge;
    private bool inCharge = false;
    private bool inShoot = false;

    private FMOD_CustonEmitter shootEmitter;

    private AnimationController _animControl;
    
    void Awake() {
        GameObject gameObject = (GameObject) Instantiate(new GameObject("SFX"), Vector3.zero, Quaternion.identity);
        gameObject.transform.parent = transform;

        shootEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        shootEmitter.Init(shoot);

        _animControl = GetComponent<AnimationController>();
    }

    public void Charge() {
        if (inCharge) return;

        inCharge = true;

        shootEmitter.SetParameter("Shoot", 0f);
        shootEmitter.SetParameter("Reset", 1f);

        if (watchCharge != null) StopCoroutine(watchCharge);
        watchCharge = StartCoroutine(WaitPlayerConfirm());
    }

        void playShoot() {
            if (shootEmitter.HasStoped()) shootEmitter.Play();
        }

    public void Shoot(float forceTime) {
        inCharge = false;
        inShoot = true;

        shootEmitter.SetParameter("Shoot", forceTime >= 1.5f ? forceTime >= 3f ? 3 : 2 : 1);
        shootEmitter.SetParameter("Reset", 0f);

        playShoot();
    }

    IEnumerator WaitPlayerConfirm() {
        yield return new WaitForSeconds(0.1f);
        if (inCharge && !inShoot) playShoot();

        watchCharge = null;
    }

    /*
    public FMODAsset shoot;

    [HideInInspector]
    public float timer = 0.0f;

    FMOD_CustonEmitter shootEmitter;
    
    AnimationController _animControl;
    GameObject _chargeParticles;
    GameObject _chargeExplosionParticles;

    private float waitToStart = 0.0f;

    private bool shooted = false;
    private bool waitingTime = false;
    private bool showParticle = false;

    void Start() {
        _chargeParticles = transform.Find("Charge Particles").gameObject;
        _chargeExplosionParticles = transform.Find("Charge Complete Particles").gameObject;
    }

    void Update() {
        ShootSFX();
    }

    #region getMessages

    void ShootSFX() {

        if (_animControl.OnCharge && shootEmitter.HasStoped()) {shootEmitter.Play(); }

        if (shooted) {
            var value = timer >= 1.5f ? timer >= 3f ? 3 : 2 : 1;

            shootEmitter.SetParameter("shoot", value);

            if (waitingTime) {
                waitingTime = false;
                waitToStart = 0f;

                shootEmitter.TimelinePosition = 1;
                Invoke("StartAudio", 0.1f);                
            }
            
            Camera.main.BroadcastMessage("Quake", value * 0.5f);

            timer = 0;
            UiController.self.charge = 0;
             
            shooted = false;
            showParticle = false;
        }

        if (Input.GetButtonDown("Fire1") && _animControl.fireTime < 0) waitingTime = true;
        
        if (_animControl.OnCharge) {
            timer += Time.deltaTime;
            UiController.self.charge = timer / 3;
        }

        if (waitToStart > 0.2f) restartShootSFX(timer);
        if (waitingTime) waitToStart += Time.deltaTime;

        ParticlesConstronller();
    }

    public void HadShoot() {
        shooted = true;
    }

    public void StartAudio() {
        shootEmitter.Play();
    }

    public void ParticlesConstronller() {
        _chargeParticles.SetActive(timer > 0);
        _chargeParticles.particleSystem.startSpeed = -2 - Mathf.Min(3, Mathf.FloorToInt(timer));
        _chargeParticles.particleSystem.startLifetime = 0.3f - Mathf.Min(0.1f, Mathf.FloorToInt(timer) / 2);

        if (timer >= 3f && !showParticle) {
            showParticle = true;
            _chargeExplosionParticles.SetActive(true);
        }
    }

    #endregion

    #region Private Methods

    private void restartShootSFX(float timer) {
        waitingTime = false;
        waitToStart = 0f;

        shootEmitter.TimelinePosition = Mathf.RoundToInt(timer * 100);
        shootEmitter.SetParameter("shoot", .1f);
    }
    
    #endregion
    */
}
