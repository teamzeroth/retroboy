﻿using UnityEngine;
using System.Collections;

public class StingerSSFX : MonoBehaviour {

    //public FMODAsset laser;
    //public FMODAsset voo;
    //public FMODAsset explosion;

    private FMOD_CustonEmitter laserEmitter;
    private FMOD_CustonEmitter vooEmitter;
    private FMOD_CustonEmitter explosionEmitter;

    private StingerShooter _enemy;

    void Awake() {
        return;
        GameObject gameObject = (GameObject)Instantiate(new GameObject("SFX"), transform.TransformPoint(Vector3.zero), Quaternion.identity);
        gameObject.transform.parent = transform;

        laserEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        //laserEmitter.Init(laser);

        vooEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        //vooEmitter.Init(voo);

        explosionEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        //explosionEmitter.Init(explosion);

        _enemy = GetComponent<StingerShooter>();
    }

    public void Laser() {
        return;
        laserEmitter.SetParameter("laser", 1f);

        laserEmitter.Play();
    }

    bool inFleing = false;
    public void Voo() {
        return;
        vooEmitter.SetParameter("voo", 1f);

        if (!inFleing) {
            inFleing = true;
            vooEmitter.Play();
        }
    }

    public void Explosion() {
        return;
        vooEmitter.Stop();
        explosionEmitter.SetParameter("Attack", 1f);
        explosionEmitter.Play();
    }

}
