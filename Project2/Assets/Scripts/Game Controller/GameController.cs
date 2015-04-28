using UnityEngine;
using System.Collections;

using MapResources;

public class GameController : MonoBehaviour {

    private SmoothFollow _camera;
    private AnimationController _player;

    void Start() {
        _camera = Camera.main.GetComponent<SmoothFollow>();

        if(GameObject.FindWithTag("Player"))
            _player = GameObject.FindWithTag("Player").GetComponent<AnimationController>();
        else
            _player = ((GameObject) Instantiate(Resources.Load<GameObject>("Characters/Nim (Player)"))).GetComponent<AnimationController>();

        print("MainDoor " + Door.doors.ContainsKey("MainDoor"));

        if (Door.doors.ContainsKey("MainDoor")){
            (_player.renderer as SpriteRenderer).color = new Color(0, 0, 0, 0.3f);
            Door.doors["MainDoor"].GetOut(_player);
        }
    }
}
