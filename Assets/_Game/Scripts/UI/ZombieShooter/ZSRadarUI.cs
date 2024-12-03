using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class ZSRadarUI : MonoBehaviour
    {
        [SerializeField] Transform itemContainer;
        [SerializeField] float mapScale = 2.0f;

        private Transform player;

        public static Dictionary<GameObject, Image> radObjects = new Dictionary<GameObject, Image>();

        private bool isScanning = false;
        public static Transform ItemContainer;

        private void Awake() {
            isScanning = false;
            if (!itemContainer)
            {
                itemContainer = this.transform;
            }
            ItemContainer = itemContainer;
        }

        public void SetPlayer(Transform player)
        {
            this.player = player;
        }

        public static void RegisterRadarObject(GameObject owner, Image img)
        {
            if (!radObjects.ContainsKey(owner))
            {
                Image image = Instantiate(img, ItemContainer);
                radObjects.Add(owner, image);
            }
        }

        public static void RemoveRadarObject(GameObject owner)
        {
            if (radObjects.ContainsKey(owner))
            {
                if (radObjects[owner])
                {
                    Destroy(radObjects[owner].gameObject);
                }
                radObjects.Remove(owner);
            }
        }

        public void ToggleRadar(bool toggle)
        {
            isScanning = toggle;
        }

        void Update()
        {
            if (player == null || !isScanning) return;
            foreach (var ro in radObjects)
            {
                Vector3 radarPos = ro.Key.transform.position - player.position;
                float distToObject = Vector3.Distance(player.position, ro.Key.transform.position) * mapScale;

                float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - player.eulerAngles.y;
                radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
                radarPos.z = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);


                RectTransform rt = itemContainer.transform as RectTransform;
                ro.Value.transform.position = new Vector3(radarPos.x + rt.pivot.x, radarPos.z + rt.pivot.y, 0) 
                        + itemContainer.transform.position;
            }
        }
    }
}