using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace X_UniTMX.Internal
{
	public class ExamplesEditor : Editor
	{
		public static bool AddedScenes = false;

		[MenuItem("X-UniTMX/Add Example Scenes")]
		public static void AddScenesMenu()
		{
			// This should never happen, as we are using Unity's MenuItem validator...
			if (AddedScenes)
			{
				Debug.Log("X-UniTMX Example Scenes were already added to Build Scenes list!");
				return;
			}
			AddedScenes = true;
			var original = EditorBuildSettings.scenes;
			var newSettings = new List<EditorBuildSettingsScene>();
			// Add User scenes, to not overwrite them
			for (int i = 0; i < original.Length; i++)
			{
				newSettings.Add(original[i]);
			}
			// Add X-UniTMX example scenes
			for (int i = 0; i < ScenesStruct.AdditionalScenes.Length; i++)
			{
				if(!CheckSceneListContainsScene(original, ScenesStruct.AdditionalScenes[i]))
					newSettings.Add(new EditorBuildSettingsScene(ScenesStruct.AdditionalScenes[i], true));
			}
			string scenePath = string.Empty;
			for (int i = 0; i < ScenesStruct.TestScenesPaths.Length; i++)
			{
				scenePath = string.Concat(ScenesStruct.TestScenesPaths[i], ScenesStruct.TestScenes[i], ".unity");
				if (!CheckSceneListContainsScene(original, scenePath))
					newSettings.Add(new EditorBuildSettingsScene(scenePath, true));
			}
			for (int i = 0; i < ScenesStruct.GameScenesPaths.Length; i++)
			{
				scenePath = string.Concat(ScenesStruct.GameScenesPaths[i], ScenesStruct.GameScenes[i], ".unity");
				if (!CheckSceneListContainsScene(original, scenePath))
					newSettings.Add(new EditorBuildSettingsScene(scenePath, true));
			}

			EditorBuildSettings.scenes = newSettings.ToArray();
			Debug.Log("X-UniTMX Example Scenes added to Build Scenes list.");
		}

		[MenuItem("X-UniTMX/Add Example Scenes", true)]
		public static bool CheckScenesMenu()
		{
			return !AddedScenes;
		}

		static bool CheckSceneListContainsScene(EditorBuildSettingsScene[] scenes, string scenePath)
		{
			if (string.IsNullOrEmpty(scenePath))
				return false;
			for (int i = 0; i < scenes.Length; i++)
			{
				if (scenes[i].path.Equals(scenePath))
					return true;
			}
			return false;
		}
	}
}
