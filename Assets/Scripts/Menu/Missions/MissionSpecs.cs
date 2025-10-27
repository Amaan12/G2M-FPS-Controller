using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace LevelManagement.Mission
{
	[Serializable]
	public class MissionSpecs
	{
		#region Inspector
		[SerializeField] protected string name;
		[SerializeField][Multiline] protected string description;
		[SerializeField] public string sceneName;
		[SerializeField] protected string id;
		[SerializeField] protected Sprite image;
		#endregion

		#region Properties
		public string Name { get { return name; } }
		public string Description { get { return description; } }
		public string SceneName { get { return sceneName; } }
		public string Id { get { return id; } }
		public Sprite Image { get { return image; } }
		#endregion
	}
}

