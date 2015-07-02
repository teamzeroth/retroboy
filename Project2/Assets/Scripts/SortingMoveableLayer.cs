﻿using UnityEngine;
using System.Collections;

public class SortingMoveableLayer : MonoBehaviour {

    public Transform positionPoint;
    public bool forcePosition = false;
    public Sprite shadow;
    
    private float position;
    private SpriteRenderer _shadow;

    Renderer _renderer = null;
    int _initialOrder;

    public float Position {
        set{
            forcePosition = true;
            position = value;
        }
    }

    public int InitialOrder {
        set {
            _initialOrder = value;
        }
    }

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

        if (positionPoint == null) positionPoint = transform.Find("Feets");
        if (positionPoint == null) positionPoint = transform.Find("feets");
        if (positionPoint == null) positionPoint = transform;

        if (positionPoint == null) {
            Debug.LogError("No Transform Coponente found to " + name);
            this.enabled = false;
            return;
        }

        if (shadow != null) CastShadow();
    }

    void Update() {
        if (!forcePosition) _renderer.sortingOrder = _initialOrder + Mathf.RoundToInt(positionPoint.position.y * -10);
        else _renderer.sortingOrder = _initialOrder + Mathf.RoundToInt(position * -10);

        if (_shadow != null) _shadow.sortingOrder = _renderer.sortingOrder - 4;
    }

    void OnDrawGizmosSelected() {
#if UNITY_EDITOR
        Transform testPositionPoint = positionPoint;

        if (testPositionPoint == null) testPositionPoint = transform.Find("Feets");
        if (testPositionPoint == null) testPositionPoint = transform.Find("feets");
        if (testPositionPoint == null) testPositionPoint = GetComponent<Transform>();

        if (testPositionPoint != null) {
            Gizmos.color = Color.magenta;
            if (!forcePosition)
                Gizmos.DrawWireSphere(testPositionPoint.position, 0.1f);
            else
                Gizmos.DrawWireSphere(new Vector3(testPositionPoint.position.x, position), 0.1f);
        }
#endif
    }

    public void CastShadow(){
        float dist = Vector2.Distance(transform.position, positionPoint.position);

        GameObject shadowGO = new GameObject("Shadow", typeof(SpriteRenderer));
        _shadow = shadowGO.renderer as SpriteRenderer;

        shadowGO.transform.position = positionPoint.position;
        shadowGO.transform.parent = transform;

        if (collider2D != null) shadowGO.transform.localScale = Vector3.back + (Vector3) collider2D.bounds.size * 4;

        _shadow.sprite = shadow;
        _shadow.sortingLayerID = _renderer.sortingLayerID;
        _shadow.color = new Color(1, 1, 1, 1 - (dist / 2));
    }
}