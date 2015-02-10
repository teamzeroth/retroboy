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
	public class RespawnPoint : MonoBehaviour
	{

		public string spawnPointName = "";

		public void SetRespawn(string respawn)
		{
			spawnPointName = respawn;
		}
	}
}
