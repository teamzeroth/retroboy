using UnityEngine;
using System.Collections;

public class SorthingMoveableLayer : MonoBehaviour {

    public Transform _positionPoint;

    Renderer _renderer = null;
    
    int _initialOrder;

    void Awake() {
        Transform t;

        t = transform.Find("Renderer");
        if (t == null) t = transform.Find("renderer");
        if (t == null) t = transform;

        _renderer = t.GetComponent<SpriteRenderer>();

        if (_renderer == null) {
            Debug.LogError("No SpriteRenderer Coponente in Renderer GameObject " + t.name);
            this.enabled = false;
            return;
        }

        _initialOrder = _renderer.sortingOrder;

        if (_positionPoint == null) _positionPoint = transform.Find("Feets");
        if (_positionPoint == null) _positionPoint = transform.Find("feets");
        if (_positionPoint == null) _positionPoint = transform;


        if (_positionPoint == null) {
            Debug.LogError("No Transform Coponente found to " + name);
            this.enabled = false;
            return;
        }
    }

    void Update() {
        _renderer.sortingOrder = _initialOrder + Mathf.RoundToInt(_positionPoint.position.y * -10);
    }

    void OnDrawGizmosSelected() {
        Transform positionPoint = _positionPoint;

        if (positionPoint == null) positionPoint = transform.Find("Feets");
        if (positionPoint == null) positionPoint = transform.Find("feets");
        if (positionPoint == null) positionPoint = GetComponent<Transform>();

        if (positionPoint != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(positionPoint.position, 0.1f);
        }
    }
}
