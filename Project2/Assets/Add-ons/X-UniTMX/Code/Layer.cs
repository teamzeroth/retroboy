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
	/// An abstract base for a Layer in a Map.
	/// </summary>
	public abstract class Layer : PropertyContainer
	{
		/// <summary>
		/// Gets the name of the layer.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the width (in tiles) of the layer.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the layer.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets or sets the depth of the layer.
		/// </summary>
		public int LayerDepth { get; set; }

		/// <summary>
		/// Gets or sets the whether the layer is visible.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the opacity of the layer.
		/// </summary>
		public float Opacity { get; set; }

		/// <summary>
		/// Base Map this Layer is inside
		/// </summary>
		public Map BaseMap { get; protected set; }

		/// <summary>
		/// Layer's Game Object
		/// </summary>
		public GameObject LayerGameObject { get; private set; }

		/// <summary>
		/// Gets if this Layer has the Generate function called
		/// </summary>
		public bool IsGenerated { get; protected set; }

		protected string _tag = "Untagged";
		/// <summary>
		/// This LayerGameObject's Tag
		/// </summary>
		public string Tag
		{
			get { return _tag; }
			set
			{
				_tag = value;
				if (LayerGameObject != null)
					LayerGameObject.tag = _tag;
			}
		}

		protected int _physicsLayer = 0;
		/// <summary>
		/// This LayerGameObject's Physics Layer
		/// </summary>
		public int PhysicsLayer
		{
			get { return _physicsLayer; }
			set
			{
				_physicsLayer = value;
				if (LayerGameObject != null)
					LayerGameObject.layer = _physicsLayer;
			}
		}

		protected Action<Layer> OnGeneratedLayer = null;

		internal Layer(string name, int width, int height, int layerDepth, bool visible, float opacity, string tag, int physicsLayer, PropertyCollection properties)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
			this.LayerDepth = layerDepth;
			this.Visible = visible;
			this.Opacity = opacity;
			this.Properties = properties;
			Tag = tag;
			PhysicsLayer = physicsLayer;
			//LayerGameObject = new GameObject(Name);
		}

		protected Layer(NanoXMLNode node, Map tileMap)
        {
            //string Type = node.Name;
            Name = node.GetAttribute("name").Value;
			if (string.IsNullOrEmpty(Name))
				Name = "Layer";

			if (node.GetAttribute("width") != null)
				Width = int.Parse(node.GetAttribute("width").Value, CultureInfo.InvariantCulture);
			else
				Width = 0;
			if (node.GetAttribute("height") != null)
				Height = int.Parse(node.GetAttribute("height").Value, CultureInfo.InvariantCulture);
			else
				Height = 0;

			if (node.GetAttribute("opacity") != null)
			{
				Opacity = float.Parse(node.GetAttribute("opacity").Value, CultureInfo.InvariantCulture);
			}
			else
				Opacity = 1;

			if (node.GetAttribute("visible") != null)
			{
				Visible = int.Parse(node.GetAttribute("visible").Value, CultureInfo.InvariantCulture) == 1;
			}
			else
				Visible = true;

            NanoXMLNode propertiesNode = node["properties"];
            if (propertiesNode != null)
            {
                Properties = new PropertyCollection(propertiesNode);
            }

			BaseMap = tileMap;
        }

		protected virtual void Generate(string tag = "", int physicsLayer = 0)
		{
			IsGenerated = true;
			LayerGameObject = new GameObject(Name);
			Transform t = LayerGameObject.transform;
			if (BaseMap != null)
			{
				t.parent = BaseMap.MapGameObject.transform;
			}
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			t.localPosition = new Vector3(0, 0, LayerDepth);

			if (!string.IsNullOrEmpty(tag))
				Tag = tag;
			if (physicsLayer != 0)
				PhysicsLayer = physicsLayer;
			LayerGameObject.tag = Tag;
			LayerGameObject.layer = PhysicsLayer;
			LayerGameObject.isStatic = true;
		}

		protected virtual void Delete()
		{
			IsGenerated = false;
			GameObject.Destroy(LayerGameObject);
		}
	}
}
