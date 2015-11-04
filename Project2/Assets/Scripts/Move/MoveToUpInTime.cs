using UnityEngine;
using System.Collections;

public class MoveToUpInTime : MonoBehaviour {

    public float totalTime;
    public float distance;

    private float timer;
    private Vector3 _localPosition;

    public Vector3 localPosition {
        set {
            _localPosition = value;
        }
    }

    public void Update() {
        timer += Time.deltaTime;
        //transform.localPosition += _localPosition + Vector3.up * (distance * timer % totalTime
    }
}
