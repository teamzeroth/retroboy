using UnityEngine;
using System.Collections;

using DG.Tweening;

public class BetaController : MonoBehaviour {
    
    public Transform target;

    [HideInInspector] public bool flipped = false;
    [HideInInspector] public bool visible = true;

    [Range(1f, 10f)] public float smoothMove = 1f;
    [Range(1f, 10f)] public float smoothRotation = 1f;
    [Range(0, 1f)] public float distance = 1f;

    public float anglesPerSeconds = 180;

    private float angle = 0;
    private float lightAngle = 0;

    private Transform _lightSprite;
    private Transform _lightParticle;
    private Transform _renderer;
    private PlayerMovementController _player;

    void Awake() {        
        _lightSprite = transform.FindChild("light");
        _lightParticle = transform.FindChild("particles");
        _renderer = transform.FindChild("renderer");

        _player = GameObject.FindWithTag("Player").GetComponent<PlayerMovementController>();

        if (target == null) target = _player.transform;
    }

    void Start() {
        transform.position = target.position;
    }

    void Update() {
        UpdateAnimation();
        UpdateMove();
    }

    void UpdateAnimation() {
        CalcRotation();
        checkFlip();
    }

    void UpdateMove() {
        CalcPosition();
    }

    Vector2 currVecAngle = Vector2.zero;

    void CalcRotation() {
        lightAngle += anglesPerSeconds * Time.deltaTime;
        lightAngle = lightAngle % 360;

        Vector3 curAngle = _lightSprite.transform.localEulerAngles;
        curAngle.z = -lightAngle;

        _lightSprite.transform.localEulerAngles = curAngle;
        _lightParticle.particleSystem.startRotation = lightAngle * Mathf.Deg2Rad;
        _lightParticle.particleSystem.startLifetime = 1;
   
    }

    void CalcPosition() {
        Vector3 curPos = transform.position;
        Vector3 toPos = target.position;

        if (target == _player.transform) {
            Vector2 v = _player.DeadDirection.normalized * -1;

            float currAngle = Mathf.LerpAngle(
                (360 + Mathf.Atan2(currVecAngle.y, currVecAngle.x) * Mathf.Rad2Deg) % 360,
                (360 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) % 360,
                Time.deltaTime * smoothRotation
            );

            currVecAngle.Set(Mathf.Cos(currAngle * Mathf.Deg2Rad), Mathf.Sin(currAngle * Mathf.Deg2Rad));
            toPos += (Vector3) currVecAngle * distance;
        }

        transform.position = Vector3.Lerp(curPos, toPos, Time.deltaTime * smoothMove);
    }

    public void checkFlip() {
        if (!_player.flipped) {  
            if (flipped) _renderer.Flip(ref flipped);
        } else{
            if (!flipped) _renderer.Flip(ref flipped);
        }
    }
}
