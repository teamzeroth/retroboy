using UnityEngine;
using System.Collections;

using MapResources;

public class GameController : MonoBehaviour {

    public static GameController self;

    private Coroutine pauseRoutine;

    private CameraController _camera;
    private UiController _ui;

    private Player _player;

    public Player player {
        get {
            return _player;
        }
    }

    public bool stopPlayer = false; // POG

    void Awake() {
        self = this;

        _player = GameObject.FindWithTag("Player") ?
            GameObject.FindWithTag("Player").GetComponent<Player>() :
            ((GameObject)Instantiate(Resources.Load<GameObject>("Characters/Nim (Player)"))).GetComponent<Player>();
    }

    void Start() {
        _camera = Camera.main.GetComponent<CameraController>();
        _ui = GetComponent<UiController>();

        StartStage();

        return;


        if (Door.doors.ContainsKey("MainDoor")) {
            _camera.target = Door.doors["MainDoor"].transform;
            _player.transform.position = Door.doors["MainDoor"].transform.position;
            (_player.GetComponent<Renderer>() as SpriteRenderer).color = Color.clear;
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            Time.timeScale = Time.timeScale == 1 ? 0.3f : 1;
        }

        if (CanRestartTheGame && Input.anyKeyDown) {
            Application.LoadLevel(Application.loadedLevel);
            return;
        }

        if (Input.GetButtonDown("Menu") || Input.GetKeyDown(KeyCode.Escape)) {
            Pause = !Pause;
        }

    }

    public void StartStage() {
        //_player.gameObject.SetActive(true);
        stopPlayer = false;

        /*if (Door.doors.ContainsKey("MainDoor")) {
            (_player.renderer as SpriteRenderer).color = new Color(0, 0, 0, 0.3f);
            Door.doors["MainDoor"].GetOut(_player);
        }*/
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
