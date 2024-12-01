using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using Cinemachine;

namespace ZombieShooter
{
    [Serializable]
    public enum eVirtualCamera
    {
        PLAYER,
        PLAYER_GAMEOVER,
    }

    [Serializable]
    public enum eOverlayCamera
    {
        MAIN
    }

    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        [SerializeField] VirtualCamPriority virtualCamDict;
        [SerializeField] SerializableDictionaryBase<eOverlayCamera, Camera> overlayCameraDict;

        private eVirtualCamera liveVirtualCamera;
        public eVirtualCamera LiveVirtualCamera { 
            get => liveVirtualCamera; 
            set 
            {
                SetVirtualCamera(value);
            }
        }

        private Camera mainCamera;

        public Camera MainCamera => mainCamera;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            this.mainCamera = Camera.main;
            ResetAllVirtualCam();
            LiveVirtualCamera = eVirtualCamera.PLAYER;
        }

        private void SetVirtualCamera(eVirtualCamera camType)
        {
            virtualCamDict[LiveVirtualCamera].VirtualCam.m_Priority = 0;
            if (!virtualCamDict.ContainsKey(camType))
                return;

            if (this.virtualCamDict[camType].VirtualCam != null)
            {
                VirtualCamData camData = virtualCamDict[camType];
                camData.VirtualCam.m_Priority = camData.Priority;
            }
            else
            {
                Debug.LogWarning($"[Veterinary] Missing ref for virtual camera of type {camType}");
            }
            liveVirtualCamera = camType;
        }

        public void ResetAllVirtualCam()
        {
            foreach (var camData in virtualCamDict)
            {
                if (camData.Value != null && camData.Value.VirtualCam != null)
                {
                    camData.Value.VirtualCam.m_Priority = 0;
                }
                else
                {
                    Debug.LogWarning($"[Veterinary] Missing ref for virtual camera of type {camData.Key}");
                }
            }
        }

        public Camera GetOverlayCamera(eOverlayCamera camType)
        {
            if (overlayCameraDict.ContainsKey(camType))
            {
                return overlayCameraDict[camType];
            }
            else
            {
                return null;
            }
        }

        public CinemachineVirtualCamera GetVirtualCamera(eVirtualCamera camType)
        {
            if (virtualCamDict.ContainsKey(camType))
            {
                return virtualCamDict[camType].VirtualCam;
            }
            else
            {
                return null;
            }
        }
    }

    [Serializable]
    public class VirtualCamPriority : SerializableDictionaryBase<eVirtualCamera, VirtualCamData> { }

    [Serializable]
    public class VirtualCamData
    {
        public CinemachineVirtualCamera VirtualCam;
        public int Priority;
    }
}