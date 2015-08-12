/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using TObject.Shared;
using X_UniTMX.Utils;

namespace X_UniTMX
{
	/// <summary>
	/// Defines the possible orientations for a Map.
	/// </summary>
	public enum Orientation : byte
	{
		/// <summary>
		/// The tiles of the map are orthogonal.
		/// </summary>
		Orthogonal,

		/// <summary>
		/// The tiles of the map are isometric.
		/// </summary>
		Isometric,

		/// <summary>
		/// The tiles of the map are isometric (staggered).
		/// </summary>
		Staggered,

		/// <summary>
		/// The tiles of the map are hexagonal (staggered).
		/// </summary>
		Hexagonal,
	}

	/// <summary>
	/// Defines the possible Rendering orders for the tiles in a Map
	/// </summary>
	public enum RenderOrder : byte
	{
		/// <summary>
		/// Tiles are rendered bottom-top, right-left
		/// </summary>
		Right_Down,
		/// <summary>
		/// Tiles are rendered top-bottom, right-left
		/// </summary>
		Right_Up,
		/// <summary>
		/// Tiles are rendered bottom-top, left-right
		/// </summary>
		Left_Down,
		/// <summary>
		/// Tiles are rendered top-bottom, left-right
		/// </summary>
		Left_Up
	}

    /// <summary>
    /// Defines the possible Stagger Axis for Isometric and Hexagonal Staggered maps
    /// </summary>
    public enum StaggerAxis : byte
    {
        /// <summary>
        /// Tiles are staggered in the X-axis
        /// </summary>
        X,
        /// <summary>
        /// Tiles are staggered in the Y-axis (default)
        /// </summary>
        Y
    }

    /// <summary>
    /// Defines the possible Stagger Index for Isometric and Hexagonal Staggered maps
    /// </summary>
    public enum StaggerIndex : byte
    {
		/// <summary>
        /// Staggering begins in an even column
        /// </summary>
        Even,
        /// <summary>
        /// Staggering begins in an odd column (default)
        /// </summary>
        Odd
    }

	/// <summary>
	/// A delegate used for searching for map objects.
	/// </summary>
	/// <param name="layer">The current layer.</param>
	/// <param name="mapObj">The current object.</param>
	/// <returns>True if this is the map object desired, false otherwise.</returns>
	public delegate bool MapObjectFinder(MapObjectLayer layer, MapObject mapObj);

	/// <summary>
	/// A class containing the Map's Render Parameters
	/// </summary>
	public class MapRenderParameters
	{
		/// <summary>
		/// Gets the orientation of the map.
		/// </summary>
		public Orientation Orientation { get; private set; }

		/// <summary>
		/// Gets this Map's RenderOrder.
		/// </summary>
		public RenderOrder MapRenderOrder { get; private set; }

		/// <summary>
		/// This Map's StaggerAxis (for isometric and hexagonal staggered maps)
		/// </summary>
		public StaggerAxis MapStaggerAxis { get; private set; }

		/// <summary>
		/// This Map's StaggerIndex (for isometric and hexagonal staggered maps)
		/// </summary>
		public StaggerIndex MapStaggerIndex { get; private set; }

		/// <summary>
		/// Gets the width (in tiles) of the map.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the map.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets the width of a tile in the map.
		/// </summary>
		public int TileWidth { get; private set; }

		/// <summary>
		/// Gets the height of a tile in the map.
		/// </summary>
		public int TileHeight { get; private set; }

		/// <summary>
		/// For Hexagonal maps, the lenght of the Hex side. Usually it is TileHeight / 2 for StaggerAxis.Y or TileWidth / 2 for StaggerAxis.X
		/// </summary>
		public int HexSideLength { get; private set; }

		/// <summary>
		/// This Map's Background Color.
		/// </summary>
		public Color BackgroundColor { get; private set; }

		/// <summary>
		/// Default constructor that initializes every render parameter
		/// </summary>
		/// <param name="orientation">Map's Orientation</param>
		/// <param name="renderOrder">Map's RenderOrder</param>
		/// <param name="staggerAxis">Map's StaggerAxis</param>
		/// <param name="staggerIndex">Map's StaggerIndex</param>
		/// <param name="width">Map's Width</param>
		/// <param name="height">Map's Height</param>
		/// <param name="tileWidth">Map's TileWidth</param>
		/// <param name="tileHeight">Map's TileHeight</param>
		/// <param name="hexSideLength">Hexagonal Map's stagger length</param>
		/// <param name="backgroundColor">Map's Background Color</param>
		public MapRenderParameters(
			Orientation orientation, RenderOrder renderOrder, StaggerAxis staggerAxis, StaggerIndex staggerIndex,
			int width, int height, int tileWidth, int tileHeight, int hexSideLength,
			Color backgroundColor)
		{
			Orientation = orientation;
			MapRenderOrder = renderOrder;
			MapStaggerAxis = staggerAxis;
			MapStaggerIndex = staggerIndex;
			Width = width;
			Height = height;
			TileWidth = tileWidth;
			TileHeight = tileHeight;
			HexSideLength = hexSideLength;
			BackgroundColor = backgroundColor;
		}
	}

	/// <summary>
	/// A full map from Tiled.
	/// </summary>
	public class Map : PropertyContainer
	{
		#region Public Variables [private setters]
		/// <summary>
		/// The difference in layer depth between layers.
		/// </summary>
		/// <remarks>
		/// The algorithm for creating the LayerDepth for each layer when enumerating from
		/// back to front is:
		/// float layerDepth = 1f - (LayerDepthSpacing * x);</remarks>
		//public const int LayerDepthSpacing = 1;

		/// <summary>
		/// How accurate will be the mapObjects that are approximated to an ellipse format.
		/// </summary>
		/// <remarks>
		/// Increasing this value will generate higher-quality ellipsoides, at the cost of more vertices.
		/// This number is the number of generated vertices the ellipsoide will have.
		/// </remarks>
		//public static int EllipsoideColliderApproximationFactor = 16;

		/// <summary>
		/// Gets the version of Tiled used to create the Map.
		/// </summary>
		public string Version { get; private set; }

		/// <summary>
		/// Gets a collection of all of the tiles in the map.
		/// </summary>
		public Dictionary<int, Tile> Tiles { get; private set; }

		/// <summary>
		/// Gets a collection of all of the layers in the map.
		/// </summary>
		public List<Layer> Layers { get; private set; }

		/// <summary>
		/// Gets a collection of all of the tile sets in the map.
		/// </summary>
		public List<TileSet> TileSets { get; private set; }

		/// <summary>
		/// Gets this map's Game Object Parent
		/// </summary>
		public GameObject Parent { get; private set; }

		/// <summary>
		/// Gets this map's Game Object
		/// </summary>
		public GameObject MapGameObject { get; private set; }

		/// <summary>
		/// This Map's NanoXMLNode node
		/// </summary>
		public NanoXMLNode MapNode { get; private set; }

		/// <summary>
		/// The next ObjectID to be used
		/// </summary>
		public int NextObjectID { get; private set; }

		/// <summary>
		/// This Map's Render Parameters
		/// </summary>
		public MapRenderParameters MapRenderParameter { get; private set; }
		#endregion

		#region Public Variables [public setters]
		/// <summary>
		/// Map's Tile Layers' initial Sorting Order
		/// </summary>
		public int DefaultSortingOrder = 0;

		/// <summary>
		/// True if is loading a map from Streaming Path or HTTP url
		/// </summary>
		public bool UsingStreamingAssetsPath = false;
		
		/// <summary>
		/// This Map's base Tile Material;
		/// </summary>
		public Material BaseTileMaterial;

		/// true to generate Unique Tiles on all tile layers
		/// </summary>
		public bool GlobalMakeUniqueTiles = false;
		#endregion

		#region Private Variables
		private readonly Dictionary<string, Layer> namedLayers = new Dictionary<string, Layer>();

		private string _mapName = "Map";
		private string _mapPath = "Map";
		private string _mapExtension = ".tmx";
		private int _numberOfTileSetsToLoad = 0;
		private int _tileSetsToLoaded = 0;

		private List<Material> TileSetsMaterials;
		#endregion

		#region Delegates
		/// <summary>
		/// Delegate that is called when this map finishes loading
		/// </summary>
		public Action<Map> OnMapFinishedLoading = null;

		/// <summary>
		/// Delegate that is called when this map finishes being generated
		/// </summary>
		public Action<Map> OnMapFinishedGeneration = null;
		#endregion

		#region Custom X-UniTMX Properties
		/// <summary>
		/// Custom Object Type for Collider Objects
		/// </summary>
		public static string Object_Type_Collider = "Collider";
		/// <summary>
		/// Custom Object Type for Trigger Objects
		/// </summary>
		public static string Object_Type_Trigger = "Trigger";
		/// <summary>
		/// Custom Object Type for Objects with no collider
		/// </summary>
		public static string Object_Type_NoCollider = "NoCollider";

		/// <summary>
		/// Custom Property for Prefabs defining its name inside Resources folder. This will be used together with Property_PrefabPath to load the prefab inside Resources folder
		/// </summary>
		public static string Property_PrefabName = "prefab";
		/// <summary>
		/// Custom Property for Prefabs defining its path inside Resources folder. This will be used together with Property_PrefabName to load the prefab inside Resources folder
		/// </summary>
		public static string Property_PrefabPath = "prefab path";
		/// <summary>
		/// Custom Property for Prefabs defining its Z position. Useful for 2.5D and 3D games
		/// </summary>
		public static string Property_PrefabZDepth = "prefab z depth";
		/// <summary>
		/// Custom Property for Prefabs defining to add a collider to the prefab. If value is set to '3D' (or just '3'), it will add a 3D collider, else it defaults to 2D.
		/// </summary>
		public static string Property_PrefabAddCollider = "prefab add collider";
		///// <summary>
		///// Custom Property for Prefabs defining to send a message to all scripts attached to this prefab
		///// </summary>
		//public const string Property_PrefabSendMessage = "prefab send message ";
		///// <summary>
		///// Custom Property for Prefabs defining to set its position equals to its generated collider
		///// </summary>
		//public static string Property_PrefabFixColliderPosition = "prefab equals position collider";

		/// <summary>
		/// Custom Property for Colliders defining the GameObject's Physics Layer by ID
		/// </summary>
		public static string Property_Layer = "layer";
		/// <summary>
		/// Custom Property for Colliders defining the GameObject's Physics Layer by name.
		/// </summary>
		public static string Property_LayerName = "layer name";
		/// <summary>
		/// Custom Property for Colliders defining the GameObject's Tag
		/// </summary>
		public static string Property_Tag = "tag";
		/// <summary>
		/// Custom Property for Colliders defining the renderer's Sorting Layer by name, if any renderer is present
		/// </summary>
		public static string Property_SortingLayerName = "sorting layer name";
		/// <summary>
		/// Custom Property for Colliders defining the renderer's Sorting Order, if any renderer is present
		/// </summary>
		public static string Property_SortingOrder = "sorting order";
		/// <summary>
		/// Custom Property for Colliders defining to generate a Mesh for debugging
		/// </summary>
		public static string Property_CreateMesh = "create mesh3d";
		/// <summary>
		/// Custom Property for Colliders defining a color for the material of this collider, if any.
		/// </summary>
		public static string Property_SetMaterialColor = "set material color";
#if !UNITY_5
		///// <summary>
		///// Custom Property for Colliders defining to add a component to the collider's GameObject
		///// </summary>
		public static string Property_AddComponent = "add component";
#endif
		/// <summary>
		/// Custom Property for Colliders defining to send a message to all scripts attached to the collider's GameObject
		/// </summary>
		public static string Property_SendMessage = "send message";
		#endregion

		// Constructors only parses the XML
		#region Constructors
		/// <summary>
		/// Create a Tiled Map using the raw XML string as parameter.
		/// </summary>
		/// <param name="mapXML">Raw map XML string</param>
		/// <param name="MapName">Map's name</param>
		/// <param name="mapPath">Path to XML folder, so we can read relative paths for tilesets</param>
		/// <param name="onMapFinishedLoading">Callback for when map xml finishes loading</param>
		public Map(string mapXML, string MapName, string mapPath, Action<Map> onMapFinishedLoading = null)
		{
			NanoXMLDocument document = new NanoXMLDocument(mapXML);

			_mapName = MapName;

			_mapPath = mapPath;

			OnMapFinishedLoading = onMapFinishedLoading;

			ParseMapXML(document);
		}

		/// <summary>
		/// Create a Tiled Map using TextAsset as parameter
		/// </summary>
		/// <param name="mapText">Map's TextAsset</param>
		/// <param name="mapPath">Path to XML folder, so we can read relative paths for tilesets</param>
		/// <param name="onMapFinishedLoading">Callback for when map xml finishes loading</param>
		public Map(TextAsset mapText, string mapPath, Action<Map> onMapFinishedLoading = null)

		{
			NanoXMLDocument document = new NanoXMLDocument(mapText.text);
			
			_mapName = mapText.name;

			_mapPath = mapPath;

			OnMapFinishedLoading = onMapFinishedLoading;
			
			ParseMapXML(document);
		}

		/// <summary>
		/// Create a Tiled Map loading the XML from a StreamingAssetPath or a HTTP path (in the pc or web)
		/// </summary>
		/// <param name="wwwPath">Map's path with http for web files or without streaming assets path for local files</param>
		/// <param name="onMapFinishedLoading">Callback for when map xml finishes loading</param>
		public Map(	string wwwPath, Action<Map> onMapFinishedLoading)
		{
			_mapName = Path.GetFileNameWithoutExtension(wwwPath);
			_mapExtension = Path.GetExtension(wwwPath);
			if (string.IsNullOrEmpty(_mapExtension))
				_mapExtension = ".tmx";

			// remove _mapName from wwwPath
			string _pathWithoutMapName = wwwPath.Replace(string.Concat(_mapName, _mapExtension), "");

			if (!wwwPath.Contains("://"))
				_mapPath = string.Concat(Application.streamingAssetsPath, Path.AltDirectorySeparatorChar, _pathWithoutMapName);
			else
			{
				_mapPath = _pathWithoutMapName;
			}

			OnMapFinishedLoading = onMapFinishedLoading;

			new Task(LoadFromPath(wwwPath), true);
		}
		#endregion

		#region XML Parsers
		/// <summary>
		/// Load Map XML from a WWW path
		/// </summary>
		/// <param name="wwwPath">WWW path to load XML from</param>
		/// <returns>is loaded or not</returns>
		IEnumerator LoadFromPath(string wwwPath)
		{
			string result;
			string filePath = string.Concat(_mapPath, _mapName, _mapExtension);
			
			if (!filePath.Contains("://"))
				filePath = string.Concat("file://", filePath);

			WWW www = new WWW(filePath);
			yield return www;
			if (www.error != null)
			{
				Debug.LogError(www.error);
			}
			else
			{
				result = www.text;

				UsingStreamingAssetsPath = true;

				NanoXMLDocument document = new NanoXMLDocument(result);

				ParseMapXML(document);
			}
		}

		/// <summary>
		/// Initializes, Reads this Map's XML info
		/// </summary>
		/// <param name="document">NanoXMLDocument containing Map's XML</param>
		void ParseMapXML(NanoXMLDocument document)
		{
			MapNode = document.RootNode;
			Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), MapNode.GetAttribute("orientation").Value, true);
			int width = int.Parse(MapNode.GetAttribute("width").Value, CultureInfo.InvariantCulture);
			int height = int.Parse(MapNode.GetAttribute("height").Value, CultureInfo.InvariantCulture);
			int tileWidth = int.Parse(MapNode.GetAttribute("tilewidth").Value, CultureInfo.InvariantCulture);
			int tileHeight = int.Parse(MapNode.GetAttribute("tileheight").Value, CultureInfo.InvariantCulture);

			if (MapNode.GetAttribute("version") != null)
			{
				Version = MapNode.GetAttribute("version").Value;
			}
			else
				Version = string.Empty;

			RenderOrder mapRenderOrder = RenderOrder.Right_Down;

			if (MapNode.GetAttribute("renderorder") != null)
			{
				string renderOrder = MapNode.GetAttribute("renderorder").Value;
				mapRenderOrder = (RenderOrder)Enum.Parse(typeof(RenderOrder), renderOrder.Replace('-', '_'), true);
			}

			StaggerAxis mapStaggerAxis = StaggerAxis.Y;

			if (MapNode.GetAttribute("staggeraxis") != null)
			{
				string staggeraxis = MapNode.GetAttribute("staggeraxis").Value;
				mapStaggerAxis = (StaggerAxis)Enum.Parse(typeof(StaggerAxis), staggeraxis, true);
			}

			StaggerIndex mapStaggerIndex = StaggerIndex.Odd;

			if (MapNode.GetAttribute("staggerindex") != null)
			{
				string staggerindex = MapNode.GetAttribute("staggerindex").Value;
				mapStaggerIndex = (StaggerIndex)Enum.Parse(typeof(StaggerIndex), staggerindex, true);
			}

			int hexSideLength = 0;

			if (MapNode.GetAttribute("hexsidelength") != null)
			{
				hexSideLength = int.Parse(MapNode.GetAttribute("hexsidelength").Value, CultureInfo.InvariantCulture);
			}

			if (MapNode.GetAttribute("nextobjectid") != null)
			{
				NextObjectID = int.Parse(MapNode.GetAttribute("nextobjectid").Value, CultureInfo.InvariantCulture);
			}
			else
				NextObjectID = 0;

			Color32 backgroundColor = new Color32(128, 128, 128, 255);

			if (MapNode.GetAttribute("backgroundcolor") != null)
			{
				string color = MapNode.GetAttribute("backgroundcolor").Value;
				string r = color.Substring(1, 2);
				string g = color.Substring(3, 2);
				string b = color.Substring(5, 2);
				backgroundColor = new Color32(
					(byte)Convert.ToInt32(r, 16),
					(byte)Convert.ToInt32(g, 16),
					(byte)Convert.ToInt32(b, 16),255);
			}

			MapRenderParameter = new MapRenderParameters(orientation, mapRenderOrder, mapStaggerAxis, mapStaggerIndex, width, height, tileWidth, tileHeight, hexSideLength, backgroundColor);

			if (_mapName == null)
				_mapName = "Map";

			if (!_mapPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
				_mapPath = _mapPath + Path.AltDirectorySeparatorChar;

			NanoXMLNode propertiesElement = MapNode["properties"];
			if (propertiesElement != null) {
				Properties = new PropertyCollection(propertiesElement);
			}

			TileSets = new List<TileSet>();
			Tiles = new Dictionary<int, Tile>();
			_tileSetsToLoaded = 0;
			_numberOfTileSetsToLoad = 0;
			// First get how many tilesets we need to load
			foreach (NanoXMLNode node in MapNode.SubNodes)
			{
				if (node.Name.Equals("tileset"))
					_numberOfTileSetsToLoad++;
			}

			// Maps might not have any tileset, being just a tool for object placement :P
			if (_numberOfTileSetsToLoad < 1)
			{
				ContinueLoadingTiledMapAfterTileSetsLoaded();
			}

			// Then load all of them. After all loaded, continue with map loading
			foreach (NanoXMLNode node in MapNode.SubNodes)
			{
				if (node.Name.Equals("tileset"))
				{
					if (node.GetAttribute("source") != null)
					{
						int firstID = int.Parse(node.GetAttribute("firstgid").Value, CultureInfo.InvariantCulture);
						if (UsingStreamingAssetsPath)
						{
							// Run coroutine for www using TaskManager
							new Task(LoadExternalTileSet(node, _mapPath, firstID), true);
						}
						else
						{
							// Parse the path
							string path = Utils.XUniTMXHelpers.ParsePath(_mapPath, node.GetAttribute("source").Value);

							TextAsset externalTileSetTextAsset = Resources.Load<TextAsset>(path);

							BuildExternalTileSet(externalTileSetTextAsset.text, Directory.GetParent(path).ToString(), firstID);
						}
					}
					else
					{
						OnFinishedLoadingTileSet(new TileSet(node, _mapPath, this, UsingStreamingAssetsPath));
					}
				}
			}
		}
		#endregion

		#region Load TileSets
		/// <summary>
		/// Load an external TileSet
		/// </summary>
		/// <param name="node">TileSet's NanoXMLNode</param>
		/// <param name="mapPath">Map's root directory</param>
		/// <param name="firstID">TileSet's firstID (external TileSet does not save this info)</param>
		/// <returns>is loaded or not</returns>
		IEnumerator LoadExternalTileSet(NanoXMLNode node, string mapPath, int firstID = 1)
		{
			string externalTileSetData = node.GetAttribute("source").Value;
			string filePath = mapPath;
			if (Application.isWebPlayer)
			{
				filePath = Path.Combine(filePath, Path.GetFileName(externalTileSetData));
			}
			else
			{
				if (Path.GetDirectoryName(externalTileSetData).Length > 0)
					filePath += Path.GetDirectoryName(externalTileSetData) + Path.AltDirectorySeparatorChar;
				if (filePath.Equals("/")) filePath = "";

				// if there's no :// assume we are using StreamingAssets path
				if (!filePath.Contains("://"))
					filePath = string.Concat("file://", Path.Combine(filePath, Path.GetFileName(externalTileSetData)));
			}
			
			WWW www = new WWW(filePath);
			yield return www;
			externalTileSetData = www.text;

			BuildExternalTileSet(externalTileSetData, mapPath, firstID);
		}

		/// <summary>
		/// Finally build TileSet info using read data
		/// </summary>
		/// <param name="tileSetData">TileSet raw XML</param>
		/// <param name="path">External TileSet root directory</param>
		/// <param name="firstID">TileSet's firstID (external TileSet does not save this info)</param>
		void BuildExternalTileSet(string tileSetData, string path, int firstID = 1)
		{
			NanoXMLDocument externalTileSet = new NanoXMLDocument(tileSetData);

			NanoXMLNode externalTileSetNode = externalTileSet.RootNode;
			OnFinishedLoadingTileSet(new TileSet(externalTileSetNode, path, this, UsingStreamingAssetsPath, firstID));
		}

		void OnFinishedLoadingTileSet(TileSet tileSet)
		{
			TileSets.Add(tileSet);
			
			_tileSetsToLoaded++;

			if (_tileSetsToLoaded >= _numberOfTileSetsToLoad)
				ContinueLoadingTiledMapAfterTileSetsLoaded();
		}

		void ContinueLoadingTiledMapAfterTileSetsLoaded()
		{
			Layers = new List<Layer>();
			int i = 0;
			foreach (NanoXMLNode layerNode in MapNode.SubNodes)
			{
				if (!(layerNode.Name.Equals("layer") || layerNode.Name.Equals("objectgroup") || layerNode.Name.Equals("imagelayer")))
					continue;

				Layer layerContent;

				int layerDepth = 1 - (XUniTMXConfiguration.Instance.LayerDepthSpacing * i);

				if (layerNode.Name.Equals("layer"))
				{
					layerContent = new TileLayer(layerNode, this, layerDepth, GlobalMakeUniqueTiles);
				}
				else if (layerNode.Name.Equals("objectgroup"))
				{
					layerContent = new MapObjectLayer(layerNode, this, layerDepth);
				}
				else if (layerNode.Name.Equals("imagelayer"))
				{
					layerContent = new ImageLayer(layerNode, this, _mapPath);
				}
				else
				{
					throw new Exception("Unknown layer name: " + layerNode.Name);
				}

				// Layer names need to be unique for our lookup system, but Tiled
				// doesn't require unique names.
				string layerName = layerContent.Name;
				int duplicateCount = 2;

				// if a layer already has the same name...
				if (Layers.Find(l => l.Name == layerName) != null)
				{
					// figure out a layer name that does work
					do
					{
						layerName = string.Format("{0}{1}", layerContent.Name, duplicateCount);
						duplicateCount++;
					} while (Layers.Find(l => l.Name == layerName) != null);

					// log a warning for the user to see
					Debug.Log("Renaming layer \"" + layerContent.Name + "\" to \"" + layerName + "\" to make a unique name.");

					// save that name
					layerContent.Name = layerName;
				}
				layerContent.LayerDepth = layerDepth;
				Layers.Add(layerContent);
				namedLayers.Add(layerName, layerContent);
				i++;
			}

			if (OnMapFinishedLoading != null)
				OnMapFinishedLoading(this);
		}
		#endregion

		// Generators generate the GameObjects from Layers, Objects etc
		#region Generators
		#region Generate TileSets
		Action OnFinishedGeneratingTileSets_NoParameter = null;
		Action<object, Action<Layer>, string, int> OnFinishedGeneratingTileSets = null;
		object OnFinishedGeneratingTileSetsReturnObject = null;
		Action<Layer> OnGeneratedLayer = null;
		protected string _tag = "";
		protected int _physicsLayer = 0;
		protected bool HasGeneratedTileSets = false;

		private void GenerateTileSetsMaterials()
		{
			// Generate Materials for Map batching
			TileSetsMaterials = new List<Material>();
			// Generate Materials
			int i = 0;
			for (i = 0; i < TileSets.Count; i++)
			{
				Material layerMat = new Material(BaseTileMaterial);
				layerMat.mainTexture = TileSets[i].Texture;
				TileSetsMaterials.Add(layerMat);
			}
			HasGeneratedTileSets = true;
			if (OnFinishedGeneratingTileSets != null)
				OnFinishedGeneratingTileSets(OnFinishedGeneratingTileSetsReturnObject, OnGeneratedLayer, _tag, _physicsLayer);
			if (OnFinishedGeneratingTileSets_NoParameter != null)
				OnFinishedGeneratingTileSets_NoParameter();

			OnFinishedGeneratingTileSets_NoParameter = null;
			OnFinishedGeneratingTileSets = null;
		}

		private void GenerateTileSets(Action<object, Action<Layer>, string, int> onFinishedGeneratingTileSets, object obj = null, Action<Layer> onGeneratedLayer = null, string tag = "", int physicsLayer = 0)
		{
			Tiles.Clear();
			_tileSetsToLoaded = 0;
			if (_numberOfTileSetsToLoad < 1)
			{
				onFinishedGeneratingTileSets(obj, onGeneratedLayer, tag, physicsLayer);
				return;
			}
			_tag = tag;
			_physicsLayer = physicsLayer;
			OnFinishedGeneratingTileSets = onFinishedGeneratingTileSets;
			OnFinishedGeneratingTileSetsReturnObject = obj;
			OnGeneratedLayer = onGeneratedLayer;
			for (int i = 0; i < TileSets.Count; i++)
			{
				TileSets[i].Generate(onFinishedGeneratingTileSet);
			}
		}

		private void GenerateTileSets(Action onFinishedGeneratingTileSets)
		{
			Tiles.Clear();
			_tileSetsToLoaded = 0;
			if (_numberOfTileSetsToLoad < 1)
			{
				onFinishedGeneratingTileSets();
				return;
			}

			OnFinishedGeneratingTileSets_NoParameter = onFinishedGeneratingTileSets;
			for (int i = 0; i < TileSets.Count; i++)
			{
				TileSets[i].Generate(onFinishedGeneratingTileSet);
			}
		}

		private void onFinishedGeneratingTileSet(TileSet tileSet)
		{
			foreach (KeyValuePair<int, Tile> item in tileSet.Tiles)
			{
				this.Tiles.Add(item.Key, item.Value);
			}
			_tileSetsToLoaded++;
			
			if (_tileSetsToLoaded >= _numberOfTileSetsToLoad)
				GenerateTileSetsMaterials();
		}
		#endregion

		#region Generate TileLayers
		int _generatedTileLayers = 0;
		int _tileLayersToGenerate = 0;
		Action OnGeneratedTileLayers = null;

        /// <summary>
        /// Generate all TileLayer in this Map
        /// </summary>
        /// <param name="onGeneratedTileLayers">Callback for when the layers finishes being generated</param>
		public void GenerateTileLayers(Action onGeneratedTileLayers = null, string tag = "", int physicsLayer = 0)
		{
			if (BaseTileMaterial == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			_generatedTileLayers = 0;
			OnGeneratedTileLayers = onGeneratedTileLayers;
			_tileLayersToGenerate = GetTileLayersCount();
			if (_tileLayersToGenerate < 1)
			{
				if (OnGeneratedTileLayers != null)
					OnGeneratedTileLayers();
			}
			else
			{
				foreach (var layerPair in namedLayers)
				{
					if (layerPair.Value is TileLayer)
					{
						TileLayer tileLayer = layerPair.Value as TileLayer;
						tileLayer.MakeUniqueTiles = (tileLayer.MakeUniqueTiles || GlobalMakeUniqueTiles);
						
						if (HasGeneratedTileSets)
							tileLayer.Generate(TileSetsMaterials, OnTileLayerGenerated, tag, physicsLayer);
						else
							GenerateTileSets(ContinueGenerateTileLayer, tileLayer, OnTileLayerGenerated, tag, physicsLayer);
					}
				}
			}
		}

		void OnTileLayerGenerated(Layer tileLayer)
		{
			_generatedTileLayers++;
			if (_generatedTileLayers >= _tileLayersToGenerate)
			{
				if (OnGeneratedTileLayers != null)
					OnGeneratedTileLayers();
			}
		}

        /// <summary>
        /// Generate a single TileLayer
        /// </summary>
        /// <param name="layerName">TileLayer's name</param>
        /// <param name="onGeneratedTileLayer">Callback for when this layer finishes being generated</param>
        /// <param name="baseMaterial">Material to be used for this layer</param>
		public void GenerateTileLayer(string layerName, Action<Layer> onGeneratedTileLayer = null, Material baseMaterial = null, string tag = "", int physicsLayer = 0)
		{
			//if (BaseTileMaterial == null)
			//	BaseTileMaterial = baseTileMaterial;

			Material mat = baseMaterial != null ? baseMaterial : BaseTileMaterial;
			if (mat == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			
			TileLayer tileLayer = GetTileLayer(layerName);
			if (tileLayer != null)
			{
				tileLayer.MakeUniqueTiles = (tileLayer.MakeUniqueTiles || GlobalMakeUniqueTiles);
				if (HasGeneratedTileSets)
					tileLayer.Generate(TileSetsMaterials, onGeneratedTileLayer, tag, physicsLayer);
				else
					GenerateTileSets(ContinueGenerateTileLayer, tileLayer, onGeneratedTileLayer, tag, physicsLayer);
			}
		}

		protected void ContinueGenerateTileLayer(object tileLayer, Action<Layer> onGeneratedLayer, string tag = "", int physicsLayer = 0)
		{
			(tileLayer as TileLayer).Generate(TileSetsMaterials, onGeneratedLayer, tag, physicsLayer);
		}
		#endregion

		#region Generate Image Layers
		int _generatedImageLayers = 0;
		int _imageLayersToGenerate = 0;
		Action OnGeneratedImageLayers = null;

        /// <summary>
        /// Generate All Image Layers in the Map
        /// </summary>
        /// <param name="onGeneratedImageLayers">Callback for when Layers finished being generated</param>
        /// <param name="baseTileMaterial">Material to be used for these layers</param>
		public void GenerateImageLayers(Action onGeneratedImageLayers = null, Material baseTileMaterial = null, string tag = "", int physicsLayer = 0)
		{
			if (BaseTileMaterial == null)
				BaseTileMaterial = baseTileMaterial;
			if (BaseTileMaterial == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			OnGeneratedImageLayers = onGeneratedImageLayers;
			_generatedImageLayers = 0;
			_imageLayersToGenerate = GetImageLayersCount();
			if (_imageLayersToGenerate < 1)
			{
				if (OnGeneratedImageLayers != null)
					OnGeneratedImageLayers();
			}
			else
			{
				foreach (var layerPair in namedLayers)
				{
					if (layerPair.Value is ImageLayer)
					{
						(layerPair.Value as ImageLayer).Generate(BaseTileMaterial, OnImageLayerGenerated, tag, physicsLayer);
					}
				}
			}
		}

		void OnImageLayerGenerated(Layer layer)
		{
			_generatedImageLayers++;
			if (_generatedImageLayers >= _imageLayersToGenerate)
			{
				if (OnGeneratedImageLayers != null)
					OnGeneratedImageLayers();
			}
		}

        /// <summary>
        /// Generate a single ImageLayer
        /// </summary>
        /// <param name="layerName">ImageLayer's name</param>
        /// <param name="onGeneratedImageLayer">Callback for when this layer finishes being generated</param>
        /// <param name="baseMaterial">Material to be used for this layer</param>
		public void GenerateImageLayer(string layerName, Action<Layer> onGeneratedImageLayer = null, Material baseMaterial = null, string tag = "", int physicsLayer = 0)
		{
            ImageLayer imageLayer = GetImageLayer(layerName);
            if (imageLayer == null)
            {
                Debug.LogError("Image Layer " + layerName + " not found!");
                return;
            }

			if (BaseTileMaterial == null && baseMaterial == null && imageLayer.LayerMaterial == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			Material mat = baseMaterial != null ? baseMaterial : BaseTileMaterial;

            if (imageLayer.LayerMaterial != null)
                imageLayer.Generate(imageLayer.LayerMaterial, onGeneratedImageLayer, tag, physicsLayer);
            else if (HasGeneratedTileSets)
                imageLayer.Generate(mat, onGeneratedImageLayer, tag, physicsLayer);
			
		}
		#endregion

		#region Generate MapObjectLayers
		int _generatedMapObjectLayers = 0;
		int _mapObjectLayersToGenerate = 0;
		Action OnGeneratedMapObjectLayers = null;

        /// <summary>
        /// Generate all MapObjectLayer in this Map
        /// </summary>
        /// <param name="onGeneratedMapObjectLayers">Callback for when the layers finishes being generated</param>
        /// <param name="baseTileMaterial">Material to be used for these layers</param>
		public void GenerateMapObjectLayers(Action onGeneratedMapObjectLayers = null, Material baseTileMaterial = null, string tag = "", int physicsLayer = 0)
		{
			if (BaseTileMaterial == null)
				BaseTileMaterial = baseTileMaterial;
			if (BaseTileMaterial == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			_generatedMapObjectLayers = 0;
			_mapObjectLayersToGenerate = GetObjectLayersCount();
			OnGeneratedMapObjectLayers = onGeneratedMapObjectLayers;
			if (_mapObjectLayersToGenerate < 1)
			{
				if (OnGeneratedMapObjectLayers != null)
					OnGeneratedMapObjectLayers();
			}
			else
			{
				foreach (var layerPair in namedLayers)
				{
					if (layerPair.Value is MapObjectLayer)
				    {
						if (HasGeneratedTileSets)
							(layerPair.Value as MapObjectLayer).Generate(TileSetsMaterials, OnMapObjectLayerGenerated, tag, physicsLayer);
						else
							GenerateTileSets(ContinueGenerateMapObjectLayer, (layerPair.Value as MapObjectLayer), OnMapObjectLayerGenerated, tag, physicsLayer);
					}
				}
			}
		}

		void OnMapObjectLayerGenerated(Layer layer)
		{
			_generatedMapObjectLayers++;
			if (_generatedMapObjectLayers >= _mapObjectLayersToGenerate)
			{
				if (OnGeneratedMapObjectLayers != null)
					OnGeneratedMapObjectLayers();
			}
		}

        /// <summary>
        /// Generate a single MapObjectLayer
        /// </summary>
        /// <param name="layerName">MapObjectLayer's name</param>
        /// <param name="onGeneratedMapObjectLayer">Callback for when this layer finishes being generated</param>
        /// <param name="baseTileMaterial">Material to be used for this layers</param>
		public void GenerateMapObjectLayer(string layerName, Action<Layer> onGeneratedMapObjectLayer = null, Material baseTileMaterial = null, string tag = "", int physicsLayer = 0)
		{
			if (BaseTileMaterial == null)
				BaseTileMaterial = baseTileMaterial;
			if (BaseTileMaterial == null)
			{
				Debug.LogError("No Default Material Set!");
				return;
			}
			MapObjectLayer objectLayer = GetObjectLayer(layerName);
			if (objectLayer != null)
			{
				if (HasGeneratedTileSets)
					objectLayer.Generate(TileSetsMaterials, onGeneratedMapObjectLayer, tag, physicsLayer);
				else
					GenerateTileSets(ContinueGenerateMapObjectLayer, objectLayer, onGeneratedMapObjectLayer, tag, physicsLayer);
			}
		}

		protected void ContinueGenerateMapObjectLayer(object mapObjectLayer, Action<Layer> onGeneratedLayer, string tag = "", int physicsLayer = 0)
		{
			(mapObjectLayer as MapObjectLayer).Generate(TileSetsMaterials, onGeneratedLayer, tag, physicsLayer);
		}
		#endregion

		#region Generic Generate Map
        /// <summary>
        /// Generate this Map and all its Layers
        /// </summary>
        /// <param name="parent">GameObject to parent this Map's GameObject to</param>
        /// <param name="baseTileMaterial">Base Material to be used for the tiles</param>
        /// <param name="sortingOrder">Base sortingOrder to be used for the tiles</param>
        /// <param name="makeUnique">Set True to generate Unique Tiles (one GameObject per Tile), false to generate one Mesh per TileLayer instead</param>
        /// <param name="setCameraAsMapBackgroundColor">Set True to automatically set MainCamera's background color to Map's background color</param>
        /// <param name="onFinishedGeneratingMap">Callback for when this Map finishes being generated</param>
		public void Generate(GameObject parent, Material baseTileMaterial, int sortingOrder = 0, bool makeUnique = false, bool setCameraAsMapBackgroundColor = false, Action<Map> onFinishedGeneratingMap = null)
		{
			MapGameObject = new GameObject(_mapName);
			Transform mapObjectTransform = MapGameObject.transform;

			Parent = parent;
			mapObjectTransform.parent = Parent.transform;

			mapObjectTransform.localPosition = Vector3.zero;
			mapObjectTransform.localRotation = Quaternion.identity;
			mapObjectTransform.localScale = Vector3.one;
			MapGameObject.layer = mapObjectTransform.parent.gameObject.layer;

			BaseTileMaterial = baseTileMaterial;
			DefaultSortingOrder = sortingOrder;
			GlobalMakeUniqueTiles = makeUnique;

			OnMapFinishedGeneration = onFinishedGeneratingMap;

			if (setCameraAsMapBackgroundColor)
			{
				Camera.main.backgroundColor = MapRenderParameter.BackgroundColor;
			}

			GenerateTileSets(ContinueGenerateMap);
		}

		int _calls = 0;
		protected void ContinueGenerateMap()
		{
			_calls = 0;
			GenerateTileLayers(OnGeneratedLayers);
			GenerateMapObjectLayers(OnGeneratedLayers);
			GenerateImageLayers(OnGeneratedLayers);
		}

		void OnGeneratedLayers()
		{
			_calls++;
			// This function will be called once from each Generate___Layers function, so there will be 3 calls :P
			if (_calls >= 3)
			{
				if (OnMapFinishedGeneration != null)
					OnMapFinishedGeneration(this);
			}
		}
		#endregion

		/// <summary>
		/// Generate Colliders from all MapObjectLayer
		/// </summary>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="is2DCollider">True to generate a 2D collider, false to generate a 3D collider</param>
		/// <param name="collidersAreTrigger">True for Trigger Colliders, false otherwise</param>
		/// <param name="collidersZDepth">Z Depth of the colliders</param>
		/// <param name="collidersWidth">Width of the colliders, in Units</param>
		/// <param name="collidersAreInner">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions</param>
		public void GenerateColliders(string tag, int physicsLayer, bool is2DCollider, bool collidersAreTrigger, float collidersZDepth, float collidersWidth, bool collidersAreInner)
		{
			foreach (var layerPair in namedLayers)
			{
				if (layerPair.Value is MapObjectLayer)
				{
					MapExtensions.GenerateCollidersFromLayer(this, layerPair.Value as MapObjectLayer, is2DCollider, collidersAreTrigger, tag, physicsLayer, collidersZDepth, collidersWidth, collidersAreInner);
				}
			}
		}

		/// <summary>
		/// Generate Prefabs from all MapObjectLayer
		/// </summary>
		/// <param name="addMapName">True to add Map's name to the prefab's name</param>
		public void GeneratePrefabs(Vector2 anchorPoint, bool addMapName = false)
		{
			foreach (var layerPair in namedLayers)
			{
				if (layerPair.Value is MapObjectLayer)
				{
					MapExtensions.GeneratePrefabsFromLayer(this, layerPair.Value as MapObjectLayer, anchorPoint, addMapName);
				}
			}
		}
		#endregion

		#region Getters
		public string MapName { get { return _mapName; } }

		/// <summary>
		/// Returns the set of all objects in the map.
		/// </summary>
		/// <returns>A new set of all objects in the map.</returns>
		public IEnumerable<MapObject> GetAllObjects()
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					yield return obj;
				}
			}
		}

		/// <summary>
		/// Finds an object in the map using a delegate.
		/// </summary>
		/// <remarks>
		/// This method is used when an object is desired, but there is no specific
		/// layer to find the object on. The delegate allows the caller to create 
		/// any logic they want for finding the object. A simple example for finding
		/// the first object named "goal" in any layer would be this:
		/// 
		/// var goal = map.FindObject((layer, obj) => return obj.Name.Equals("goal"));
		/// 
		/// You could also use the layer name or any other logic to find an object.
		/// The first object for which the delegate returns true is the object returned
		/// to the caller. If the delegate never returns true, the method returns null.
		/// </remarks>
		/// <param name="finder">The delegate used to search for the object.</param>
		/// <returns>The MapObject if the delegate returned true, null otherwise.</returns>
		public MapObject FindObject(MapObjectFinder finder)
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					if (finder(objLayer, obj))
						return obj;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds a collection of objects in the map using a delegate.
		/// </summary>
		/// <remarks>
		/// This method performs basically the same process as FindObject, but instead
		/// of returning the first object for which the delegate returns true, it returns
		/// a collection of all objects for which the delegate returns true.
		/// </remarks>
		/// <param name="finder">The delegate used to search for the object.</param>
		/// <returns>A collection of all MapObjects for which the delegate returned true.</returns>
		public IEnumerable<MapObject> FindObjects(MapObjectFinder finder)
		{
			foreach (var layer in Layers)
			{
				MapObjectLayer objLayer = layer as MapObjectLayer;
				if (objLayer == null)
					continue;

				foreach (var obj in objLayer.Objects)
				{
					if (finder(objLayer, obj))
						yield return obj;
				}
			}
		}

		/// <summary>
		/// Gets a layer by name.
		/// </summary>
		/// <param name="name">The name of the layer to retrieve.</param>
		/// <returns>The layer with the given name.</returns>
		public Layer GetLayer(string name)
		{
			if (namedLayers.ContainsKey(name))
				return namedLayers[name];
			return null;
		}

		/// <summary>
		/// Gets a tile layer by name.
		/// </summary>
		/// <param name="name">The name of the tile layer to retrieve.</param>
		/// <returns>The tile layer with the given name.</returns>
		public TileLayer GetTileLayer(string name)
		{
			if (namedLayers.ContainsKey(name))
				return namedLayers[name] as TileLayer;
			return null;
		}

		/// <summary>
		/// Gets all TileLayers from this Map
		/// </summary>
		/// <returns>A List containing all TileLayers</returns>
		public List<TileLayer> GetAllTileLayers()
		{
			List<TileLayer> tileLayers = new List<TileLayer>();
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is TileLayer)
					tileLayers.Add(Layers[i] as TileLayer);
			}

			return tileLayers;
		}

		/// <summary>
		/// Gets an object layer by name.
		/// </summary>
		/// <param name="name">The name of the object layer to retrieve.</param>
		/// <returns>The object layer with the given name.</returns>
		public MapObjectLayer GetObjectLayer(string name)
		{
			if (namedLayers.ContainsKey(name))
				return namedLayers[name] as MapObjectLayer;
			return null;
		}

		/// <summary>
		/// Gets all MapObjectLayers from this Map
		/// </summary>
		/// <returns>A List containing all MapObjectLayers</returns>
		public List<MapObjectLayer> GetAllObjectLayers()
		{
			List<MapObjectLayer> moLayers = new List<MapObjectLayer>();
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is MapObjectLayer)
					moLayers.Add(Layers[i] as MapObjectLayer);
			}

			return moLayers;
		}

		/// <summary>
		/// Gets an ImageLayer by name.
		/// </summary>
		/// <param name="name">The name of the ImageLayer to retrieve.</param>
		/// <returns>The ImageLayer with the given name.</returns>
		public ImageLayer GetImageLayer(string name)
		{
			if (namedLayers.ContainsKey(name))
				return namedLayers[name] as ImageLayer;
			return null;
		}

		/// <summary>
		/// Gets all ImageLayers from this Map
		/// </summary>
		/// <returns>A List containing all ImageLayers</returns>
		public List<ImageLayer> GetAllImageLayers()
		{
			List<ImageLayer> iLayers = new List<ImageLayer>();
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is ImageLayer)
					iLayers.Add(Layers[i] as ImageLayer);
			}

			return iLayers;
		}

		/// <summary>
		/// Gets the number of TileLayer in this Map
		/// </summary>
		/// <returns>number of TileLayer</returns>
		public int GetTileLayersCount()
		{
			int count = 0;
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is TileLayer)
					count++;
			}

			return count;
		}

		/// <summary>
		/// Gets the number of MapObjectLayer in this Map
		/// </summary>
		/// <returns>number of MapObjectLayer</returns>
		public int GetObjectLayersCount()
		{
			int count = 0;
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is MapObjectLayer)
					count++;
			}

			return count;
		}

		/// <summary>
		/// Gets the number of ImageLayer in this Map
		/// </summary>
		/// <returns>number of ImageLayer</returns>
		public int GetImageLayersCount()
		{
			int count = 0;
			for (int i = 0; i < Layers.Count; i++)
			{
				if (Layers[i] is ImageLayer)
					count++;
			}

			return count;
		}
		#endregion

		public override string ToString()
		{
			return string.Concat(
				"Map Size (", MapRenderParameter.Width.ToString(), ", ", MapRenderParameter.Height.ToString(), ")",
				"\nTile Size (", MapRenderParameter.TileWidth.ToString(), ", ", MapRenderParameter.TileHeight.ToString(), ")",
				"\nOrientation: ", MapRenderParameter.Orientation.ToString(),
				"\nMap Render Order: ", MapRenderParameter.MapRenderOrder.ToString(),
				"\nMap Stagger Axis: ", MapRenderParameter.MapStaggerAxis.ToString(),
				"\nMap Stagger Index: ", MapRenderParameter.MapStaggerIndex.ToString(),
				"\nHex Side Length: ", MapRenderParameter.HexSideLength.ToString(),
				"\nTiled Version: ", Version.ToString());
		}
	}
}
