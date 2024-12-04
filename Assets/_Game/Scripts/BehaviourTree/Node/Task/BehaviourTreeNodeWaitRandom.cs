using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeWaitRandom : BehaviourTreeNodeTask
    {
        public Vector2 duration = Vector2.one;
        float startTime;

        float currentDuration;

        protected override void OnStart() {
            startTime = Time.time;
            currentDuration = UnityEngine.Random.Range(duration.x, duration.y);
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (Time.time - startTime > currentDuration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}