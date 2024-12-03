using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeExitStage : BehaviourTreeNodeTask
    {
        private bool isStageExit = false;

        protected override void OnStart()
        {
            // Do ST
            isStageExit = true;
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (isStageExit)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }
    }
}