using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeTimeout : BehaviourTreeNodeSingle
    {
        public float duration = 1.0f;
        float startTime;

        protected override void OnStart() {
            startTime = Time.time;
            base.OnStart();
        }

        protected override State OnUpdate() {
            if (Time.time - startTime > duration) {
                return State.Failure;
            }

            return child.Update();
        }
    }
}