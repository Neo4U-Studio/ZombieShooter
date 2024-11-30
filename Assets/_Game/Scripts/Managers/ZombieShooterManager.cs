using System;
using System.Collections;
using System.Collections.Generic;
using AudioPlayer;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieShooterManager : MonoBehaviour
    {
        public enum eGameState
        {
            Init,
            ReadToStart,
            Start,
            Playing,
            End
        }

        public static ZombieShooterManager Instance;
        public static Action ON_END_GAME;

        [SerializeField] PlayerController playerControl;

        public eGameState CurrentState { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Start() {
            EnterState(eGameState.Init);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update() {
            UpdateState();
        }

        private void Initialize()
        {
            RegisterEvent();
            SoundManager.Instance?.LoadSoundMap(SoundType.ZOMBIE_SHOOTER);
            playerControl.Initialize();
            EnterState(eGameState.ReadToStart);
        }

        public void StartGame()
        {
            EnterState(eGameState.Start);
        }

        private void OnStartGame()
        {
            playerControl.IsPlaying = true;
            EnterState(eGameState.Playing);
        }

        public void EndGame()
        {
            EnterState(eGameState.End);
        }

        private void OnEndGame()
        {
            UnregisterEvent();
        }

        private void RegisterEvent()
        {
            ON_END_GAME += EndGame;
        }

        private void UnregisterEvent()
        {
            ON_END_GAME -= EndGame;
        }

#region Game State
        private void EnterState(eGameState newState)
        {
            Debug.Log("-- Enter State " + newState);
            CurrentState = newState;
            switch (CurrentState)
            {
                case eGameState.Init:
                    Initialize();
                break;
                case eGameState.ReadToStart:
                    // Do nothing
                break;
                case eGameState.Start:
                    OnStartGame();
                break;
                case eGameState.Playing:
                    // Do nothing
                break;
                case eGameState.End:
                    OnEndGame();
                break;
            }
        }

        private void UpdateState()
        {
            switch (CurrentState)
            {
                case eGameState.ReadToStart:
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        StartGame();
                    }
                break;
                case eGameState.Playing:
                    // Do nothing
                break;
            }
        }
#endregion
    }
}