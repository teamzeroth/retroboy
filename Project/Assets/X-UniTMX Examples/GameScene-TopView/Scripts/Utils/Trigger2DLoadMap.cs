/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace X_UniTMX.Internal
{
	public class Trigger2DLoadMap : MonoBehaviour
	{

		public string nextMapToLoad = "Missing Level Configuration";
		public string[] tagsCanCollide = { "Player" };

		void OnTriggerEnter2D(Collider2D other)
		{
			foreach (string currentTag in tagsCanCollide)
			{
				if (currentTag.Equals(other.tag))
				{
					// Find the first instance of TiledMapComponent and delete!
					Destroy((GameObject.FindObjectOfType<TiledMapComponent>()).gameObject);
					GameObject newMapPrefab = Resources.Load<GameObject>(nextMapToLoad);

					if (newMapPrefab != null)
					{
						GameObject map = Instantiate(newMapPrefab) as GameObject;
						map.name = newMapPrefab.name;
					}
					else
					{
						Debug.LogError("Error load map: " + nextMapToLoad);
					}
					return;
				}
			}
		}

		public void SetNextMap(string map)
		{
			nextMapToLoad = map;
		}
		public void ClearTagsCanCollide()
		{
			tagsCanCollide = null;
		}

		public void AddTagsCanCollide(string value)
		{
			List<string> listTags = new List<string>();

			if (tagsCanCollide != null)
			{
				foreach (string s in tagsCanCollide)
				{
					listTags.Add(s);
				}
			}
			listTags.Add(value);
			tagsCanCollide = listTags.ToArray();
		}
	}
}
