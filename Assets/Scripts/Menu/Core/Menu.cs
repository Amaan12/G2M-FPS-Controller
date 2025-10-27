using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SampleGame;

namespace LevelManagement
{
	public abstract class Menu<T> : Menu where T : Menu<T>
	{
		static T _instance;
		public static T Instance { get { return _instance; } }

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = (T)this;
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
				return;
			}
		}

		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

		public static void Open()
		{
			if (MenuManager.Instance != null && Instance != null)
			{
				MenuManager.Instance.OpenMenu(Instance);
			}
		}
	}

	[RequireComponent(typeof(Canvas))]
	public abstract class Menu : MonoBehaviour
	{
		public virtual void OnBackPressed()
		{
			MenuManager menuManager = MenuManager.Instance;
			if (menuManager != null) menuManager.CloseMenu();
		}

		// Add this method
	    // public virtual GameObject GetFirstButton()
	    // {
	    //     Button button = GetComponentInChildren<Button>();
	    //     return button != null ? button.gameObject : null;
	    // }
	}
}
