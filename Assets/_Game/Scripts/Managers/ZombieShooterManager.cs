using System;
using System.Collections;
using System.Collections.Generic;
using AudioPlayer;
using UnityEditor;
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
        [SerializeField] List<ZSTargetPoint> missionList;

        public eGameState CurrentState { get; private set; }
        public ZSPlayerController Player => playerControl;

        private ZSTargetPoint currentMission = null;
        private ZombieShooterUI mainUI;

        public bool IsWin { get; private set; }

        private AudioSource themeMusic = null;
        
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
            StopCurrentMusic();
            Instance = null;
        }

        private void Initialize()
        {
            RegisterEvent();
            SoundManager.Instance?.LoadSoundMap(SoundType.ZOMBIE_SHOOTER);
            PlayThemeMusic();
            var scene = UIManager.Instance.PushMenu(MenuType.ZOMBIE_SHOOTER_MAIN);
            if (scene is ZombieShooterUI)
            {
                mainUI = scene as ZombieShooterUI;
                mainUI.Initialize(Player);
            }
            Player.Initialize();
            IsWin = false;
            EnterState(eGameState.ReadToStart);
        }

        public void StartGame()
        {
            EnterState(eGameState.Start);
        }

        private void OnStartGame()
        {
            CameraManager.Instance.LiveVirtualCamera = eVirtualCamera.PLAYER;
            Player.IsPlaying = true;
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
            if (IsWin)
            {
                PlayVictoryMusic();
                UIEvents.UI_SHOW_POPUP?.Invoke(PopupType.POPUP_VICTORY, null);
            }
            else
            {
                UIEvents.UI_SHOW_POPUP?.Invoke(PopupType.POPUP_LOSE, null);
            }
            UnregisterEvent();
        }

        private void RegisterEvent()
        {
            ON_END_GAME += EndGame;
            missionList.ForEach(mission => {
                mission.OnPlayerCompleteMission += CheckCompleteMission;
            });
        }

        private void UnregisterEvent()
        {
            ON_END_GAME -= EndGame;
            missionList.ForEach(mission => {
                mission.OnPlayerCompleteMission -= CheckCompleteMission;
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
                    StartGame();
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
            playerControl.HandleWin();
            IsWin = true;
            EndGame();
        }
#endregion

#region Sfx
        public void PlayThemeMusic()
        {
            StopCurrentMusic();
            themeMusic = SoundManager.Instance.PlayMusic(SoundID.MUSIC_THEME_GAME);
        }

        public void PlayVictoryMusic()
        {
            StopCurrentMusic();
            themeMusic = SoundManager.Instance.PlayMusic(SoundID.MUSIC_VICTORY);
        }

        public void PlayBossThemeMusic()
        {
            StopCurrentMusic();
            themeMusic = SoundManager.Instance.PlayMusic(SoundID.MUSIC_THEME_BOSS);
        }

        private void StopCurrentMusic()
        {
            if (themeMusic != null)
            {
                themeMusic.Stop();
                themeMusic = null;
            }
        }
#endregion
    }
}