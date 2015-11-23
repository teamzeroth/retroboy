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
	/// An arbitrary Object placed on an ObjectLayer.
	/// </summary>
	public class MapObject : Object
	{
		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the type of the object.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		/// Gets the object TileID
		/// </summary>
		public int GID { get; private set; }

		/// <summary>
		/// Gets or sets the whether the object is visible.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// The Object Layer this MapObject belongs to
		/// </summary>
		public MapObjectLayer ParentObjectLayer { get; set; }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		public MapObject(string name, string type) : this(name, type, new Rect(), new PropertyCollection(), 0, new List<Vector2>(), 0, null) { }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		/// <param name="bounds">The initial bounds of the object.</param>
		public MapObject(string name, string type, Rect bounds) : this(name, type, bounds, new PropertyCollection(), 0, new List<Vector2>(), 0, null) { }

		/// <summary>
		/// Creates a new MapObject.
		/// </summary>
		/// <param name="name">The name of the object.</param>
		/// <param name="type">The type of object to create.</param>
		/// <param name="bounds">The initial bounds of the object.</param>
		/// <param name="properties">The initial property collection or null to create an empty property collection.</param>
		/// <param name="rotation">This object's rotation</param>
		/// <param name="gid">Object's ID</param>
		/// <param name="parentObjectLayer">This MapObject's MapObjectLayer parent</param>
		/// <param name="points">This MapObject's Point list</param>
		public MapObject(string name, string type, Rect bounds, PropertyCollection properties, int gid, List<Vector2> points, float rotation, MapObjectLayer parentObjectLayer)
			: base(ObjectType.Box, bounds, rotation, points, properties)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException(null, "name");

			Name = name;
			Type = type;
			Bounds = bounds;
			GID = gid;
			Points = points;
			Visible = true;
			Rotation = rotation;
			ParentObjectLayer = parentObjectLayer;
		}

		/// <summary>
		/// Creates a MapObject from node
		/// </summary>
		/// <param name="node">NanoXMLNode XML to parse</param>
		/// <param name="parentObjectLayer">This MapObject's MapObjectLayer parent</param>
		public MapObject(NanoXMLNode node, MapObjectLayer parentObjectLayer)
			: base(node)
        {
			if (node.GetAttribute("name") != null)
			{
				Name = node.GetAttribute("name").Value;
			}
			else
			{
				Name = "Object";
			}

			if (node.GetAttribute("type") != null)
			{
				Type = node.GetAttribute("type").Value;
			}
			else
				Type = string.Empty;

			if (node.GetAttribute("visible") != null)
			{
				Visible = int.Parse(node.GetAttribute("visible").Value, CultureInfo.InvariantCulture) == 1;
			}
			else
				Visible = true;

			if (node.GetAttribute("gid") != null)
				GID = int.Parse(node.GetAttribute("gid").Value, CultureInfo.InvariantCulture);
			else
				GID = 0;

			ParentObjectLayer = parentObjectLayer;
        }
	}
}
