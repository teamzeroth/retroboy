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
	public class IsometricTrigger2DLoadMap : MonoBehaviour
	{

		public string _nextMapToLoad = null;
		public string[] _tagsCanCollide = { "Player" };
		IsometricGameController _igc = null;

		void Awake()
		{
			_igc = GameObject.FindObjectOfType<IsometricGameController>();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(_nextMapToLoad) || _igc == null)
				return;

			for (int i = 0; i < _tagsCanCollide.Length; i++)
			{
				if (other.CompareTag(_tagsCanCollide[i]))
				{
					_igc.LoadMap(_nextMapToLoad, transform.position);
				}
			}
		}

		public void SetNextMap(string map)
		{
			_nextMapToLoad = map;
		}
	}
}
