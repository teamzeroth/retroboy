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
	public class Trigger2DRespawnSave : MonoBehaviour
	{
		public string[] tagsCanCollide = { "Player" };
		private string respawnToSave = "Start";

		void Start()
		{
			if (PlayerPrefs.GetString("PlayerRespawn", "").Equals(""))
			{
				PlayerPrefs.SetString("PlayerRespawn", respawnToSave);
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			foreach (string currentTag in tagsCanCollide)
			{
				if (currentTag.Equals(other.tag))
				{
					// Save the next respawn point
					PlayerPrefs.SetString("PlayerRespawn", respawnToSave);
					return;
				}
			}
		}

		public void SaveRespawn(string respawn)
		{
			respawnToSave = respawn;
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

