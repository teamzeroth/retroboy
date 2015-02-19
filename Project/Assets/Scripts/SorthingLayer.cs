using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SorthingLayer : MonoBehaviour {

    Renderer _renderer = null;
    int initialOrder;

    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        initialOrder = _renderer.sortingOrder;
    }

    void Update() {
        _renderer.sortingOrder = initialOrder + (int) Mathf.Round(transform.position.y * -2);
    }
}
