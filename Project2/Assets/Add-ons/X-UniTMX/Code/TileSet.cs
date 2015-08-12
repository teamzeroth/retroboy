/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using TObject.Shared;
using System.IO;
using UnityEngine;
using System.Collections;

namespace X_UniTMX
{
	/// <summary>
	/// A class containing the main TileSet Image variables
	/// </summary>
	public class TileSetImage
	{
		/// <summary>
		/// This Image's Width
		/// </summary>
		public int Width = 0;

		/// <summary>
		/// This Image's Height
		/// </summary>
		public int Height = 0;

		/// <summary>
		/// This TileSet's parsed image (sprite) path
		/// </summary>
		public string Image = string.Empty;

		/// <summary>
		/// This TileSet's image full path
		/// </summary>
		public string FullPath = string.Empty;

		/// <summary>
		/// Color to set as transparency
		/// </summary>
		public Color? ColorKey;

		/// <summary>
		/// This TileSet's loaded Texture
		/// </summary>
		public Texture2D Texture;
	}

	/// <summary>
	/// A Container for a Tile Set properties and its Tiles.
	/// </summary>
	public class TileSet
	{
		#region Public Variables
		/// <summary>
		/// This TileSet's First ID
		/// </summary>
		public int FirstId;
		
		/// <summary>
		/// This TileSet's Name
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Width of the Tile in this set
		/// </summary>
		public int TileWidth;
		
		/// <summary>
		/// Height of the Tile in this set
		/// </summary>
		public int TileHeight;
		
		/// <summary>
		/// Spacing in pixels between Tiles
		/// </summary>
		public int Spacing = 0;
		
		/// <summary>
		/// Margin in pixels from Tiles to border of Texture
		/// </summary>
		public int Margin = 0;
		
		/// <summary>
		/// Offset in pixels in X Axis to draw the tiles from this tileset
		/// </summary>
		public int TileOffsetX = 0;
		
		/// <summary>
		/// Offset in pixels in Y Axis to draw the tiles from this tileset
		/// </summary>
		public int TileOffsetY = 0;

		/// <summary>
		/// This TileSet's loaded Texture
		/// </summary>
		public Texture2D Texture 
		{
			get
			{
				if (TileSetImages == null)
					return null;
				return TileSetImages[0].Texture;
			}
		}

		/// <summary>
		/// List of this TileSet's Images (1 on normal TileSets, multiple on ImageCollection TileSets)
		/// </summary>
		public List<TileSetImage> TileSetImages = new List<TileSetImage>();

		/// <summary>
		/// Dictionary of tiles in this tileset. Key = Tile ID, Value = Tile reference
		/// </summary>
		public Dictionary<int, Tile> Tiles = new Dictionary<int, Tile>();

		/// <summary>
		/// Dictionary of tile objects in this tileset. Key = Tile ID, Value = TileObject reference
		/// </summary>
		public Dictionary<int, List<TileObject>> TilesObjects = new Dictionary<int, List<TileObject>>();

		/// <summary>
		/// Dictionary of tile properties in this tileset. Key = Tile ID, Value = PropertyCollection reference
		/// </summary>
		public Dictionary<int, PropertyCollection> TileProperties = new Dictionary<int, PropertyCollection>();

		/// <summary>
		/// Dictionary of animated tiles in this tileset. Key = Tile ID, Value = TileAnimation reference
		/// </summary>
		public Dictionary<int, TileAnimation> AnimatedTiles = new Dictionary<int, TileAnimation>();

		/// <summary>
		/// True if this TileSet was already generated
		/// </summary>
		public bool IsGenerated = false;

		/// <summary>
		/// True if this TileSet is an ImageCollection that was packed
		/// </summary>
		public bool WasPacked { get; private set; }
		#endregion

		/// <summary>
		/// Delegate to call when this tileset finishes loading
		/// </summary>
		Action<TileSet> OnFinishedGeneratingTileSet = null;

		bool _useWWWToLoad = false;
		int _mapTileWidth = 0;
		int _mapTileHeight = 0;
		int _loadedTexturesCount = 0;

		/// <summary>
		/// Load this TileSet's information from node
		/// </summary>
		/// <param name="node">NanoXMLNode to parse</param>
		/// <param name="map">Reference to the Map this TileSet is in</param>
		/// <param name="firstGID">First ID is a per-Map property, so External TileSets won't have this info in the node</param>
		protected TileSet(NanoXMLNode node, Map map, int firstGID = 1)
		{
			if (node.GetAttribute("firstgid") == null || !int.TryParse(node.GetAttribute("firstgid").Value, out FirstId))
				FirstId = firstGID;
			
			//this.FirstId = int.Parse(node.GetAttribute("firstgid").Value, CultureInfo.InvariantCulture);
			this.Name = node.GetAttribute("name").Value;
			this.TileWidth = int.Parse(node.GetAttribute("tilewidth").Value, CultureInfo.InvariantCulture);
			this.TileHeight = int.Parse(node.GetAttribute("tileheight").Value, CultureInfo.InvariantCulture);

			if (node.GetAttribute("spacing") != null)
			{
				this.Spacing = int.Parse(node.GetAttribute("spacing").Value, CultureInfo.InvariantCulture);
			}

			if (node.GetAttribute("margin") != null)
			{
				this.Margin = int.Parse(node.GetAttribute("margin").Value, CultureInfo.InvariantCulture);
			}

			NanoXMLNode tileOffset = node["tileoffset"];
			if (tileOffset != null)
			{
				this.TileOffsetX = int.Parse(tileOffset.GetAttribute("x").Value, CultureInfo.InvariantCulture);
				this.TileOffsetY = -int.Parse(tileOffset.GetAttribute("y").Value, CultureInfo.InvariantCulture);
			}

			AddTileSetImage(node["image"]);

			_mapTileWidth = map.MapRenderParameter.TileWidth;
			_mapTileHeight = map.MapRenderParameter.TileHeight;
			
			foreach (NanoXMLNode subNode in node.SubNodes)
			{
				if (subNode.Name.Equals("tile"))
				{
					int id = this.FirstId + int.Parse(subNode.GetAttribute("id").Value, CultureInfo.InvariantCulture);
					
					// Load Tile Properties, if any
					NanoXMLNode propertiesNode = subNode["properties"];
					if (propertiesNode != null)
					{
						PropertyCollection properties = new PropertyCollection(propertiesNode);//Property.ReadProperties(propertiesNode);
						this.TileProperties.Add(id, properties);
					}

					// Load Tile Animation, if any
					NanoXMLNode animationNode = subNode["animation"];
					if (animationNode != null)
					{

						TileAnimation _tileAnimation = new TileAnimation();
						foreach (NanoXMLNode frame in animationNode.SubNodes)
						{
							if (!frame.Name.Equals("frame"))
								continue;
							int tileid = int.Parse(frame.GetAttribute("tileid").Value, CultureInfo.InvariantCulture) + FirstId;
							int duration = int.Parse(frame.GetAttribute("duration").Value, CultureInfo.InvariantCulture);
							_tileAnimation.AddTileFrame(tileid, duration);
						}
						this.AnimatedTiles.Add(id, _tileAnimation);
					}

					// Load Tile Objects, if any
					NanoXMLNode objectsNode = subNode["objectgroup"];
					List<TileObject> tileObjects = null;
					if (objectsNode != null)
					{
						tileObjects = new List<TileObject>();
						foreach (NanoXMLNode tileObjNode in objectsNode.SubNodes)
						{
							TileObject tObj = new TileObject(tileObjNode, _mapTileWidth, _mapTileHeight);
							//tObj.ScaleObject(map.TileWidth, map.TileHeight, map.Orientation);
							tObj = tObj.ScaleObject(map.MapRenderParameter) as TileObject;
							tileObjects.Add(tObj);
						}
						// There's a bug in Tiled 0.10.1- where the objectgroup node won't go away even if you delete all objects from a tile's collision group.
						if(tileObjects.Count > 0)
							TilesObjects.Add(id, tileObjects);
					}

					// Load Tile Image, if this is an ImageCollection TileSet
					AddTileSetImage(subNode["image"], tileObjects);
				}
			}
		}

		void AddTileSetImage(NanoXMLNode imageNode, List<TileObject> tileObjects = null)
		{
			if (imageNode == null)
				return;

			TileSetImage tsi = new TileSetImage();

			tsi.FullPath = tsi.Image = imageNode.GetAttribute("source").Value;
			if(imageNode.GetAttribute("width") != null)
				tsi.Width = int.Parse(imageNode.GetAttribute("width").Value, CultureInfo.InvariantCulture);
			if (imageNode.GetAttribute("height") != null)
			tsi.Height = int.Parse(imageNode.GetAttribute("height").Value, CultureInfo.InvariantCulture);

			if (imageNode.GetAttribute("trans") != null)
			{
				string color = imageNode.GetAttribute("trans").Value;
				string r = color.Substring(0, 2);
				string g = color.Substring(2, 2);
				string b = color.Substring(4, 2);
				tsi.ColorKey = new Color((byte)Convert.ToInt32(r, 16), (byte)Convert.ToInt32(g, 16), (byte)Convert.ToInt32(b, 16));
			}

			// alter the tile objects, if any, to use this image's width and height as its TileWidth and TileHeight values
			// used for ImageCollections, where tiles can be of different sizes inside the same tileset
			if (tileObjects != null)
			{
				for (int i = 0; i < tileObjects.Count; i++)
				{
					tileObjects[i].TileWidth = tsi.Width;
					tileObjects[i].TileHeight = tsi.Height;
				}
			}
			TileSetImages.Add(tsi);
		}

		/// <summary>
		/// Load this TileSet's information from node and builds its tiles
		/// </summary>
		/// <param name="node">NanoXMLNode to parse</param>
		/// <param name="mapPath">Map's directory</param>
		/// <param name="map">Reference to the Map this TileSet is in</param>
		/// <param name="isUsingStreamingPath">true if is using StreamingAssets path or HTTP URL (WWW)</param>
		/// <param name="onFinishedGeneratingTileSet">Delegate to call when this TileSet finishes loading</param>
		/// <param name="firstID">First ID is a per-Map property, so External TileSets won't have this info in the node</param>
		public TileSet(NanoXMLNode node, string mapPath, Map map, bool isUsingStreamingPath = false, int firstID = 1) 
			: this(node, map, firstID)
		{
			_useWWWToLoad = isUsingStreamingPath;

			// Build tiles from this tileset
			string[] texturePath = new string[TileSetImages.Count];//mapPath;
			for (int i = 0; i < texturePath.Length; i++)
			{
				if (_useWWWToLoad)
				{
					texturePath[i] = string.Concat(mapPath, TileSetImages[i].Image);

					if (!texturePath[i].Contains("://"))
						texturePath[i] = string.Concat("file://", texturePath[i]);
				}
				else
				{
					texturePath[i] = Utils.XUniTMXHelpers.ParsePath(mapPath, TileSetImages[i].Image);
				}

				TileSetImages[i].Image = texturePath[i];
			}
			
		}

		public void Generate(Action<TileSet> onFinishedGeneratingTileSet = null)
		{
			if (IsGenerated)
			{
				if (OnFinishedGeneratingTileSet != null)
					OnFinishedGeneratingTileSet(this);
				return;
			}
			OnFinishedGeneratingTileSet = onFinishedGeneratingTileSet;

			if (!_useWWWToLoad)
			{
				for (int i = 0; i < TileSetImages.Count; i++)
				{
					TileSetImages[i].Texture = Resources.Load<Texture2D>(TileSetImages[i].Image);
					if (TileSetImages[i].Texture == null)
					{
						throw new System.IO.FileNotFoundException("Could not load TileSet Texture at " + TileSetImages[i].Image + "\nCheck if the Image is in the correct path and it is inside a Resources folder");
					}
					else
						TileSetImages[i].Texture.filterMode = XUniTMXConfiguration.Instance.TileSetsFilterMode;
				}
				BuildTiles(_mapTileWidth);
			}
			else
			{
				_loadedTexturesCount = 0;
				// Run Coroutine for WWW using TaskManager.
				for (int i = 0; i < TileSetImages.Count; i++)
				{
					new X_UniTMX.Utils.Task(LoadTileSetTexture(TileSetImages[i].Image, i), true);
				}
			}
		}

		IEnumerator LoadTileSetTexture(string path, int index)
		{
			TileSetImages[index].Texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			WWW www = new WWW(path);
			yield return www;
			if (www.error != null)
			{
				throw new System.IO.FileNotFoundException("Could not load TileSet Texture at " + path + "\nWWW Error: " + www.error);
			}
			else
			{
				www.LoadImageIntoTexture(TileSetImages[index].Texture);
				TileSetImages[index].Texture.name = Path.GetFileNameWithoutExtension(path);
				TileSetImages[index].Texture.filterMode = XUniTMXConfiguration.Instance.TileSetsFilterMode;
			}
			OnFinishedLoadingTexture();
			//BuildTiles(mapTileWidth);
		}

		void OnFinishedLoadingTexture()
		{
			_loadedTexturesCount++;

			if (_loadedTexturesCount >= TileSetImages.Count)
				BuildTiles(_mapTileWidth);
		}

		void BuildTiles(int mapTileWidth)
		{
			Texture2D _texture = TileSetImages[0].Texture;
			Rect[] _packSourceRects = null;
			// First, if there is more than one texture, pack'em
			if(TileSetImages.Count > 1) 
			{
				// Tiled saves to an ImageCollection TileSet the higher values of TileWidth and TileHeight from the tiles it contains,
				// but this breaks the positioning function the plugin uses, so we force the Map's original TileWidth and TileHeight values.
				TileWidth = _mapTileWidth;
				TileHeight = _mapTileHeight;
				Texture2D[] _texturesArray = new Texture2D[TileSetImages.Count];
				for (int i = 0; i < _texturesArray.Length; i++)
				{
					_texturesArray[i] = TileSetImages[i].Texture;
				}

				Spacing = 2;

				_texture = GameObject.Instantiate(TileSetImages[0].Texture) as Texture2D;
				_packSourceRects = _texture.PackTextures(_texturesArray, Spacing);

				// Now multiply the sources by _texture.width
				for (int i = 0; i < _packSourceRects.Length; i++)
				{
					_packSourceRects[i] = new Rect(
						_packSourceRects[i].xMin * _texture.width + XUniTMXConfiguration.Instance.PixelCorrection,
						_packSourceRects[i].yMin * _texture.height + XUniTMXConfiguration.Instance.PixelCorrection,
						_packSourceRects[i].width * _texture.width - XUniTMXConfiguration.Instance.PixelCorrection,
						_packSourceRects[i].height * _texture.height - XUniTMXConfiguration.Instance.PixelCorrection
						);
				}

				TileSetImages[0].Texture = _texture;
				//_texture.filterMode = FilterMode.Point;

				WasPacked = true;
			}
			if (!WasPacked)
			{
				// figure out how many frames fit on the X axis
				int frameCountX = -(2 * Margin - Spacing - _texture.width) / (TileWidth + Spacing);

				// figure out how many frames fit on the Y axis
				int frameCountY = -(2 * Margin - Spacing - _texture.height) / (TileHeight + Spacing);

				// make our tiles. tiles are numbered by row, left to right.
				for (int y = 0; y < frameCountY; y++)
				{
					for (int x = 0; x < frameCountX; x++)
					{
						// calculate the source rectangle
						int rx = Margin + x * (TileWidth + Spacing);
						int ry = _texture.height + Spacing - (Margin + (y + 1) * (TileHeight + Spacing));
						Rect Source = new Rect(
							rx + XUniTMXConfiguration.Instance.PixelCorrection,
							ry + XUniTMXConfiguration.Instance.PixelCorrection,
							TileWidth - XUniTMXConfiguration.Instance.PixelCorrection,
							TileHeight - XUniTMXConfiguration.Instance.PixelCorrection);
						//Debug.Log(Source);
						// get any properties from the tile set
						int index = FirstId + (y * frameCountX + x);
						PropertyCollection Properties = new PropertyCollection();
						if (TileProperties.ContainsKey(index))
						{
							Properties = TileProperties[index];
						}

						// save the tile
						Tiles.Add(index, new Tile(this, Source, index, Properties, Vector2.zero, mapTileWidth));
					}
				}
			}
			else
			{
				for (int i = 0; i < TileSetImages.Count; i++)
				{
					Rect Source = _packSourceRects[i];
					
					// get any properties from the tile set
					int index = FirstId + i;
					PropertyCollection Properties = new PropertyCollection();
					if (TileProperties.ContainsKey(index))
					{
						Properties = TileProperties[index];
					}

					// save the tile
					Tiles.Add(index, new Tile(this, Source, index, Properties, Vector2.zero, mapTileWidth));
				}
			}
			IsGenerated = true;
			if (OnFinishedGeneratingTileSet != null)
				OnFinishedGeneratingTileSet(this);
		}
	}

}
