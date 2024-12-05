using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class ZSGunClipUI : MonoBehaviour
    {
        [SerializeField] GameObject normalBG;
        [SerializeField] GameObject selectedBG;
        [SerializeField] Image icon;
        [SerializeField] Text ammoText;
        [SerializeField] Text ammoMaxText;
        [SerializeField] Text remainingAmmoText;

        [SerializeField] Color normalColor;
        [SerializeField] Color midColor;
        [SerializeField] Color lowColor;

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetAmmo(int current, int max)
        {
            ammoMaxText.text = max < 0 ? "Inf" : max.ToString();
            ammoText.text = current < 0 ? "Inf" : current.ToString();
            ammoText.color = GetColor(current, max);
        }

        public void SetRemainingAmmo(int value)
        {
            remainingAmmoText.text = value < 0 ? "Inf" : value.ToString();
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

        public void ToggleSelect(bool toggle)
        {
            normalBG.SetActive(!toggle);
            selectedBG.SetActive(toggle);
        }
    }
}