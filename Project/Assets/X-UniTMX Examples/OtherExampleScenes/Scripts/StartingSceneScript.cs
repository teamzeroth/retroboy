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
	public struct ScenesStruct
	{
		public static string[] TestScenesNames = new string[] { 
			"(Ortho) Map Static Loaded", "(Ortho) Ortho Map with Tile Collisions", "(Ortho) Map Loading on Scene Start", "(Ortho) Map Loading from Script", "(Ortho) Map Loading from Script w/ Render Order", "(Ortho) Map Load Stress Tests", "(Ortho) Streamed Assets Test",
			"(Iso) Map Simple Test", "(Iso) Map Loading on Scene Start", "(Iso) Simple Test with dif. perspective", "(Iso) Simple Test with dif. perspective",
			"(Staggered) Map Loading on Scene Start"
		};
		public static string[] TestScenes = new string[] { 
			"OrthoMapStaticLoaded", "OrthoMapStaticLoadedWithTileCollisions", "OrthoMapLoadingOnSceneStart", "OrthoMapLoadingFromScript", "OrthoMapLoadingFromScriptRenderOrder", "OrthoMapLoadStressTest", "OrthoMapStreamedAssetsTest",
			"IsoMapSimpleTest", "IsoMapLoadingOnSceneStart", "IsoMapSimpleTestWithDifPerspective1", "IsoMapSimpleTestWithDifPerspective2",
			"StaggeredMapLoadingOnSceneStart"
		};
		public static string[] TestScenesPaths = new string[] { 
			"Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/",
			"Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/", "Assets/X-UniTMX Examples/OtherExampleScenes/",
			"Assets/X-UniTMX Examples/OtherExampleScenes/"
		};

		public static string[] GameScenesNames = new string[] {
			"Game Example - Top View",
			"Game Example - 2D Platformer",
			"Game Example - 3D Platformer",
			"Game Example - Tile Objects 2D Platformer",
			"Game Example - 2D Isometric Dungeon"
		};

		public static string[] GameScenes = new string[] {
			"GameTopViewMenu",
			"Game2DPlatform",
			"Game3DPlatform",
			"GameTileCollider2DPlatform",
			"GameScene-Isometric-Test"
		};
		public static string[] GameScenesPaths = new string[] {
			"Assets/X-UniTMX Examples/GameScene-TopView/Scenes/",
			"Assets/X-UniTMX Examples/GameScene-Plataform/Scenes/",
			"Assets/X-UniTMX Examples/GameScene-3DPlataform/Scenes/",
			"Assets/X-UniTMX Examples/GameScene-TileColliderPlatform/Scene/",
			"Assets/X-UniTMX Examples/GameScene-Isometric/Scenes/"
		};
		public static string[] AdditionalScenes = new string[] {
			"Assets/X-UniTMX Examples/StartingScene.unity",
			"Assets/X-UniTMX Examples/GameScene-TopView/Scenes/GameTopViewGameOver.unity",
			"Assets/X-UniTMX Examples/GameScene-TopView/Scenes/GameTopView.unity",
		};
	}
	public class StartingSceneScript : MonoBehaviour
	{

		int selGrid = -1;

		void OnGUI()
		{
			GUILayout.BeginArea(new Rect(0.05f * Screen.width, 0.05f * Screen.height, 0.9f * Screen.width, 0.9f * Screen.height), "Test Scenes", "box");

			GUILayout.FlexibleSpace();
			selGrid = GUILayout.SelectionGrid(selGrid, ScenesStruct.TestScenesNames, 2);

			if (selGrid > -1)
				Application.LoadLevel(ScenesStruct.TestScenes[selGrid]);

			GUILayout.FlexibleSpace();

			for (int i = 0; i < Mathf.Min(ScenesStruct.GameScenes.Length, ScenesStruct.GameScenesNames.Length); i++)
			{
				if (GUILayout.Button(ScenesStruct.GameScenesNames[i]))
					Application.LoadLevel(ScenesStruct.GameScenes[i]);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
		}
	}
}
