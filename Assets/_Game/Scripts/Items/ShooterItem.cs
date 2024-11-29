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
                ToggleItem(!value);
            }
        }

        private bool consumed = false;

        private void Awake() {
            this.tag = Utilities.ITEM_TAG;
            consumed = false;
            ToggleItem(true);
        }

        public void ToggleItem(bool toggle)
        {
            this.gameObject.SetActive(toggle);
        }

        protected virtual eItemType GetItemType() { return eItemType.Empty; }
    }
}