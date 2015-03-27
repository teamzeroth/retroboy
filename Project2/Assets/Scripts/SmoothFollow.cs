using UnityEngine;
using System.Collections;



public class SmoothFollow : MonoBehaviour {
    public Transform target;
    public Vector3 cameraOffset;

    public float smoothDampTime = 0.2f;
    public bool useFixedUpdate = false;

    private Vector3 _smoothDampVelocity;
    private float timer;
    private int onQuake;

    void LateUpdate() {
        if (!useFixedUpdate) updateCameraPosition();
    }


    void FixedUpdate() {
        if (useFixedUpdate) updateCameraPosition();
    }


    public void Quake(int level) {
        print("QUake in level: " + level);
        onQuake = level;
        timer = 0;
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

    void updateCameraPosition() {
        Vector3 targetPos = getTargetPosition();

        transform.position = Vector3.SmoothDamp(transform.position, targetPos - cameraOffset, ref _smoothDampVelocity, onQuake == 0 ? smoothDampTime : -1);
        return;
    }
}
