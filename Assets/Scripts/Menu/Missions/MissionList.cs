using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement.Mission
{
	[CreateAssetMenu(fileName = "MissionList", menuName = "Missions/Create Mission List", order = 1)]
	public class MissionList : ScriptableObject
	{
		#region Inspector
		[SerializeField] List<MissionSpecs> missions;
		#endregion

		#region Properties
		public int TotalMissions { get { return missions.Count; } }
		#endregion

		public MissionSpecs GetMission(int index)
		{
			return missions[index];
		}
	}
}
