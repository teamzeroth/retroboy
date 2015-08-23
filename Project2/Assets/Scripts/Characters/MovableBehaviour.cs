using UnityEngine;
using System.Collections;

public class MovableBehaviour : MonoBehaviour {

    private CollisionLevel _collisionLevel;
    private SortingOrder _sortingOrder;

    [HideInInspector]
    public bool flipped;

    #region Getters & Setters

    public CollisionLevel collisionLevel {
        get { return _collisionLevel; }
    }

    public SortingOrder sortingOrder {
        get { return _sortingOrder; }
    }

    public Transform Feet {
        get { return _sortingOrder.positionPoint; }
    }

    public Transform Collider {
        get { return transform.Find("collider"); }
    }

    public SpriteRenderer Renderer {
        get { return _sortingOrder.getRenderer(); }
    }

    public int Level {
        get { return _collisionLevel.Level; }
        set { _collisionLevel.Level = value; }
    }

    #endregion

    void Awake() {
        _collisionLevel = gameObject.TryAddComponent<CollisionLevel>();
        _sortingOrder = gameObject.TryAddComponent<SortingOrder>();
    }

    protected void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
