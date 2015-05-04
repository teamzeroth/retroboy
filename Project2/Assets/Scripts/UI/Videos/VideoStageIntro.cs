using UnityEngine;
using System.Collections;

using DG.Tweening;

public class VideoStageIntro : MonoBehaviour {

    Animator _anim;
    GameController _game;

    void Start() {
        _anim = GetComponent<Animator>();
        _game = GameController.self;
    }

    public void OnFinishIntro() {
        _game.StartStage();
        Destroy(gameObject);
    }
}
