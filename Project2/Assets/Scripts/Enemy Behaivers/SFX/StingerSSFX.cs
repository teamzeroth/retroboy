using UnityEngine;
using System.Collections;

public class StingerSSFX : MonoBehaviour {

    public FMODAsset laser;
    public FMODAsset voo;

    private FMOD_CustonEmitter laserEmitter;
    private FMOD_CustonEmitter vooEmitter;

    private StingerShooter _enemy;

    void Awake() {
        GameObject gameObject = (GameObject)Instantiate(new GameObject("SFX"), transform.TransformPoint(Vector3.zero), Quaternion.identity);
        gameObject.transform.parent = transform;

        laserEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        laserEmitter.Init(laser);

        vooEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        vooEmitter.Init(voo);

        _enemy = GetComponent<StingerShooter>();
    }

    public void Laser() {
        laserEmitter.SetParameter("laser", 1);

        laserEmitter.Play();
    }

    bool inFleing = false;
    public void Voo() {
        vooEmitter.SetParameter("voo", 1);

        if (!inFleing) {
            inFleing = true;
            vooEmitter.Play();
        }
    }

}
