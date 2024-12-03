using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeDebugLog : BehaviourTreeNodeTask
    {
        public string Message;

        protected override void OnStart()
        {
            Debug.Log($"[Playmobil] Node DebugLog OnStart {Message}");
            base.OnStart();
        }
        
        protected override void OnStop()
        {
            Debug.Log($"[Playmobil] Node DebugLog OnStop {Message}");
            base.OnStop();
        }

        protected override State OnUpdate()
        {
            Debug.Log($"[Playmobil] Node DebugLog OnUpdate {Message}");
            return State.Success;
        }
    }
}