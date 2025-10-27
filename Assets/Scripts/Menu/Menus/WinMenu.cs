using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement
{
	public class WinMenu : Menu<WinMenu>
	{
        public void OnNextLevelPressed()
		{
			base.OnBackPressed();
			LevelLoader.LoadNextLevel();
		}

		public void OnRestartPressed()
		{
			base.OnBackPressed();
			LevelLoader.ReloadLevel();
		}

		public void OnMainMenuPressed()
		{
			LevelLoader.LoadMainMenuLevel();
			MainMenu.Open();
		}
	}
}

