using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class ZSGunClipUI : MonoBehaviour
    {
        [SerializeField] Text ammoText;
        [SerializeField] Text ammoMaxText;

        [SerializeField] Color normalColor;
        [SerializeField] Color midColor;
        [SerializeField] Color lowColor;

        public void SetAmmo(int current, int max)
        {
            ammoMaxText.text = max.ToString();
            ammoText.text = current.ToString();
            ammoText.color = GetColor(current, max);
        }

        private Color GetColor(int current, int max)
        {
            float percentage = Mathf.Clamp01((float)current / (float)max);
            if (percentage > 0.5f)
            {
                return normalColor;
            }
            else if (0.2f < percentage && percentage <= 0.5f)
            {
                return midColor;
            }
            else
            {
                return lowColor;
            }
        }
    }
}