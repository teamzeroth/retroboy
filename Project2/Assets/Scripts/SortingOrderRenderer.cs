using UnityEngine;
using System.Collections;

public class SortingOrderRenderer : MonoBehaviour {
	
	public Transform positionPoint;
	public float initialOrder;
	public bool ignoreRenderer = false;
	public Sprite shadow;
	
	private float position;
	public bool forcePosition = false;
	
	private Transform _shadow;
	private Transform _renderer;
	private SpriteRenderer _sRenderer;

	int lastOrder;

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
	}

	public void SetOrder(int newOrder){
		lastOrder = _sRenderer.sortingOrder;
		_sRenderer.sortingOrder = newOrder;
	}

	public void BackOrder(){
		_sRenderer.sortingOrder = lastOrder;
	}
}
