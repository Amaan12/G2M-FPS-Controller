using System;

namespace LevelManagement.Data
{
	[Serializable]
	public class SaveData
	{
		public string playerName;
		readonly string defaultPlayerName = "Player";

		public float masterVolume;
		public float musicVolume;
		public float sfxVolume;

		public string hashValue;

		public SaveData()
		{
			playerName = defaultPlayerName;
			masterVolume = 0f;
			musicVolume = 0f;
			sfxVolume = 0f;
			hashValue = String.Empty;
		}
	}
}