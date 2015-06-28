using UnityEngine;
using System.Collections;

using DG.Tweening;

public class Beta : MonoBehaviour {

    public Transform target;

    [HideInInspector] public bool flipped = false;
    [HideInInspector] public bool visible = true;

    [Range(1f, 10f)] public float smoothMove = 1f;
    [Range(1f, 10f)] public float smoothRotation = 1f;
    [Range(0, 1.5f)] public float totalDistance = 1f;

    public float anglesPerSeconds = 180;

    //private float angle = 0;
    private float lightAngle = 0;
    private float distance = 0;
    private float visibleDelta = 0;

    private bool canAppear;

    private Transform _lightSprite;
    private Transform _lightParticle;
    private Transform _renderer;
    private Player _player;

    Tween DoDisappear;

    void Awake() {
        distance = totalDistance;
        canAppear = true;

        _lightSprite = transform.FindChild("light");
        _lightParticle = transform.FindChild("particles");
        _renderer = transform.FindChild("renderer");

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (target == null) target = _player.transform;
    }

    void Start() {
        Disappear(0);

        visible = false;
        transform.position = target.position;
        _lightParticle.particleSystem.maxParticles = 0;
    }

    void Update() {
        UpdateAnimation();
        UpdateMove();
    }

    void UpdateAnimation() {
        CalcRotation();
        checkFlip();
        checkVisibility();
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
            toPos += (Vector3)currVecAngle * distance;
        }

        transform.position = Vector3.Lerp(curPos, toPos, Time.deltaTime * smoothMove);
    }

    void checkFlip() {
        if (!_player.flipped) {
            if (flipped) _renderer.Flip(ref flipped);
        } else {
            if (!flipped) _renderer.Flip(ref flipped);
        }
    }

    void checkVisibility() {
        if ((_player.BetaVisible && !visible && canAppear) || (!_player.BetaVisible && visible)) {
            visible = !visible;

            var time = 0f;

            var to = visible ? 1f : 0;
            var totalTime = visible ? Game.TIME_BETA_DISAPPEAR : Game.TIME_BETA_DISAPPEAR / 2;

            if (DoDisappear != null) {
                time = DoDisappear.fullPosition;
                DoDisappear.Kill();
            }

            if (!visible) {
                _lightParticle.particleSystem.maxParticles = 0;
                StartCoroutine(WaitToAppear());
            }

            DoDisappear = DOTween.To(() => visibleDelta, x => Disappear(x), to, totalTime - time)
                .SetEase(visible ? Ease.OutQuad : Ease.InQuad)
                .OnComplete(() => {
                    if (visible) _lightParticle.particleSystem.maxParticles = 50;
                    DoDisappear = null;
                });
        }
    }

    void Disappear(float delta) {
        visibleDelta = delta;

        Color c;

        c = _lightSprite.GetComponent<SpriteRenderer>().color;
        c.a = delta * delta;
        _lightSprite.GetComponent<SpriteRenderer>().color = c;

        c = _renderer.GetComponent<SpriteRenderer>().color;
        c.a = delta * delta;
        _renderer.GetComponent<SpriteRenderer>().color = c;

        Vector3 t;

        t = _lightSprite.localScale;
        //t.y = delta * 0.5f;
        _lightSprite.localScale = t;

        distance = totalDistance * delta;
    }

    IEnumerator WaitToAppear() {
        canAppear = false;
        yield return new WaitForSeconds(Game.BETA_WAIT_APPEAR);
        Disappear(0);
        canAppear = true;
    }
}
