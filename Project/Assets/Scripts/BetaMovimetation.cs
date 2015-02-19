using UnityEngine;
using System.Collections;

public class BetaMovimetation : MonoBehaviour {

    public Transform player;

    [Range(1f, 10f)]
    public float smooth = 1f;
    [Range(0, 1f)]
    public float distance = 1f;

    public float anglesPerSeconds = 10;
    public float lightAnglesPerSeconds = 180;

    private float angle = 0;
    private float lightAngle = 0;

    private GameObject _lightSprite;

    void Awake() {
        _lightSprite = transform.FindChild("light").gameObject;
    }
    void FixedUpdate() {
        Vector3 curPos = transform.position;
        Vector3 toPos = player.position;

        curPos.x = Mathf.Lerp(curPos.x, toPos.x, Time.deltaTime * smooth);
        curPos.y = Mathf.Lerp(curPos.y, toPos.y, Time.deltaTime * smooth);

        angle += anglesPerSeconds * Time.deltaTime;
        Vector3 orbi = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );

        transform.position = curPos + orbi * distance;
    }

    void Update() {
        lightAngle += lightAnglesPerSeconds * Time.deltaTime;
        lightAngle = lightAngle % 360;

        Vector3 curAngle = _lightSprite.transform.localEulerAngles;
        curAngle.z = -lightAngle;

        _lightSprite.transform.localEulerAngles = curAngle;
        _lightSprite.particleSystem.startRotation = lightAngle * Mathf.Deg2Rad;

        if (player.GetComponent<AnimationController>().onAnyMoveButton)
            _lightSprite.particleSystem.startLifetime = 1;
        else
            _lightSprite.particleSystem.startLifetime = 0;
    }
}
