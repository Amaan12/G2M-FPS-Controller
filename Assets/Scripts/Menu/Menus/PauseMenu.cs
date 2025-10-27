using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SampleGame;

namespace LevelManagement
{
	public class PauseMenu : Menu<PauseMenu>
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) OnResumePressed();
		}

		public void OnResumePressed()
		{
			Time.timeScale = 1f;
			Cursor.lockState = CursorLockMode.Locked;
        	Cursor.visible = false;
			base.OnBackPressed();
		}

		public void OnRestartPressed()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			OnResumePressed();
		}

		public void OnMainMenuPressed()
		{
			Time.timeScale = 1f;
			LevelLoader.LoadMainMenuLevel();
			MainMenu.Open();
		}

		public void OnQuitPressed()
		{
			Application.Quit();
		}
	}
}