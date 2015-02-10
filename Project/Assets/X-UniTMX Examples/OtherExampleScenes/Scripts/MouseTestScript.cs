/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections;

namespace X_UniTMX.Internal
{
	public class MouseTestScript : MonoBehaviour
	{
		public TiledMapComponent TMComponent;

		public GameObject SelectedTile = null;
		Vector3 mouseWorldPos;

		Vector2 offset = Vector2.zero;

		void Start()
		{
			if (TMComponent.TiledMap.Orientation == X_UniTMX.Orientation.Isometric)
				offset.x = -0.5f;
		}

		// Update is called once per frame
		void Update()
		{
			if (TMComponent != null)
			{
				//if(Input.GetMouseButtonDown(0)) 
				//{
				//	Debug.Log(TMComponent.TiledMap.WorldPointToTileIndex(new Vector2(PosX, PosY)));//Camera.main.ScreenToWorldPoint(Input.mousePosition))
				//	Debug.Log(TMComponent.TiledMap.TiledPositionToWorldPoint(PosX, PosY));
				//}
				if (SelectedTile != null)
				{
					mouseWorldPos = Input.mousePosition;
					mouseWorldPos.z = 10;
					mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseWorldPos);
					
					SelectedTile.transform.position = TMComponent.TiledMap.TiledPositionToWorldPoint(TMComponent.TiledMap.WorldPointToTileIndex((Vector2)mouseWorldPos)) + offset;
				}
			}
		}
	}
}

