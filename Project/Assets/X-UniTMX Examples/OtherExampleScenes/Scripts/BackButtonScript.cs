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
	public class BackButtonScript : MonoBehaviour
	{

		public string SceneToLoad = "StartingScene";

		void OnGUI()
		{
			if (GUI.Button(new Rect(0.9f * Screen.width, 0.9f * Screen.height, 0.1f * Screen.width, 0.1f * Screen.height), "Back"))
				Application.LoadLevel("StartingScene");
		}
	}
}
