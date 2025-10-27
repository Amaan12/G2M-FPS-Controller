using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SampleGame;
using LevelManagement.Data;

namespace LevelManagement
{
	public class MainMenu : Menu<MainMenu>
	{
		[SerializeField] float _playDelay = 0.5f;
		[SerializeField] TransitionFader startTransitionPrefab;

		DataManager dataManager;
		[SerializeField] InputField inputField;

		protected override void Awake()
		{
			base.Awake();
			dataManager = FindFirstObjectByType<DataManager>();
		}

		// Loading
		void Start()
		{
			LoadData();
		}

		public void LoadData()
		{
			if (dataManager == null || inputField == null) return;
			dataManager.Load();
			inputField.text = dataManager.PlayerName;
		}

		// Saving
		public void OnValueChanged(string text)
		{
			if (dataManager != null)
				dataManager.PlayerName = text;
		}

		public void OnEndEdit()
		{
			if (dataManager != null)
				dataManager.Save();
		}

		public void OnPlayPressed()
		{
			// StartCoroutine(OnPlayPressedRoutine());

			LevelSelectMenu.Open();
		}

		// IEnumerator OnPlayPressedRoutine()
		// {
		// 	TransitionFader.PlayTransition(startTransitionPrefab);	
		// 	LevelLoader.LoadNextLevel();
		// 	yield return new WaitForSeconds(_playDelay);
		// 	GameMenu.Open();
		// }

		public void OnSettingsPressed()
		{
			// PREVIOUSLY.
			// MenuManager menuManager = MenuManager.Instance;
			// // GameObject settingsMenuObject = transform.parent.Find("SettingsMenu(Clone)").gameObject; // alternatively
			// // GameObject settingsMenuObject = menuManager._menuParent.Find("SettingsMenu(Clone)").gameObject; // alternatively
			// if (menuManager != null || settingsMenuObject != null)
			// {
			// 	Menu settingsMenu = settingsMenuObject.GetComponent<Menu>();
			// 	if (settingsMenu != null) menuManager.OpenMenu(settingsMenu);
			// }

			// CURRENTLY.
			SettingsMenu.Open();
		}

		public void OnCreditsPressed()
		{
			CreditsMenu.Open();
		}

		public override void OnBackPressed()
		{
			Application.Quit();
		}
	}
}



