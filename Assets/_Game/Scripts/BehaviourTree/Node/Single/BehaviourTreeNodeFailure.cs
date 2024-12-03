using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeFailure : BehaviourTreeNodeSingle
    {
        protected override State OnUpdate() {
            var childState = child.Update();
            if (childState == State.Success) {
                return State.Failure;
            }
            return childState;
        }
    }
}