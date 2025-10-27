using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SampleGame
{  
    [RequireComponent(typeof(Collider))]
    public class Objective : MonoBehaviour
    {
        [SerializeField] string _playerTag = "Player";

        bool _isComplete; // is the objective complete?
        public bool IsComplete { get { return _isComplete; } }

        public void CompleteObjective(){ _isComplete = true; }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == _playerTag) CompleteObjective();
        }
    }
}
