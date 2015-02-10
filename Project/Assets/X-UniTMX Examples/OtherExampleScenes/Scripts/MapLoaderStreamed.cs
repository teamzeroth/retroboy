/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections.Generic;

namespace X_UniTMX.Internal
{
	public class MapLoaderStreamed : MonoBehaviour
	{

		public Material defaultMaterial;
		public int sortingOrder = 0;
		public string[] Maps;
		public int CurrentMap = 0;
		public int tileObjectEllipsePrecision = 16;
		public bool simpleTileObjectCalculation = false;

		Map TiledMap;

		public CameraController _camera;

		float _startTime;

		// Use this for initialization
		void Start()
		{
			LoadMap();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetButtonDown("Fire2"))
			{
				CurrentMap--;
				if (CurrentMap < 0)
					CurrentMap = Maps.Length - 1;
				LoadMap();
			}
			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetButtonDown("Fire1"))
			{
				CurrentMap++;
				if (CurrentMap > Maps.Length - 1)
					CurrentMap = 0;
				LoadMap();
			}
		}

		void UnloadCurrentMap()
		{
			var children = new List<GameObject>();
			foreach (Transform child in this.transform) children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));

			MeshFilter filter = GetComponent<MeshFilter>();
			if (filter)
				Destroy(filter);
		}

		void LoadMap()
		{
			//Application.streamingAssetsPath + "/" + 
			UnloadCurrentMap();
			_startTime = Time.realtimeSinceStartup;
			TiledMap = new Map(Maps[CurrentMap], this.gameObject, defaultMaterial, sortingOrder, true, 
								OnMapLoaded, tileObjectEllipsePrecision, simpleTileObjectCalculation);
		}

		void OnMapLoaded(Map map)
		{
			Debug.Log("Time to Load: " + (Time.realtimeSinceStartup - _startTime) + "s");
			//Debug.Log(TiledMap.ToString());

			// Center camera
			_camera.CamPos = TiledMap.TiledPositionToWorldPoint(TiledMap.Width / 2.0f, TiledMap.Height / 2.0f, _camera.CamPos.z);//new Vector3(TiledMap.Width / 2.0f, -TiledMap.Height / 2.0f, _camera.CamPos.z);
			_camera.PixelsPerUnit = TiledMap.TileHeight;
		}
	}
}
