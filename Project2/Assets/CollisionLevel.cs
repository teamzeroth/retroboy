using UnityEngine;
using System.Collections;
using System;

public class CollisionLevel : MonoBehaviour {

    [SerializeField]
    private int level;

    public int Level {
        get { return level; }
        set {
            level = value;
            setSorthingOrder();
        }
    }


    bool inicialized = false;

    SpriteRenderer _renderer;

    public void Start() {
        inicialized = true;

        _renderer = GetComponent<SpriteRenderer>();
        setSorthingOrder();
    }

    private void setSorthingOrder() {
        SpriteRenderer renderer;

        if (!inicialized)
            renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
            renderer = _renderer;
        else
            return;

        _renderer.sortingOrder = Level;
    }

    public void SetWall(string level) {
        gameObject.tag = "Wall";

        int parser = 0;
        Int32.TryParse(level, out parser);

        Level = parser;
    }

}
