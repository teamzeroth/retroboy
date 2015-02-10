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
	public class MapLoader : MonoBehaviour
	{

		public Material defaultMaterial;
		public int sortingOrder = 0;
		public TextAsset[] Maps;
		public int CurrentMap = 0;

		Map TiledMap;
		public string MapsPath = "Maps";

		public CameraController _camera;

		public int tileObjectEllipsePrecision = 16;
		public bool simpleTileObjectCalculation = false;
		public double clipperArcTolerance = 0.25;
		public double clipperMiterLimit = 2.0;
		public ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound;
		public ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon;
		public float clipperDeltaOffset = 0;

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
			UnloadCurrentMap();
			new Map(Maps[CurrentMap], MapsPath, this.gameObject, defaultMaterial,
							   sortingOrder, false, OnMapLoaded,
							   tileObjectEllipsePrecision, simpleTileObjectCalculation,
							   clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);

		}
		void OnMapLoaded(Map map)
		{
			TiledMap = map;
			if (TiledMap.GetObjectLayer("Collider_0") != null)
				TiledMap.GenerateCollidersFromLayer("Collider_0");
			if (TiledMap.GetObjectLayer("Colliders") != null)
				TiledMap.GenerateCollidersFromLayer("Colliders");
			Debug.Log(TiledMap.ToString());
			MapObjectLayer mol = TiledMap.GetObjectLayer("PropertyTest");
			if (mol != null)
			{
				Debug.Log(mol.GetPropertyAsBoolean("test"));
			}
			// Center camera
			_camera.CamPos = TiledMap.TiledPositionToWorldPoint(TiledMap.Width / 2.0f, TiledMap.Height / 2.0f, _camera.CamPos.z);
			_camera.PixelsPerUnit = TiledMap.TileHeight;
		}
	}
}