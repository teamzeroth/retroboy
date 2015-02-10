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
	public class CameraFolow2D : MonoBehaviour
	{
		public float smoothMovement = 3.0f;
		public Vector2 relativeDistance = Vector2.zero;
		public string tagToFollow = "Player";
		private GameObject playerRefence = null;
		private Transform myTransform;

		private void Awake()
		{
			myTransform = transform;
		}

		Vector3 GetPlayerPosition()
		{
			return new Vector3(playerRefence.transform.position.x + relativeDistance.x, // get X from player
								playerRefence.transform.position.y + relativeDistance.y, // get Yfrom player
								myTransform.position.z);  // get X from camera
		}

		void TryToFindPlayer()
		{
			// Obtem a instancia do objeto do jogador
			playerRefence = GameObject.FindWithTag(tagToFollow) as GameObject;
			if (playerRefence != null)
			{
				// Set current position camera to player
				myTransform.position = GetPlayerPosition();
			}
		}

		void Start()
		{
			TryToFindPlayer();
		}

		void Update()
		{
			if (playerRefence == null)
			{
				TryToFindPlayer();
				if (playerRefence == null)
				{
					return;
				}
			}

			Vector3 newPosition = GetPlayerPosition();

			// Interpolate camera position to player
			myTransform.position = Vector3.Lerp(myTransform.position, newPosition, smoothMovement * Time.deltaTime);
		}
	}
}