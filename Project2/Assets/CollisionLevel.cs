using UnityEngine;
using System.Collections;

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

    SpriteRenderer _renderer;


    public void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        setSorthingOrder();
    }

    private void setSorthingOrder() {
        if (_renderer != null)
            _renderer.sortingOrder = Level;
    }

}
