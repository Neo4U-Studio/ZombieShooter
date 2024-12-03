using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeRepeat : BehaviourTreeNodeSingle
    {
        public bool restartOnSuccess = true;
        public bool restartOnFailure = false;

        protected override State OnUpdate() {
            if (child != null)
            {
                switch (child.Update()) {
                    case State.Running:
                        break;
                    case State.Failure:
                        if (restartOnFailure) {
                            return State.Running;
                        } else {
                            return State.Failure;
                        }
                    case State.Success:
                        if (restartOnSuccess) {
                            return State.Running;
                        } else {
                            return State.Success;
                        }
                }
                return State.Running;
            }
            else
            {
                return State.Failure;
            }
            
        }
    }
}