using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace ZombieShooter
{
    public enum eCrosshairState
    {
        DEFAULT,
        IDLE,
        RUN,
        SHOOT,
        RELOAD
    }
    public class ZSCrosshairUI : MonoBehaviour
    {
#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "Components" };
#endif
        [SerializeField] Transform crosshair;
        // [SerializeField] Image crosshairImage;

#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Params" };
#endif
        [SerializeField] Vector3 scrossHairScaleDefalut = Vector3.one;
        [SerializeField] Vector3 scrossHairScaleIdle = Vector3.one;
        [SerializeField] Vector3 scrossHairScaleRun = Vector3.one;
        [SerializeField] Vector3 scrossHairScaleShoot = Vector3.one * 0.8f;
        [SerializeField] Vector3 scrossHairScaleReload = Vector3.zero;

        private eCrosshairState currentState;

        private void Awake() {
            ToggleCrosshair(false);
        }

        public void Initialize()
        {
            crosshair.localScale = scrossHairScaleDefalut;
            currentState = eCrosshairState.DEFAULT;
        }

        public void SwitchCrosshairState(eCrosshairState newState, float duration = 0.5f)
        {
            if (currentState == newState) return;
            currentState = newState;
            crosshair.DOKill();
            crosshair.DOScale(GetCrosshairScale(currentState), duration);
        }

        public Vector3 GetCrosshairScale(eCrosshairState state)
        {
            return state switch
            {
                eCrosshairState.IDLE => scrossHairScaleIdle,
                eCrosshairState.RUN => scrossHairScaleRun,
                eCrosshairState.SHOOT => scrossHairScaleShoot,
                eCrosshairState.RELOAD => scrossHairScaleReload,
                _ => scrossHairScaleDefalut
            };
        }

        public void ToggleCrosshair(bool toggle)
        {
            crosshair.gameObject.SetActive(toggle);
        }
    }
}