/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using TObject.Shared;

namespace X_UniTMX
{
	[AddComponentMenu("Tiled Map/Tiled Map Component")]
	public class TiledMapComponent : MonoBehaviour
	{
		public Material materialDefaultFile;
		public TextAsset MapTMX;
		public string MapTMXPath = "GameMaps/";
		public int DefaultSortingOrder = 0;

		public int tileObjectElipsePrecision = 16;
		public bool simpleTileObjectCalculation = true;
		public double clipperArcTolerance = 0.25;
		public double clipperMiterLimit = 2.0;
		public ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound;
		public ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon;
		public float clipperDeltaOffset = 0;

		public bool GenerateTileCollisions = true;
		public bool foldoutTileCollisions = false;
		public float TileCollisionsZDepth = 0;
		public float TileCollisionsWidth = 1;
		public bool TileCollisionsIsInner = false;
		public bool TileCollisionsIsTrigger = false;
		public bool TileCollisionsIs2D = true;

		#region Editor variables
		public string[] objectLayers = null;
		public string[] imageLayers = null;
		public string[] tileLayers = null;

		public bool[] generateCollider = null;
		public float[] collidersZDepth = null;
		public float[] collidersWidth = null;
		public bool[] collidersIsInner = null;
		public bool[] collidersIsTrigger = null;
		public bool[] collidersIs2D = null;
		public bool isToLoadOnStart = false;
		public bool addTileNameToColliderName = true;
		
		public bool foldoutObjectsInLayer = false;
		public bool foldoutImageLayers = false;
		public bool foldoutObjectLayers = false;
		public bool foldoutLayers = false;
		public bool foldoutMapProperties = false;
		public bool foldoutMapCustomProperties = false;

		public bool GlobalMakeUniqueTiles = false;
		public bool[] MakeUniqueTiles = null;

		public bool[] tileLayersFoldoutProperties = null;
		public bool[] objectLayersFoldoutProperties = null;
		public bool[] imageLayersFoldoutProperties = null;
		#endregion

		private Map tiledMap;

		public Map TiledMap
		{
			get { return tiledMap; }
			set { tiledMap = value; }
		}

		public void Awake()
		{
			if (isToLoadOnStart)
			{
				EraseMap();
				Initialize();
			}
		}

		// Use this for map generation
		public bool Initialize()
		{
			EraseMap();
			GenerateMap();

			return TiledMap != null;
		}

		void EraseMap()
		{
			// Destroy any previous map entities
			var children = new List<GameObject>();
			foreach (Transform child in transform)
				children.Add(child.gameObject);
			children.ForEach(child => DestroyImmediate(child, true));
			MeshFilter filter = GetComponent<MeshFilter>();
			if (filter)
				DestroyImmediate(filter, true);

			tiledMap = null;
		}

		public void LoadMap()
		{
			tiledMap = new Map(MapTMX, MapTMXPath);
		}

		void GenerateMap()
		{
			if (tiledMap == null)
				LoadMap();

			for (int i = 0; i < MakeUniqueTiles.Length; i++)
			{
				if (tileLayers[i] != null)
				{
					tiledMap.GetTileLayer(tileLayers[i]).MakeUniqueTiles = MakeUniqueTiles[i];
				}
			}
			tiledMap.Generate(gameObject, materialDefaultFile, DefaultSortingOrder, GlobalMakeUniqueTiles, OnGenerateFinished);
		}

		void OnGenerateFinished(Map map)
		{
			tiledMap = map;
			Resources.UnloadUnusedAssets();
			GenerateColliders();
		}

		public void GenerateColliders()
		{
			if (GenerateTileCollisions)
			{
				tiledMap.GenerateTileCollisions(TileCollisionsIs2D, TileCollisionsIsTrigger, TileCollisionsZDepth, TileCollisionsWidth, TileCollisionsIsInner,
					tileObjectElipsePrecision, simpleTileObjectCalculation,
					clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);
			}
			for (int i = 0; i < generateCollider.Length; i++)
			{
				if (generateCollider[i])
				{
					tiledMap.GenerateCollidersFromLayer(objectLayers[i], collidersIs2D[i], collidersIsTrigger[i], collidersZDepth[i], collidersWidth[i], collidersIsInner[i]);
					tiledMap.GeneratePrefabsFromLayer(objectLayers[i], collidersIs2D[i], addTileNameToColliderName);
				}
			}

		}
	}
}