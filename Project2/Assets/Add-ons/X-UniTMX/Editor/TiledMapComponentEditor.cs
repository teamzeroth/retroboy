/*!
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013 Guilherme "Chaoseiro" Maia
 * Released under the MIT license
 * Check LICENSE.MIT for more details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using X_UniTMX;
using UnityEditor;
using TObject.Shared;
using UnityEngine;
using System.Reflection;

namespace X_UniTMX
{
	[CustomEditor (typeof(TiledMapComponent))]
	//[CanEditMultipleObjects] many errors where occurring when multiple editing components, decided to deactivate this function
	public class TiledMapComponentEditor : Editor
	{
		#region Menu GameObject
		[MenuItem ("GameObject/Create Other/Tiled Game Map")]
		static void CreateGameObject () {
			GameObject obj = new GameObject("Tiled Game Map");
			obj.AddComponent<TiledMapComponent>();
			Undo.RegisterCreatedObjectUndo(obj, "Created Tiled Game Map");
		}
		#endregion

		#region Icon Textures
		public static Texture2D imageIcon;
		public static Texture2D objectIcon;
		public static Texture2D layerIcon;
		public static Texture2D componentIcon;
		public static Texture2D objectTypeIcon_Box;
		public static Texture2D objectTypeIcon_Ellipse;
		public static Texture2D objectTypeIcon_Polyline;
		public static Texture2D objectTypeIcon_Polygon;
		#endregion

		private static bool _changedMap = false;
		private static bool _reloadedMap = false;

		private string _fullMapPath;
		private string _pathToResources;
		private GUIStyle _wrappable;

		private static TiledMapComponent _tiledMapComponent;

		#region Serialized Properties
		SerializedProperty materialDefaultFile;
		SerializedProperty DefaultSortingOrder;
		SerializedProperty isToLoadOnStart;

		//SerializedProperty tileObjectElipsePrecision;
		//SerializedProperty simpleTileObjectCalculation;
		//SerializedProperty clipperArcTolerance;
		//SerializedProperty clipperMiterLimit;
		//SerializedProperty clipperJoinType;
		//SerializedProperty clipperEndType;
		//SerializedProperty clipperDeltaOffset;

		//SerializedProperty GenerateTileCollisions;
		//SerializedProperty foldoutTileCollisions;
		//SerializedProperty TileCollisionsTag;
		//SerializedProperty TileCollisionsPhysicsLayer;
		//SerializedProperty TileCollisionsPhysicsMaterial3D;
		//SerializedProperty TileCollisionsPhysicsMaterial2D;
		//SerializedProperty TileCollisionsZDepth;
		//SerializedProperty TileCollisionsWidth;
		//SerializedProperty TileCollisionsIsInner;
		//SerializedProperty TileCollisionsIsTrigger;
		//SerializedProperty TileCollisionsIs2D;
		//SerializedProperty TileCollisionsIsPolygon;

		SerializedProperty GlobalMakeUniqueTiles;

		//SerializedProperty GeneratePrefab;
		//SerializedProperty PrefabSavePath;

		SerializedProperty SetCameraAsMapBackgroundColor;

		#endregion

		string _untaggedTag = "Untagged";

		void OnEnable()
		{
			_tiledMapComponent = (TiledMapComponent)target;

			DirectoryInfo rootDir = new DirectoryInfo(Application.dataPath);
			FileInfo[] files = rootDir.GetFiles("TiledMapComponentEditor.cs", SearchOption.AllDirectories);
			string editorIconPath = Path.GetDirectoryName(files[0].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
			editorIconPath = editorIconPath + "/Icons";
			imageIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath( editorIconPath + "/layer-image.png");
			objectIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath( editorIconPath + "/layer-object.png");
			layerIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath( editorIconPath + "/layer-tile.png");
			componentIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath( editorIconPath + "/TiledMapComponent Icon.png");
			objectTypeIcon_Box = (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorIconPath + "/insert-rectangle.png");
			objectTypeIcon_Ellipse = (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorIconPath + "/insert-ellipse.png");
			objectTypeIcon_Polyline = (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorIconPath + "/insert-polyline.png");
			objectTypeIcon_Polygon = (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorIconPath + "/insert-polygon.png");

			// Serializable properties setup
			materialDefaultFile = serializedObject.FindProperty("materialDefaultFile");
			DefaultSortingOrder = serializedObject.FindProperty("DefaultSortingOrder");
			isToLoadOnStart = serializedObject.FindProperty("isToLoadOnStart");

			GlobalMakeUniqueTiles = serializedObject.FindProperty("GlobalMakeUniqueTiles");

			//GeneratePrefab = serializedObject.FindProperty("GeneratePrefab");
			//PrefabSavePath = serializedObject.FindProperty("PrefabSavePath");
			SetCameraAsMapBackgroundColor = serializedObject.FindProperty("setCameraAsMapBackgroundColor");

			_wrappable = new GUIStyle();
			_wrappable.wordWrap = true;
		}

		private void ClearCurrentmap()
		{
			// Destroy any previous map entities
			var children = new List<GameObject>();
			foreach (Transform child in _tiledMapComponent.transform)
				children.Add(child.gameObject);
			children.ForEach(child => Undo.DestroyObjectImmediate(child));
			MeshFilter filter = _tiledMapComponent.GetComponent<MeshFilter>();
			if (filter)
				Undo.DestroyObjectImmediate(filter);
		}

		private void DoImportMapButtonGUI()
		{
			if (GUILayout.Button("Import as static Tile Map"))
			{
				ClearCurrentmap();

				if (_tiledMapComponent.Initialize())
				{
					// Generate the prefab
					//if (GeneratePrefab.boolValue)
					//{
					//	string path = PrefabSavePath.stringValue;
					//	while (path.StartsWith("/"))
					//		path.Remove(0, 1);
					//	while (path.EndsWith("/"))
					//		path.Remove(path.Length - 1, 1);

					//	if(!Directory.Exists(string.Concat(Application.dataPath, "/", path)))
					//		AssetDatabase.CreateFolder("Assets", path);

					//	path = string.Concat("Assets/", path, "/", _tiledMapComponent.TiledMap.MapObject.name, ".prefab");
						
					//	PrefabUtility.CreatePrefab(path, _tiledMapComponent.TiledMap.MapObject);
					//}
					Debug.Log("Map succesfully generated!");
				}
			}
		}

		private void ReadPropertiesAndVariables()
		{
			if (_tiledMapComponent.tileLayers != null && _tiledMapComponent.MakeUniqueTiles != null &&
				(_tiledMapComponent.tileLayers.Length > _tiledMapComponent.MakeUniqueTiles.Length ||
				_tiledMapComponent.tileLayers.Length > _tiledMapComponent.GenerateTileCollisions.Length))
				_reloadedMap = true;

			if (_tiledMapComponent.TiledMap == null || _changedMap || _reloadedMap)
			{
				_tiledMapComponent.LoadMap();
			}

			if (_tiledMapComponent.TiledMap == null)
			{
				Debug.LogWarning("Could not load map!");
				return;
			}

			if (_changedMap || _reloadedMap ||
				_tiledMapComponent.objectLayers == null ||
				_tiledMapComponent.generateCollider == null ||
				_tiledMapComponent.collidersTag == null ||
				_tiledMapComponent.collidersPhysicsLayer == null ||
				_tiledMapComponent.collidersIs2D == null ||
				_tiledMapComponent.collidersPhysicsMaterial2D == null ||
				_tiledMapComponent.collidersPhysicsMaterial3D == null ||
				_tiledMapComponent.collidersWidth == null ||
				_tiledMapComponent.collidersZDepth == null ||
				_tiledMapComponent.collidersIsInner == null ||
				_tiledMapComponent.collidersIsTrigger == null ||
				_tiledMapComponent.tileLayers == null ||
				_tiledMapComponent.imageLayers == null ||
				_tiledMapComponent.tileLayersFoldoutProperties == null ||
				_tiledMapComponent.tileLayersMaterials == null ||
				_tiledMapComponent.objectLayersFoldoutProperties == null ||
				_tiledMapComponent.imageLayersFoldoutProperties == null ||
				_tiledMapComponent.imageLayersMaterials == null ||
				_tiledMapComponent.MakeUniqueTiles == null ||
				_tiledMapComponent.GenerateTileCollisions == null)
			{
				_reloadedMap = true;
				List<string> objectLayers = new List<string>();
				List<bool> generateCollider = new List<bool>();
				List<string> collidersTag = new List<string>();
				List<int> collidersPhysicsLayer = new List<int>();
				List<PhysicMaterial> collidersPhysicsMaterial3D = new List<PhysicMaterial>();
				List<PhysicsMaterial2D> collidersPhysicsMaterial2D = new List<PhysicsMaterial2D>();
				List<bool> collidersIs2D = new List<bool>();
				List<float> collidersWidth = new List<float>();
				List<float> collidersZDepth = new List<float>();
				List<bool> collidersIsInner = new List<bool>();
				List<bool> collidersIsTrigger = new List<bool>();

				List<bool> generatePrefabs = new List<bool>();
				List<Anchor> prefabAnchor = new List<Anchor>();
				List<Vector2> prefabAnchorValue = new List<Vector2>();
				List<bool> prefabNameAsObjName = new List<bool>();
				List<bool> prefabNameAddMapName = new List<bool>();
				
				List<string> tileLayers = new List<string>();
				List<string> imageLayers = new List<string>();

				List<bool> makeUniqueTiles = new List<bool>();
				
				List<bool> tileLayersFoldoutProperties = new List<bool>();
				List<Material> tileLayersMaterials = new List<Material>();
				List<string> tileLayersTag = new List<string>();
				List<int> tileLayersPhysicsLayer = new List<int>();

				List<bool> objectLayersFoldoutProperties = new List<bool>();
				List<bool> imageLayersFoldoutProperties = new List<bool>();
				List<Material> imageLayersMaterials = new List<Material>();
				List<string> imageLayersTag = new List<string>();
				List<int> imageLayersPhysicsLayer = new List<int>();

				// Tile Collisions
				List<bool> simpleTileObjectCalculation = new List<bool>();// = true;
				List<float> clipperArcTolerance = new List<float>();// = 0.25;
				List<float> clipperMiterLimit = new List<float>();// = 2.0;
				List<ClipperLib.JoinType> clipperJoinType = new List<ClipperLib.JoinType>();// = ClipperLib.JoinType.jtRound;
				List<ClipperLib.EndType> clipperEndType = new List<ClipperLib.EndType>();// = ClipperLib.EndType.etClosedPolygon;
				List<float> clipperDeltaOffset = new List<float>();// = 0;

				List<bool> GenerateTileCollisions = new List<bool>();// = true;
				List<bool> foldoutTileCollisions = new List<bool>();// = false;
				List<string> TileCollisionsTag = new List<string>();// = "Untagged";
				List<int> TileCollisionsPhysicsLayer = new List<int>();// = 0;
				List<PhysicMaterial> TileCollisionsPhysMaterial3D = new List<PhysicMaterial>();// = null;
				List<PhysicsMaterial2D> TileCollisionsPhysMaterial2D = new List<PhysicsMaterial2D>();// = null;
				List<float> TileCollisionsZDepth = new List<float>();// = 0;
				List<float> TileCollisionsWidth = new List<float>();// = 1;
				List<bool> TileCollisionsIsInner = new List<bool>();// = false;
				List<bool> TileCollisionsIsTrigger = new List<bool>();// = false;
				List<bool> TileCollisionsIs2D = new List<bool>();// = true;
				List<bool> TileCollisionsIsPolygon = new List<bool>();

				foreach (Layer layerNode in _tiledMapComponent.TiledMap.Layers)
				{
					if (layerNode is MapObjectLayer)
					{
						objectLayers.Add(layerNode.Name);
						generateCollider.Add(false);
						collidersTag.Add(_untaggedTag);
						collidersPhysicsLayer.Add(0);
						collidersIs2D.Add(true);
						collidersWidth.Add(1);
						collidersZDepth.Add(0);
						collidersIsInner.Add(false);
						collidersIsTrigger.Add(false);
						collidersPhysicsMaterial3D.Add(null);
						collidersPhysicsMaterial2D.Add(null);
						// prefabs
						generatePrefabs.Add(false);
						prefabNameAddMapName.Add(false);
						prefabNameAsObjName.Add(false);
						prefabAnchor.Add(Anchor.Center);
						prefabAnchorValue.Add(new Vector2(0.5f, 0.5f));
						// properties
						objectLayersFoldoutProperties.Add(false);
					}
					if (layerNode is TileLayer)
					{
						tileLayers.Add(layerNode.Name);
						// Make Unique Tiles
						makeUniqueTiles.Add(false);
						// properties
						tileLayersFoldoutProperties.Add(false);
						// specific material
						tileLayersMaterials.Add(null);

						tileLayersTag.Add(_untaggedTag);
						tileLayersPhysicsLayer.Add(0);

						// Tile Collisions
						GenerateTileCollisions.Add(true);
						foldoutTileCollisions.Add(false);

						simpleTileObjectCalculation.Add(true);
						clipperArcTolerance.Add(0.25f);
						clipperMiterLimit.Add(2.0f);
						clipperJoinType.Add(ClipperLib.JoinType.jtRound);
						clipperEndType.Add(ClipperLib.EndType.etClosedPolygon);
						clipperDeltaOffset.Add(0);

						TileCollisionsTag.Add(_untaggedTag);
						TileCollisionsPhysicsLayer.Add(0);
						TileCollisionsPhysMaterial3D.Add(null);
						TileCollisionsPhysMaterial2D.Add(null);
						TileCollisionsZDepth.Add(0);
						TileCollisionsWidth.Add(1);
						TileCollisionsIsInner.Add(false);
						TileCollisionsIsTrigger.Add(false);
						TileCollisionsIs2D.Add(true);
						TileCollisionsIsPolygon.Add(true);
					}
					if (layerNode is ImageLayer)
					{
						imageLayers.Add(layerNode.Name);
						// properties
						imageLayersFoldoutProperties.Add(false);
						// specific material
						imageLayersMaterials.Add(null);

						imageLayersTag.Add(_untaggedTag);
						imageLayersPhysicsLayer.Add(0);
					}
				}
				bool rebuildArrays = _changedMap || _reloadedMap;
				
				if (rebuildArrays)
				{
					_tiledMapComponent.tileLayers = tileLayers.ToArray();
					_tiledMapComponent.tileLayersFoldoutProperties = tileLayersFoldoutProperties.ToArray();
					_tiledMapComponent.tileLayersMaterials = tileLayersMaterials.ToArray();
					_tiledMapComponent.MakeUniqueTiles = makeUniqueTiles.ToArray();
					_tiledMapComponent.tileLayersTag = tileLayersTag.ToArray();
					_tiledMapComponent.tileLayersPhysicsLayer = tileLayersPhysicsLayer.ToArray();

					_tiledMapComponent.imageLayers = imageLayers.ToArray();
					_tiledMapComponent.imageLayersFoldoutProperties = imageLayersFoldoutProperties.ToArray();
					_tiledMapComponent.imageLayersMaterials = imageLayersMaterials.ToArray();
					_tiledMapComponent.imageLayersTag = imageLayersTag.ToArray();
					_tiledMapComponent.imageLayersPhysicsLayer = imageLayersPhysicsLayer.ToArray();

					_tiledMapComponent.objectLayers = objectLayers.ToArray();
					_tiledMapComponent.objectLayersFoldoutProperties = objectLayersFoldoutProperties.ToArray();

					_tiledMapComponent.generateCollider = generateCollider.ToArray();
					_tiledMapComponent.collidersTag = collidersTag.ToArray();
					_tiledMapComponent.collidersPhysicsLayer = collidersPhysicsLayer.ToArray();
					_tiledMapComponent.collidersIs2D = collidersIs2D.ToArray();
					_tiledMapComponent.collidersPhysicsMaterial2D = collidersPhysicsMaterial2D.ToArray();
					_tiledMapComponent.collidersPhysicsMaterial3D = collidersPhysicsMaterial3D.ToArray();
					_tiledMapComponent.collidersWidth = collidersWidth.ToArray();
					_tiledMapComponent.collidersZDepth = collidersZDepth.ToArray();
					_tiledMapComponent.collidersIsInner = collidersIsInner.ToArray();
					_tiledMapComponent.collidersIsTrigger = collidersIsTrigger.ToArray();

					_tiledMapComponent.generatePrefabs = generatePrefabs.ToArray();
					_tiledMapComponent.addMapNameToPrefabName = prefabNameAddMapName.ToArray();
					_tiledMapComponent.prefabNameAsObjName = prefabNameAsObjName.ToArray();
					_tiledMapComponent.prefabAnchor = prefabAnchor.ToArray();
					_tiledMapComponent.prefabAnchorValue = prefabAnchorValue.ToArray();

					_tiledMapComponent.GenerateTileCollisions = GenerateTileCollisions.ToArray();
					_tiledMapComponent.foldoutTileCollisions = foldoutTileCollisions.ToArray();

					_tiledMapComponent.simpleTileObjectCalculation = simpleTileObjectCalculation.ToArray();
					_tiledMapComponent.clipperArcTolerance = clipperArcTolerance.ToArray();
					_tiledMapComponent.clipperMiterLimit = clipperMiterLimit.ToArray();
					_tiledMapComponent.clipperJoinType = clipperJoinType.ToArray();
					_tiledMapComponent.clipperEndType = clipperEndType.ToArray();
					_tiledMapComponent.clipperDeltaOffset = clipperDeltaOffset.ToArray();

					_tiledMapComponent.TileCollisionsTag = TileCollisionsTag.ToArray();
					_tiledMapComponent.TileCollisionsPhysicsLayer = TileCollisionsPhysicsLayer.ToArray();
					_tiledMapComponent.TileCollisionsPhysMaterial3D = TileCollisionsPhysMaterial3D.ToArray();
					_tiledMapComponent.TileCollisionsPhysMaterial2D = TileCollisionsPhysMaterial2D.ToArray();
					_tiledMapComponent.TileCollisionsZDepth = TileCollisionsZDepth.ToArray();
					_tiledMapComponent.TileCollisionsWidth = TileCollisionsWidth.ToArray();
					_tiledMapComponent.TileCollisionsIsInner = TileCollisionsIsInner.ToArray();
					_tiledMapComponent.TileCollisionsIsTrigger = TileCollisionsIsTrigger.ToArray();
					_tiledMapComponent.TileCollisionsIs2D = TileCollisionsIs2D.ToArray();
					_tiledMapComponent.TileCollisionsIsPolygon = TileCollisionsIsPolygon.ToArray();
				}

				_changedMap = false;
				_reloadedMap = false;
			}
		}

		private void DoLayersGUI()
		{
			ReadPropertiesAndVariables();

			EditorGUIUtility.labelWidth = 250;

			DoTileSetsGUI();

			DoMapPropertiesGUI();

			_tiledMapComponent.foldoutLayers = EditorGUILayout.Foldout(_tiledMapComponent.foldoutLayers, new GUIContent("Map Layers",componentIcon));
			
			if (_tiledMapComponent.foldoutLayers)
			{
				EditorGUI.indentLevel++;
				DoTileLayersGUI();
				DoObjectLayersGUI();
				DoImageLayersGUI();
				EditorGUI.indentLevel--;
			}
		}

		void DoTileSetsGUI()
		{
			_tiledMapComponent.foldoutMapTileSets = EditorGUILayout.Foldout(
				_tiledMapComponent.foldoutMapTileSets, 
				new GUIContent("TileSets Expected Texture's Paths", componentIcon)
			);
			if (_tiledMapComponent.foldoutMapTileSets)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < _tiledMapComponent.TiledMap.TileSets.Count; i++)
				{
					for (int j = 0; j < _tiledMapComponent.TiledMap.TileSets[i].TileSetImages.Count; j++)
					{
						string rootPath = string.Concat(_pathToResources, _tiledMapComponent.MapTMXPath);
						string imageFilePath = _tiledMapComponent.TiledMap.TileSets[i].TileSetImages[j].FullPath;
						while (imageFilePath.StartsWith("../"))
						{
							rootPath = rootPath.Replace(Directory.GetParent(rootPath).Name + Path.AltDirectorySeparatorChar, "");
							imageFilePath = imageFilePath.Remove(0, 3);
						}
						rootPath = Path.Combine(rootPath, imageFilePath);
						
						if (!rootPath.Contains("Resources") || !File.Exists(Path.Combine(Application.dataPath, rootPath)))
							_wrappable.normal.textColor = Color.red;
						else
							_wrappable.normal.textColor = Color.black;
						EditorGUILayout.LabelField(rootPath, _wrappable);
					}
				}
				EditorGUI.indentLevel--;
			}
		}

		void DoMapPropertiesGUI()
		{
			if (_tiledMapComponent.TiledMap == null)
				return;

			_tiledMapComponent.foldoutMapProperties = EditorGUILayout.Foldout(_tiledMapComponent.foldoutMapProperties, new GUIContent("Map Properties", componentIcon) );
			EditorGUI.indentLevel++;
			if (_tiledMapComponent.foldoutMapProperties)
			{
				EditorGUILayout.SelectableLabel(string.Concat("Size (Tiles): ", _tiledMapComponent.TiledMap.MapRenderParameter.Width.ToString(), "x", _tiledMapComponent.TiledMap.MapRenderParameter.Height.ToString()));
				EditorGUILayout.SelectableLabel(string.Concat("Tile Size: ", _tiledMapComponent.TiledMap.MapRenderParameter.TileWidth.ToString(), "x", _tiledMapComponent.TiledMap.MapRenderParameter.TileHeight.ToString()));
				EditorGUILayout.SelectableLabel(string.Concat("Orientation: ", _tiledMapComponent.TiledMap.MapRenderParameter.Orientation.ToString()));
				EditorGUILayout.SelectableLabel(string.Concat("Render Order: ", _tiledMapComponent.TiledMap.MapRenderParameter.MapRenderOrder.ToString()));
				EditorGUILayout.SelectableLabel(string.Concat("Stagger Axis: ", _tiledMapComponent.TiledMap.MapRenderParameter.MapStaggerAxis.ToString()));
				EditorGUILayout.SelectableLabel(string.Concat("Stagger Index: ", _tiledMapComponent.TiledMap.MapRenderParameter.MapStaggerIndex.ToString()));
				if (_tiledMapComponent.TiledMap.MapRenderParameter.Orientation.Equals(Orientation.Hexagonal))
				{
					EditorGUILayout.SelectableLabel(string.Concat("Hex Side Length: ", _tiledMapComponent.TiledMap.MapRenderParameter.HexSideLength.ToString()));
				}
				if (_tiledMapComponent.TiledMap.Properties != null && _tiledMapComponent.TiledMap.Properties.Count > 0)
				{
					_tiledMapComponent.foldoutMapCustomProperties = EditorGUILayout.Foldout(_tiledMapComponent.foldoutMapCustomProperties, new GUIContent("Map Custom Properties"));
					EditorGUI.indentLevel++;
					if (_tiledMapComponent.foldoutMapCustomProperties)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(150.0f));
						EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.MaxWidth(150.0f));
						EditorGUILayout.EndHorizontal();
						foreach (var property in _tiledMapComponent.TiledMap.Properties)
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.SelectableLabel(property.Name, GUILayout.MaxHeight(20));
							EditorGUILayout.SelectableLabel(property.RawValue, GUILayout.MaxHeight(20));
							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.indentLevel--;
		}

		void DoObjectLayersGUI()
		{
			if (_tiledMapComponent.objectLayers == null || _tiledMapComponent.objectLayers.Length < 1)
				return;
			if (_tiledMapComponent.generateCollider == null ||
				_tiledMapComponent.collidersIs2D == null || _tiledMapComponent.collidersTag == null ||
				_tiledMapComponent.collidersPhysicsLayer == null || _tiledMapComponent.collidersIsTrigger == null ||
				_tiledMapComponent.collidersWidth == null || _tiledMapComponent.collidersZDepth == null ||
				_tiledMapComponent.collidersIsInner == null || _tiledMapComponent.collidersPhysicsMaterial3D == null ||
				_tiledMapComponent.collidersPhysicsMaterial2D == null || _tiledMapComponent.prefabAnchor == null ||
				_tiledMapComponent.generatePrefabs == null || _tiledMapComponent.addMapNameToPrefabName == null ||
				_tiledMapComponent.prefabAnchorValue == null || _tiledMapComponent.prefabNameAsObjName == null)
			{
				_reloadedMap = true;
				return;
			}
			_tiledMapComponent.foldoutObjectLayers = EditorGUILayout.Foldout(_tiledMapComponent.foldoutObjectLayers,new GUIContent("Object Layers",objectIcon) );
			EditorGUI.indentLevel++;
			if (_tiledMapComponent.foldoutObjectLayers)
			{
				for (int i = 0; i < _tiledMapComponent.objectLayers.Length; i++)
				{
					if (i >= _tiledMapComponent.generateCollider.Length ||
						i >= _tiledMapComponent.collidersIs2D.Length || i >= _tiledMapComponent.collidersTag.Length ||
						i >= _tiledMapComponent.collidersPhysicsLayer.Length || i >= _tiledMapComponent.collidersIsTrigger.Length ||
						i >= _tiledMapComponent.collidersWidth.Length || i >= _tiledMapComponent.collidersZDepth.Length ||
						i >= _tiledMapComponent.collidersIsInner.Length || i >= _tiledMapComponent.collidersPhysicsMaterial3D.Length ||
						i >= _tiledMapComponent.collidersPhysicsMaterial2D.Length || i >= _tiledMapComponent.prefabAnchor.Length ||
						i >= _tiledMapComponent.generatePrefabs.Length || i >= _tiledMapComponent.addMapNameToPrefabName.Length || 
						i >= _tiledMapComponent.prefabAnchorValue.Length || i >= _tiledMapComponent.prefabNameAsObjName.Length)
					{
						_reloadedMap = true;
						break;
					}
					MapObjectLayer mapObjectLayer = _tiledMapComponent.TiledMap.GetObjectLayer(_tiledMapComponent.objectLayers[i]);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
					EditorGUIUtility.labelWidth = 70;
					EditorGUILayout.LabelField(new GUIContent(_tiledMapComponent.objectLayers[i], objectIcon), GUILayout.ExpandWidth(true));
					if (GUILayout.Button("View Objects", GUILayout.ExpandWidth(true)))
						ShowObjectsWindow(mapObjectLayer);
					EditorGUIUtility.labelWidth = 260;
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					Undo.RecordObject(_tiledMapComponent, "Set Tiled Map Component Object Layer Variables");
					_tiledMapComponent.generateCollider[i] = EditorGUILayout.BeginToggleGroup("Generate Colliders?", _tiledMapComponent.generateCollider[i]);
					if (_tiledMapComponent.generateCollider[i])
					{
						_tiledMapComponent.collidersIs2D[i] = EditorGUILayout.Toggle("Create 2D Colliders for this layer?", _tiledMapComponent.collidersIs2D[i]);
						_tiledMapComponent.collidersTag[i] = EditorGUILayout.TagField("Tag for Colliders", _tiledMapComponent.collidersTag[i]);
						_tiledMapComponent.collidersPhysicsLayer[i] = EditorGUILayout.LayerField("Physics Layer for Colliders", _tiledMapComponent.collidersPhysicsLayer[i]);
						_tiledMapComponent.collidersIsTrigger[i] = EditorGUILayout.Toggle("Set this layer as a trigger layer?", _tiledMapComponent.collidersIsTrigger[i]);
						if (!_tiledMapComponent.collidersIs2D[i])
						{
							EditorGUILayout.LabelField("For 3D configuration:");
							_tiledMapComponent.collidersWidth[i] = EditorGUILayout.FloatField("This layer width", _tiledMapComponent.collidersWidth[i]);
							_tiledMapComponent.collidersZDepth[i] = EditorGUILayout.FloatField("This layer Z depth", _tiledMapComponent.collidersZDepth[i]);
							_tiledMapComponent.collidersIsInner[i] = EditorGUILayout.Toggle("Set this layer with inner collisions?", _tiledMapComponent.collidersIsInner[i]);
							_tiledMapComponent.collidersPhysicsMaterial3D[i] = EditorGUILayout.ObjectField(
								new GUIContent("Physics Material", "Physics Material to be applied to generated colliders"),
								_tiledMapComponent.collidersPhysicsMaterial3D[i],
								typeof(PhysicMaterial),
								false) as PhysicMaterial;
						}
						else
						{
							_tiledMapComponent.collidersPhysicsMaterial2D[i] = EditorGUILayout.ObjectField(
								new GUIContent("Physics Material", "Physics Material to be applied to generated colliders"),
								_tiledMapComponent.collidersPhysicsMaterial2D[i],
								typeof(PhysicsMaterial2D),
								false) as PhysicsMaterial2D;
						}
					}
					EditorGUILayout.EndToggleGroup();
					
					// prefabs
					_tiledMapComponent.generatePrefabs[i] = EditorGUILayout.BeginToggleGroup("Generate Prefabs?", _tiledMapComponent.generatePrefabs[i]);
					if (_tiledMapComponent.generatePrefabs[i])
					{
						_tiledMapComponent.prefabNameAsObjName[i] = EditorGUILayout.Toggle(
							new GUIContent("Set prefabs' name to Object's name?", "Set the name of the generated Prefab to the MapObject's name set in Tiled. Please note that duplicated names get an added index at its end, to avoid duplicated names in Unity."),
							_tiledMapComponent.prefabNameAsObjName[i]);
						
						_tiledMapComponent.addMapNameToPrefabName[i] = EditorGUILayout.Toggle(
							new GUIContent("Add Map's name to prefabs' name?", "Add the Map's name as the prefix of the Prefab's name, like this: MapName_PrefabName"),
							_tiledMapComponent.addMapNameToPrefabName[i]);
						
						_tiledMapComponent.prefabAnchor[i] = (Anchor)EditorGUILayout.EnumPopup(
							new GUIContent("Prefabs Anchor", "The Anchor to be used with prefabs generated from this layer"),
							_tiledMapComponent.prefabAnchor[i]);

						if (_tiledMapComponent.prefabAnchor[i].Equals(Anchor.Custom))
						{
							float x = _tiledMapComponent.prefabAnchorValue[i].x;
							float y = _tiledMapComponent.prefabAnchorValue[i].y;
							x = EditorGUILayout.Slider(
								new GUIContent("Anchor Point X", "The custom anchor point's X value."),
								x, 0, 1);
							y = EditorGUILayout.Slider(
								new GUIContent("Anchor Point Y", "The custom anchor point's Y value."),
								y, 0, 1);
							_tiledMapComponent.prefabAnchorValue[i].Set(x, y);
						}
						else
						{
							_tiledMapComponent.prefabAnchorValue[i] = ObjectExtensions.GetAnchorPointValue(_tiledMapComponent.prefabAnchor[i]);
						}
					}
					EditorGUILayout.EndToggleGroup();

					EditorGUI.indentLevel--;
					DoPropertiesGUI(mapObjectLayer, i, _tiledMapComponent.objectLayersFoldoutProperties);
					EditorGUI.indentLevel--;
					EditorGUILayout.Space();
				}
			}
			EditorGUI.indentLevel--;
		}

		void DoTileLayersGUI()
		{
			if (_tiledMapComponent.tileLayers == null || _tiledMapComponent.tileLayers.Length < 1)
				return;
			if (_tiledMapComponent.MakeUniqueTiles == null || _tiledMapComponent.tileLayersMaterials == null ||
				_tiledMapComponent.tileLayersTag == null || _tiledMapComponent.tileLayersPhysicsLayer == null)
			{
				_reloadedMap = true;
				return;
			}
			_tiledMapComponent.foldoutObjectsInLayer = EditorGUILayout.Foldout(_tiledMapComponent.foldoutObjectsInLayer, new GUIContent("Tile Layers",layerIcon));
			EditorGUI.indentLevel++;
			if (_tiledMapComponent.foldoutObjectsInLayer)
			{
				for (int i = 0; i < _tiledMapComponent.tileLayers.Length; i++)
				{
					if (i >= _tiledMapComponent.MakeUniqueTiles.Length || i >= _tiledMapComponent.tileLayersMaterials.Length ||
						i >= _tiledMapComponent.tileLayersTag.Length || i >= _tiledMapComponent.tileLayersPhysicsLayer.Length)
					{
						_reloadedMap = true;
						break;
					}
					EditorGUILayout.LabelField(new GUIContent(_tiledMapComponent.tileLayers[i], layerIcon), GUILayout.Height(20));
					Undo.RecordObject(_tiledMapComponent, "Set Tiled Map Component Tile Layer Variables");
					EditorGUI.indentLevel++;
					
					_tiledMapComponent.MakeUniqueTiles[i] = EditorGUILayout.ToggleLeft("Make Unique Tiles?", _tiledMapComponent.MakeUniqueTiles[i]);
					
					_tiledMapComponent.tileLayersMaterials[i] = EditorGUILayout.ObjectField(
						new GUIContent("Layer Specific Material", "If set, this Tile Layer will use this Material instead of Default Material"),
						_tiledMapComponent.tileLayersMaterials[i], typeof(Material), false) as Material;
					
					_tiledMapComponent.tileLayersTag[i] = EditorGUILayout.TagField("Layer GameObject's Tag", _tiledMapComponent.tileLayersTag[i]);
					_tiledMapComponent.tileLayersPhysicsLayer[i] = EditorGUILayout.LayerField("Layer GameObject's Physics Layer", _tiledMapComponent.tileLayersPhysicsLayer[i]);

					DoTileCollisionsGUI(i);
					EditorGUI.indentLevel--;
					DoPropertiesGUI(_tiledMapComponent.TiledMap.GetTileLayer(_tiledMapComponent.tileLayers[i]), i, _tiledMapComponent.tileLayersFoldoutProperties);
				}
			}
			EditorGUI.indentLevel--;
		}

		void DoImageLayersGUI()
		{
			if (_tiledMapComponent.imageLayers == null || _tiledMapComponent.imageLayers.Length < 1)
				return;
			if (_tiledMapComponent.imageLayersMaterials == null ||
				_tiledMapComponent.imageLayersTag == null || _tiledMapComponent.imageLayersPhysicsLayer == null)
			{
				_reloadedMap = true;
				return;
			}
			_tiledMapComponent.foldoutImageLayers = EditorGUILayout.Foldout(_tiledMapComponent.foldoutImageLayers, new GUIContent("Image Layers",imageIcon));
			EditorGUI.indentLevel++;
			if (_tiledMapComponent.foldoutImageLayers)
			{
			    for (int i = 0; i < _tiledMapComponent.imageLayers.Length; i++)
				{
					EditorGUILayout.LabelField(new GUIContent(_tiledMapComponent.imageLayers[i], imageIcon), GUILayout.Height(20));
					if (i >= _tiledMapComponent.imageLayersMaterials.Length ||
						i >= _tiledMapComponent.imageLayersTag.Length || i >= _tiledMapComponent.imageLayersPhysicsLayer.Length)
					{
						_reloadedMap = true;
						break;
					}
					_tiledMapComponent.imageLayersMaterials[i] = EditorGUILayout.ObjectField(
						new GUIContent("Layer Specific Material", "If set, this Image Layer will use this Material instead of Default Material"),
						_tiledMapComponent.imageLayersMaterials[i], typeof(Material), false) as Material;

					_tiledMapComponent.imageLayersTag[i] = EditorGUILayout.TagField("Layer GameObject's Tag", _tiledMapComponent.imageLayersTag[i]);
					_tiledMapComponent.imageLayersPhysicsLayer[i] = EditorGUILayout.LayerField("Layer GameObject's Physics Layer", _tiledMapComponent.imageLayersPhysicsLayer[i]);
					DoPropertiesGUI(_tiledMapComponent.TiledMap.GetImageLayer(_tiledMapComponent.imageLayers[i]), i, _tiledMapComponent.imageLayersFoldoutProperties);
				}
			}
			EditorGUI.indentLevel--;
		}

		void DoPropertiesGUI(Layer layer, int layerNumber, bool[] foldout)
		{
			if (layer == null || layer.Properties == null || layer.Properties.Count < 1)
				return;

			EditorGUI.indentLevel++;
			foldout[layerNumber] = EditorGUILayout.Foldout(foldout[layerNumber], new GUIContent("Properties"));
			if (foldout[layerNumber])
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(150.0f));
				EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.MaxWidth(150.0f));
				EditorGUILayout.EndHorizontal();
				foreach (var property in layer.Properties)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.SelectableLabel(property.Name, GUILayout.MaxHeight(20));
					EditorGUILayout.SelectableLabel(property.RawValue, GUILayout.MaxHeight(20));
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		void DoTileCollisionsGUI(int layerNumber)
		{
			if (_tiledMapComponent.GenerateTileCollisions == null || _tiledMapComponent.foldoutTileCollisions == null ||
				_tiledMapComponent.TileCollisionsIs2D == null || _tiledMapComponent.TileCollisionsIsTrigger == null ||
				_tiledMapComponent.TileCollisionsIsPolygon == null || _tiledMapComponent.TileCollisionsTag == null ||
				_tiledMapComponent.TileCollisionsPhysicsLayer == null || _tiledMapComponent.TileCollisionsPhysMaterial2D == null ||
				_tiledMapComponent.TileCollisionsPhysMaterial3D == null || _tiledMapComponent.simpleTileObjectCalculation == null ||
				_tiledMapComponent.clipperArcTolerance == null || _tiledMapComponent.clipperDeltaOffset == null ||
				_tiledMapComponent.clipperEndType == null || _tiledMapComponent.clipperJoinType == null ||
				_tiledMapComponent.clipperMiterLimit == null || _tiledMapComponent.TileCollisionsZDepth == null ||
				_tiledMapComponent.TileCollisionsWidth == null || _tiledMapComponent.TileCollisionsIsInner == null ||
				layerNumber >= _tiledMapComponent.GenerateTileCollisions.Length || layerNumber >= _tiledMapComponent.foldoutTileCollisions.Length ||
				layerNumber >= _tiledMapComponent.TileCollisionsIs2D.Length || layerNumber >= _tiledMapComponent.TileCollisionsIsTrigger.Length ||
				layerNumber >= _tiledMapComponent.TileCollisionsIsPolygon.Length || layerNumber >= _tiledMapComponent.TileCollisionsTag.Length ||
				layerNumber >= _tiledMapComponent.TileCollisionsPhysicsLayer.Length || layerNumber >= _tiledMapComponent.TileCollisionsPhysMaterial2D.Length ||
				layerNumber >= _tiledMapComponent.TileCollisionsPhysMaterial3D.Length || layerNumber >= _tiledMapComponent.simpleTileObjectCalculation.Length ||
				layerNumber >= _tiledMapComponent.clipperArcTolerance.Length || layerNumber >= _tiledMapComponent.clipperDeltaOffset.Length ||
				layerNumber >= _tiledMapComponent.clipperEndType.Length || layerNumber >= _tiledMapComponent.clipperJoinType.Length ||
				layerNumber >= _tiledMapComponent.clipperMiterLimit.Length || layerNumber >= _tiledMapComponent.TileCollisionsZDepth.Length ||
				layerNumber >= _tiledMapComponent.TileCollisionsWidth.Length || layerNumber >= _tiledMapComponent.TileCollisionsIsInner.Length
				)
			{
				_reloadedMap = true;
				return;
			}

			EditorGUIUtility.labelWidth = 260;
			Undo.RecordObject(_tiledMapComponent, "Set Tiled Map Component Tile Collisions");
			_tiledMapComponent.GenerateTileCollisions[layerNumber] = EditorGUILayout.BeginToggleGroup(new GUIContent("Generate Tile Collisions?", TiledMapComponentEditor.objectIcon), _tiledMapComponent.GenerateTileCollisions[layerNumber]);
			if (_tiledMapComponent.GenerateTileCollisions[layerNumber])
			{
				EditorGUI.indentLevel++;
				_tiledMapComponent.foldoutTileCollisions[layerNumber] = EditorGUILayout.Foldout(_tiledMapComponent.foldoutTileCollisions[layerNumber], new GUIContent("Tile Collisions", TiledMapComponentEditor.objectIcon));
				if (_tiledMapComponent.foldoutTileCollisions[layerNumber])
				{
					_tiledMapComponent.TileCollisionsIs2D[layerNumber] = EditorGUILayout.Toggle("Create 2D tile collisions?", _tiledMapComponent.TileCollisionsIs2D[layerNumber]);
					_tiledMapComponent.TileCollisionsIsTrigger[layerNumber] = EditorGUILayout.Toggle("Set tile collisions as a trigger?", _tiledMapComponent.TileCollisionsIsTrigger[layerNumber]);
					_tiledMapComponent.TileCollisionsIsPolygon[layerNumber] = EditorGUILayout.Toggle(
						new GUIContent("Generate Polygonal tile collisions?", "If set, will generate PolygonColliders, else will generate EdgeColliders"),
						_tiledMapComponent.TileCollisionsIsPolygon[layerNumber]);
					_tiledMapComponent.TileCollisionsTag[layerNumber] = EditorGUILayout.TagField("Tag for tile collisions", _tiledMapComponent.TileCollisionsTag[layerNumber]);
					_tiledMapComponent.TileCollisionsPhysicsLayer[layerNumber] = EditorGUILayout.LayerField("Physics Layer for tile collisions", _tiledMapComponent.TileCollisionsPhysicsLayer[layerNumber]);

					if (_tiledMapComponent.TileCollisionsIs2D[layerNumber])
						_tiledMapComponent.TileCollisionsPhysMaterial2D[layerNumber] = EditorGUILayout.ObjectField(
								new GUIContent("Physics Material", "Physics Material to be applied to generated colliders"),
								_tiledMapComponent.TileCollisionsPhysMaterial2D[layerNumber],
								typeof(PhysicsMaterial2D),
								false) as PhysicsMaterial2D;
					else
						_tiledMapComponent.TileCollisionsPhysMaterial3D[layerNumber] = EditorGUILayout.ObjectField(
								new GUIContent("Physics Material", "Physics Material to be applied to generated colliders"),
								_tiledMapComponent.TileCollisionsPhysMaterial3D[layerNumber],
								typeof(PhysicMaterial),
								false) as PhysicMaterial;

					_tiledMapComponent.simpleTileObjectCalculation[layerNumber] = EditorGUILayout.ToggleLeft("Simple Calculation Tile Objects", _tiledMapComponent.simpleTileObjectCalculation[layerNumber]);
					if (!_tiledMapComponent.simpleTileObjectCalculation[layerNumber])
					{
						_tiledMapComponent.clipperArcTolerance[layerNumber] = EditorGUILayout.FloatField("Clipper Arc Tolerance", (float)_tiledMapComponent.clipperArcTolerance[layerNumber]);
						_tiledMapComponent.clipperDeltaOffset[layerNumber] = EditorGUILayout.FloatField("Clipper Delta Offset", (float)_tiledMapComponent.clipperDeltaOffset[layerNumber]);
						_tiledMapComponent.clipperEndType[layerNumber] = (ClipperLib.EndType)EditorGUILayout.EnumPopup("Clipper Offset End Type", _tiledMapComponent.clipperEndType[layerNumber]);
						_tiledMapComponent.clipperJoinType[layerNumber] = (ClipperLib.JoinType)EditorGUILayout.EnumPopup("Clipper Offset Join Type", _tiledMapComponent.clipperJoinType[layerNumber]);
						_tiledMapComponent.clipperMiterLimit[layerNumber] = EditorGUILayout.FloatField("Clipper Miter Limit", (float)_tiledMapComponent.clipperMiterLimit[layerNumber]);
					}

					if (!_tiledMapComponent.TileCollisionsIs2D[layerNumber])
					{
						EditorGUILayout.LabelField("For 3D configuration:");
						_tiledMapComponent.TileCollisionsWidth[layerNumber] = EditorGUILayout.FloatField("Tile collisions width", _tiledMapComponent.TileCollisionsWidth[layerNumber]);
						_tiledMapComponent.TileCollisionsZDepth[layerNumber] = EditorGUILayout.FloatField("Tile collisions Z depth", _tiledMapComponent.TileCollisionsZDepth[layerNumber]);
						_tiledMapComponent.TileCollisionsIsInner[layerNumber] = EditorGUILayout.Toggle("Set tile collisions with inner collisions?", _tiledMapComponent.TileCollisionsIsInner[layerNumber]);
					}
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();
		}

		void ShowObjectsWindow(MapObjectLayer objectLayer)
		{
			TiledMapObjectsWindow.Init(objectLayer);
		}

		private void DoClearMapButtonGUI()
		{
			if (GUILayout.Button("Clear Tile Map"))
			{
				ClearCurrentmap();
				Debug.Log("Map cleared!");
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			Undo.RecordObject(_tiledMapComponent, "Set Tiled Map Component Variables");
			EditorGUIUtility.labelWidth = 150;
			//if (!serializedObject.isEditingMultipleObjects)
			//{
			EditorGUI.BeginChangeCheck();
			TextAsset oldRefMapTMX = _tiledMapComponent.MapTMX;
			_tiledMapComponent.MapTMX = (TextAsset)EditorGUILayout.ObjectField(new GUIContent("Map xml:", componentIcon), _tiledMapComponent.MapTMX, typeof(TextAsset), true);
			if (EditorGUI.EndChangeCheck())
			{
				if (oldRefMapTMX == null)
				{
					_changedMap = true;
				}
				else
				{
					if (EditorUtility.DisplayDialog("Confirm!", "Some configuration can be lost, ok?", "OK", "Cancel"))
					{
						if (oldRefMapTMX.name.Equals(_tiledMapComponent.MapTMX.name))
							_reloadedMap = true;
						else
							_changedMap = true;
					}
				}
			}
			//}
			if(_tiledMapComponent.MapTMX != null) {
				// Think a better solution for clean path... AssetDatabase only works inside the Editor
				_fullMapPath = AssetDatabase.GetAssetPath(_tiledMapComponent.MapTMX);
				_pathToResources = string.Empty;
				string[] listPath = _fullMapPath.Split('/');
				int resourcesIndex = 0;
				for (int i = 0; i < listPath.Length; i++)
				{
					if(i > 0)
						_pathToResources = string.Concat(_pathToResources, listPath[i], '/');
					if (listPath[i].Equals("Resources"))
					{
						resourcesIndex = i;
						break;
					}
				}
				if(resourcesIndex < 1)
				{
					EditorGUILayout.HelpBox("Map file must be in a Resources folder!", MessageType.Error, true);
					return;
				}
				_tiledMapComponent.MapTMXPath = "";
				for(int i = resourcesIndex + 1; i < listPath.Length - 1; i++)
				{
					_tiledMapComponent.MapTMXPath += listPath[i] + "/";
				}

				//if (!serializedObject.isEditingMultipleObjects)
				EditorGUILayout.LabelField ("Path Map:", _tiledMapComponent.MapTMXPath);
                
				materialDefaultFile.objectReferenceValue = (Material)EditorGUILayout.ObjectField("Default material tile map", materialDefaultFile.objectReferenceValue, typeof(Material), true);
				if(materialDefaultFile.objectReferenceValue != null)
				{
					if (GUILayout.Button("Reload XML MAP"))
					{
						if(EditorUtility.DisplayDialog("Confirm!", "Some object layer configuration can be lost, ok?","OK","Cancel")) {
							_reloadedMap = true;
						}
					}
				}
				else
				{
					EditorGUILayout.HelpBox ("Missing default material for map, please select a material!", MessageType.Error, true);
				}
				isToLoadOnStart.boolValue = EditorGUILayout.Toggle(
					new GUIContent("Load this on awake?", "Check to generate the map when the scene starts."), 
					isToLoadOnStart.boolValue);
				GlobalMakeUniqueTiles.boolValue = EditorGUILayout.Toggle(
					new GUIContent("Make unique tiles?", "Check to make all TileLayers generate Unique Tiles."), 
					GlobalMakeUniqueTiles.boolValue);

				DefaultSortingOrder.intValue = EditorGUILayout.IntField(
					new GUIContent("Default Sorting Order", "This will be the starting Sorting Order value for generated tiles/meshes."), 
					DefaultSortingOrder.intValue);

				SetCameraAsMapBackgroundColor.boolValue = EditorGUILayout.Toggle(
					new GUIContent("Set Camera Color?", "Set camera background color as map background color."), 
					SetCameraAsMapBackgroundColor.boolValue);

				//GeneratePrefab.boolValue = EditorGUILayout.Toggle(
				//	new GUIContent("Generate Prefab?", "Check to generate a prefab from the generated map."),
				//	GeneratePrefab.boolValue);
				//if (GeneratePrefab.boolValue)
				//{
				//	PrefabSavePath.stringValue = EditorGUILayout.TextField(
				//		new GUIContent("Prefab Save Path:", "Path to folder where the generated prefab will be saved. The name of the map will be used as the Prefab's name."),
				//		PrefabSavePath.stringValue);
				//}

				DoLayersGUI();

				if (materialDefaultFile.objectReferenceValue != null)
				{
					DoImportMapButtonGUI();
					DoClearMapButtonGUI();
				}
			}
			else
			{
				EditorGUILayout.HelpBox ("Missing map file, please select a XML map!", MessageType.Error, true);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
