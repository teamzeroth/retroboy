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
	public abstract class Enemy2DAbstract : MonoBehaviour
	{
		public float maxDistanceToPlayer = 5.0f;

		public float timeMove = 0.0f;

		public float timeMaxToShoot = 10.0f;
		public float timeMinToShoot = 5.0f;

		public float velocityShoot = 5.0f;

		public GameObject prefabShoot = null;

		public Vector2[] sequenceDirections = { Vector2.up, Vector2.right, -Vector2.up, -Vector2.right };

		// To remember -Vector.up = down and -Vector.right = left
		public Vector2 directionEnemy = Vector2.up;


		// Is possible to see the player
		protected bool playerVisible = false;
		protected Character2DController player;

		// Player distance is always positive!
		protected float playerDistance = -1;

		protected Vector3 startPostion;

		protected int indexTypeEnemy = 0;

		protected float timeShoot = 0.0f;

		protected float countTimeMove = 0.0f;
		protected float countTimeShoot = 0.0f;

		protected Animator scriptAnimator = null;

		protected Vector2 directionPlayer = Vector2.zero;

		protected int indexDirection = 0;

		void Awake()
		{
			// Set start position of enemy
			startPostion = transform.position;
			scriptAnimator = GetComponent<Animator>();
		}

		void Start()
		{
			player = FindObjectOfType<Character2DController>();
			timeMinToShoot = Random.Range(timeMinToShoot, timeMaxToShoot);
		}

		void FixedUpdate()
		{
			/*
			RaycastHit2D hit = Physics2D.Raycast(transform.position,directionEnemy,maxDistanceToPlayer);
			if("Player".Equals(hit.collider.gameObject.tag)) {
				playerDistance = hit.fraction;
				directionPlayer = hit.normal;
				playerVisible = true;
			} else {
				playerVisible = false;
				playerDistance = -1;
				directionPlayer = Vector2.zero;
			}
			*/
		}

		void SetStateAnimation()
		{
			int value = 0;
			// Down
			if (directionEnemy.Equals(-Vector2.up)) value = 1;
			// Up
			if (directionEnemy.Equals(Vector2.up)) value = 2;
			// Left
			if (directionEnemy.Equals(Vector2.right)) value = 3;
			// Right
			if (directionEnemy.Equals(-Vector2.right)) value = 4;

			GetComponent<Animator>().SetInteger("State", value);
		}

		void Update()
		{
			if (timeMove > 0)
			{
				DoCountTime(ref countTimeMove);
				if (countTimeMove >= timeMove)
				{
					DoZeroTime(ref countTimeMove);
					DoMove();
				}
			}

			DoCountTime(ref countTimeShoot);
			if (countTimeShoot >= timeShoot)
			{
				DoZeroTime(ref countTimeShoot);
				timeShoot = Random.Range(timeMinToShoot, timeMaxToShoot);
				DoShoot();
			}
			SetStateAnimation();
		}

		protected void DoCountTime(ref float timeToCount)
		{
			timeToCount += Time.deltaTime;
		}

		protected void DoZeroTime(ref float timeToCount)
		{
			timeToCount = 0.0f;
		}

		void OnCollisionEnter(Collision other)
		{
			DoChangeDirection();
		}

		protected virtual void DoMove()
		{

		}

		protected virtual void DoShoot()
		{
			if (prefabShoot != null)
			{
				GameObject clone = Instantiate(prefabShoot) as GameObject;
				clone.transform.position = gameObject.transform.position;
				clone.transform.parent = gameObject.transform.parent;
				clone.renderer.sortingLayerName = "Foreground";
				clone.rigidbody2D.velocity = (directionEnemy * velocityShoot);
			}
		}

		protected virtual void DoChangeDirection()
		{

		}
	}
}