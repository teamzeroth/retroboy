using UnityEngine;
using System.Collections;

public class SortingStart : MonoBehaviour {

	public Transform positionPoint;
	public float initialOrder;
	public bool ignoreRenderer = false;
	
	private float position;
	public bool forcePosition = false;
	
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
		
	}
	
	/// <summary>
	/// Set the Z position of the object, if the position is set out of here whit the set atributte Position the flag forcePosition will be setted
	/// </summary>
	void Start() {
		
		_renderer.position = new Vector3(
				_renderer.position.x, _renderer.position.y, initialOrder + (!forcePosition ? positionPoint.position.y : position)
			);
		
	}
}
