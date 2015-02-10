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
	public class TriggerCollision2DDestroy : MonoBehaviour
	{

		public string[] tagsCanCollide = { "Player" };

		void OnTriggerEnter2D(Collider2D other)
		{
			DestroyThisCollision(other.gameObject);
		}

		void OnCollisionEnter2D(Collision2D other)
		{
			DestroyThisCollision(other.gameObject);
		}

		private void DestroyThisCollision(GameObject other)
		{
			foreach (string currentTag in tagsCanCollide)
			{
				if (other.tag.Equals(currentTag))
				{
					Destroy(gameObject);
					return;
				}
			}
		}
	}
}
