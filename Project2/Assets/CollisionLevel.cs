using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class CollisionLevel : MonoBehaviour {

    public SpriteRenderer renderer;

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


    public void Start() {
        if (renderer == null) renderer = GetComponent<SpriteRenderer>();
        setSorthingOrder();
    }

    private void setSorthingOrder() {
        SpriteRenderer renderer = this.renderer != null ? this.renderer : GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        renderer.sortingOrder = Level;
    }

    public void SetWall(string level) {
        gameObject.tag = "Wall";
        SetLevel(level);
    }

    public void SetLevel(string level) {
        int parser = 0;
        Int32.TryParse(level, out parser);

        Level = parser;
    }


#if UNITY_EDITOR
    public void OnDrawGizmosSelected() {
        Vector3 p = /*renderer != null ? renderer.bounds.max : */transform.position + Vector3.one * 0.75f;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;

        Handles.Label(p, Level.ToString(), style);
    }
#endif

}
