using UnityEngine;
using System.Collections;

using MapResources;

public static class Game {
    public const float TIME = 0;
    public const float TIME_BETA_DISAPPEAR = 0.3f;
}

public class GameController : MonoBehaviour {

    public static GameController self;

    private SmoothFollow _camera;
    private AnimationController _player;

    void Awake() {
        self = this;
    }

    void Start() {
        _camera = Camera.main.GetComponent<SmoothFollow>();

        if (GameObject.FindWithTag("Player"))
            _player = GameObject.FindWithTag("Player").GetComponent<AnimationController>();
        else
            _player = ((GameObject)Instantiate(Resources.Load<GameObject>("Characters/Nim (Player)"))).GetComponent<AnimationController>();

        if (Door.doors.ContainsKey("MainDoor")) {
            _camera.target = Door.doors["MainDoor"].transform;
            _player.transform.position = Door.doors["MainDoor"].transform.position;
            (_player.renderer as SpriteRenderer).color = Color.clear;
        }
    }

    public void StartStage() {
        _player.gameObject.SetActive(true);

        if (Door.doors.ContainsKey("MainDoor")){
            (_player.renderer as SpriteRenderer).color = new Color(0, 0, 0, 0.3f);
            Door.doors["MainDoor"].GetOut(_player);
        }
    }
}
