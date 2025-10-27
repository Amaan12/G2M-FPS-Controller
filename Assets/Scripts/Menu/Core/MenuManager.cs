using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace LevelManagement
{
	public class MenuManager : MonoBehaviour
	{
		[SerializeField] MainMenu mainMenuPrefab;
		[SerializeField] SettingsMenu settingsMenuPrefab;
		[SerializeField] CreditsMenu creditsScreenPrefab;
		[SerializeField] PauseMenu pauseMenuPrefab;
		[SerializeField] GameMenu gameMenuPrefab;
		[SerializeField] WinMenu winMenuPrefab;
		[SerializeField] LevelSelectMenu levelSelectMenuPrefab;

		public Transform _menuParent;

		Stack<Menu> _menuStack = new Stack<Menu>();

		static MenuManager _instance;
        public static MenuManager Instance { get { return _instance; } }


		void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
				return;
			}
			InitializeMenus();

			transform.SetParent(null);
			DontDestroyOnLoad(gameObject);
		}

		void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

		void InitializeMenus()
		{
			if (_menuParent == null)
			{
				GameObject _menuParentObject = new GameObject("Menus");
				_menuParent = _menuParentObject.transform;
			}
			DontDestroyOnLoad(_menuParent.gameObject);

			// Menu[] menuPrefabs = { mainMenuPrefab, settingsMenuPrefab, creditsScreenPrefab, gameMenuPrefab, pauseMenuPrefab, winMenuPrefab };
			System.Type myType = this.GetType();
			BindingFlags myFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			FieldInfo[] fields = myType.GetFields(myFlags);

			// foreach (var prefab in menuPrefabs)
			foreach (FieldInfo field in fields)
			{
				Menu prefab = field.GetValue(this) as Menu;
				if (prefab != null)
				{
					Menu menuInstance = Instantiate(prefab, _menuParent);
					if (prefab == mainMenuPrefab) OpenMenu(menuInstance);
					else menuInstance.gameObject.SetActive(false);
				}
			}
		}

		public void OpenMenu(Menu menuInstance)
		{
			if (menuInstance == null)
			{
				Debug.LogWarning("MenuManager: OpenMenu ERROR: invalid menu");
				return;
			}

			if (_menuStack.Count > 0)
			{
				foreach (Menu menu in _menuStack)
				{
					menu.gameObject.SetActive(false);
				}
			}

			_menuStack.Push(menuInstance);
			menuInstance.gameObject.SetActive(true);

			// NEW CODE
		    // ✅ Reset CanvasGroup if present
			CanvasGroup cg = menuInstance.GetComponent<CanvasGroup>();
			if (cg != null)
			{
				cg.interactable = true;
				cg.blocksRaycasts = true;
			}
			
			// ✅ Auto-select first button if exists
		    // GameObject firstButton = menuInstance.GetFirstButton();
		    // if (firstButton != null)
		    // {
		    //     UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstButton);
		    // }
		}

		public void CloseMenu()
		{
			if (_menuStack.Count == 0)
			{
				Debug.LogWarning("MenuManager: CloseMenu ERROR: No menus in stack");
				return;
			}
			Menu topMenu = _menuStack.Pop();
			topMenu.gameObject.SetActive(false);

			if (_menuStack.Count > 0)
			{
				Menu nextMenu = _menuStack.Peek();
				nextMenu.gameObject.SetActive(true);
			}
		}
	}
}
