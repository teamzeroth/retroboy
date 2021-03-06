﻿using UnityEngine;
using System.Collections;

public class FMOD_CustonEmitter : FMOD_StudioEventEmitter {

    public delegate void Action();

    public void Init(FMODAsset asset) {
        this.asset = asset;
        startEventOnAwake = false;
    }

    public void Init(string path = "") {
        this.path = path;
        startEventOnAwake = false;
    }

    void Start() {
        base.Start();

        if (transform.parent != null)
            transform.position = transform.parent.TransformPoint(Vector3.zero);
    }

    void StartOnNextFrame() {
        StartCoroutine(WaitNextFrame(() => { Start(); }));
    }

    public void SetParameter(string name, float value) {
        FMOD.Studio.ParameterInstance parameter = getParameter(name);
        parameter.setValue(value);
    }

    public float GetParameter(string name) {
        float value;

        FMOD.Studio.ParameterInstance parameter = getParameter(name);
        parameter.getValue(out value);

        return value;
    }

    public bool HasStoped() {
        return getPlaybackState() == FMOD.Studio.PLAYBACK_STATE.STOPPED || getPlaybackState() == FMOD.Studio.PLAYBACK_STATE.STOPPING;
    }

    public void Play(float stopAfter) {
        Play();
        Invoke("Stop", stopAfter);
    }

    public void Release() {
        if (evt != null) {
            ERRCHECK(evt.release());
        } else {
            FMOD.Studio.UnityUtil.Log("Tried to play event without a valid instance: " + path);
            return;
        }
    }

    public int TimelinePosition {
        get { int outValue; evt.getTimelinePosition(out outValue); return outValue; }
        set { evt.setTimelinePosition(value); }

    }

    public float Volume {
        get { float outValue; evt.getVolume(out outValue); return outValue; }
        set { evt.setVolume(value); }
    }

    IEnumerator WaitNextFrame(Action action) {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        action();
    }
}