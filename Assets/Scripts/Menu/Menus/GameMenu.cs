using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SampleGame;

namespace LevelManagement
{
	public class GameMenu : Menu<GameMenu>
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape)) OnPauseButtonPressed();
		}

		public void OnPauseButtonPressed()
		{
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
			PauseMenu.Open();
		}
	}
}

