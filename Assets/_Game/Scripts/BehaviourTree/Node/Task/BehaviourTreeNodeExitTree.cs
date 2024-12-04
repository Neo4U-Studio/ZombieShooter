using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeExitTree : BehaviourTreeNodeTask
    {
        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}