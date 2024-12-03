using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeSucceed : BehaviourTreeNodeSingle
    {
        protected override State OnUpdate() {
            var childState = child.Update();
            if (childState == State.Failure) {
                return State.Success;
            }
            return childState;
        }
    }
}