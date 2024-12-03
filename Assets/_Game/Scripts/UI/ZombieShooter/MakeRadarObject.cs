using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class MakeRadarObject : MonoBehaviour
    {
        public Image image;

        private void OnEnable() {
            ZSRadarUI.RegisterRadarObject(this.gameObject, image);
        }

        private void OnDisable() {
            ZSRadarUI.RemoveRadarObject(this.gameObject);
        }
    }
}