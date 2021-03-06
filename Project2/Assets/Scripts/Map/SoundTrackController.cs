﻿using UnityEngine;
using System.Collections;

public class SoundTrackController : MonoBehaviour {

    public static SoundTrackController self;

    public FMODAsset soundTrack;

    private FMOD_CustonEmitter soundTrackEmitter;

    bool init = false;
    float lastChange = -2;
    float latParameter = 0;

    void Start() {
        self = this;

        Vector3 emitterPosition = transform.TransformPoint(Vector3.zero);
        emitterPosition.z = 0;

        GameObject gameObject = new GameObject("SFX");
        gameObject.transform.position = emitterPosition;
        gameObject.transform.parent = transform;

        soundTrackEmitter = gameObject.AddComponent<FMOD_CustonEmitter>();
        soundTrackEmitter.Init(soundTrack);
        soundTrackEmitter.Start();
    }

    public void Update() {
        if (lastChange + 2 < Time.time && latParameter != 0) {
            SetBackgroundSound(latParameter);
            latParameter = 0;
        }
    }

    public void SetBackgroundSound(float soundTrack) {
        if (!init) {
            soundTrackEmitter.Play();
            init = true;
        }

        if (soundTrackEmitter.GetParameter("Perigo") == soundTrack) return;

        if (lastChange + 2 > Time.time) {
            latParameter = soundTrack;
            return;
        }

        lastChange = Time.time;
        soundTrackEmitter.SetParameter("Perigo", soundTrack);
    }
}
