using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement
{
	public class LevelLoader : MonoBehaviour
	{
		static int mainMenuBuildIndex = 0; // Why not 0? Good question, that's because 0 is our splash screen loader... in case you don't want that, just remove it from build settings, and update this value to 0.

		public static void LoadLevel(string levelName)
		{
			if (Application.CanStreamedLevelBeLoaded(levelName)) SceneManager.LoadScene(levelName);
			else Debug.LogWarning("LevelLoader: LoadLevel Error: invalid scene specified!");
		}

		public static void LoadLevel(int levelIndex)
		{
			if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
			{
				if (levelIndex == mainMenuBuildIndex) MainMenu.Open();
				SceneManager.LoadScene(levelIndex);
			}
			else Debug.LogWarning("LevelLoader: LoadLevel Error: invalid scene specified!");
		}

		public static void ReloadLevel() { LoadLevel(SceneManager.GetActiveScene().buildIndex); }

		public static void LoadNextLevel()
		{
			Scene currentScene = SceneManager.GetActiveScene();
			int currentSceneIndex = currentScene.buildIndex;
			int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
			if (nextSceneIndex == 0) nextSceneIndex++; // Don't wanna reload splash screen, that is only at application start.
			LoadLevel(nextSceneIndex);
			Debug.Log("Trying to load next level.");
		}

		public static void LoadMainMenuLevel()
		{
			LoadLevel(mainMenuBuildIndex);
		}
	}
}
