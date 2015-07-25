using UnityEngine;
using System.Collections;

public class BetaMovimetation : MonoBehaviour {

    public Transform target;

    [Range(1f, 10f)]
    public float smooth = 1f;
    [Range(0, 1f)]
    public float distance = 1f;

    public float anglesPerSeconds = 10;
    public float lightAnglesPerSeconds = 180;

    private float angle = 0;
    private float lightAngle = 0;
    private bool flipped = false;

    private bool disappeared = false;

    private GameObject _lightSprite;
    private GameObject _lightParticle;
    private AnimationController _playerController;

    void Awake() {
        if (target == null) target = GameObject.FindWithTag("Player").transform;

        _lightSprite = transform.FindChild("light").gameObject;
        _lightParticle = transform.FindChild("particles").gameObject;
        _playerController = target.GetComponent<AnimationController>();
    }

    void Start() {
        transform.position = _playerController.transform.position;
    }

    void FixedUpdate() {
        if (!disappeared) {
            CalcPosition();
        } else {
            transform.position = target.position;
        }

        if (_playerController.flipped != flipped)
            Flip();

        if (!_playerController.NormalState)
            Desapear();
        else
            Apear();
    }

    void Update() {
        lightAngle += lightAnglesPerSeconds * Time.deltaTime;
        lightAngle = lightAngle % 360;

        Vector3 curAngle = _lightSprite.transform.localEulerAngles;
        curAngle.z = flipped ? lightAngle : -lightAngle;

        _lightSprite.transform.localEulerAngles = curAngle;

        _lightParticle.GetComponent<ParticleSystem>().startRotation = lightAngle * Mathf.Deg2Rad;
        _lightParticle.GetComponent<ParticleSystem>().startLifetime = 1;
    }

    void CalcPosition() {
        Vector3 curPos = transform.position;
        Vector3 toPos = target.position;

        curPos = Vector3.Lerp(curPos, toPos, Time.deltaTime * smooth);

        angle += anglesPerSeconds * Time.deltaTime;
        Vector3 orbi = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );

        transform.position = curPos + orbi * distance;
    }

    public void Flip() {
        flipped = !flipped;
        checkFlip();
    }

    private void checkFlip() {
        Vector3 localScale = transform.localScale;

        if (flipped && localScale.x > 0)
            localScale.x *= -1;
        else if (!flipped && localScale.x < 0)
            localScale.x *= -1;

        transform.localScale = localScale;
    }

    static float TIME_DESAPEAR = 0.2f;
    static float EXTRA_TIME_DESAPEAR = 0.1f;
    static Color pink = new Color((float) 254 / 255, (float) 4 / 255, (float) 110 / 255);

    float disappearTime = 0;

    public void Desapear() {
        if (disappearTime > TIME_DESAPEAR) {
            disappeared = true;
            disappearTime = TIME_DESAPEAR + EXTRA_TIME_DESAPEAR;
            return;
        }

        checkFlip();

        _lightParticle.GetComponent<ParticleSystem>().maxParticles = 0;
        disappearTime = disappearTime + Time.deltaTime;

        animSetScale(disappearTime);
    }

    public void Apear() {
        if (disappearTime < TIME_DESAPEAR && disappeared) {
            disappeared = false;
        }

        if (disappearTime <= 0f) _lightParticle.GetComponent<ParticleSystem>().maxParticles = 10;

        if (disappearTime < 0f) { 
            disappearTime = 0f;  
            return;
        }

        checkFlip();

        disappearTime = disappearTime - Time.deltaTime;

        animSetScale(disappearTime);
    }

    private void animSetScale(float desapearTime){
        if (desapearTime > TIME_DESAPEAR || desapearTime < 0) return;

        float delta = 1 - Helper.EaseCirc(desapearTime, 0, 1, TIME_DESAPEAR);
        float sign = Mathf.Sign(transform.localScale.x);

        _lightSprite.transform.localScale = new Vector3(sign * 0.5f, delta * 0.5f, 1);
        transform.localScale = new Vector3(sign + (1 - delta), delta, 1);

        (GetComponent<Renderer>() as SpriteRenderer).color = new Color(1, 1, 1, delta * delta);
        (_lightSprite.GetComponent<Renderer>() as SpriteRenderer).color = Color.Lerp(Color.white, pink, delta * delta);
    }
}
