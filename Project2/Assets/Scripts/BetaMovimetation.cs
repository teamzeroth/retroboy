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
    private bool flipped = false;

    private GameObject _lightSprite;
    private AnimationController _playerController;

    void Awake() {
        _lightSprite = transform.FindChild("light").gameObject;
        _playerController = player.GetComponent<AnimationController>();
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

        if (_playerController.flipped != flipped)
            Flip();

        if (_playerController.onShoot)
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
        _lightSprite.particleSystem.startRotation = lightAngle * Mathf.Deg2Rad;

        _lightSprite.particleSystem.startLifetime = 1;
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

    static float TOTAL_TIME_DESAPEAR = 0.2f;
    static Color pink = new Color((float) 254 / 255, (float) 4 / 255, (float) 110 / 255);

    float desapearTime = 0;

    public void Desapear() {
        if (desapearTime > .9f){return;}
        checkFlip();

        _lightSprite.particleSystem.maxParticles = 0;
        desapearTime = desapearTime + Time.deltaTime < TOTAL_TIME_DESAPEAR ? desapearTime + Time.deltaTime : TOTAL_TIME_DESAPEAR;

        animSetScale(desapearTime);
    }

    public void Apear() {
        if (desapearTime < 0f) {return; }
        checkFlip();

        desapearTime = desapearTime - Time.deltaTime > 0f ? desapearTime - Time.deltaTime : 0f;

        if (desapearTime == 0f) _lightSprite.particleSystem.maxParticles = 10;

        animSetScale(desapearTime);
    }

    private void animSetScale(float desapearTime){
        var delta = 1 - Helper.EaseCirc(desapearTime, 0, 1f, TOTAL_TIME_DESAPEAR);
        var sign = Mathf.Sign(transform.localScale.x);

        _lightSprite.transform.localScale = new Vector3(_lightSprite.transform.localScale.x, delta * 1.3f, 1);
        transform.localScale = new Vector3(sign + (1 - delta), delta, 1);

        (renderer as SpriteRenderer).color = new Color(1, 1, 1, delta * delta);
        (_lightSprite.renderer as SpriteRenderer).color = Color.Lerp(Color.white, pink, delta * delta);
    }
}
