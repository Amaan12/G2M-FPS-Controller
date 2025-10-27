using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement.Mission
{
	public class MissionSelector : MonoBehaviour
	{
		#region Inspector
		[SerializeField] protected MissionList missionList;
		#endregion

		#region Protected
		protected int currentIndex;
		#endregion

		#region Properties
		public int CurrentIndex { get { return currentIndex; } }
		#endregion

		#region Custom Methods
		public void ClampIndex()
		{
			if (missionList.TotalMissions == 0)
			{
				Debug.LogWarning("MissionSelector ClampIndex(): Missing missing setup!");
				return;
			}
			if (currentIndex >= missionList.TotalMissions) currentIndex = 0;
			if (currentIndex < 0) currentIndex = missionList.TotalMissions - 1;
		}

		public void SetIndex(int index)
		{
			currentIndex = index;
			ClampIndex();
		}

		public void IncrementIndex() { SetIndex(++currentIndex); }
		public void DecrementIndex() { SetIndex(--currentIndex); }

		public MissionSpecs GetMission(int index) { return missionList.GetMission(index); }
		public MissionSpecs GetCurrentMission() { return GetMission(currentIndex); }
		#endregion
	}
}
