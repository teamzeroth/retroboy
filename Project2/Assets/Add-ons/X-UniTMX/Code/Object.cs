/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using System;
using System.Collections.Generic;
using TObject.Shared;
using System.Globalization;
using UnityEngine;

namespace X_UniTMX
{
	/// <summary>
	/// Object Type, from Tiled's Objects types
	/// </summary>
	public enum ObjectType : byte
	{
		/// <summary>
		/// A Tile Object (An Object with GID)
		/// </summary>
		Tile,
		/// <summary>
		/// A Box Object
		/// </summary>
		Box,
		/// <summary>
		/// An Ellipse Object
		/// </summary>
		Ellipse,
		/// <summary>
		/// A Polygon Object
		/// </summary>
		Polygon,
		/// <summary>
		/// A Polyline Object
		/// </summary>
		Polyline
	}

	/// <summary>
	/// Possible Anchor points, used for prefab spawning
	/// </summary>
    public enum Anchor : byte
    {
		// Top left anchor (X: 0, Y: 0)
        Top_Left,
		// Top anchor (X: TileWidth / 2, Y: 0)
        Top,
		// Top right anchor (X: TileWidth, Y: 0)
        Top_Right,
		// Left anchor (X: 0, Y: TileHeight / 2)
		Left,
		// Center anchor [default] (X: TileWidth / 2, Y: TileHeight / 2)
		Center,
		// Right anchor (X: TileWidth, Y: TileHeight / 2)
		Right,
		// Bottom left anchor (X: 0, Y: TileHeight)
		Bottom_Left,
		// Bottom anchor (X: TileWidth / 2, Y: TileHeight)
		Bottom,
		// Bottom right anchor (X: TileWidth, Y: TileHeight / 2)
		Bottom_Right,
		// Custom anchor (X: User defined [0-1] * TileWidth, Y: User defined [0-1] * TileHeight)
		Custom
    }

	/// <summary>
	/// An abstract base for an object in a map.
	/// </summary>
	public abstract class Object : PropertyContainer
	{
		/// <summary>
		/// Gets the ObjectType of the object.
		/// </summary>
		public ObjectType ObjectType { get; protected set; }

		/// <summary>
		/// Gets or sets the bounds of the object.
		/// </summary>
		public Rect Bounds { get; set; }

		/// <summary>
		/// Gets or sets this object's rotation
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		/// Gets a list of the object's points
		/// </summary>
		public List<Vector2> Points { get; protected set; }

		/// <summary>
		/// Gets the Object's ID
		/// </summary>
		public int ID { get; private set; }

		/// <summary>
		/// Gets or Sets a GameObject to be linked to this Object (eg: Colliders)
		/// </summary>
		public GameObject LinkedGameObject { get; set; }

		/// <summary>
		/// Only derived classes should use this constructor. Creates an Object from parameters
		/// </summary>
		/// <param name="objectType">The ObjectType</param>
		/// <param name="bounds">The Rect bounds</param>
		/// <param name="rotation">Object's rotation</param>
		/// <param name="points">Object's list of points</param>
		internal Object(ObjectType objectType, Rect bounds, float rotation, List<Vector2> points, PropertyCollection properties)
		{
			ObjectType = objectType;
			Bounds = bounds;
			Rotation = rotation;
			Points = points;
			Properties = properties ?? new PropertyCollection();
		}

		/// <summary>
		/// Only derived classes should use this constructor. Creates an Object from a XML node
		/// </summary>
		/// <param name="node">XML node to parse</param>
		protected Object(NanoXMLNode node)
        {
			if (node.GetAttribute("rotation") != null)
			{
				Rotation = 360 - float.Parse(node.GetAttribute("rotation").Value, CultureInfo.InvariantCulture);
			}
			else
				Rotation = 0;

			// values default to 0 if the attribute is missing from the node
			ID = node.GetAttribute("id") != null ? int.Parse(node.GetAttribute("id").Value, CultureInfo.InvariantCulture) : 0;
			float x = node.GetAttribute("x") != null ? float.Parse(node.GetAttribute("x").Value, CultureInfo.InvariantCulture) : 0;
			float y = node.GetAttribute("y") != null ? float.Parse(node.GetAttribute("y").Value, CultureInfo.InvariantCulture) : 0;
			float width = node.GetAttribute("width") != null ? float.Parse(node.GetAttribute("width").Value, CultureInfo.InvariantCulture) : 1;
			float height = node.GetAttribute("height") != null ? float.Parse(node.GetAttribute("height").Value, CultureInfo.InvariantCulture) : 1;

			Bounds = new Rect(x, y, width, height);

			this.ObjectType = ObjectType.Box;

			// stores a string of points to parse out if this object is a polygon or polyline
			string pointsAsString = null;

			if (node["ellipse"] != null)
			{
				ObjectType = ObjectType.Ellipse;
			}
			// if there's a polygon node, it's a polygon object
			else if (node["polygon"] != null)
			{
				pointsAsString = node["polygon"].GetAttribute("points").Value;
				ObjectType = ObjectType.Polygon;
			}
			// if there's a polyline node, it's a polyline object
			else if (node["polyline"] != null)
			{
				pointsAsString = node["polyline"].GetAttribute("points").Value;
				ObjectType = ObjectType.Polyline;
			}

			// if we have some points to parse, we do that now
			if (pointsAsString != null)
			{
				// points are separated first by spaces
				Points = new List<Vector2>();
				string[] pointPairs = pointsAsString.Split(' ');
				foreach (string p in pointPairs)
				{
					// then we split on commas
					string[] coords = p.Split(',');

					// then we parse the X/Y coordinates
					Points.Add(new Vector2(
						float.Parse(coords[0], CultureInfo.InvariantCulture),
						float.Parse(coords[1], CultureInfo.InvariantCulture)));
				}
			}

			NanoXMLNode propertiesNode = node["properties"];
			if (propertiesNode != null)
			{
				Properties = new PropertyCollection(propertiesNode);
			}
		}
		/*
		/// <summary>
		/// Scale this object's dimensions using map's tile size.
		/// We need to do this as Tiled saves object dimensions in Pixels, but we need to convert it to Unity's Unit
		/// </summary>
		/// <param name="TileWidth">Tiled Map Tile Width</param>
		/// <param name="TileHeight">Tiled Map Tile Height</param>
		protected void ScaleObject(float TileWidth, float TileHeight)
		{
			this.Bounds = new Rect(this.Bounds.x / TileWidth, this.Bounds.y / TileHeight, this.Bounds.width / TileWidth, this.Bounds.height / TileHeight);

			if (this.Points != null)
			{
				for (int i = 0; i < this.Points.Count; i++)
				{
					this.Points[i] = new Vector2(this.Points[i].x / TileWidth, this.Points[i].y / TileHeight);
				}
			}
		}
		/// <summary>
		/// Scale this object's dimensions using map's tile size.
		/// We need to do this as Tiled saves object dimensions in Pixels, but we need to convert it to Unity's Unit
		/// </summary>
		/// <param name="TileWidth">Tiled Map Tile Width</param>
		/// <param name="TileHeight">Tiled Map Tile Height</param>
		/// <param name="orientation">Tiled Map Orientation</param>
		/// <param name="staggerAxis">Tiled Map Stagger Axis, for staggered projections</param>
		/// <param name="staggerIndex">Tiled Map Stagger Index, for staggered projections</param>
		public void ScaleObject(float TileWidth, float TileHeight, Orientation orientation, StaggerAxis staggerAxis = StaggerAxis.Y, StaggerIndex staggerIndex = StaggerIndex.Odd)
		{
			float x = 0, y = 0, width = 0, height = 0;
			switch (orientation)
			{
				case Orientation.Isometric:
					// In Isometric maps, we must consider tile width == height for objects so their size can be correctly calculated
					ScaleObject(TileHeight, TileHeight);
					break;
				case Orientation.Staggered:
					// In Staggered maps, we must pre-alter object position and size, as it comes mixed between staggered and orthogonal properties
					x = Bounds.x / (float)TileWidth;
					y = Bounds.y / (float)TileHeight * 2.0f;
					width = Bounds.width / (float)TileWidth;
					height = Bounds.height / (float)TileWidth;

					if (staggerAxis.Equals(StaggerAxis.Y))
					{
						if ((staggerIndex.Equals(StaggerIndex.Odd) && Mathf.FloorToInt(Mathf.Abs(y)) % 2 > 0) ||
							(staggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(y)) % 2 < 1))
							x -= 0.5f;
					}
					else
					{
						x *= 2;
						y /= 2;
						if ((staggerIndex.Equals(StaggerIndex.Odd) && Mathf.FloorToInt(Mathf.Abs(x)) % 2 > 0) ||
							(staggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(x)) % 2 < 1))
							y -= 0.5f;
					}

					Bounds = new Rect(x, y, width, height);

					if (Points != null)
					{
						for (int i = 0; i < Points.Count; i++)
						{
							Points[i] = new Vector2(Points[i].x / (float)TileWidth, Points[i].y / (float)TileHeight * 2.0f);
						}
					}
					break;
				case Orientation.Hexagonal:
					// In Hexagonal maps, we must pre-alter object position and size, as it comes mixed between staggered and orthogonal properties
					x = Bounds.x / (float)TileWidth;
					y = Bounds.y / (float)TileHeight;
					width = Bounds.width / (float)TileWidth;
					height = Bounds.height / (float)TileHeight;

					Bounds = new Rect(x, y, width, height);

					if (Points != null)
					{
						for (int i = 0; i < Points.Count; i++)
						{
							Points[i] = new Vector2(Points[i].x / (float)TileWidth, Points[i].y / (float)TileHeight);
						}
					}
					break;
				default:
					ScaleObject(TileWidth, TileHeight);
					break;
			}
		}*/
	}
}
