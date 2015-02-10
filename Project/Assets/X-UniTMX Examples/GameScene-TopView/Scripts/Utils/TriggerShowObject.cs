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
	public class TriggerShowObject : MonoBehaviour
	{

		public GameObject objectToShow = null;
		public string[] tagsCanCollide = { "Player" };

		void OnTriggerEnter(Collider other)
		{
			foreach (string currentTag in tagsCanCollide)
			{
				if (other.tag.Equals(currentTag))
				{
					objectToShow.SetActive(true);
					Character2DController jogador = (Character2DController)FindObjectOfType(typeof(Character2DController));
					jogador.SetStateAnimation(EnumPlayerState.STAY);
					return;
				}
			}
		}
	}
}
