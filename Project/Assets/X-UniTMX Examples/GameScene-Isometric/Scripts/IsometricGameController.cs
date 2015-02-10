/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections.Generic;
using X_UniTMX;

namespace X_UniTMX.Internal
{
	public class IsometricGameController : MonoBehaviour
	{

		public TiledMapComponent _tiledMapComponent;
		public Dictionary<string, Map> _loadedMaps;
		public TextAsset[] _maps;
		Map _currentMap;
		GameObject _player;

		// Use this for initialization
		void Start()
		{
			_loadedMaps = new Dictionary<string, Map>();
			_currentMap = _tiledMapComponent.TiledMap;
			_loadedMaps.Add(_currentMap.MapObject.name, _currentMap);

			_player = GameObject.FindGameObjectWithTag("Player");
			_player.transform.parent = gameObject.transform;
		}

		public void LoadMap(string map, Vector2 _doorFromPosition)
		{
			if (map.Equals(_currentMap.MapObject.name))
				return;

			Map lastMap = _currentMap;
			_currentMap.MapObject.SetActive(false);

			Map nextMap = null;
			// If map was already loaded, just enable its GameObject
			if (_loadedMaps.TryGetValue(map, out nextMap))
			{
				nextMap.MapObject.SetActive(true);
				_currentMap = nextMap;
			}
			else
			{
				// Else, load the map and generate its collisions
				for (int i = 0; i < _maps.Length; i++)
				{
					if (_maps[i].name.Equals(map))
					{
						nextMap = new Map(_maps[i].text, map, _tiledMapComponent.MapTMXPath, _tiledMapComponent.gameObject, _tiledMapComponent.materialDefaultFile, _tiledMapComponent.DefaultSortingOrder, true);
						nextMap.GenerateTileCollisions();
						nextMap.GenerateCollidersFromLayer("WallColliders");
						nextMap.GenerateCollidersFromLayer("Doors", true, true);
						_loadedMaps.Add(map, nextMap);
						_currentMap = nextMap;
						break;
					}
				}
			}
			if (nextMap == null)
				return;

			// Set the player's position on the new map
			SetPlayerNewPosition(lastMap, _doorFromPosition);
		}

		void SetPlayerNewPosition(Map lastMap, Vector2 _doorFromPosition)
		{
			Vector2 fromDoorIndex = lastMap.WorldPointToTileIndex(_doorFromPosition);

			// Change door tile to opened door :P
			int originalID = lastMap.GetTileLayer("Layer 0").Tiles[fromDoorIndex.x, fromDoorIndex.y].OriginalID;
			lastMap.GetTileLayer("Layer 0").SetTile(fromDoorIndex.x, fromDoorIndex.y, originalID + 2);

			int lastMapWidth = lastMap.Width;
			int lastMapHeight = lastMap.Height;
			// Position player
			MapObjectLayer mol = _currentMap.GetObjectLayer("Doors");
			for (int i = 0; i < mol.Objects.Count; i++)
			{
				// This is a Left Door - X = 0
				if (mol.Objects[i].Bounds.x < 1)
				{
					// And we came from a Right Door!
					if (fromDoorIndex.x >= lastMapWidth - 1)
					{
						_player.transform.localPosition = _currentMap.TiledPositionToWorldPoint(1.5f, mol.Objects[i].Bounds.y + 0.5f);
					}
				}
				// This is a Right Door - X = _currentMap.Width - 1
				if (mol.Objects[i].Bounds.x >= _currentMap.Width - 1)
				{
					// And we came from a Left Door!
					if (fromDoorIndex.x < 1)
					{
						_player.transform.localPosition = _currentMap.TiledPositionToWorldPoint(mol.Objects[i].Bounds.x - 1, mol.Objects[i].Bounds.y + 0.5f);
					}
				}
				// This is an Up Door - Y = 0
				if (mol.Objects[i].Bounds.y < 1)
				{
					// And we came from a Down Door!
					if (fromDoorIndex.y >= lastMapHeight - 1)
					{
						_player.transform.localPosition = _currentMap.TiledPositionToWorldPoint(mol.Objects[i].Bounds.x + 0.5f, 1.4f);
					}
				}
				// This is a Down Door - Y = _currentMap.Height - 1
				if (mol.Objects[i].Bounds.y >= _currentMap.Height - 1)
				{
					// And we came from an Up Door!
					if (fromDoorIndex.y < 1)
					{
						_player.transform.localPosition = _currentMap.TiledPositionToWorldPoint(mol.Objects[i].Bounds.x, mol.Objects[i].Bounds.y - 0.5f);
					}
				}
			}

			_player.gameObject.GetComponent<X_UniTMX.Utils.SortingOrderAutoCalculator>().SetMap(_currentMap);
		}
	}
}