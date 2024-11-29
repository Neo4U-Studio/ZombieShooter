using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public abstract class ShooterItem : MonoBehaviour
    {
        public eItemType Type => GetItemType();
        public bool Consumed
        {
            get => consumed;
            set
            {
                consumed = value;
                if (value)
                {
                    PlayItemConsumeSound();
                }
                ToggleItem(!value);
            }
        }

        private bool consumed = false;

        protected virtual eItemType GetItemType() { return eItemType.Empty; }

        private void Awake() {
            this.tag = Utilities.ITEM_TAG;
            consumed = false;
            ToggleItem(true);
        }

        public void ToggleItem(bool toggle)
        {
            this.gameObject.SetActive(toggle);
        }

        public virtual void PlayItemConsumeSound()
        { 
            // Init in subclass
        }
    }
}