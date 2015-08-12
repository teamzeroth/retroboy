﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    CameraController self;

    public Transform target;
    public Vector3 cameraOffset;

    public float smoothDampTime = 0.2f;
    public bool useFixedUpdate = false;

    private Camera cam;

    private Vector3 _smoothDampVelocity;
    private float timer;
    private float onQuake;

    #region MonoBehaviour
    public void Awake() {
        self = this;
        cam = GetComponent<Camera>();
    }

    public void Start() {
        if (target == null) target = GameObject.FindWithTag("Player").transform;
        updatePixelPerfect();
    }

    void LateUpdate() {
        if (!useFixedUpdate) OnUpdate();
    }

    void FixedUpdate() {
        if (useFixedUpdate) OnUpdate();
    }

    void OnUpdate() {
        updateCameraPosition();
    }

    #endregion

    #region Private Methods

    void updatePixelPerfect() {
        cam.orthographicSize = cam.pixelHeight / (Game.SPRITE_PPU * 2);
    }

    void updateCameraPosition() {
        Vector3 targetPos = getTargetPosition();
        Vector3 targetCameraPos = targetPos - cameraOffset;
        targetCameraPos.z = -cameraOffset.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetCameraPos, ref _smoothDampVelocity, onQuake == 0 ? smoothDampTime : -1);
    }


    Vector3 getTargetPosition() {
        if (onQuake == 0) return target.position;

        timer += Time.deltaTime;

        if (timer >= 0.5f) {
            onQuake = 0;
            return target.position;
        }

        return target.position + new Vector3(
            Random.value * onQuake / 7,
            Random.value * onQuake / 7,
            Random.value * onQuake / 7
        );
    }

    #endregion

    #region Messages

    public void Quake(float level) {
        onQuake = level;
        timer = 0;
    }

    #endregion
}
