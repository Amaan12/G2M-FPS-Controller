using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SampleGame;
using LevelManagement.Data;

namespace LevelManagement
{
	public class SettingsMenu : Menu<SettingsMenu>
	{
		[SerializeField] Slider masterVolumeSlider;
		[SerializeField] Slider musicVolumeSlider;
		[SerializeField] Slider sfxVolumeSlider;

		DataManager dataManager;

		protected override void Awake()
		{
			base.Awake();
			dataManager = FindFirstObjectByType<DataManager>();
		}

		void Start()
		{
			LoadData();
		}

		public void OnMasterVolumeChanged(float volume)
		{
			if (dataManager != null)
			{
				dataManager.MasterVolume = volume;
			}
			// PlayerPrefs.SetFloat("MasterVolume", volume);
		}

		public void OnMusicVolumeChanged(float volume)
		{
			if (dataManager != null)
			{
				dataManager.MusicVolume = volume;
			}
			// PlayerPrefs.SetFloat("MusicVolume", volume);
		}

		public void OnSFXVolumeChanged(float volume)
		{
			if (dataManager != null)
			{
				dataManager.SFXVolume = volume;
			}
			// PlayerPrefs.SetFloat("SFXVolume", volume);
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			if (dataManager != null) dataManager.Save();
			// PlayerPrefs.Save();
		}

		public void LoadData()
		{
			dataManager.Load();
			if (dataManager == null || masterVolumeSlider == null || musicVolumeSlider == null || sfxVolumeSlider == null) return;
			masterVolumeSlider.value = dataManager.MasterVolume;
			musicVolumeSlider.value = dataManager.MusicVolume;
			sfxVolumeSlider.value = dataManager.SFXVolume;
			// masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
			// musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
			// sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
		}
	}
}