/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections;

namespace X_UniTMX.Internal
{
	public class Character2DIsometricController : MonoBehaviour
	{

		public enum EnumPlayerState : int
		{
			IDLE = 0,
			RIGHT,
			RIGHT_DOWN,
			RIGHT_UP,
			LEFT,
			LEFT_DOWN,
			LEFT_UP,
			DOWN,
			UP
		};

		public float _velocity = 2.0f;
		private Vector3 _velocityVector = Vector3.zero;
		private EnumPlayerState _stateAnimation = EnumPlayerState.IDLE;
		private Animator _scriptAnimator = null;
		Rigidbody2D _rigidbody;
		float
			_horizontalAxis = 0,
			_horizontalAxisAbsolute = 0,
			_verticalAxis = 0,
			_verticalAxisAbsolute = 0;

		// Use this for initialization
		void Start()
		{
			_scriptAnimator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody2D>();
			if (_rigidbody == null)
				_rigidbody = gameObject.AddComponent<Rigidbody2D>();
			_rigidbody.fixedAngle = true;
			_rigidbody.gravityScale = 0;
		}

		// Update is called once per frame
		void Update()
		{
			UpdateInput();
		}

		void FixedUpdate()
		{
			_rigidbody.velocity = _velocityVector;
		}

		void UpdateInput()
		{
			_verticalAxis = Input.GetAxis("Vertical");
			_horizontalAxis = Input.GetAxis("Horizontal");
			_horizontalAxisAbsolute = Mathf.Abs(_horizontalAxis);
			_verticalAxisAbsolute = Mathf.Abs(_verticalAxis);
			if (_horizontalAxisAbsolute < 0.1f && _verticalAxisAbsolute < 0.1f)
			{
				_velocityVector = Vector3.zero;
				if (_scriptAnimator)
					_scriptAnimator.speed = 0;
			}
			else
			{
				if (_scriptAnimator)
					_scriptAnimator.speed = 1;
				if (_horizontalAxis >= 0.1f && _verticalAxisAbsolute < 0.4f)
				{
					// Right direction
					_stateAnimation = EnumPlayerState.RIGHT;
				}
				if (_horizontalAxis <= -0.1f && _verticalAxisAbsolute < 0.4f)
				{
					// Left direction
					_stateAnimation = EnumPlayerState.LEFT;
				}
				if (_horizontalAxis >= 0.1f && _verticalAxis >= 0.4f)
				{
					_stateAnimation = EnumPlayerState.RIGHT_UP;
				}
				if (_horizontalAxis >= 0.1f && _verticalAxis <= -0.4f)
				{
					_stateAnimation = EnumPlayerState.RIGHT_DOWN;
				}
				if (_horizontalAxis <= -0.1f && _verticalAxis >= 0.4f)
				{
					_stateAnimation = EnumPlayerState.LEFT_UP;
				}
				if (_horizontalAxis <= -0.1f && _verticalAxis <= -0.4f)
				{
					_stateAnimation = EnumPlayerState.LEFT_DOWN;
				}
				if (_verticalAxis <= -0.1f && _horizontalAxisAbsolute < 0.4f)
				{
					// Down direction
					_stateAnimation = EnumPlayerState.DOWN;
				}
				if (_verticalAxis >= -0.1f && _horizontalAxisAbsolute < 0.4f)
				{
					// Up direction
					_stateAnimation = EnumPlayerState.UP;
				}
				Vector3 vetorHorizontal = Vector3.right * _velocity * _horizontalAxis;
				Vector3 vetorVertical = Vector3.up * _velocity * _verticalAxis;
				_velocityVector = Vector3.ClampMagnitude(vetorHorizontal + vetorVertical, _velocity); ;
			}
		}
	}
}