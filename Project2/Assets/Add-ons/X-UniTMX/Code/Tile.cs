/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace X_UniTMX
{
    /// <summary>
    /// Tile SpriteEffects apllied in a TileLayer
    /// </summary>
    public class SpriteEffects : IEquatable<SpriteEffects>
    {
        /// <summary>
        /// Flag for Tile Flipped Horizontally
        /// </summary>
        public bool flippedHorizontally = false;
        /// <summary>
        /// Flag for Tile Flipped Vertically
        /// </summary>
        public bool flippedVertically = false;
        /// <summary>
        /// Flag for Tile Flipped AntiDiagonally (Diagonally reversed)
        /// </summary>
        public bool flippedAntiDiagonally = false;

		public bool Equals(SpriteEffects other)
		{
			return flippedHorizontally == other.flippedHorizontally &&
				flippedAntiDiagonally == other.flippedAntiDiagonally &&
				flippedVertically == other.flippedVertically;
		}
	}

    /// <summary>
    /// A single Tile in a TileLayer.
    /// </summary>
    public class Tile : PropertyContainer
    {
        /// <summary>
        /// Gets this Tile's original ID (the first set in Tiled)
        /// </summary>
        public int OriginalID { get; private set; }

        /// <summary>
        /// Gets this Tile's current ID (this can be changed ingame when TileLayer.SetTile is called)
        /// </summary>
        public int CurrentID { get; set; }

        /// <summary>
        /// Gets the Texture2D to use when drawing the tile.
        /// </summary>
        public TileSet TileSet { get; set; }

        /// <summary>
        /// Gets the source rectangle of the tile.
        /// </summary>
        public Rect Source { get; private set; }

        /// <summary>
        /// Gets or sets a color associated with the tile.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the SpriteEffects applied when drawing this tile.
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        /// Gets or sets this Tile Unity's GameObject
        /// </summary>
        public GameObject TileGameObject { get; set; }

        /// <summary>
        /// Gets or sets this Tile's Sprite
        /// </summary>
        public Sprite TileSprite { 
			get 
			{
				if (_tileSprite == null)
					CreateSprite();
				return _tileSprite;
			} 
			set
			{
				_tileSprite = value;
			} 
		}

        /// <summary>
        /// Gets the Map's Tile Width, used to calculate Texture's pixelsToUnits
        /// </summary>
        public int MapTileWidth { get; protected set; }

		Vector2 _spritePivot;
		Sprite _tileSprite;

        /// <summary>
        /// Creates a new Tile object.
        /// </summary>
        /// <param name="tileSet">The TileSet that contains the tile image.</param>
        /// <param name="source">The source rectangle of the tile.</param>
        /// <param name="OriginalID">This Tile's ID</param>
        public Tile(TileSet tileSet, Rect source, int OriginalID) : this(tileSet, source, OriginalID, new PropertyCollection(), Vector2.zero) { }

        /// <summary>
        /// Creates a new Tile object.
        /// </summary>
        /// <param name="tileSet">The TileSet that contains the tile image.</param>
        /// <param name="source">The source rectangle of the tile.</param>
        /// <param name="OriginalID">This Tile's ID</param>
        /// <param name="properties">The initial property collection or null to create an empty property collection.</param>
        /// <param name="pivot">The Tile's Sprite Pivot Point</param>
        /// <param name="mapTileWidth">The Map's TileWidth this tile is inside, used to calculate sprite's pixelsToUnits</param>
        public Tile(TileSet tileSet, Rect source, int OriginalID, PropertyCollection properties, Vector2 pivot, int mapTileWidth = 0)
        {
            if (tileSet == null)
                throw new ArgumentNullException("tileSet");

            this.OriginalID = OriginalID;
            CurrentID = OriginalID;
            TileSet = tileSet;
            Source = source;
            Properties = properties ?? new PropertyCollection();
            Color = Color.white;
            SpriteEffects = new X_UniTMX.SpriteEffects();

            MapTileWidth = mapTileWidth;
            if (mapTileWidth <= 0)
                MapTileWidth = TileSet.TileWidth;

			_spritePivot = pivot;
            //CreateSprite(pivot);
        }

        /// <summary>
        /// Creates a new Tile without creating the Sprite
        /// </summary>
        /// <param name="tileSet">The TileSet that contains the tile image.</param>
        /// <param name="source">The source rectangle of the tile.</param>
        /// <param name="OriginalID">This Tile's ID</param>
        /// <param name="properties">The initial property collection or null to create an empty property collection.</param>
        internal Tile(TileSet tileSet, Rect source, int OriginalID, PropertyCollection properties, int mapTileWidth = 0)
        {
            this.OriginalID = OriginalID;
            CurrentID = OriginalID;
            TileSet = tileSet;
            Source = source;
            Properties = properties ?? new PropertyCollection();
            Color = Color.white;
            SpriteEffects = new X_UniTMX.SpriteEffects();
            if (mapTileWidth <= 0)
                MapTileWidth = TileSet.TileWidth;
        }

        /// <summary>
        /// Creates this Tile's Sprite
        /// </summary>
        protected void CreateSprite()
        {
            // Create Sprite
            _tileSprite = Sprite.Create(TileSet.Texture, Source, _spritePivot, MapTileWidth, (uint)(TileSet.Spacing * 2));
            _tileSprite.name = OriginalID.ToString();
        }

        /// <summary>
        /// Creates this Tile's GameObject (TileGameObject)
        /// </summary>
        /// <param name="objectName">Desired name</param>
        /// <param name="parent">GameObject's parent</param>
        /// <param name="sortingLayerName">Sprite's sorting layer name</param>
        /// <param name="sortingLayerOrder">Sprite's sorting layer order</param>
        /// <param name="position">GameObject's position</param>
        /// <param name="materials">List of shared materials</param>
        /// <param name="opacity">This Object's Opacity</param>
        public void CreateTileObject(string objectName, Transform parent, string sortingLayerName, int sortingLayerOrder, Vector3 position, List<Material> materials, float opacity = 1.0f)
        {
            for (int k = 0; k < materials.Count; k++)
            {
                if (materials[k].mainTexture.name == TileSet.Texture.name)
                {
                    CreateTileObject(objectName, parent, sortingLayerName, sortingLayerOrder, position, materials[k], opacity);
                    break;
                }
            }
        }

        /// <summary>
        /// Creates this Tile's GameObject (TileGameObject) using a specific material
        /// </summary>
        /// <param name="objectName">Desired name</param>
        /// <param name="parent">GameObject's parent</param>
        /// <param name="sortingLayerName">Sprite's sorting layer name</param>
        /// <param name="sortingLayerOrder">Sprite's sorting layer order</param>
        /// <param name="position">GameObject's position</param>
        /// <param name="material">Material to be used in the Tile Object</param>
        /// <param name="opacity">This Object's Opacity</param>
        public void CreateTileObject(string objectName, Transform parent, string sortingLayerName, int sortingLayerOrder, Vector3 position, Material material, float opacity = 1.0f)
        {
            TileGameObject = new GameObject(objectName);
            TileGameObject.transform.parent = parent;

            SpriteRenderer tileRenderer = TileGameObject.AddComponent<SpriteRenderer>();
			if (TileSprite == null)
				CreateSprite();
            tileRenderer.sprite = TileSprite;

            // Use Layer's name as Sorting Layer
            tileRenderer.sortingLayerName = sortingLayerName;
            tileRenderer.sortingOrder = sortingLayerOrder;

            TileGameObject.transform.localScale = new Vector2(1, 1);
            TileGameObject.transform.localPosition = new Vector3(position.x, position.y, position.z);

            if (this.SpriteEffects != null)
            {
                if (this.SpriteEffects.flippedHorizontally ||
                    this.SpriteEffects.flippedVertically ||
                    this.SpriteEffects.flippedAntiDiagonally)
                {
                    // MARIO: Fixed flippedHorizontally, flippedVertically and flippedAntiDiagonally effects
                    float ratioHW = TileSet.TileHeight / (float)MapTileWidth;

                    if (this.SpriteEffects.flippedHorizontally == true &&
                       this.SpriteEffects.flippedVertically == false &&
                       this.SpriteEffects.flippedAntiDiagonally == false)
                    {
                        TileGameObject.transform.localScale = new Vector2(-1, 1);
                        TileGameObject.transform.localPosition = new Vector3(position.x + 1, position.y, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == false &&
                       this.SpriteEffects.flippedVertically == true &&
                       this.SpriteEffects.flippedAntiDiagonally == false)
                    {
                        TileGameObject.transform.localScale = new Vector2(1, -1);
                        TileGameObject.transform.localPosition = new Vector3(position.x, position.y + ratioHW, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == true &&
                       this.SpriteEffects.flippedVertically == true &&
                       this.SpriteEffects.flippedAntiDiagonally == false)
                    {
                        TileGameObject.transform.localScale = new Vector2(-1, -1);
                        TileGameObject.transform.localPosition = new Vector3(position.x + 1, position.y + ratioHW, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == false &&
                       this.SpriteEffects.flippedVertically == false &&
                       this.SpriteEffects.flippedAntiDiagonally == true)
                    {
                        TileGameObject.transform.Rotate(Vector3.forward, 90);
                        TileGameObject.transform.localScale = new Vector2(-1, 1);
                        TileGameObject.transform.localPosition = new Vector3(position.x + ratioHW, position.y + 1, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == true &&
                       this.SpriteEffects.flippedVertically == false &&
                       this.SpriteEffects.flippedAntiDiagonally == true)
                    {
                        TileGameObject.transform.Rotate(Vector3.forward, -90);
                        TileGameObject.transform.localPosition = new Vector3(position.x, position.y + 1, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == false &&
                       this.SpriteEffects.flippedVertically == true &&
                       this.SpriteEffects.flippedAntiDiagonally == true)
                    {
                        TileGameObject.transform.Rotate(Vector3.forward, 90);
                        TileGameObject.transform.localPosition = new Vector3(position.x + ratioHW, position.y, position.z);
                    }

                    if (this.SpriteEffects.flippedHorizontally == true &&
                       this.SpriteEffects.flippedVertically == true &&
                       this.SpriteEffects.flippedAntiDiagonally == true)
                    {
                        TileGameObject.transform.Rotate(Vector3.forward, 90);
                        TileGameObject.transform.localScale = new Vector2(1, -1);
                        TileGameObject.transform.localPosition = new Vector3(position.x, position.y, position.z);
                    }
                }
            }

            if (material.name != TileSet.Texture.name)
            {
                material = new Material(material);
                material.mainTexture = TileSet.Texture;
            }
            tileRenderer.sharedMaterial = material;

            if (opacity < 1)
                tileRenderer.sharedMaterial.color = new Color(1, 1, 1, opacity);
        }

        /// <summary>
        /// Creates a copy of the current tile.
        /// </summary>
        /// <returns>A new Tile with the same properties as the current tile.</returns>
        public virtual Tile Clone()
        {
            Tile t = new Tile(TileSet, Source, OriginalID, Properties);
            t.TileSprite = _tileSprite;
            t.SpriteEffects = SpriteEffects;
            t.MapTileWidth = MapTileWidth;
            return t;
        }

        /// <summary>
        /// Creates a copy of the current tile with a different pivot point.
        /// </summary>
        /// <param name="pivot">New pivot point</param>
        /// <returns>A new Tile with the same properties as the current tile.</returns>
        public virtual Tile Clone(Vector2 pivot)
        {
            return new Tile(TileSet, Source, OriginalID, Properties, pivot);
        }
    }
}
