using UnityEngine;
using System.Collections;

public class SortingOrder : MonoBehaviour {

    public Transform positionPoint;
    public float initialOrder;
    public bool ignoreRenderer = false;
    public Sprite shadow;

    private float position;
    public bool forcePosition = false;

    private Transform _shadow;
    private Transform _renderer;
    private SpriteRenderer _sRenderer;

    #region Getters And Setters

    public float Position {
        set {
            forcePosition = true;
            position = value;
        }
    }

    public SpriteRenderer getRenderer() {
        return _sRenderer;
    }

    public float InitialOrder {
        set {
            initialOrder = value;
        }
    }

    #endregion

    void Awake() {
        _renderer = transform.Find("renderer");
        if (ignoreRenderer || _renderer == null)
            _renderer = transform;

        _sRenderer = _renderer.GetComponent<SpriteRenderer>();

        positionPoint = transform.Find("feet") ?? transform;

        if (positionPoint == null) {
            Debug.LogError("No Transform Coponente found to " + name);
            this.enabled = false;
            return;
        }

        if (shadow != null) CastShadow();
    }

    /// <summary>
    /// Set the Z position of the object, if the position is set out of here whit the set atributte Position the flag forcePosition will be setted
    /// </summary>
    void Update() {
        _renderer.position = new Vector3(
            _renderer.position.x, _renderer.position.y, initialOrder + (!forcePosition ? positionPoint.position.y : position)
        );

        if (_shadow != null) _shadow.position.Set(
            _shadow.position.x, _shadow.position.y, _renderer.position.z - 1
        );
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw a small mangenta circle in the feet point of the object
    /// </summary>
    void OnDrawGizmosSelected() {

        Transform testPositionPoint = transform.Find("feet") ?? transform;

        if (testPositionPoint != null) {
            Vector3 point = testPositionPoint.position;
            if (forcePosition) point.y = position;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(point, 0.1f);
        }
    }
#endif

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
