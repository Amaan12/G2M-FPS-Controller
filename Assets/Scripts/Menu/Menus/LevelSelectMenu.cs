using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LevelManagement.Mission;

namespace LevelManagement
{
	[RequireComponent(typeof(MissionSelector))]
	public class LevelSelectMenu : Menu<LevelSelectMenu>
	{
		#region Inspector
		[SerializeField] protected Text nameText;
		[SerializeField] protected Text descriptionText;
		[SerializeField] protected Image previewImage;

		[SerializeField] float playDelay = 0.5f;
		[SerializeField] TransitionFader startTransitionPrefab;
		#endregion

		#region Protected
		protected MissionSelector missionSelector;
		protected MissionSpecs currentMission;
		#endregion

		protected override void Awake()
		{
			base.Awake();
			missionSelector = GetComponent<MissionSelector>();
			UpdateInfo();
		}

		void OnEnable()
		{
			UpdateInfo();
		}

		public void UpdateInfo()
		{
			currentMission = missionSelector.GetCurrentMission();

			if (currentMission != null)
			{
				nameText.text = currentMission.Name;
				descriptionText.text = currentMission.Description;
				previewImage.sprite = currentMission.Image;
			}
		}

		public void OnNextPressed()
		{
			missionSelector.IncrementIndex();
			UpdateInfo();
		}

		public void OnPreviousPressed()
		{
			missionSelector.DecrementIndex();
			UpdateInfo();
		}

		public void OnPlayPressed()
		{
			StartCoroutine(PlayMissionRoutine(currentMission.sceneName));
		}

		IEnumerator PlayMissionRoutine(string sceneName)
		{
			TransitionFader.PlayTransition(startTransitionPrefab);
			LevelLoader.LoadLevel(sceneName);
			yield return new WaitForSeconds(playDelay);
			GameMenu.Open();
		}
	}
}
