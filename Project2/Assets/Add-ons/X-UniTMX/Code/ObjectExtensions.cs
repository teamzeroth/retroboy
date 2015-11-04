using UnityEngine;
using System.Collections.Generic;

namespace X_UniTMX
{
	/// <summary>
	/// Class containing Extension Functions for Objects, to unclutter the object files.
	/// </summary>
	public static class ObjectExtensions
	{
		#region Scale Object
		public static Object ScaleObjectHelper(Object obj, int TileWidth, int TileHeight)
		{
			obj.Bounds = new Rect(obj.Bounds.x / (float)TileWidth, obj.Bounds.y / (float)TileHeight, obj.Bounds.width / (float)TileWidth, obj.Bounds.height / (float)TileHeight);

			if (obj.Points != null)
			{
				for (int i = 0; i < obj.Points.Count; i++)
				{
					obj.Points[i] = new Vector2(obj.Points[i].x / (float)TileWidth, obj.Points[i].y / (float)TileHeight);
				}
			}
			return obj;
		}

		/// <summary>
		/// Scale this object's dimensions using map's tile size.
		/// We need to do this as Tiled saves object dimensions in Pixels, but we need to convert it to Unity's Unit
		/// </summary>
		/// <param name="mrp">The MapRenderParameters to which the object will be scaled</param>
		public static Object ScaleObject(this Object obj, MapRenderParameters mrp)
		{
			float x = 0, y = 0, width = 0, height = 0;
			switch (mrp.Orientation)
			{
				case Orientation.Isometric:
					// In Isometric maps, we must consider tile width == height for objects so their size can be correctly calculated
					return ScaleObjectHelper(obj, mrp.TileHeight, mrp.TileHeight);

				case Orientation.Staggered:
					// In Staggered maps, we must pre-alter object position and size, as it comes mixed between staggered and orthogonal properties
					x = obj.Bounds.x / (float)mrp.TileWidth;
					y = obj.Bounds.y / (float)mrp.TileHeight * 2.0f;
					width = obj.Bounds.width / (float)mrp.TileWidth;
					height = obj.Bounds.height / (float)mrp.TileWidth;

					if (mrp.MapStaggerAxis.Equals(StaggerAxis.Y))
					{
						if ((mrp.MapStaggerIndex.Equals(StaggerIndex.Odd) && Mathf.FloorToInt(Mathf.Abs(y)) % 2 > 0) ||
							(mrp.MapStaggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(y)) % 2 < 1))
							x -= 0.5f;
					}
					else
					{
						x *= 2;
						y /= 2;
						if ((mrp.MapStaggerIndex.Equals(StaggerIndex.Odd) && Mathf.FloorToInt(Mathf.Abs(x)) % 2 > 0) ||
							(mrp.MapStaggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(x)) % 2 < 1))
							y -= 0.5f;
					}

					obj.Bounds = new Rect(x, y, width, height);

					if (obj.Points != null)
					{
						for (int i = 0; i < obj.Points.Count; i++)
						{
							obj.Points[i] = new Vector2(obj.Points[i].x / (float)mrp.TileWidth, obj.Points[i].y / (float)mrp.TileHeight * 2.0f);
						}
					}
					return obj;

				case Orientation.Hexagonal:
					// In Hexagonal maps, we must pre-alter object position and size, as it comes mixed between staggered and orthogonal properties
					float halfGap = 1;
					float tileDisplacement = 1;

					if (mrp.MapStaggerAxis.Equals(StaggerAxis.Y))
					{
						halfGap = (mrp.TileHeight - mrp.HexSideLength) / 2.0f;
						tileDisplacement = halfGap + mrp.HexSideLength;

						x = obj.Bounds.x / mrp.TileWidth;
						y = obj.Bounds.y / (tileDisplacement / (float)mrp.TileHeight) / (float)mrp.TileHeight;

						if (InAStaggeredRowOrColumn(mrp.MapStaggerIndex, y))
							x -= 0.5f;
						
						if (obj.Points != null)
						{
							for (int i = 0; i < obj.Points.Count; i++)
							{
								obj.Points[i] = new Vector2(
									obj.Points[i].x / mrp.TileWidth,
									obj.Points[i].y / (tileDisplacement / (float)mrp.TileHeight) / (float)mrp.TileHeight
									);

								if (InAStaggeredRowOrColumn(mrp.MapStaggerIndex, obj.Points[i].y))
									obj.Points[i] -= new Vector2(0.5f, 0);
							}
						}
					}
					else
					{
						halfGap = (mrp.TileWidth - mrp.HexSideLength) / 2.0f;
						tileDisplacement = halfGap + mrp.HexSideLength;

						x = obj.Bounds.x / tileDisplacement;
						y = obj.Bounds.y / (mrp.TileHeight / (float)mrp.TileWidth) / (float)mrp.TileHeight;

						if (InAStaggeredRowOrColumn(mrp.MapStaggerIndex, x))
							y -= 0.5f * (mrp.TileHeight / (float)mrp.TileWidth);


						if (obj.Points != null)
						{
							for (int i = 0; i < obj.Points.Count; i++)
							{
								obj.Points[i] = new Vector2(
									obj.Points[i].x / tileDisplacement,
									obj.Points[i].y / (mrp.TileHeight / (float)mrp.TileWidth) / (float)mrp.TileHeight
									);

								if (InAStaggeredRowOrColumn(mrp.MapStaggerIndex, obj.Points[i].x))
									obj.Points[i] -= new Vector2(0, 0.5f * (mrp.TileHeight / (float)mrp.TileWidth));
							}
						}
					}

					width = obj.Bounds.width / (float)mrp.TileWidth;
					height = obj.Bounds.height / (float)mrp.TileHeight;

					obj.Bounds = new Rect(x, y, width, height);

					return obj;

				default:
					return ScaleObjectHelper(obj, mrp.TileWidth, mrp.TileHeight);
			}
		}

		public static bool InAStaggeredRowOrColumn(StaggerIndex staggerIndex, float coordinate)
		{
			return 
				(!staggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(coordinate)) % 2 > 0) ||
				(staggerIndex.Equals(StaggerIndex.Even) && Mathf.FloorToInt(Mathf.Abs(coordinate)) % 2 < 1);
		}
		#endregion

		/// <summary>
		/// Creates this Tile Object (an object that has OriginalID) if applicable
		/// </summary>
		/// <param name="tiledMap">The base Tile Map</param>
		/// <param name="layerDepth">Layer's zDepth</param>
		/// <param name="sortingLayerName">Layer's SortingLayerName</param>
		/// <param name="parent">Transform to parent this object to</param>
		/// <param name="materials">List of TileSet Materials</param>
		public static GameObject CreateTileObject(this MapObject obj, Map tiledMap, string sortingLayerName, int layerDepth, List<Material> materials, Transform parent = null)
		{
			if (obj.GID > 0)
			{
				Tile objTile = null;
				if (tiledMap.MapRenderParameter.Orientation != Orientation.Orthogonal)
					objTile = tiledMap.Tiles[obj.GID].Clone(new Vector2(0.5f, 0.5f));
				else
					objTile = tiledMap.Tiles[obj.GID].Clone();

				objTile.CreateTileObject(obj.Name,
					parent != null ? parent : obj.ParentObjectLayer.LayerGameObject.transform,
					sortingLayerName,
					tiledMap.DefaultSortingOrder + tiledMap.GetSortingOrder(obj.Bounds.x, obj.Bounds.y),
					tiledMap.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, layerDepth),
					materials);
				obj.Bounds.Set(obj.Bounds.x, obj.Bounds.y - 1, obj.Bounds.width, obj.Bounds.height);

				objTile.TileGameObject.SetActive(obj.Visible);

				// Since Tiled 0.12, TileObjects' Bounds will have the desired width and height of the image, so scale accordingly
				if(obj.Bounds.width != 1 || obj.Bounds.height != 1)
					objTile.TileGameObject.transform.localScale = new Vector3(obj.Bounds.width / objTile.TileSprite.bounds.size.x, obj.Bounds.height / objTile.TileSprite.bounds.size.y);

				return objTile.TileGameObject;
			}
			return null;
		}

		/// <summary>
		/// Returns the actual Anchor Point Vector value based on anchor
		/// </summary>
		/// <param name="anchor">Anchor to be used to calculate the actual value</param>
		/// <returns>A Vector2 containing the actual Anchor Point value, normalized.</returns>
		public static Vector2 GetAnchorPointValue(Anchor anchor)
		{
			Vector2 a = Vector2.zero;
			switch (anchor)
			{
				case Anchor.Bottom:
					a.x = 0.5f;
					a.y = 1;
					break;
				case Anchor.Bottom_Left:
					a.y = 1;
					break;
				case Anchor.Bottom_Right:
					a.x = a.y = 1;
					break;
				case Anchor.Center:
					a.x = a.y = 0.5f;
					break;
				case Anchor.Custom:
					break;
				case Anchor.Left:
					a.y = 0.5f;
					break;
				case Anchor.Right:
					a.x = 1;
					a.y = 0.5f;
					break;
				case Anchor.Top:
					a.x = 0.5f;
					break;
				case Anchor.Top_Left:
					break;
				case Anchor.Top_Right:
					a.x = 1;
					break;
			}
			return a;
		}
	}
}
