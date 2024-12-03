using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeWait : BehaviourTreeNodeTask
    {
        public float duration = 1;
        float startTime;

        protected override void OnStart() {
            startTime = Time.time;
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (Time.time - startTime > duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}