using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeSelector : BehaviourTreeNodeMulti
    {
        protected int currentIndex = 0;

        protected override void OnStart() {
            currentIndex = 0;
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            for (int i = currentIndex; i < children.Count; ++i) {
                currentIndex = i;
                var child = children[currentIndex];

                switch (child.Update()) {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                    case State.Failure:
                        continue;
                }
            }

            return State.Failure;
        }
    }
}