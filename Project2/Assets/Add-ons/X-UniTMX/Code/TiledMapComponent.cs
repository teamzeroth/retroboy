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

		protected Action OnTiledMapInitialized = null;
		//public bool GeneratePrefab = false;
		//public string PrefabSavePath = "Resources/Prefabs/GameMaps";

		#region Editor variables
		public string[] objectLayers = null;
		public string[] imageLayers = null;
		public string[] tileLayers = null;

		public bool[] generateCollider = null;
		public string[] collidersTag = null;
		public int[] collidersPhysicsLayer = null;
		public PhysicMaterial[] collidersPhysicsMaterial3D = null;
		public PhysicsMaterial2D[] collidersPhysicsMaterial2D = null;
		public float[] collidersZDepth = null;
		public float[] collidersWidth = null;
		public bool[] collidersIsInner = null;
		public bool[] collidersIsTrigger = null;
		public bool[] collidersIs2D = null;

		public bool[] generatePrefabs = null;
		public bool[] addMapNameToPrefabName = null;
		public Anchor[] prefabAnchor = null;
		public Vector2[] prefabAnchorValue = null;
		public bool[] prefabNameAsObjName = null;
		
		public bool isToLoadOnStart = false;

		public bool[] simpleTileObjectCalculation;// = true;
		public float[] clipperArcTolerance;// = 0.25;
		public float[] clipperMiterLimit;// = 2.0;
		public ClipperLib.JoinType[] clipperJoinType;// = ClipperLib.JoinType.jtRound;
		public ClipperLib.EndType[] clipperEndType;// = ClipperLib.EndType.etClosedPolygon;
		public float[] clipperDeltaOffset;// = 0;

		public bool[] GenerateTileCollisions;// = true;
		public bool[] foldoutTileCollisions;// = false;
		public string[] TileCollisionsTag;// = "Untagged";
		public int[] TileCollisionsPhysicsLayer;// = 0;
		public PhysicMaterial[] TileCollisionsPhysMaterial3D;// = null;
		public PhysicsMaterial2D[] TileCollisionsPhysMaterial2D;// = null;
		public float[] TileCollisionsZDepth;// = 0;
		public float[] TileCollisionsWidth;// = 1;
		public bool[] TileCollisionsIsInner;// = false;
		public bool[] TileCollisionsIsTrigger;// = false;
		public bool[] TileCollisionsIs2D;// = true;
		public bool[] TileCollisionsIsPolygon;// = false;
		
		public bool foldoutObjectsInLayer = false;
		public bool foldoutImageLayers = false;
		public bool foldoutObjectLayers = false;
		public bool foldoutLayers = false;
		public bool foldoutMapProperties = false;
		public bool foldoutMapCustomProperties = false;
		public bool foldoutMapTileSets = false;

		public bool GlobalMakeUniqueTiles = false;
		public bool[] MakeUniqueTiles = null;

		public bool[] tileLayersFoldoutProperties = null;
		public Material[] tileLayersMaterials = null;
		public string[] tileLayersTag = null;
		public int[] tileLayersPhysicsLayer = null;

		public bool[] objectLayersFoldoutProperties = null;
		public bool[] imageLayersFoldoutProperties = null;
		public Material[] imageLayersMaterials = null;
		public string[] imageLayersTag = null;
		public int[] imageLayersPhysicsLayer = null;
		public bool setCameraAsMapBackgroundColor = false;
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

		/// <summary>
		/// Erases any existing Map and generates a new one using all available information
		/// </summary>
		/// <returns>true if map generation succeeded, false otherwhise</returns>
		public bool Initialize(Action onTiledMapInitialized = null)
		{
			OnTiledMapInitialized = onTiledMapInitialized;
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

		/// <summary>
		/// Loads the Map's XML
		/// </summary>
		public void LoadMap()
		{
			tiledMap = new Map(MapTMX, MapTMXPath);
		}

		void GenerateMap()
		{
			if (tiledMap == null)
				LoadMap();

			for (int i = 0; i < tileLayers.Length; i++)
			{
				if (tileLayers[i] != null)
				{
					TileLayer t = tiledMap.GetTileLayer(tileLayers[i]);
					if (t == null)
						continue;

					t.MakeUniqueTiles = MakeUniqueTiles[i];

					if (tileLayersMaterials != null && i < tileLayersMaterials.Length && tileLayersMaterials[i] != null)
						t.LayerMaterial = tileLayersMaterials[i];

					if (tileLayersTag != null && i < tileLayersTag.Length && tileLayersTag[i] != null)
						t.Tag = tileLayersTag[i];

					if (tileLayersPhysicsLayer != null && i < tileLayersPhysicsLayer.Length)
						t.PhysicsLayer = tileLayersPhysicsLayer[i];
				}
			}

			for (int i = 0; i < imageLayers.Length; i++)
            {
                if (imageLayers[i] != null)
                {
					ImageLayer img = tiledMap.GetImageLayer(imageLayers[i]);
					if (img == null)
						continue;

					if (imageLayersMaterials != null && i < imageLayersMaterials.Length && imageLayersMaterials[i] != null)
						img.LayerMaterial = imageLayersMaterials[i];

					if (imageLayersTag != null && i < imageLayersTag.Length && imageLayersTag[i] != null)
						img.Tag = imageLayersTag[i];

					if (imageLayersPhysicsLayer != null && i < imageLayersPhysicsLayer.Length)
						img.PhysicsLayer = imageLayersPhysicsLayer[i];
                }
            }
			tiledMap.Generate(gameObject, materialDefaultFile, DefaultSortingOrder, GlobalMakeUniqueTiles, setCameraAsMapBackgroundColor, OnGenerateFinished);
		}

		void OnGenerateFinished(Map map)
		{
			tiledMap = map;
			Resources.UnloadUnusedAssets();
			GenerateColliders();
			GeneratePrefabs();
			if (OnTiledMapInitialized != null)
				OnTiledMapInitialized();
		}

		/// <summary>
		/// Generate all Tile Collisions and Object Colliders from layers where they are enabled
		/// </summary>
		public void GenerateColliders()
		{
			for (int i = 0; i < GenerateTileCollisions.Length; i++)
			{
				if (GenerateTileCollisions[i])
				{
					if(TileCollisionsIs2D[i])
						tiledMap.GenerateTileCollision2DFromLayer(tileLayers[i], TileCollisionsIsTrigger[i], TileCollisionsIsPolygon[i], TileCollisionsTag[i], TileCollisionsPhysicsLayer[i], TileCollisionsPhysMaterial2D[i], TileCollisionsZDepth[i],
							simpleTileObjectCalculation[i], clipperArcTolerance[i], clipperMiterLimit[i], clipperJoinType[i], clipperEndType[i], clipperDeltaOffset[i]);
					else
						tiledMap.GenerateTileCollision3DFromLayer(tileLayers[i], TileCollisionsIsTrigger[i], TileCollisionsIsPolygon[i], TileCollisionsTag[i], TileCollisionsPhysicsLayer[i], TileCollisionsPhysMaterial3D[i], TileCollisionsZDepth[i], TileCollisionsWidth[i], TileCollisionsIsInner[i],
							simpleTileObjectCalculation[i], clipperArcTolerance[i], clipperMiterLimit[i], clipperJoinType[i], clipperEndType[i], clipperDeltaOffset[i]);
				}
			}
			for (int i = 0; i < generateCollider.Length; i++)
			{
				if (generateCollider[i])
				{
					MapObjectLayer m = tiledMap.GetObjectLayer(objectLayers[i]);
					m.Tag = collidersTag[i];
					m.PhysicsLayer = collidersPhysicsLayer[i];

					if(collidersIs2D[i])
						tiledMap.GenerateColliders2DFromLayer(m, collidersIsTrigger[i], collidersTag[i], collidersPhysicsLayer[i], collidersPhysicsMaterial2D[i], collidersZDepth[i]);
					else
						tiledMap.GenerateColliders3DFromLayer(m, collidersIsTrigger[i], collidersTag[i], collidersPhysicsLayer[i], collidersPhysicsMaterial3D[i], collidersZDepth[i], collidersWidth[i], collidersIsInner[i]);
				}
			}

		}

		public void GeneratePrefabs()
		{
			if (prefabAnchorValue == null || prefabNameAsObjName == null)
				return;

			for (int i = 0; i < generatePrefabs.Length; i++)
			{
				if (generatePrefabs[i])
				{
					tiledMap.GeneratePrefabsFromLayer(objectLayers[i], prefabAnchorValue[i], addMapNameToPrefabName[i], prefabNameAsObjName[i]);
				}
			}
		}
	}
}