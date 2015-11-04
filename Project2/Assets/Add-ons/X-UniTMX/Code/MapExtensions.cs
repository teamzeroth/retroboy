using UnityEngine;
using System.Collections.Generic;
using X_UniTMX.Utils;

namespace X_UniTMX
{
	/// <summary>
	/// Class containing Extension Functions for the Map, to unclutter the map file.
	/// </summary>
	public static class MapExtensions
	{
		#region Sorting Order Calculation
		/// <summary>
		/// Calculate Tile's SortingOrder based on Map's RenderOrder, Map's Orientation and the Tile's index
		/// </summary>
		/// <param name="x">Tile X index</param>
		/// <param name="y">Tile Y index</param>
		/// <returns>SortingOrder to be used for a Renderer in this Tile index</returns>
		public static int GetSortingOrder(this Map map, int x, int y)
		{
			int sortingOrder = 0;
			switch (map.MapRenderParameter.MapRenderOrder)
			{
				case RenderOrder.Right_Down:
					sortingOrder = y * map.MapRenderParameter.Width + x;
					break;
				case RenderOrder.Right_Up:
					sortingOrder = (map.MapRenderParameter.Height - y) * map.MapRenderParameter.Width + x;
					break;
				case RenderOrder.Left_Down:
					sortingOrder = y * map.MapRenderParameter.Width + map.MapRenderParameter.Height - x;
					break;
				case RenderOrder.Left_Up:
					sortingOrder = (map.MapRenderParameter.Height - y) * map.MapRenderParameter.Width + map.MapRenderParameter.Height - x;
					break;
			}

			return sortingOrder;
		}

		/// <summary>
		/// Calculate Tile's SortingOrder based on Map's RenderOrder, Map's Orientation and the Tile's index
		/// </summary>
		/// <param name="x">Tile X index</param>
		/// <param name="y">Tile Y index</param>
		/// <returns>SortingOrder to be used for a Renderer in this Tile index</returns>
		public static int GetSortingOrder(this Map map, float x, float y)
		{
			return GetSortingOrder(map, Mathf.RoundToInt(x), Mathf.RoundToInt(y));
		}
		#endregion

		#region Position Converters
		#region World Point to Tiled Position
		/// <summary>
		/// Converts a point in world space into tiled space.
		/// </summary>
		/// <param name="worldPoint">The point in world space to convert into tiled space.</param>
		/// <returns>The point in Tiled space.</returns>
		public static Vector2 WorldPointToTiledPosition(this Map map, Vector2 worldPoint)
		{
			Vector2 p = new Vector2();

			bool staggeredInY = map.MapRenderParameter.MapStaggerAxis.Equals(StaggerAxis.Y);
			bool staggeredEven = map.MapRenderParameter.MapStaggerIndex.Equals(StaggerIndex.Even);

			MapRenderParameters mrp = map.MapRenderParameter;

			if (mrp.Orientation == X_UniTMX.Orientation.Orthogonal)
			{
				// simple conversion to tile indices
				p.x = worldPoint.x;
				p.y = -worldPoint.y;
			}
			else if (mrp.Orientation == X_UniTMX.Orientation.Isometric)
			{
				float ratio = mrp.TileHeight / (float)mrp.TileWidth;
				// for some easier calculations, convert wordPoint to pixels
				Vector2 point = new Vector2(worldPoint.x * mrp.TileWidth, -worldPoint.y / ratio * mrp.TileHeight);

				// Code almost straight from Tiled's libtiled :P

				point.x -= mrp.Height * mrp.TileWidth / 2.0f;
				float tileX = point.x / (float)mrp.TileWidth;
				float tileY = point.y / (float)mrp.TileHeight;

				p.x = tileY + tileX;
				p.y = tileY - tileX;
			}
			else if (mrp.Orientation == X_UniTMX.Orientation.Staggered)
			{
				float ratio = mrp.TileHeight / (float)mrp.TileWidth;
				// for some easier calculations, convert wordPoint to pixels
				Vector2 point = new Vector2(worldPoint.x * (float)mrp.TileWidth, -worldPoint.y / ratio * (float)mrp.TileHeight);

				float halfTileHeight = mrp.TileHeight / 2.0f;
				float halfTileWidth = mrp.TileWidth / 2.0f;

				// Code almost straight from Tiled's libtiled :P

				if (!staggeredInY)
					point.y -= staggeredEven ? halfTileHeight : 0;
				else
					point.x -= staggeredEven ? halfTileWidth : 0;

				int tileX = 0, tileY = 0;
				float relX = 0, relY = 0;

				if (staggeredInY)
				{
					// Getting grid-aligned tile index
					tileX = Mathf.FloorToInt(point.x / (float)mrp.TileWidth);
					tileY = Mathf.FloorToInt(point.y / (float)mrp.TileHeight) * 2;

					// Relative x and y pos to tile
					relX = point.x - tileX * (float)mrp.TileWidth;
					relY = point.y - tileY / 2.0f * (float)mrp.TileHeight;
				}
				else
				{
					// Getting grid-aligned tile index
					tileX = Mathf.FloorToInt(point.x / (float)mrp.TileWidth) * 2;
					tileY = Mathf.FloorToInt(point.y / (float)mrp.TileHeight);

					// Relative x and y pos to tile
					relX = point.x - tileX / 2.0f * (float)mrp.TileWidth;
					relY = point.y - tileY * (float)mrp.TileHeight;
				}

				float ypos = relX * ratio;

				// Top-Left
				if (halfTileHeight - ypos > relY)
				{
					if (staggeredInY)
					{
						p.y = tileY - 1;
						if ((!staggeredEven && tileY % 2 > 0) ||
							(staggeredEven && tileY % 2 < 1))
							p.x = tileX;
						else
							p.x = tileX - 1;
					}
					else
					{
						p.x = tileX - 1;
						if ((!staggeredEven && tileX % 2 > 0) ||
							(staggeredEven && tileX % 2 < 1))
							p.y = tileY;
						else
							p.y = tileY - 1;
					}
				}
				// Top Right
				else if (-halfTileHeight + ypos > relY)
				{
					if (staggeredInY)
					{
						p.y = tileY - 1;
						if ((!staggeredEven && tileY % 2 > 0) ||
							(staggeredEven && tileY % 2 < 1))
							p.x = tileX + 1;
						else
							p.x = tileX;
					}
					else
					{
						p.x = tileX + 1;
						if ((!staggeredEven && tileX % 2 > 0) ||
							(staggeredEven && tileX % 2 < 1))
							p.y = tileY;
						else
							p.y = tileY - 1;
					}
				}
				// Bottom Left
				else if (halfTileHeight + ypos < relY)
				{
					if (staggeredInY)
					{
						p.y = tileY + 1;
						if ((!staggeredEven && tileY % 2 > 0) ||
							(staggeredEven && tileY % 2 < 1))
							p.x = tileX;
						else
							p.x = tileX - 1;
					}
					else
					{
						p.x = tileX - 1;
						if ((!staggeredEven && tileX % 2 > 0) ||
							(staggeredEven && tileX % 2 < 1))
							p.y = tileY + 1;
						else
							p.y = tileY;
					}
				}
				// Bottom Right
				else if (halfTileHeight * 3 - ypos < relY)
				{
					if (staggeredInY)
					{
						p.y = tileY + 1;
						if ((!staggeredEven && tileY % 2 > 0) ||
							(staggeredEven && tileY % 2 < 1))
							p.x = tileX + 1;
						else
							p.x = tileX;
					}
					else
					{
						p.x = tileX + 1;
						if ((!staggeredEven && tileX % 2 > 0) ||
							(staggeredEven && tileX % 2 < 1))
							p.y = tileY + 1;
						else
							p.y = tileY;
					}
				}
				else
				{
					p.x = tileX;
					p.y = tileY;
				}
			}
			else if (mrp.Orientation == X_UniTMX.Orientation.Hexagonal)
			{
				// for some easier calculations, convert wordPoint to pixels
				Vector2 point = new Vector2(worldPoint.x * (float)mrp.TileWidth, -worldPoint.y * (float)mrp.TileHeight);

				// Code almost straight from Tiled's libtiled :P

				int sideLengthX = 0;
				int sideLengthY = 0;

				float sideOffsetX = mrp.TileWidth / 2;
				float sideOffsetY = mrp.TileHeight / 2;

				if (!staggeredInY)
				{
					sideLengthX = mrp.HexSideLength;
					sideOffsetX = (mrp.TileWidth - sideLengthX) / 2.0f;
					point.x -= staggeredEven ? mrp.TileWidth : sideOffsetX;
				}
				else
				{
					sideLengthY = mrp.HexSideLength;
					sideOffsetY = (mrp.TileHeight - sideLengthY) / 2.0f;
					point.y -= staggeredEven ? mrp.TileHeight : sideOffsetY;
				}

				int tileX = 0, tileY = 0;
				Vector2 rel;

				float columnWidth = sideOffsetX + sideLengthX;
				float rowHeight = sideOffsetY + sideLengthY;

				if (staggeredInY)
				{
					// Getting 'grid-aligned' tile index. This grid has HexSideLength separation instead of tile width/height
					tileX = Mathf.FloorToInt(point.x / (float)(mrp.TileWidth + sideLengthX));
					tileY = Mathf.FloorToInt(point.y / (float)(mrp.TileHeight + sideLengthY)) * 2;

					// Relative x and y pos to tile
					rel.x = point.x - tileX * (float)(mrp.TileWidth + sideLengthX);
					rel.y = point.y - tileY / 2.0f * (float)(mrp.TileHeight + sideLengthY);
				}
				else
				{
					// Getting 'grid-aligned' tile index. This grid has HexSideLength separation instead of tile width/height
					tileX = Mathf.FloorToInt(point.x / (float)(mrp.TileWidth + sideLengthX)) * 2;
					tileY = Mathf.FloorToInt(point.y / (float)(mrp.TileHeight + sideLengthY));

					// Relative x and y pos to tile
					rel.x = point.x - tileX / 2.0f * (float)(mrp.TileWidth + sideLengthX);
					rel.y = point.y - tileY * (float)(mrp.TileHeight + sideLengthY);
				}

				Vector2[] centers = new Vector2[4];
				if (staggeredInY)
				{
					float top = sideLengthY / 2.0f;
					float centerX = mrp.TileWidth / 2.0f;
					float centerY = top + rowHeight;

					centers[0].Set(centerX, top);
					centers[1].Set(centerX - columnWidth, centerY);
					centers[2].Set(centerX + columnWidth, centerY);
					centers[3].Set(centerX, centerY + rowHeight);
				}
				else
				{
					float left = sideLengthX / 2.0f;
					float centerX = left + columnWidth;
					float centerY = mrp.TileHeight / 2.0f;

					centers[0].Set(left, centerY);
					centers[1].Set(centerX, centerY - rowHeight);
					centers[2].Set(centerX, centerY + rowHeight);
					centers[3].Set(centerX + columnWidth, centerY);
				}

				int nearest = 0;
				float minDist = Mathf.Infinity;

				for (int i = 0; i < 4; i++)
				{
					float dist = (centers[i] - rel).sqrMagnitude;
					if (dist < minDist)
					{
						minDist = dist;
						nearest = i;
					}
				}

				Vector2[] offsetsStaggerX = {
					Vector2.zero,
					new Vector2(+1, -1),
					new Vector2(+1,  0),
					new Vector2(+2,  0),
				};
				Vector2[] offsetsStaggerY = {
					Vector2.zero,
					new Vector2(-1, +1),
					new Vector2(0,  +1),
					new Vector2(0,  +2),
				};

				Vector2 offsets = staggeredInY ? offsetsStaggerY[nearest] : offsetsStaggerX[nearest];
				p.x = offsets.x + tileX + ((!staggeredInY && staggeredEven) ? 1 : 0);
				p.y = offsets.y + tileY + ((staggeredInY && staggeredEven) ? 1 : 0);
			}

			return p;
		}

		/// <summary>
		/// Converts a point in world space into tile indices that can be used to index into a TileLayer.
		/// </summary>
		/// <param name="worldPoint">The point in world space to convert into tile indices.</param>
		/// <returns>A Point containing the X/Y indices of the tile that contains the point.</returns>
		public static Vector2 WorldPointToTileIndex(this Map map, Vector2 worldPoint)
		{
			Vector2 p = WorldPointToTiledPosition(map, worldPoint);

			p.x = Mathf.FloorToInt(p.x);
			p.y = Mathf.FloorToInt(p.y);
			return p;
		}
		#endregion

		#region Tiled Position to World Position
		/// <summary>
		/// Converts a tile index or position into world coordinates
		/// </summary>
		/// <param name="posX">Tile index or position of object in tiled</param>
		/// <param name="posY">Tile index or position of object in tiled</param>
		/// <param name="tile">Tile to get size from</param>
		/// <returns>World's X and Y position</returns>
		public static Vector2 TiledPositionToWorldPoint(this Map map ,float posX, float posY, Tile tile = null)
		{
			Vector2 p = Vector2.zero;

			MapRenderParameters mrp = map.MapRenderParameter;

			float currentTileWidth = mrp.TileWidth;
			float currentTileHeight = mrp.TileHeight;

			bool staggeredInY = mrp.MapStaggerAxis.Equals(StaggerAxis.Y);
			bool staggeredEven = mrp.MapStaggerIndex.Equals(StaggerIndex.Even);

			//if (tile == null)
			//{
			//	Dictionary<int, Tile>.ValueCollection.Enumerator enumerator = map.Tiles.Values.GetEnumerator();
			//	enumerator.MoveNext();
			//	if (enumerator.Current != null && enumerator.Current.TileSet != null)
			//	{
			//		currentTileWidth = enumerator.Current.TileSet.TileWidth;
			//		currentTileHeight = enumerator.Current.TileSet.TileHeight;
			//	}
			//}
			//else
			if(tile != null)
			{
				if (tile.TileSet != null)
				{
					currentTileWidth = tile.TileSet.TileWidth;
					currentTileHeight = tile.TileSet.TileHeight;
				}
			}

			if (mrp.Orientation == Orientation.Orthogonal)
			{
				p.x = posX * (mrp.TileWidth / currentTileWidth);
				p.y = -posY * (mrp.TileHeight / currentTileHeight) * (currentTileHeight / currentTileWidth);
			}
			else if (mrp.Orientation == Orientation.Isometric)
			{
				p.x = (mrp.TileWidth / 2.0f * (mrp.Width - posY + posX)) / (float)mrp.TileWidth;//(TileWidth / 2.0f * (Width / 2.0f - posY + posX)) / (float)TileWidth;//
				p.y = -mrp.Height + mrp.TileHeight * (mrp.Height - ((posX + posY) / (mrp.TileWidth / (float)mrp.TileHeight)) / 2.0f) / (float)mrp.TileHeight;
			}
			else if (mrp.Orientation == X_UniTMX.Orientation.Staggered)
			{
				if (staggeredInY)
				{
					p.x = posX * (mrp.TileWidth / currentTileWidth);
					if ((!staggeredEven && Mathf.FloorToInt(Mathf.Abs(posY)) % 2 > 0) ||
						(staggeredEven && Mathf.FloorToInt(Mathf.Abs(posY)) % 2 < 1))
						p.x += 0.5f;
					p.y = -posY * (mrp.TileHeight / 2.0f / currentTileHeight) * (currentTileHeight / currentTileWidth);
				}
				else
				{
					p.y = -posY * (mrp.TileHeight / 2.0f / currentTileHeight);
					if ((!staggeredEven && Mathf.FloorToInt(Mathf.Abs(posX)) % 2 > 0) ||
						(staggeredEven && Mathf.FloorToInt(Mathf.Abs(posX)) % 2 < 1))
						p.y -= (mrp.TileHeight / 2.0f / currentTileHeight) * (currentTileHeight / currentTileWidth);

					p.x = posX * (mrp.TileWidth / currentTileWidth) * (currentTileHeight / currentTileWidth);
				}
			}
			else if (mrp.Orientation == X_UniTMX.Orientation.Hexagonal)
			{
				if (staggeredInY)
				{
					float halfGap = (mrp.TileHeight - mrp.HexSideLength) / 2.0f;
					float tileDisplacement = halfGap + mrp.HexSideLength;

					p.x = posX * (mrp.TileWidth / currentTileWidth);
					if ((!staggeredEven && Mathf.FloorToInt(Mathf.Abs(posY)) % 2 > 0) ||
						(staggeredEven && Mathf.FloorToInt(Mathf.Abs(posY)) % 2 < 1))
						p.x += (mrp.TileWidth / currentTileWidth) / 2.0f;

					p.y = -posY * (tileDisplacement / currentTileHeight);
				}
				else
				{
					float halfGap = (mrp.TileWidth - mrp.HexSideLength) / 2.0f;
					float tileDisplacement = halfGap + mrp.HexSideLength;

					p.y = -posY * (mrp.TileHeight / currentTileHeight) * (currentTileHeight / currentTileWidth);
					if ((!staggeredEven && Mathf.FloorToInt(Mathf.Abs(posX)) % 2 > 0) ||
						(staggeredEven && Mathf.FloorToInt(Mathf.Abs(posX)) % 2 < 1))
						p.y -= (mrp.TileHeight / currentTileHeight) / 2.0f * (currentTileHeight / currentTileWidth);

					p.x = posX * (tileDisplacement / currentTileWidth);
				}
			}

			return p;
		}

		/// <summary>
		/// Converts a tile index or position into 3D world coordinates
		/// </summary>
		/// <param name="posX">Tile index or position of object in tiled</param>
		/// <param name="posY">Tile index or position of object in tiled</param>
		/// <param name="posZ">zIndex of object</param>
		/// <param name="tile">Tile to get size from</param>
		/// <returns>World's X, Y and Z position</returns>
		public static Vector3 TiledPositionToWorldPoint(this Map map, float posX, float posY, float posZ, Tile tile = null)
		{
			Vector3 p = new Vector3();

			Vector2 p2d = TiledPositionToWorldPoint(map, posX, posY, tile);
			// No need to change Z value, this function is just a helper
			p.x = p2d.x;
			p.y = p2d.y;
			p.z = posZ;
			return p;
		}

		/// <summary>
		/// Converts a tile index or position into 3D world coordinates
		/// </summary>
		/// <param name="position">Tile index or position of object in Tiled</param>
		/// <param name="tile">Tile to get size from</param>
		/// <returns>World's X and Y position</returns>
		public static Vector2 TiledPositionToWorldPoint(this Map map, Vector2 position, Tile tile = null)
		{
			return TiledPositionToWorldPoint(map, position.x, position.y, tile);
		}
		#endregion
		#endregion

		#region Colliders Generators
		#region Tile Collisions Generator
		public static GameObject[] GenerateTileCollision2DFromLayer(
			this Map map, TileLayer layer,
			bool isTrigger = false, bool generateClosedPolygon = true, string tag = "Untagged",
			int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 1,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			if (layer == null)
				return null;

			if (layer.LayerTileCollisions == null)
			{
				layer.LayerTileCollisions = new GameObject(layer.Name + " Tile Collisions");
				Transform t = layer.LayerTileCollisions.transform;
				if (layer.BaseMap != null)
				{
					t.parent = layer.BaseMap.MapGameObject.transform;
				}
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				layer.LayerTileCollisions.isStatic = true;
			}
			layer.LayerTileCollisions.tag = tag;
			layer.LayerTileCollisions.layer = physicsLayer;

			List<GameObject> newSubCollider = new List<GameObject>();

			List<List<Vector2>> points = GenerateClipperPathPoints(layer, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);

			for (int i = 0; i < points.Count; i++)
			{
				newSubCollider.Add(new GameObject("Tile Collisions " + layer.Name + "_" + i));
				newSubCollider[i].transform.parent = layer.LayerTileCollisions.transform;
				newSubCollider[i].transform.localPosition = new Vector3(0, 0, zDepth);
				newSubCollider[i].transform.localScale = Vector3.one;
				newSubCollider[i].transform.localRotation = Quaternion.identity;
				newSubCollider[i].tag = tag;
				newSubCollider[i].layer = physicsLayer;

				// Add the last point equals to the first to close the collider area
				// it's necessary only if the first point is diffent from the first one
				if (points[i][0].x != points[i][points[i].Count - 1].x || points[i][0].y != points[i][points[i].Count - 1].y)
				{
					points[i].Add(points[i][0]);
				}

				Vector2[] pointsVec = points[i].ToArray();

				for (int j = 0; j < pointsVec.Length; j++)
				{
					pointsVec[j] = map.TiledPositionToWorldPoint(pointsVec[j]);
				}

				if (generateClosedPolygon)
				{
					PolygonCollider2D polyCollider = newSubCollider[i].AddComponent<PolygonCollider2D>();
					polyCollider.isTrigger = isTrigger;
					polyCollider.points = pointsVec;

					if (physicsMaterial != null)
						polyCollider.sharedMaterial = physicsMaterial;
				}
				else
				{
					EdgeCollider2D edgeCollider = newSubCollider[i].AddComponent<EdgeCollider2D>();
					edgeCollider.isTrigger = isTrigger;
					edgeCollider.points = pointsVec;

					if (physicsMaterial != null)
						edgeCollider.sharedMaterial = physicsMaterial;
				}	
			}
			return newSubCollider.ToArray();
		}

		public static GameObject[] GenerateTileCollision2DFromLayer(
			this Map map, string layer,
			bool isTrigger = false, bool generateClosedPolygon = true, string tag = "Untagged",
			int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 1,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			return GenerateTileCollision2DFromLayer(map, map.GetTileLayer(layer), isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial, zDepth, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);
		}

		public static GameObject[] GenerateTileCollision3DFromLayer(
			this Map map, TileLayer layer,
			bool isTrigger = false, bool generateClosedPolygon = true, string tag = "Untagged", 
			int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 1, 
			float colliderWidth = 1, bool innerCollision = false,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			if (layer == null)
				return null;

			if (layer.LayerTileCollisions == null)
			{
				layer.LayerTileCollisions = new GameObject(layer.Name + " Tile Collisions");
				Transform t = layer.LayerTileCollisions.transform;
				if (layer.BaseMap != null)
				{
					t.parent = layer.BaseMap.MapGameObject.transform;
				}
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				layer.LayerTileCollisions.isStatic = true;
			}
			layer.LayerTileCollisions.tag = tag;
			layer.LayerTileCollisions.layer = physicsLayer;
			layer.LayerTileCollisions.transform.localScale = Vector3.one;

			List<GameObject> newSubCollider = new List<GameObject>();

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<List<Vector2>> points = GenerateClipperPathPoints(layer, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);

			for (int i = 0; i < points.Count; i++)
			{
				newSubCollider.Add(new GameObject("Tile Collisions " + layer.Name + "_" + i));
				newSubCollider[i].transform.parent = layer.LayerTileCollisions.transform;
				newSubCollider[i].transform.localPosition = new Vector3(0, 0, zDepth);
				newSubCollider[i].transform.localScale = Vector3.one;
				newSubCollider[i].transform.localRotation = Quaternion.identity;
				newSubCollider[i].tag = tag;
				newSubCollider[i].layer = physicsLayer;

				vertices.Clear();
				triangles.Clear();
				Mesh colliderMesh = new Mesh();
				colliderMesh.name = "TileCollider_" + layer.Name + "_" + i;
				MeshCollider mc = newSubCollider[i].AddComponent<MeshCollider>();

				mc.isTrigger = isTrigger;

				GenerateVerticesAndTris(map, points[i], vertices, triangles, zDepth, colliderWidth, innerCollision, true, true);

				// Connect last point with first point (create the face between them)
				triangles.Add(vertices.Count - 1);
				triangles.Add(1);
				triangles.Add(0);

				triangles.Add(0);
				triangles.Add(vertices.Count - 2);
				triangles.Add(vertices.Count - 1);

				if (generateClosedPolygon)
					FillFaces(points[i], triangles);

				colliderMesh.vertices = vertices.ToArray();
				colliderMesh.uv = new Vector2[colliderMesh.vertices.Length];
				//colliderMesh.uv1 = colliderMesh.uv;
				colliderMesh.uv2 = colliderMesh.uv;
				colliderMesh.triangles = triangles.ToArray();
				colliderMesh.RecalculateNormals();

				mc.sharedMesh = colliderMesh;

				if (physicsMaterial != null)
					mc.sharedMaterial = physicsMaterial;

				newSubCollider[i].isStatic = true;
			}

			return newSubCollider.ToArray();
		}

		public static GameObject[] GenerateTileCollision3DFromLayer(
			this Map map, string layer,
			bool isTrigger = false, bool generateClosedPolygon = true, string tag = "Untagged",
			int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 1,
			float colliderWidth = 1, bool innerCollision = false,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			return GenerateTileCollision3DFromLayer(map, map.GetTileLayer(layer), isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial, zDepth, colliderWidth, innerCollision, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);
		}

		public static List<List<Vector2>> GenerateClipperPathPoints(TileLayer tileLayer,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			ClipperLib.Clipper clipper = new ClipperLib.Clipper();
			List<List<ClipperLib.IntPoint>> pathsList = new List<List<ClipperLib.IntPoint>>();
			List<List<ClipperLib.IntPoint>> solution = new List<List<ClipperLib.IntPoint>>();
			List<List<Vector2>> points = new List<List<Vector2>>();

			for (int x = 0; x < tileLayer.Tiles.Width; x++)
			{
				for (int y = 0; y < tileLayer.Tiles.Height; y++)
				{
					Tile t = tileLayer.Tiles[x, y];
					if (t == null || t.TileSet == null || t.TileSet.TilesObjects == null)
						continue;
					if (t.TileSet.TilesObjects.ContainsKey(t.OriginalID))
					{
						List<TileObject> tileObjs = t.TileSet.TilesObjects[t.OriginalID];
						foreach (var tileObj in tileObjs)
						{
							pathsList.Add(tileObj.GetPath(x, y, t.SpriteEffects, tileLayer.BaseMap.MapRenderParameter.TileWidth, tileLayer.BaseMap.MapRenderParameter.TileHeight));
						}
					}
				}
			}
			// Add the paths to be merged to ClipperLib
			clipper.AddPaths(pathsList, ClipperLib.PolyType.ptSubject, true);
			// Merge it!
			//clipper.PreserveCollinear = false;
			//clipper.ReverseSolution = true;
			clipper.StrictlySimple = simpleTileObjectCalculation;
			if (!clipper.Execute(ClipperLib.ClipType.ctUnion, solution))
				return points;
			clipper.Execute(ClipperLib.ClipType.ctUnion, solution);
			// Now solution should contain all vertices of the collision object, but they are still multiplied by TileObject.ClipperScale!

			#region Implementation of increase and decrease offset polygon.
			if (simpleTileObjectCalculation == false)
			{
				// Link of the example of ClipperLib:
				// http://www.angusj.com/delphi/clipper/documentation/Docs/Units/ClipperLib/Classes/ClipperOffset/_Body.htm

				ClipperLib.ClipperOffset co = new ClipperLib.ClipperOffset(clipperMiterLimit, clipperArcTolerance);
				foreach (List<ClipperLib.IntPoint> item in solution)
				{
					co.AddPath(item, clipperJoinType, clipperEndType);
				}
				solution.Clear();
				co.Execute(ref solution, clipperDeltaOffset * TileObject.ClipperScale);
			}
			#endregion

			for (int i = 0; i < solution.Count; i++)
			{
				if (solution[i].Count < 1)
					continue;
				points.Add(new List<Vector2>());
				for (int j = 0; j < solution[i].Count; j++)
				{
					points[i].Add(
						new Vector2(
							solution[i][j].X / (float)TileObject.ClipperScale,
							solution[i][j].Y / (float)TileObject.ClipperScale
						)
					);
				}
			}

			return points;
		}

		/// <summary>
		/// Generate Colliders based on Tile Collisions
		/// </summary>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="generateClosedPolygon">True to generate a Polygon Collider. False will generate Edge Collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="physicsMaterial3D">Physics Material for 3D collider</param>
		/// <param name="physicsMaterial2D">Physics Material for 2D collider</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="simpleTileObjectCalculation">true to generate simplified tile collisions</param>
		/// <param name="clipperArcTolerance">Clipper arc angle tolerance</param>
		/// <param name="clipperMiterLimit">Clipper limit for Miter join type</param>
		/// <param name="clipperJoinType">Clipper join type</param>
		/// <param name="clipperEndType">Clipper Polygon end type</param>
		/// <param name="clipperDeltaOffset">Clipper delta offset</param>
		/// <returns>A GameObject containing all generated mapObjects</returns>
		public static GameObject[] GenerateTileCollisions(this Map map, bool used2DColider = true, bool isTrigger = false, bool generateClosedPolygon = true,
			string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial3D = null, PhysicsMaterial2D physicsMaterial2D = null,
			float zDepth = 0, float colliderWidth = 1, bool innerCollision = false,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			if (used2DColider)
				return GenerateTileCollisions2D(map, isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial2D, zDepth, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);
			else
				return GenerateTileCollisions3D(map, isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial3D, zDepth, colliderWidth, innerCollision, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset);
		}

		/// <summary>
		/// Generate 3D Colliders based on Tile Collisions
		/// </summary>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="generateClosedPolygon">True to generate a Polygon Collider. False will generate Edge Collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="physicsMaterial3D">Physics Material for 3D collider</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="simpleTileObjectCalculation">true to generate simplified tile collisions</param>
		/// <param name="clipperArcTolerance">Clipper arc angle tolerance</param>
		/// <param name="clipperMiterLimit">Clipper limit for Miter join type</param>
		/// <param name="clipperJoinType">Clipper join type</param>
		/// <param name="clipperEndType">Clipper Polygon end type</param>
		/// <param name="clipperDeltaOffset">Clipper delta offset</param>
		/// <returns></returns>
		public static GameObject[] GenerateTileCollisions3D(this Map map, bool isTrigger = false, bool generateClosedPolygon = true,
			string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial3D = null,
			float zDepth = 0, float colliderWidth = 1, bool innerCollision = false,
			bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			List<GameObject> tileCollisions = new List<GameObject>();
			// Iterate over each Tile Layer, grab all TileObjects inside this layer and use their Paths with ClipperLib to generate one polygon collider
			foreach (var layer in map.Layers)
			{
				if (layer is TileLayer)
				{
					tileCollisions.AddRange(GenerateTileCollision3DFromLayer(map, layer as TileLayer, isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial3D, zDepth, colliderWidth, innerCollision, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset));
				}
			}

			return tileCollisions.ToArray();
		}

		/// <summary>
		/// Generate 2D Colliders based on Tile Collisions
		/// </summary>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="generateClosedPolygon">True to generate a Polygon Collider. False will generate Edge Collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="physicsMaterial2D">Physics Material for 2D collider</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="simpleTileObjectCalculation">true to generate simplified tile collisions</param>
		/// <param name="clipperArcTolerance">Clipper arc angle tolerance</param>
		/// <param name="clipperMiterLimit">Clipper limit for Miter join type</param>
		/// <param name="clipperJoinType">Clipper join type</param>
		/// <param name="clipperEndType">Clipper Polygon end type</param>
		/// <param name="clipperDeltaOffset">Clipper delta offset</param>
		/// <returns></returns>
		public static GameObject[] GenerateTileCollisions2D(this Map map, bool isTrigger = false, bool generateClosedPolygon = true,
			string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial2D = null,
			float zDepth = 0, bool simpleTileObjectCalculation = true,
			double clipperArcTolerance = 0.25, double clipperMiterLimit = 2.0,
			ClipperLib.JoinType clipperJoinType = ClipperLib.JoinType.jtRound,
			ClipperLib.EndType clipperEndType = ClipperLib.EndType.etClosedPolygon,
			float clipperDeltaOffset = 0)
		{
			List<GameObject> tileCollisions = new List<GameObject>();
			// Iterate over each Tile Layer, grab all TileObjects inside this layer and use their Paths with ClipperLib to generate one polygon collider
			foreach (var layer in map.Layers)
			{
				if (layer is TileLayer)
				{
					tileCollisions.AddRange(GenerateTileCollision2DFromLayer(map, layer as TileLayer, isTrigger, generateClosedPolygon, tag, physicsLayer, physicsMaterial2D, zDepth, simpleTileObjectCalculation, clipperArcTolerance, clipperMiterLimit, clipperJoinType, clipperEndType, clipperDeltaOffset));
				}
			}

			return tileCollisions.ToArray();
		}
		#endregion

		#region Box Colliders
		/// <summary>
		/// Adds a 3D BoxCollider to a GameObject
		/// </summary>
		/// <param name="gameObject">GameObject to add the collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="physicsMaterial">PhysicMaterial to be set to the collider</param>
		/// <param name="zDepth">Z Depth of the collider</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddBoxCollider3D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject gameObjectMesh = null;
			// Orthogonal and Staggered maps can use BoxCollider, Isometric maps must use polygon collider
			if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric)
			{
				BoxCollider boxCollider = null;
				if (obj.GetPropertyAsBoolean(Map.Property_CreateMesh))
				{
					gameObjectMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
					gameObjectMesh.name = obj.Name;
					gameObjectMesh.transform.parent = gameObject.transform;
					gameObjectMesh.transform.localPosition = new Vector3(0.5f, -0.5f);
					boxCollider = gameObjectMesh.GetComponent<BoxCollider>();
				}
				else
				{
					boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.center = new Vector3(0.5f, -0.5f);
				}
				boxCollider.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

				if (physicsMaterial != null)
					boxCollider.sharedMaterial = physicsMaterial;

				gameObject.transform.localScale = new Vector3(obj.Bounds.width, obj.Bounds.height, colliderWidth);
			}
			else
			{
				List<Vector2> points = new List<Vector2>();
				points.Add(new Vector2(obj.Bounds.xMin - obj.Bounds.x, obj.Bounds.yMax - obj.Bounds.y));
				points.Add(new Vector2(obj.Bounds.xMin - obj.Bounds.x, obj.Bounds.yMin - obj.Bounds.y));
				points.Add(new Vector2(obj.Bounds.xMax - obj.Bounds.x, obj.Bounds.yMin - obj.Bounds.y));
				points.Add(new Vector2(obj.Bounds.xMax - obj.Bounds.x, obj.Bounds.yMax - obj.Bounds.y));
				X_UniTMX.MapObject isoBox = new MapObject(obj.Name, obj.Type, obj.Bounds, obj.Properties, obj.GID, points, obj.Rotation, obj.ParentObjectLayer);

				AddPolygonCollider3D(map, gameObject, isoBox, isTrigger, physicsMaterial, zDepth, colliderWidth);
				//gameObject = GeneratePolygonCollider3D(isoBox, isTrigger, zDepth, colliderWidth);
			}

			if (createRigidbody)
			{
				Rigidbody r = gameObject.AddComponent<Rigidbody>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			if (gameObjectMesh != null)
				ApplyCustomProperties(gameObjectMesh, obj);
			else
				ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		/// <summary>
		/// Creates a 3D BoxCollider
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="tag">Tag for the generated GameObject</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObject</param>
		/// <param name="physicsMaterial">PhysicMaterial to be set to the collider</param>
		/// <param name="zDepth">Z Depth of the collider</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider</returns>
		public static GameObject GenerateBoxCollider3D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			if (obj.ParentObjectLayer.LayerGameObject != null)
			{
				newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			}
			else
			{
				newCollider.transform.parent = map.MapGameObject.transform;
			}
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddBoxCollider3D(map, newCollider, obj, isTrigger, physicsMaterial, 0, colliderWidth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		/// <summary>
		/// Adds a BoxCollider2D to a GameObject
		/// </summary>
		/// <param name="gameObject">GameObject to add the collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="physicsMaterial">PhysicsMaterial2D to be set to the collider</param>
		/// <param name="zDepth">Z Depth of the collider</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddBoxCollider2D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			// Orthogonal and Staggered maps can use BoxCollider, Isometric maps must use polygon collider
			if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric)
			{
				BoxCollider2D bx = gameObject.AddComponent<BoxCollider2D>();
				bx.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);
#if UNITY_5
				bx.offset = new Vector2(obj.Bounds.width / 2.0f, -obj.Bounds.height / 2.0f);
#else
				bx.center = new Vector2(obj.Bounds.width / 2.0f, -obj.Bounds.height / 2.0f);
#endif
				bx.size = new Vector2(obj.Bounds.width, obj.Bounds.height);
				if (physicsMaterial != null)
					bx.sharedMaterial = physicsMaterial;
			}
			else if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Isometric)
			{
				PolygonCollider2D pc = gameObject.AddComponent<PolygonCollider2D>();
				pc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);
				Vector2[] points = new Vector2[4];
				points[0] = map.TiledPositionToWorldPoint(obj.Bounds.xMin - obj.Bounds.x, obj.Bounds.yMax - obj.Bounds.y);
				points[1] = map.TiledPositionToWorldPoint(obj.Bounds.xMin - obj.Bounds.x, obj.Bounds.yMin - obj.Bounds.y);
				points[2] = map.TiledPositionToWorldPoint(obj.Bounds.xMax - obj.Bounds.x, obj.Bounds.yMin - obj.Bounds.y);
				points[3] = map.TiledPositionToWorldPoint(obj.Bounds.xMax - obj.Bounds.x, obj.Bounds.yMax - obj.Bounds.y);
				points[0].x -= map.MapRenderParameter.Width / 2.0f;
				points[1].x -= map.MapRenderParameter.Width / 2.0f;
				points[2].x -= map.MapRenderParameter.Width / 2.0f;
				points[3].x -= map.MapRenderParameter.Width / 2.0f;
				pc.SetPath(0, points);
				if (physicsMaterial != null)
					pc.sharedMaterial = physicsMaterial;
			}

			if (createRigidbody)
			{
				Rigidbody2D r = gameObject.AddComponent<Rigidbody2D>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="tag">Tag for the generated GameObject</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObject</param>
		/// <param name="physicsMaterial">PhysicsMaterial2D to be set to the collider</param>
		/// <param name="zDepth">Z Depth of the collider</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider</returns>
		public static GameObject GenerateBoxCollider2D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddBoxCollider2D(map, newCollider, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;

		}

		/// <summary>
		/// Generate a Box collider mesh for 3D, or a BoxCollider2D for 2D (a PolygonCollider2D will be created for Isometric maps).
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider.</returns>
		public static GameObject GenerateBoxCollider(this Map map, MapObject obj, bool used2DColider = true, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			return used2DColider ?
				map.GenerateBoxCollider2D(obj, isTrigger, tag, physicsLayer, null, zDepth, createRigidbody, rigidbodyIsKinematic) :
				map.GenerateBoxCollider3D(obj, isTrigger, tag, physicsLayer, null, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
		}

		/// <summary>
		/// Adds a Box Collider 2D or 3D to an existing GameObject using one MapObject as properties source
		/// </summary>
		/// <param name="gameObject">GameObject to add a Box Collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddBoxCollider(Map map, GameObject gameObject, MapObject obj, bool used2DColider = true, bool isTrigger = false, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				AddBoxCollider2D(map, gameObject, obj, isTrigger, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				AddBoxCollider3D(map, gameObject, obj, isTrigger, null, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
		}
		#endregion

		#region Ellipse/Circle/Capsule Colliders
		private static void ApproximateEllipse2D(Map map, GameObject newCollider, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			// since there's no "EllipseCollider2D", we must create one by approximating a polygon collider
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);

			PolygonCollider2D polygonCollider = newCollider.AddComponent<PolygonCollider2D>();

			polygonCollider.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			int segments = XUniTMXConfiguration.Instance.EllipsoideColliderApproximationFactor;

			// Segments per quadrant
			int incFactor = Mathf.FloorToInt(segments / 4.0f);
			float minIncrement = 2 * Mathf.PI / (incFactor * segments / 2.0f);
			int currentInc = 0;
			// grow represents if we are going right on x-axis (true) or left (false)
			bool grow = true;

			Vector2[] points = new Vector2[segments];
			// Ellipsoide center
			Vector2 center = new Vector2(obj.Bounds.width / 2.0f, obj.Bounds.height / 2.0f);

			float r = 0;
			float angle = 0;
			for (int i = 0; i < segments; i++)
			{
				// Calculate radius at each point
				angle += currentInc * minIncrement;

				r = obj.Bounds.width * obj.Bounds.height / Mathf.Sqrt(Mathf.Pow(obj.Bounds.height * Mathf.Cos(angle), 2) + Mathf.Pow(obj.Bounds.width * Mathf.Sin(angle), 2)) / 2.0f;
				// Define the point localization using the calculated radius, angle and center
				points[i] = r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) + center;

				points[i] = map.TiledPositionToWorldPoint(points[i].x, points[i].y);

				// Offset points where needed
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Isometric)
					points[i].x -= map.MapRenderParameter.Width / 2.0f;
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Staggered)
					points[i].y *= map.MapRenderParameter.TileWidth / (float)map.MapRenderParameter.TileHeight * 2.0f;
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Hexagonal)
				{
					points[i].y *= map.MapRenderParameter.TileWidth / (float)map.MapRenderParameter.TileHeight * 2.0f;
				}

				// if we are "growing", increment the angle, else, start decrementing it to close the polygon
				if (grow)
					currentInc++;
				else
					currentInc--;
				if (currentInc > incFactor - 1 || currentInc < 1)
					grow = !grow;

				// POG :P -> Orthogonal and Staggered Isometric generated points are slightly offset on Y
				if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric)
				{
					if (i < 1 || i == segments / 2 - 1)
						points[i].y -= obj.Bounds.height / 20.0f;
					if (i >= segments - 1 || i == segments / 2)
						points[i].y += obj.Bounds.height / 20.0f;
				}
			}

			polygonCollider.SetPath(0, points);

			if (physicsMaterial != null)
				polygonCollider.sharedMaterial = physicsMaterial;
		}

		private static void ApproximateEllipse3D(Map map, GameObject newCollider, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			// since there's no "EllipseCollider", we must create one by approximating a polygon collider
			//newCollider.transform.localPosition = TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);

			Mesh colliderMesh = new Mesh();
			colliderMesh.name = "Collider_" + obj.Name;
			MeshCollider mc = newCollider.AddComponent<MeshCollider>();
			mc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			int segments = XUniTMXConfiguration.Instance.EllipsoideColliderApproximationFactor;

			// Segments per quadrant
			int incFactor = Mathf.FloorToInt(segments / 4.0f);
			float minIncrement = 2 * Mathf.PI / (incFactor * segments / 2.0f);
			int currentInc = 0;
			bool grow = true;

			Vector2[] points = new Vector2[segments];

			float width = obj.Bounds.width;
			float height = obj.Bounds.height;

			Vector2 center = new Vector2(width / 2.0f, height / 2.0f);

			float r = 0;
			float angle = 0;
			for (int i = 0; i < segments; i++)
			{
				// Calculate radius at each point
				//angle = i * increment;
				angle += currentInc * minIncrement;
				r = width * height / Mathf.Sqrt(Mathf.Pow(height * Mathf.Cos(angle), 2) + Mathf.Pow(width * Mathf.Sin(angle), 2)) / 2.0f;
				points[i] = r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) + center;
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Staggered)
					points[i].y *= -1;

				if (grow)
					currentInc++;
				else
					currentInc--;
				if (currentInc > incFactor - 1 || currentInc < 1)
					grow = !grow;

				// POG :P
				if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric)
				{
					if (i < 1 || i == segments / 2 - 1)
						points[i].y += obj.Bounds.height / 20.0f;
					if (i >= segments - 1 || i == segments / 2)
						points[i].y -= obj.Bounds.height / 20.0f;
				}
			}

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			GenerateVerticesAndTris(map, new List<Vector2>(points), vertices, triangles, zDepth, colliderWidth, false, !(map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Staggered));

			// Connect last point with first point (create the face between them)
			triangles.Add(vertices.Count - 1);
			triangles.Add(1);
			triangles.Add(0);

			triangles.Add(0);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);

			FillFaces(points, triangles);

			colliderMesh.vertices = vertices.ToArray();
			colliderMesh.uv = new Vector2[colliderMesh.vertices.Length];
			//colliderMesh.uv1 = colliderMesh.uv;
			colliderMesh.uv2 = colliderMesh.uv;
			colliderMesh.triangles = triangles.ToArray();
			colliderMesh.RecalculateNormals();

			mc.sharedMesh = colliderMesh;

			if (physicsMaterial != null)
				mc.sharedMaterial = physicsMaterial;
		}

		public static void AddEllipseCollider3D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject gameObjectMesh = null;
			if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric && obj.Bounds.width == obj.Bounds.height)
			{
				CapsuleCollider cc = null;
				if (obj.GetPropertyAsBoolean(Map.Property_CreateMesh))
				{
					gameObjectMesh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
					gameObjectMesh.name = obj.Name;
					gameObjectMesh.transform.parent = gameObject.transform;
					gameObjectMesh.transform.localPosition = new Vector3(obj.Bounds.height / 2.0f, -obj.Bounds.width / 2.0f);

					cc = gameObjectMesh.GetComponent<CapsuleCollider>();
					cc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);
					gameObjectMesh.transform.localScale = new Vector3(obj.Bounds.width, colliderWidth, obj.Bounds.height);
					gameObjectMesh.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
				}
				else
				{
					cc = gameObject.AddComponent<CapsuleCollider>();

					cc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

					cc.center = new Vector3(obj.Bounds.height / 2.0f, -obj.Bounds.width / 2.0f);

					cc.direction = 0;
					cc.radius = obj.Bounds.height / 2.0f;
					cc.height = obj.Bounds.width;
				}
				if (physicsMaterial != null)
					cc.sharedMaterial = physicsMaterial;
			}
			else
			{
				ApproximateEllipse3D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
			}

			if (createRigidbody)
			{
				Rigidbody r = gameObject.AddComponent<Rigidbody>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			if (gameObjectMesh)
				ApplyCustomProperties(gameObjectMesh, obj);
			else
				ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GenerateEllipseCollider3D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddEllipseCollider3D(map, newCollider, obj, isTrigger, physicsMaterial, 0, colliderWidth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		public static void AddEllipseCollider2D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (map.MapRenderParameter.Orientation != X_UniTMX.Orientation.Isometric && obj.Bounds.width == obj.Bounds.height)
			{
				CircleCollider2D cc = gameObject.AddComponent<CircleCollider2D>();
				cc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

				gameObject.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
#if UNITY_5
				cc.offset = new Vector2(obj.Bounds.width / 2.0f, -obj.Bounds.height / 2.0f);
#else
				cc.center = new Vector2(obj.Bounds.width / 2.0f, -obj.Bounds.height / 2.0f);
#endif

				cc.radius = obj.Bounds.width / 2.0f;
				if (physicsMaterial != null)
					cc.sharedMaterial = physicsMaterial;

			}
			else
			{
				ApproximateEllipse2D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
			}


			if (createRigidbody)
			{
				Rigidbody2D r = gameObject.AddComponent<Rigidbody2D>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GenerateEllipseCollider2D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddEllipseCollider2D(map, newCollider, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		/// <summary>
		/// Generate an Ellipse Collider mesh.
		/// To mimic Tiled's Ellipse Object properties, a Capsule collider is created if map projection is Orthogonal and ellipse inside Tiled is a circle.
		/// For 2D, a CircleCollider2D will be created if ellipse is a circle, else a PolygonCollider will be approximated to an ellipsoid, for 3D, a PolygonCollider mesh will be approximated to an ellipsoid
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider.</returns>
		public static GameObject GenerateEllipseCollider(this Map map, MapObject obj, bool used2DColider = true, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			return used2DColider ?
				map.GenerateEllipseCollider2D(obj, isTrigger, tag, physicsLayer, null, zDepth, createRigidbody, rigidbodyIsKinematic) :
				map.GenerateEllipseCollider3D(obj, isTrigger, tag, physicsLayer, null, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
		}

		/// <summary>
		/// Adds an Ellipse Collider to an existing GameObject using one MapObject as properties source
		/// </summary>
		/// <param name="gameObject">GameObject to add an Ellipse Collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddEllipseCollider(Map map, GameObject gameObject, MapObject obj, bool used2DColider = true, bool isTrigger = false, float zDepth = 0, float colliderWidth = 1.0f, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				AddEllipseCollider2D(map, gameObject, obj, isTrigger, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				AddEllipseCollider3D(map, gameObject, obj, isTrigger, null, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
		}

		#endregion

		#region 3D Helpers
		private static void CreateFrontBackPoints(Map map, Vector3 refPoint, out Vector3 refFront, out Vector3 refBack, float zDepth, float colliderWidth, bool calculateWorldPos = true, bool ignoreOrientation = false)
		{
			if (calculateWorldPos)
			{
				refFront = map.TiledPositionToWorldPoint(refPoint.x, refPoint.y, zDepth - colliderWidth / 2.0f);
				refBack = map.TiledPositionToWorldPoint(refPoint.x, refPoint.y, zDepth + colliderWidth / 2.0f);
			}
			else
			{
				refFront = new Vector3(refPoint.x, refPoint.y, zDepth - colliderWidth / 2.0f);
				refBack = new Vector3(refPoint.x, refPoint.y, zDepth + colliderWidth / 2.0f);
			}
			if (!ignoreOrientation && map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Isometric)
			{
				refFront.x -= map.MapRenderParameter.Width / 2.0f;
				refBack.x -= map.MapRenderParameter.Width / 2.0f;
			}
		}

		private static void GenerateVerticesAndTris(Map map, List<Vector2> points, List<Vector3> generatedVertices, List<int> generatedTriangles, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool calculateWorldPos = true, bool ignoreOrientation = false)
		{
			Vector3 firstPoint = (Vector3)points[0];
			Vector3
				firstFront = Vector3.zero,
				firstBack = Vector3.zero,
				secondPoint = Vector3.zero,
				secondFront = Vector3.zero,
				secondBack = Vector3.zero;

			CreateFrontBackPoints(map, firstPoint, out firstFront, out firstBack, zDepth, colliderWidth, calculateWorldPos, ignoreOrientation);

			if (innerCollision)
			{
				generatedVertices.Add(firstBack); // 3
				generatedVertices.Add(firstFront); // 2
			}
			else
			{
				generatedVertices.Add(firstFront); // 3
				generatedVertices.Add(firstBack); // 2
			}

			// Calculate line planes
			for (int i = 1; i < points.Count; i++)
			{
				secondPoint = (Vector3)points[i];
				CreateFrontBackPoints(map, secondPoint, out secondFront, out secondBack, zDepth, colliderWidth, calculateWorldPos, ignoreOrientation);

				if (innerCollision)
				{
					generatedVertices.Add(secondBack); // 1
					generatedVertices.Add(secondFront); // 0
				}
				else
				{
					generatedVertices.Add(secondFront); // 1
					generatedVertices.Add(secondBack); // 0
				}

				generatedTriangles.Add((i - 1) * 2 + 3);
				generatedTriangles.Add((i - 1) * 2 + 2);
				generatedTriangles.Add((i - 1) * 2 + 0);

				generatedTriangles.Add((i - 1) * 2 + 0);
				generatedTriangles.Add((i - 1) * 2 + 1);
				generatedTriangles.Add((i - 1) * 2 + 3);

				firstPoint = secondPoint;
				firstFront = secondFront;
				firstBack = secondBack;
			}
		}

		private static void FillFaces(List<Vector2> points, List<int> generatedTriangles)
		{
			FillFaces(points.ToArray(), generatedTriangles);
		}

		private static void FillFaces(Vector2[] points, List<int> generatedTriangles)
		{
			// First we pass to the algorithm the object points
			Triangulator tr = new Triangulator(points);
			int[] indices = tr.Triangulate();
			// now, indices[] contains the vertices in a triangulated order, but the mesh has 2 vertices per indice[] (front and back)
			// so we must iterate this list and add front and back triangles accordingly
			// we get each triangle from indices[] and add to triangles list the corrected indices based on vertices list
			for (int i = 0; i < indices.Length; i += 3)
			{
				generatedTriangles.Add(indices[i + 2] * 2);
				generatedTriangles.Add(indices[i + 1] * 2);
				generatedTriangles.Add(indices[i] * 2);

				generatedTriangles.Add(indices[i] * 2 + 1);
				generatedTriangles.Add(indices[i + 1] * 2 + 1);
				generatedTriangles.Add(indices[i + 2] * 2 + 1);
			}
		}
		#endregion

		#region Polyline Colliders
		public static void AddPolylineCollider3D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			Mesh colliderMesh = new Mesh();
			colliderMesh.name = "Collider_" + obj.Name;
			MeshCollider mc = gameObject.AddComponent<MeshCollider>();

			mc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			GenerateVerticesAndTris(map, obj.Points, vertices, triangles, zDepth, colliderWidth, innerCollision);

			colliderMesh.vertices = vertices.ToArray();
			colliderMesh.uv = new Vector2[colliderMesh.vertices.Length];
			//colliderMesh.uv1 = colliderMesh.uv;
			colliderMesh.uv2 = colliderMesh.uv;
			colliderMesh.triangles = triangles.ToArray();
			colliderMesh.RecalculateNormals();

			mc.sharedMesh = colliderMesh;

			if (physicsMaterial != null)
				mc.sharedMaterial = physicsMaterial;

			if (createRigidbody)
			{
				Rigidbody r = gameObject.AddComponent<Rigidbody>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			if (obj.GetPropertyAsBoolean(Map.Property_CreateMesh))
			{
				if (gameObject.GetComponent<MeshFilter>() == null)
					gameObject.AddComponent<MeshFilter>();

				if (gameObject.GetComponent<MeshRenderer>() == null)
					gameObject.AddComponent<MeshRenderer>();

				MeshFilter _meshFilter = gameObject.GetComponent<MeshFilter>();
				if (mc != null)
				{
					mc.sharedMesh.RecalculateBounds();
					mc.sharedMesh.RecalculateNormals();
					MathfExtensions.CalculateMeshTangents(mc.sharedMesh);
					_meshFilter.sharedMesh = mc.sharedMesh;
				}
			}
			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GeneratePolylineCollider3D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddPolylineCollider3D(map, newCollider, obj, isTrigger, physicsMaterial, 0, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);
			return newCollider;
		}

		public static void AddPolylineCollider2D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

			edgeCollider.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			Vector2[] points = obj.Points.ToArray();

			for (int i = 0; i < points.Length; i++)
			{
				points[i] = map.TiledPositionToWorldPoint(points[i].x, points[i].y);
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Isometric)
					points[i].x -= map.MapRenderParameter.Width / 2.0f;
			}

			edgeCollider.points = points;
			if (physicsMaterial != null)
				edgeCollider.sharedMaterial = physicsMaterial;

			if (createRigidbody)
			{
				Rigidbody2D r = gameObject.AddComponent<Rigidbody2D>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GeneratePolylineCollider2D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddPolylineCollider2D(map, newCollider, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		/// <summary>
		/// Generate a Polyline collider mesh, or a sequence of EdgeCollider2D for 2D collisions.
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider.</returns>
		public static GameObject GeneratePolylineCollider(this Map map, MapObject obj, bool used2DColider = true, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			return used2DColider ?
				map.GeneratePolylineCollider2D(obj, isTrigger, tag, physicsLayer, null, zDepth, createRigidbody, rigidbodyIsKinematic) :
				map.GeneratePolylineCollider3D(obj, isTrigger, tag, physicsLayer, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}

		/// <summary>
		/// Adds a Polyline collider mesh, or a sequence of EdgeCollider2D for 2D collisions, to an existing GameObject using one MapObject as properties source
		/// </summary>
		/// <param name="gameObject">GameObject to add a Polyline Collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddPolylineCollider(Map map, GameObject gameObject, MapObject obj, bool used2DColider = true, bool isTrigger = false, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				AddPolylineCollider2D(map, gameObject, obj, isTrigger, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				AddPolylineCollider3D(map, gameObject, obj, isTrigger, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}
		#endregion

		#region Polygon Colliders
		public static void AddPolygonCollider2D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();

			polygonCollider.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			Vector2[] points = obj.Points.ToArray();

			for (int i = 0; i < points.Length; i++)
			{
				points[i] = map.TiledPositionToWorldPoint(points[i].x, points[i].y);
				if (map.MapRenderParameter.Orientation == X_UniTMX.Orientation.Isometric)
					points[i].x -= map.MapRenderParameter.Width / 2.0f;
			}

			polygonCollider.SetPath(0, points);

			if (physicsMaterial != null)
				polygonCollider.sharedMaterial = physicsMaterial;

			if (createRigidbody)
			{
				Rigidbody2D r = gameObject.AddComponent<Rigidbody2D>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);

			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GeneratePolygonCollider2D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddPolygonCollider2D(map, newCollider, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		public static void AddPolygonCollider3D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			Mesh colliderMesh = new Mesh();
			colliderMesh.name = "Collider_" + obj.Name;
			MeshCollider mc = gameObject.AddComponent<MeshCollider>();

			mc.isTrigger = isTrigger || obj.Type.Equals(Map.Object_Type_Trigger);

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			GenerateVerticesAndTris(map, obj.Points, vertices, triangles, zDepth, colliderWidth, innerCollision);

			// Connect last point with first point (create the face between them)
			triangles.Add(vertices.Count - 1);
			triangles.Add(1);
			triangles.Add(0);

			triangles.Add(0);
			triangles.Add(vertices.Count - 2);
			triangles.Add(vertices.Count - 1);

			// Fill Faces
			FillFaces(obj.Points, triangles);

			colliderMesh.vertices = vertices.ToArray();
			colliderMesh.uv = new Vector2[colliderMesh.vertices.Length];
			//colliderMesh.uv1 = colliderMesh.uv;
			colliderMesh.uv2 = colliderMesh.uv;
			colliderMesh.triangles = triangles.ToArray();
			colliderMesh.RecalculateNormals();

			mc.sharedMesh = colliderMesh;

			if (physicsMaterial != null)
				mc.sharedMaterial = physicsMaterial;

			if (createRigidbody)
			{
				Rigidbody r = gameObject.AddComponent<Rigidbody>();
				r.isKinematic = rigidbodyIsKinematic;
			}

			if (obj.Rotation != 0)
			{
				gameObject.transform.localRotation = Quaternion.AngleAxis(obj.Rotation, Vector3.forward);
			}

			if (obj.GetPropertyAsBoolean(Map.Property_CreateMesh))
			{
				if (gameObject.GetComponent<MeshFilter>() == null)
					gameObject.AddComponent<MeshFilter>();

				if (gameObject.GetComponent<MeshRenderer>() == null)
					gameObject.AddComponent<MeshRenderer>();

				MeshFilter _meshFilter = gameObject.GetComponent<MeshFilter>();
				if (mc != null)
				{
					mc.sharedMesh.RecalculateBounds();
					mc.sharedMesh.RecalculateNormals();
					MathfExtensions.CalculateMeshTangents(mc.sharedMesh);
					_meshFilter.sharedMesh = mc.sharedMesh;
				}
			}

			ApplyCustomProperties(gameObject, obj);

			// Link this collider to the MapObject
			obj.LinkedGameObject = gameObject;
		}

		public static GameObject GeneratePolygonCollider3D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject newCollider = new GameObject(obj.Name);
			newCollider.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
			newCollider.transform.localPosition = map.TiledPositionToWorldPoint(obj.Bounds.x, obj.Bounds.y, zDepth);
			newCollider.transform.localScale = Vector3.one;
			newCollider.transform.localRotation = Quaternion.identity;
			newCollider.tag = tag;
			newCollider.layer = physicsLayer;

			AddPolygonCollider3D(map, newCollider, obj, isTrigger, physicsMaterial, 0, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);

			newCollider.isStatic = true;
			newCollider.SetActive(obj.Visible);

			return newCollider;
		}

		/// <summary>
		/// Generate a Polygon collider mesh, or a PolygonCollider2D for 2D collisions.
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider.</returns>
		public static GameObject GeneratePolygonCollider(this Map map, MapObject obj, bool used2DColider = true, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			return used2DColider ?
				map.GeneratePolygonCollider2D(obj, isTrigger, tag, physicsLayer, null, zDepth, createRigidbody, rigidbodyIsKinematic) :
				map.GeneratePolygonCollider3D(obj, isTrigger, tag, physicsLayer, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}

		/// <summary>
		/// Adds a Polygon collider mesh, or a PolygonCollider2D for 2D collisions, to an existing GameObject using one MapObject as properties source
		/// </summary>
		/// <param name="gameObject">GameObject to add a Polygon Collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="isTrigger">True for Trigger Collider, false otherwise</param>
		/// <param name="zDepth">Z Depth of the collider.</param>
		/// <param name="colliderWidth">Width of the collider, in Units</param>
		/// <param name="used2DColider">True to generate a 2D collider, false to generate a 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddPolygonCollider(Map map, GameObject gameObject, MapObject obj, bool used2DColider = true, bool isTrigger = false, float zDepth = 0, float colliderWidth = 1.0f, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				AddPolygonCollider2D(map, gameObject, obj, isTrigger, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				AddPolygonCollider3D(map, gameObject, obj, isTrigger, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}
		#endregion

		#region Generic Generate Colliders
		/// <summary>
		/// Generate a collider based on object type
		/// </summary>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="isTrigger">true to generate Trigger collider</param>
		/// <param name="used2DColider">True to generate 2D mapObjects, otherwise 3D mapObjects will be generated.</param>
		/// <param name="zDepth">Z Depth of the 3D collider.</param>
		/// <param name="colliderWidth">>Width of the 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		/// <returns>Generated Game Object containing the Collider.</returns>
		public static GameObject GenerateCollider(this Map map, MapObject obj, bool used2DColider = true, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, float zDepth = 0, float colliderWidth = 1, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				return map.GenerateCollider2D(obj, isTrigger, tag, physicsLayer, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				return map.GenerateCollider3D(obj, isTrigger, tag, physicsLayer, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}


		public static GameObject GenerateCollider2D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject col = null;

			switch (obj.ObjectType)
			{
				case ObjectType.Box:
					col = map.GenerateBoxCollider2D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Ellipse:
					col = map.GenerateEllipseCollider2D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polygon:
					col = map.GeneratePolygonCollider2D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polyline:
					col = map.GeneratePolylineCollider2D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
			}

			return col;
		}

		public static GameObject GenerateCollider3D(this Map map, MapObject obj, bool isTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			GameObject col = null;

			switch (obj.ObjectType)
			{
				case ObjectType.Box:
					col = map.GenerateBoxCollider3D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Ellipse:
					col = map.GenerateEllipseCollider3D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polygon:
					col = map.GeneratePolygonCollider3D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polyline:
					col = map.GeneratePolylineCollider3D(obj, isTrigger, tag, physicsLayer, physicsMaterial, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
					break;
			}

			return col;
		}
		#endregion

		#region Generic Add Collider
		/// <summary>
		/// Adds a collider to an existing GameObject based on obj type.
		/// </summary>
		/// <param name="gameObject">GameObject to add a collider</param>
		/// <param name="obj">MapObject which properties will be used to generate this collider.</param>
		/// <param name="isTrigger">true to generate Trigger collider</param>
		/// <param name="used2DColider">True to generate 2D mapObjects, otherwise 3D mapObjects will be generated.</param>
		/// <param name="zDepth">Z Depth of the 3D collider.</param>
		/// <param name="colliderWidth">>Width of the 3D collider.</param>
		/// <param name="innerCollision">If true, calculate normals facing the anchor of the collider (inside collisions), else, outside collisions.</param>
		/// <param name="createRigidbody">True to attach a Rigidbody to the created collider</param>
		/// <param name="rigidbodyIsKinematic">Sets if the attached rigidbody is kinematic or not</param>
		public static void AddCollider(Map map, GameObject gameObject, MapObject obj, bool used2DColider = true, bool isTrigger = false, float zDepth = 0, float colliderWidth = 1, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			if (used2DColider)
				AddCollider2D(map, gameObject, obj, isTrigger, null, zDepth, createRigidbody, rigidbodyIsKinematic);
			else
				AddCollider3D(map, gameObject, obj, isTrigger, null, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
		}

		public static void AddCollider2D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicsMaterial2D physicsMaterial = null, float zDepth = 0, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			switch (obj.ObjectType)
			{
				case ObjectType.Box:
					AddBoxCollider2D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Ellipse:
					AddEllipseCollider2D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polygon:
					AddPolygonCollider2D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polyline:
					AddPolylineCollider2D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, createRigidbody, rigidbodyIsKinematic);
					break;
			}
		}

		public static void AddCollider3D(Map map, GameObject gameObject, MapObject obj, bool isTrigger = false, PhysicMaterial physicsMaterial = null, float zDepth = 0, float colliderWidth = 1, bool innerCollision = false, bool createRigidbody = false, bool rigidbodyIsKinematic = true)
		{
			switch (obj.ObjectType)
			{
				case ObjectType.Box:
					AddBoxCollider3D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Ellipse:
					AddEllipseCollider3D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, colliderWidth, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polygon:
					AddPolygonCollider3D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
					break;
				case ObjectType.Polyline:
					AddPolylineCollider3D(map, gameObject, obj, isTrigger, physicsMaterial, zDepth, colliderWidth, innerCollision, createRigidbody, rigidbodyIsKinematic);
					break;
			}
		}
		#endregion

		#region Generate Colliders from MapObjectLayer
		/// <summary>
		/// Generates Colliders from a MapObjectLayer. Every Object in it will generate a GameObject with a Collider.
		/// </summary>
		/// <param name="objectLayerName">MapObject Layer's name</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="collidersAreTrigger">true to generate Trigger mapObjects, false otherwhise.</param>
		/// <param name="is2DCollider">true to generate 2D mapObjects, false for 3D mapObjects</param>
		/// <param name="collidersZDepth">Z position of the mapObjects</param>
		/// <param name="collidersWidth">Width for 3D mapObjects</param>
		/// <param name="collidersAreInner">true to generate inner collisions for 3D mapObjects</param>
		/// <returns>An Array containing all generated GameObjects</returns>
		public static GameObject[] GenerateCollidersFromLayer(this Map map, string objectLayerName, bool is2DCollider = true, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, float collidersZDepth = 0, float collidersWidth = 1, bool collidersAreInner = false)
		{
			if (is2DCollider)
				return map.GenerateColliders2DFromLayer(objectLayerName, collidersAreTrigger, tag, physicsLayer, null, collidersZDepth);
			else
				return map.GenerateColliders3DFromLayer(objectLayerName, collidersAreTrigger, tag, physicsLayer, null, collidersZDepth, collidersWidth, collidersAreInner);
		}

		public static GameObject[] GenerateColliders2DFromLayer(this Map map, string objectLayerName, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float collidersZDepth = 0)
		{
			MapObjectLayer objectLayer = map.GetObjectLayer(objectLayerName);
			if (objectLayer != null)
			{
				return map.GenerateColliders2DFromLayer(objectLayer, collidersAreTrigger, tag, physicsLayer, physicsMaterial, collidersZDepth);
			}
			else
			{
				Debug.LogWarning("There's no Layer \"" + objectLayerName + "\" in tile map.");
			}

			return null;
		}

		public static GameObject[] GenerateColliders3DFromLayer(this Map map, string objectLayerName, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float collidersZDepth = 0, float collidersWidth = 1, bool collidersAreInner = false)
		{
			MapObjectLayer objectLayer = map.GetObjectLayer(objectLayerName);
			if (objectLayer != null)
			{
				return map.GenerateColliders3DFromLayer(objectLayer, collidersAreTrigger, tag, physicsLayer, physicsMaterial, collidersZDepth, collidersWidth, collidersAreInner);
			}
			else
			{
				Debug.LogWarning("There's no Layer \"" + objectLayerName + "\" in tile map.");
			}

			return null;
		}

		/// <summary>
		/// Generates Colliders from an MapObject Layer. Every Object in it will generate a GameObject with a Collider.
		/// </summary>
		/// <param name="objectLayer">MapObjectLayer</param>
		/// <param name="tag">Tag for the generated GameObjects</param>
		/// <param name="physicsLayer">Physics Layer for the generated GameObjects</param>
		/// <param name="collidersAreTrigger">true to generate Trigger mapObjects, false otherwhise.</param>
		/// <param name="is2DCollider">true to generate 2D mapObjects, false for 3D mapObjects</param>
		/// <param name="collidersZDepth">Z position of the mapObjects</param>
		/// <param name="collidersWidth">Width for 3D mapObjects</param>
		/// <param name="collidersAreInner">true to generate inner collisions for 3D mapObjects</param>
		/// <returns>An Array containing all generated GameObjects</returns>
		public static GameObject[] GenerateCollidersFromLayer(this Map map, MapObjectLayer objectLayer, bool is2DCollider = true, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, float collidersZDepth = 0, float collidersWidth = 1, bool collidersAreInner = false)
		{
			if (is2DCollider)
				return map.GenerateColliders2DFromLayer(objectLayer, collidersAreTrigger, tag, physicsLayer, null, collidersZDepth);
			else
				return map.GenerateColliders3DFromLayer(objectLayer, collidersAreTrigger, tag, physicsLayer, null, collidersZDepth, collidersWidth, collidersAreInner);
		}

		public static GameObject[] GenerateColliders2DFromLayer(this Map map, MapObjectLayer objectLayer, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicsMaterial2D physicsMaterial = null, float collidersZDepth = 0)
		{
			if (objectLayer != null)
			{
				List<GameObject> generatedGameObjects = new List<GameObject>();

				List<MapObject> colliders = objectLayer.Objects;
				foreach (MapObject colliderObjMap in colliders)
				{
					// This function should not try to generate a collider where a prefab will be generated!
					if (colliderObjMap.HasProperty(Map.Property_PrefabName))
						continue;
					// Also, do not generate collider for a TileObject
					if (colliderObjMap.GID > 0)
						continue;
					GameObject newColliderObject = null;
					if (colliderObjMap.Type.Equals(Map.Object_Type_NoCollider) == false)
					{
						newColliderObject = map.GenerateCollider2D(colliderObjMap, collidersAreTrigger, tag, physicsLayer, physicsMaterial, collidersZDepth);
					}

					if (newColliderObject) generatedGameObjects.Add(newColliderObject);
				}

				return generatedGameObjects.ToArray();
			}

			return null;
		}

		public static GameObject[] GenerateColliders3DFromLayer(this Map map, MapObjectLayer objectLayer, bool collidersAreTrigger = false, string tag = "Untagged", int physicsLayer = 0, PhysicMaterial physicsMaterial = null, float collidersZDepth = 0, float collidersWidth = 1, bool collidersAreInner = false)
		{
			if (objectLayer != null)
			{
				List<GameObject> generatedGameObjects = new List<GameObject>();

				List<MapObject> colliders = objectLayer.Objects;
				foreach (MapObject colliderObjMap in colliders)
				{
					// This function should not try to generate a collider where a prefab will be generated!
					if (colliderObjMap.HasProperty(Map.Property_PrefabName))
						continue;
					// Also, do not generate collider for a TileObject
					if (colliderObjMap.GID > 0)
						continue;
					GameObject newColliderObject = null;
					if (colliderObjMap.Type.Equals(Map.Object_Type_NoCollider) == false)
					{
						newColliderObject = map.GenerateCollider3D(colliderObjMap, collidersAreTrigger, tag, physicsLayer, physicsMaterial, collidersZDepth, collidersWidth, collidersAreInner);
					}

					if (newColliderObject) generatedGameObjects.Add(newColliderObject);
				}

				return generatedGameObjects.ToArray();
			}

			return null;
		}
		#endregion
		#endregion

		#region Prefabs Generation
		/// <summary>
		/// Adds prefabs referenced in a MapObjectLayer
		/// </summary>
		/// <param name="objectLayer">MapObjectLayer to add prefabs from</param>
		/// <param name="addMapName">true to add Map's name to generated prefabs</param>
		/// <returns>An array containing all generated Prebafs</returns>
		public static GameObject[] GeneratePrefabsFromLayer(this Map map, MapObjectLayer objectLayer, Vector2 anchorPoint, bool addMapName = false, bool setNameAsObjectName = false)
		{
			if (objectLayer != null)
			{
				List<GameObject> generatedPrefabs = new List<GameObject>();
				List<MapObject> mapObjects = objectLayer.Objects;
				foreach (MapObject mapObj in mapObjects)
				{
					generatedPrefabs.Add(map.GeneratePrefab(mapObj, anchorPoint, objectLayer.LayerGameObject, addMapName, setNameAsObjectName));
				}

				return generatedPrefabs.ToArray();
			}

			return null;
		}

		public static GameObject[] GeneratePrefabsFromLayer(this Map map, MapObjectLayer objectLayer, bool addMapName = false, bool setNameAsObjectName = false)
		{
			return GeneratePrefabsFromLayer(map, objectLayer, XUniTMXConfiguration.Instance.GetTilePrefabsAnchorPoint(), addMapName, setNameAsObjectName);
		}

		/// <summary>
		/// Adds prefabs referenced in a MapObjectLayer
		/// </summary>
		/// <param name="objectLayer">MapObjectLayer to add prefabs from</param>
		/// <param name="addMapName">true to add Map's name to generated prefabs</param>
		/// <returns>An array containing all generated Prebafs</returns>
		public static GameObject[] GeneratePrefabsFromLayer(this Map map, string objectLayerName, Vector2 anchorPoint, bool addMapName = false, bool setNameAsObjectName = false)
		{

			MapObjectLayer objectLayer = map.GetObjectLayer(objectLayerName);
			if (objectLayer != null)
			{
				return map.GeneratePrefabsFromLayer(objectLayer, anchorPoint, addMapName, setNameAsObjectName);
			}
			else
			{
				Debug.LogWarning("There's no Layer \"" + objectLayerName + "\" in tile map.");
			}

			return null;
		}

		public static GameObject[] GeneratePrefabsFromLayer(this Map map, string objectLayerName, bool addMapName = false, bool setNameAsObjectName = false)
		{
			return GeneratePrefabsFromLayer(map, objectLayerName, XUniTMXConfiguration.Instance.GetTilePrefabsAnchorPoint(), addMapName, setNameAsObjectName);
		}

		/// <summary>
		/// Generate a prefab based in object layer
		/// </summary>
		/// <param name="obj">Object which properties will be used to generate a prefab.</param>
		/// <param name="parent">if null add relative parent object,.</param>
		/// <param name="addMapName">true to add Map's name to the prefab name</param>
		/// <returns>Generated GameObject from the Prefab.</returns>
		public static GameObject GeneratePrefab(this Map map, MapObject obj, Vector2 anchorPointValue, GameObject parent = null, bool addMapName = true, bool setNameAsObjectName = false)
		{
			if (obj.HasProperty(Map.Property_PrefabName))
			{
				string prefabName = obj.GetPropertyAsString(Map.Property_PrefabName);
				string baseResourcePath = obj.GetPropertyAsString(Map.Property_PrefabPath);
				UnityEngine.Object resourceObject = Resources.Load(baseResourcePath + prefabName);
				Resources.UnloadUnusedAssets();
				if (resourceObject != null)
				{
					float zDepth = obj.GetPropertyAsFloat(Map.Property_PrefabZDepth);
					GameObject newPrefab = UnityEngine.Object.Instantiate(resourceObject) as GameObject;

					newPrefab.transform.parent = obj.ParentObjectLayer != null ? obj.ParentObjectLayer.LayerGameObject.transform : map.MapGameObject.transform;
					newPrefab.transform.localPosition = map.TiledPositionToWorldPoint(
						new Vector3(obj.Bounds.xMin + obj.Bounds.width * anchorPointValue.x,
							obj.Bounds.yMin + obj.Bounds.height * anchorPointValue.y, 
							zDepth));

					if (obj.HasProperty(Map.Property_PrefabAddCollider))
					{
						string colliderType = obj.GetPropertyAsString(Map.Property_PrefabAddCollider);
						AddCollider(map, newPrefab, obj, !colliderType.Contains("3"));
					}

					// since custom properties are automatically added only when a collider is added, we must enforce them to be parsed
					ApplyCustomProperties(newPrefab, obj);

					if (parent)
						newPrefab.transform.parent = parent.transform;

					if (setNameAsObjectName)
						newPrefab.name = obj.Name;

					if (addMapName)
						newPrefab.name = string.Concat(map.MapName, "_", newPrefab.name);

					obj.LinkedGameObject = newPrefab;

					return newPrefab;
				}
				else
				{
					Debug.LogError("Prefab doesn't exist at: Resources/" + baseResourcePath + prefabName);
				}
			}
			return null;
		}

		public static GameObject GeneratePrefab(this Map map, MapObject obj, GameObject parent = null, bool addMapName = true, bool setNameAsObjectName = false)
		{
			return GeneratePrefab(map, obj, XUniTMXConfiguration.Instance.GetTilePrefabsAnchorPoint(), parent, addMapName, setNameAsObjectName);
		}

		#endregion

		#region Parse and Apply MapObjects, Layers and Tiles X-UniTMX Properties
		/// <summary>
		/// Applies to gameObject any custom X-UniTMX properties present on obj
		/// </summary>
		/// <param name="gameObject">GameObject to apply custom properties to</param>
		/// <param name="obj">MapObject to read custom properties from</param>
		public static void ApplyCustomProperties(GameObject gameObject, Object obj)
		{
			ApplyCustomProperties(gameObject, obj.Properties);
		}

		/// <summary>
		/// Applies to gameObject any custom X-UniTMX properties present on layer
		/// </summary>
		/// <param name="gameObject">GameObject to apply custom properties to</param>
		/// <param name="layer">Layer to read custom properties from</param>
		public static void ApplyCustomProperties(GameObject gameObject, Layer layer)
		{
			ApplyCustomProperties(gameObject, layer.Properties);
		}

		/// <summary>
		/// Applies to gameObject any custom X-UniTMX properties present on tile
		/// </summary>
		/// <param name="gameObject">GameObject to apply custom properties to</param>
		/// <param name="tile">Tile to read custom properties from</param>
		public static void ApplyCustomProperties(GameObject gameObject, Tile tile)
		{
			ApplyCustomProperties(gameObject, tile.Properties);
		}

		/// <summary>
		/// Applies to a GameObject any custom property found in PropertyCollection
		/// </summary>
		/// <param name="gameObject">GameObject to apply custom properties to</param>
		/// <param name="properties">PropertyCollection to read custom properties from</param>
		public static void ApplyCustomProperties(GameObject gameObject, PropertyCollection properties)
		{
			// nothing to do here...
			if (gameObject == null || properties == null)
				return;

			// Set a layer number for gameObject
			if (properties.HasProperty(Map.Property_Layer))
				gameObject.layer = properties.GetPropertyAsInt(Map.Property_Layer);

			if (properties.HasProperty(Map.Property_LayerName))
				gameObject.layer = LayerMask.NameToLayer(properties.GetPropertyAsString(Map.Property_LayerName));

			// Add a tag for gameObject
			if (properties.HasProperty(Map.Property_Tag))
				gameObject.tag = properties.GetPropertyAsString(Map.Property_Tag);

			int c = 1;
			// Unity 5 Removed AddComponent("string")...
#if !UNITY_5
			// Add Components for this gameObject
			while (properties.HasProperty(Map.Property_AddComponent + c))
			{
				try
				{
					gameObject.AddComponent(properties.GetPropertyAsString(Map.Property_AddComponent + c));
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
				}
				c++;
			}
			c = 1;
#endif
			// Can only send messages while playing
			if (Application.isPlaying)
			{
				while (properties.HasProperty(Map.Property_SendMessage + c))
				{
					string messageToSend = properties.GetPropertyAsString(Map.Property_SendMessage + c);
					string[] menssage = messageToSend.Split('|');
					if (menssage.Length == 2)
					{
						gameObject.BroadcastMessage(menssage[0], menssage[1], SendMessageOptions.DontRequireReceiver);
					}
					if (menssage.Length == 1)
					{
						gameObject.BroadcastMessage(menssage[0], SendMessageOptions.DontRequireReceiver);
					}
					c++;
				}
			}
			Renderer renderer = gameObject.GetComponent<Renderer>();
			if (renderer != null)
			{
				if (properties.HasProperty(Map.Property_SortingLayerName))
					renderer.sortingLayerName = properties.GetPropertyAsString(Map.Property_SortingLayerName);

				if (properties.HasProperty(Map.Property_SortingOrder))
					renderer.sortingOrder = properties.GetPropertyAsInt(Map.Property_SortingOrder);

				if (properties.HasProperty(Map.Property_SetMaterialColor))
				{
					string[] splitColor = properties.GetPropertyAsString(Map.Property_SetMaterialColor).Split(',');
					if (splitColor.Length >= 1)
					{
						Material mat;
#if !UNITY_5
						mat = new Material(Shader.Find("Diffuse"));
#else
						mat = new Material(Shader.Find("Standard"));
						mat.SetFloat("_Mode", 3);
						mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
						mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						mat.SetInt("_ZWrite", 0);
						mat.DisableKeyword("_ALPHATEST_ON");
						mat.EnableKeyword("_ALPHABLEND_ON");
						mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
						mat.renderQueue = 3000;
#endif
						mat.SetColor("_Color", new Color32(
							((byte)(int.Parse(string.IsNullOrEmpty(splitColor[0]) ? "255" : splitColor[0]))),
							splitColor.Length >= 2 ? ((byte)(int.Parse(splitColor[1]))) : (byte)255,
							splitColor.Length >= 3 ? ((byte)(int.Parse(splitColor[2]))) : (byte)255,
							splitColor.Length >= 4 ? ((byte)(int.Parse(splitColor[3]))) : (byte)255));
						renderer.material = mat;
					}
				}
			}
		}
		#endregion

	}
}
