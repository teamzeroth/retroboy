using UnityEngine;
using System.Collections;

using MapResources;

public static class Game {
    public const float TIME = 0;
    public const float TIME_BETA_DISAPPEAR = 0.3f;
    public const float TIME_PLAYER_SHOOTING = 0.5f;
    public const float TIME_PLAYER_DAMAGE = 0.6f;

    public const int LEVEL_LAYER = 8;
    public const int ENEMY_LAYER = 9;
    public const int ALLY_SHOOT_LAYER = 10;
    public const int ENEMY_SHOOT_LAYER = 11;
}

public class GameController : MonoBehaviour {

    public static GameController self;

    private Coroutine pauseRoutine;

    private SmoothFollow _camera;
    private UiController _ui;
    private Player _player;

    void Awake() {
        self = this;
    }

    void Start() {
        _camera = Camera.main.GetComponent<SmoothFollow>();
        _ui = GetComponent<UiController>();

        return;

        if (GameObject.FindWithTag("Player"))
            _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        else
            _player = ((GameObject)Instantiate(Resources.Load<GameObject>("Characters/Nim (Player)"))).GetComponent<Player>();

        if (Door.doors.ContainsKey("MainDoor")) {
            _camera.target = Door.doors["MainDoor"].transform;
            _player.transform.position = Door.doors["MainDoor"].transform.position;
            (_player.renderer as SpriteRenderer).color = Color.clear;
        }
    }

    public void Update() {
        if (CanRestartTheGame && Input.anyKeyDown) {
            Application.LoadLevel(Application.loadedLevel);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            Pause = !Pause;
        }

    }

    public void StartStage() {
        _player.gameObject.SetActive(true);

        if (Door.doors.ContainsKey("MainDoor")) {
            (_player.renderer as SpriteRenderer).color = new Color(0, 0, 0, 0.3f);
            Door.doors["MainDoor"].GetOut(_player);
        }
    } 

    public void PauseGame(bool pause) {
        if (pause) {
            Time.timeScale = 0;
            _ui.TogglePauseGame(pause);

            pauseRoutine = StartCoroutine(IgnoreTimeUpdate());
        } else {
            Time.timeScale = 1;
            _ui.TogglePauseGame(pause);

            StopCoroutine(pauseRoutine);
        }
    }

    IEnumerator IgnoreTimeUpdate() {
        yield return new WaitForSeconds(0.1f);

        while (Pause) {
            Update();
            yield return null;
        }
    }

    #region getters and setters
    
    private bool _pause;
    public bool Pause {
        get {
            return _pause;
        }
        set {
            if (_pause != value) {
                _pause = value;
                PauseGame(value);
            }
        }
    }

    public bool CanRestartTheGame = false;

    #endregion
}
