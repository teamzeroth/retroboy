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
	public enum TypeRefreshPlayerPrefs { INT, STRING, FLOAT };

	public class RefreshGUITextPlayerPrefs : MonoBehaviour
	{

		public TypeRefreshPlayerPrefs typeRefresh = TypeRefreshPlayerPrefs.INT;
		public float timeRefresh = 0.5f;
		public string keyPlayerPrefs;
		public string defaultValueString;
		public int defaultValueInt;
		public float defaultValueFloat;
		private float countTime = 0.0f;

		// Update is called once per frame
		void Update()
		{
			countTime += Time.deltaTime;
			if (countTime >= timeRefresh)
			{
				countTime = 0.0f;
				switch (typeRefresh)
				{
					case TypeRefreshPlayerPrefs.INT:
						guiText.text = "" + PlayerPrefs.GetInt(keyPlayerPrefs, defaultValueInt);
						break;
					case TypeRefreshPlayerPrefs.STRING:
						guiText.text = PlayerPrefs.GetString(keyPlayerPrefs, defaultValueString);
						break;
					case TypeRefreshPlayerPrefs.FLOAT:
						guiText.text = "" + PlayerPrefs.GetFloat(keyPlayerPrefs, defaultValueFloat);
						break;
				}
			}
		}
	}
}
