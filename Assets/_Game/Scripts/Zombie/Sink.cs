using System.Collections;
using System.Collections.Generic;
using Pooling;
using UnityEngine;
using DG.Tweening;

namespace ZombieShooter
{
    public class Sink : MonoBehaviour
    {
        public static void StartSink(GameObject target, float duration, float delay)
        {
            var destroyHeight = Terrain.activeTerrain.SampleHeight(target.transform.position) - 5;
            if (target.gameObject.CompareTag(Utilities.ZOMBIE_TAG))
            {
                target.gameObject.GetComponent<ZombieController>().ToggleZombieCollider(false);
            }
            else
            {
                Collider[] colList = target.transform.GetComponentsInChildren<Collider>();
                foreach (Collider c in colList)
                {
                    c.enabled = false;
                }
            }
            
            SinkIntoGround(target, destroyHeight, duration, delay);
        }

        private static void SinkIntoGround(GameObject target, float destroyPosY, float duration = 1f, float delay = 0f)
        {
            target.transform.DOMoveY(destroyPosY, duration).SetDelay(delay).OnComplete(() => {
                if(target.gameObject.CompareTag(Utilities.ZOMBIE_TAG))
                {
                    target.gameObject.GetComponent<ZombieController>().DestroyZombie();
                }
                else
                {
                    Destroy(target.gameObject);
                }
            });
        }
    }
}