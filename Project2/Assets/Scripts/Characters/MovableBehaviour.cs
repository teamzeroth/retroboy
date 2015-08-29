using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionLevel))]
[RequireComponent(typeof(SortingOrder))]
public class MovableBehaviour : MonoBehaviour {

    private CollisionLevel _collisionLevel;
    private SortingOrder _sortingOrder;
    private Rigidbody2D _rigidbody;

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

    /// This get method don't make much sence
    public Transform Collider {
        get { return transform.Find("collider"); }
    }

    public SpriteRenderer renderer {
        get { return _sortingOrder.getRenderer(); }
    }

    public Rigidbody2D Rigidbody {
        get { return _rigidbody; }
    }

    public int Level {
        get { return _collisionLevel.Level; }
        set { _collisionLevel.Level = value; }
    }

    #endregion

    public void Awake() {
        _collisionLevel = gameObject.TryAddComponent<CollisionLevel>();
        _sortingOrder = gameObject.TryAddComponent<SortingOrder>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
}
