using UnityEngine;
using System.Collections;

public class SorthingMoveableLayer : MonoBehaviour {

    Renderer _renderer = null;
    Transform _positionPoint;

    int _initialOrder;

    void Awake() {
        Transform t = transform.Find("Renderer") != null ? transform.Find("Renderer") : transform;
        _renderer = t.GetComponent<SpriteRenderer>();

        if (_renderer == null) {
            Debug.LogError("No SpriteRenderer Coponente in Renderer GameObject " + t.gameObject);
            this.enabled = false;
            return;
        }

        _positionPoint = transform.Find("Feets");
        if (_positionPoint == null) _positionPoint = transform;

        _initialOrder = _renderer.sortingOrder;
    }

    void Update() {
        _renderer.sortingOrder = _initialOrder + Mathf.RoundToInt(_positionPoint.position.y * -10);
    }
}
