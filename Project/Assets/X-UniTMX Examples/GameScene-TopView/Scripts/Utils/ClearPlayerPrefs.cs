using UnityEngine;
using System.Collections;

namespace X_UniTMX.Internal
{
	public class ClearPlayerPrefs : MonoBehaviour
	{

		public bool clearOnQuitGame = false;

		public void SetClearOnQuitGame(string value)
		{

			clearOnQuitGame = bool.Parse(value);
		}

		public bool clearOnLevelWasLoaded = false;

		public void SetClearOnLevelWasLoaded(string value)
		{

			clearOnLevelWasLoaded = bool.Parse(value);
		}

		// Call once at begin of the game
		void OnApplicationQuit()
		{
			if (clearOnQuitGame) PlayerPrefs.DeleteAll();
		}
		void OnLevelWasLoaded(int level)
		{
			if (clearOnLevelWasLoaded) PlayerPrefs.DeleteAll();
		}

	}
}
