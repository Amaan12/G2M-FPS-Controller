using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.SceneManagement;
using LevelManagement;

namespace SampleGame
{
    public class GameManager : MonoBehaviour
    {
        // ThirdPersonCharacter _player; // reference to player
        GoalEffect _goalEffect; // reference to goal effect
        Objective _objective; // reference to objective

        bool _isGameOver;
        public bool IsGameOver { get { return _isGameOver; } }

        [SerializeField] TransitionFader _endTransitionPrefab;

        static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

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

            // initialize references
            // _player = Object.FindObjectOfType<ThirdPersonCharacter>();
            _objective = FindFirstObjectByType<Objective>();
            _goalEffect = FindFirstObjectByType<GoalEffect>();
        }

		void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

        // end the level
        public void EndLevel()
        {
            // if (_player != null)
            // {
            //     // disable the player controls
            //     ThirdPersonUserControl thirdPersonControl = _player.GetComponent<ThirdPersonUserControl>();
            //     if (thirdPersonControl != null) thirdPersonControl.enabled = false;

            //     // remove any existing motion on the player
            //     Rigidbody rbody = _player.GetComponent<Rigidbody>();
            //     if (rbody != null) rbody.velocity = Vector3.zero;
            //     _player.Move(Vector3.zero, false, false); // force the player to a stand still
            // }

            // check if we have set IsGameOver to true, only run this logic once
            if (!_isGameOver)
            {
                _isGameOver = true;
                if (_goalEffect != null) _goalEffect.PlayEffect();
                StartCoroutine(WinRoutine());
                // LoadNextLevel();
            }
        }

        IEnumerator WinRoutine()
        {
            TransitionFader.PlayTransition(_endTransitionPrefab);
            float fadeDelay = (_endTransitionPrefab != null) ? _endTransitionPrefab.Delay + _endTransitionPrefab.FadeOnDuration : 0f;
            yield return new WaitForSeconds(fadeDelay);

            WinMenu.Open();
        }

        // check for the end game condition on each frame
        void Update()
        {
            if (_objective != null && _objective.IsComplete) EndLevel();
        }

    }
}