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

        [SerializeField] ZSPlayerController playerControl;
        [SerializeField] ZombieShooterUI mainUI;

        [SerializeField] List<ZSTargetPoint> missionList;

        public eGameState CurrentState { get; private set; }

        private ZSTargetPoint currentMission = null;
        
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
            mainUI.Initialize(playerControl);
            ZSGameStats.Instance?.RefreshValue();
            EnterState(eGameState.ReadToStart);
        }

        public void StartGame()
        {
            EnterState(eGameState.Start);
        }

        private void OnStartGame()
        {
            CameraManager.Instance.LiveVirtualCamera = eVirtualCamera.PLAYER;
            playerControl.IsPlaying = true;
            mainUI.ToggleUI(true);
            TriggerNextTarget();
            EnterState(eGameState.Playing);
        }

        public void EndGame()
        {
            EnterState(eGameState.End);
        }

        private void OnEndGame()
        {
            if (ZSGameStats.IsWin)
            {
                playerControl.HandleWin();
            }
            UnregisterEvent();
        }

        private void RegisterEvent()
        {
            ON_END_GAME += EndGame;
            missionList.ForEach(mission => {
                mission.OnPlayerEnterTarget += CheckCompleteMission;
            });
        }

        private void UnregisterEvent()
        {
            ON_END_GAME -= EndGame;
            missionList.ForEach(mission => {
                mission.OnPlayerEnterTarget -= CheckCompleteMission;
            });
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
                    if (ZSGameStats.GameOver)
                    {
                        EndGame();
                    }
                break;
            }
        }
#endregion

#region Mission
        private void TriggerNextTarget()
        {
            if (missionList != null && missionList.Count > 0)
            {
                currentMission = null;
                foreach (var mission in missionList)
                {
                    if (!mission.IsCompleted)
                    {
                        currentMission = mission;
                        break;
                    }
                }

                if (currentMission != null)
                {
                    currentMission.IsActive = true;
                    mainUI.Compass.SetTarget(currentMission.gameObject);
                    mainUI.Compass.ToggleCompass(true);
                }
                else
                {
                    mainUI.Compass.ToggleCompass(false);
                    OnCompletedAllMission();
                }
            }
        }

        private void CheckCompleteMission(ZSTargetPoint mission)
        {
            if (currentMission != null && currentMission == mission)
            {
                currentMission.IsCompleted = true;
                TriggerNextTarget();
            }
        }

        private void OnCompletedAllMission()
        {
            // Victory
        }
#endregion
    }
}