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
	public class PermantDestroyPlayerPrefs : MonoBehaviour
	{
		public string PlayerPrefsKey = "";
		public bool useObjectNameAsPlayerPrefKey = false;
		public bool DestroyInTriggerEvent = false;
		public bool DestroyInColisionEvent = false;
		public string[] tagsCanCollide = { "Player" };

		void Start()
		{
			if (PlayerPrefs.GetInt(useObjectNameAsPlayerPrefKey ? gameObject.name : PlayerPrefsKey, 0) == 1)
			{
				Destroy(gameObject);
			}
		}

		private void CheckCollider(GameObject other)
		{

			foreach (string currentTag in tagsCanCollide)
			{
				if (other.tag.Equals(currentTag))
				{
					PlayerPrefs.SetInt(useObjectNameAsPlayerPrefKey ? gameObject.name : PlayerPrefsKey, 1);
					Destroy(gameObject);
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (DestroyInTriggerEvent)
			{
				CheckCollider(other.gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other)
		{
			if (DestroyInColisionEvent)
			{
				CheckCollider(other.gameObject);
			}
		}
	}
}
