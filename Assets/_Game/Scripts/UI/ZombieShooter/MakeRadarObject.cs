using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class MakeRadarObject : MonoBehaviour
    {
        public Image image;

        private void OnEnable() {
            DOVirtual.DelayedCall(0.5f, () => ZSRadarUI.RegisterRadarObject(this.gameObject, image));
        }

        private void OnDisable() {
            ZSRadarUI.RemoveRadarObject(this.gameObject);
        }
    }
}