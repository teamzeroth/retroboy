using UnityEngine;
using System.Collections;

public class SortingMoveableLayer : MonoBehaviour {

    public Transform positionPoint;
    public bool forcePosition = false;
    public bool ignoreRenderer = false;
    public Sprite shadow;

    [SerializeField]
    public float initialOrder;

    private float position;
    private Transform _shadow;

    Transform _renderer;

    public float Position {
        set {
            forcePosition = true;
            position = value;
        }
    }

    public float InitialOrder {
        set {
            initialOrder = value;
        }
    }

    void Awake() {
        if (ignoreRenderer) _renderer = transform;

        if (_renderer == null) _renderer = transform.Find("Renderer");
        if (_renderer == null) _renderer = transform.Find("renderer");
        if (_renderer == null) _renderer = transform;

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
        //if (name == "Shoot(Clone)") print(initialOrder);

        _renderer.position = new Vector3(
            _renderer.position.x, _renderer.position.y,
            initialOrder + positionPoint.position.y
        );

        if (_shadow != null) _shadow.position.Set(
            _shadow.position.x, _shadow.position.y,
            _renderer.position.z - 4
        );

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

    public void CastShadow() {
        float dist = Vector2.Distance(transform.position, positionPoint.position);

        GameObject shadowGO = new GameObject("Shadow", typeof(SpriteRenderer));
        SpriteRenderer shadowRenderer = shadowGO.GetComponent<Renderer>() as SpriteRenderer;

        _shadow = shadowGO.transform;

        shadowGO.transform.position = positionPoint.position;
        shadowGO.transform.parent = transform;

        if (GetComponent<Collider2D>() != null) shadowGO.transform.localScale = Vector3.back + (Vector3)GetComponent<Collider2D>().bounds.size * 4;

        shadowRenderer.sprite = shadow;
        shadowRenderer.sortingLayerID = _renderer.GetComponent<SpriteRenderer>().sortingLayerID;
        shadowRenderer.color = new Color(1, 1, 1, 1 - (dist / 2));
    }
}
