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
	public class Trigger2DNextScene : MonoBehaviour
	{

		public string nextScene = "Missing Level Configuration";
		public string[] tagsCanCollide = { "Player" };

		void OnTriggerEnter2D(Collider2D other)
		{
			foreach (string currentTag in tagsCanCollide)
			{
				if (currentTag.Equals(other.tag))
				{
					Application.LoadLevel(nextScene);
					return;
				}
			}
		}

		public void SetNextScene(string value)
		{
			nextScene = value;
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