using UnityEngine;
using System.Collections;

using DG.Tweening;

public class VideoIntro : MonoBehaviour {

    static float TIME_TO_PRESS_START = 228f / 302f;

    Animator _anim;
    FMOD_CustonEmitter _fmod;

    void Start() {
        _anim = GetComponent<Animator>();
        _fmod = GetComponent<FMOD_CustonEmitter>();

        _fmod.StartEvent();
    }

    void Update() {
        if (Input.GetButtonDown("Submit") && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= TIME_TO_PRESS_START) {
            _fmod.SetParameter("startGame", 1.0f);
            _anim.SetInteger("Video", 1);
        }
    }

	public void OnFinishAnimation(){
        if (_anim.GetInteger("Video") == 0)
            _anim.Play("video_intro", -1, 242f / 302f);
        else
            DOTween.To(() => _fmod.Volume, x => _fmod.Volume = x, 0, 1)
                .SetEase(Ease.InCirc)
                .OnComplete(OnFinishIntro);
    }

    public void OnFinishIntro() {
        Application.LoadLevel("Cave1");
    }
}
