using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeResetDelay : BehaviourTreeNodeSingle
    {
        public float delay = 10.0f;
        float startTime;

        protected override void OnStart() {
            startTime = Time.time;
            base.OnStart();
        }

        protected override State OnUpdate() {
            if (Time.time - startTime > delay) {
                AbortChild();
                startTime = Time.time;
            }

            return child.Update();
        }

        void AbortChild() {
            child.Abort();
        }
    }
}