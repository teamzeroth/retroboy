using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SorthingMoveableLayer : MonoBehaviour {

    Renderer _renderer = null;
    Transform _positionPoint;

    int _initialOrder;

    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();

        _positionPoint = transform.Find("Feets");
        if (_positionPoint == null) _positionPoint = transform;

        _initialOrder = _renderer.sortingOrder;
    }

    void Update() {
        _renderer.sortingOrder = _initialOrder + Mathf.RoundToInt(_positionPoint.position.y * -10);
    }
}
