using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
namespace Quantum
{
    public class ResetGame : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button newGame;
        public static event Action OnReset;
        private void Awake()
        {
            Debug.Log("ResetGame Awake - Adding Listener");
            newGame.onClick.AddListener(SignalOnReset);
        }

        private void SignalOnReset()
        {
            OnReset?.Invoke();
        }
    }
}
