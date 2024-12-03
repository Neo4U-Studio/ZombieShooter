using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeRandomSelector : BehaviourTreeNodeMulti
    {
        protected int selectIndex = 0;

        protected override void OnStart() {
            selectIndex = -1;
            if (children.Count > 0)
            {
                selectIndex = UnityEngine.Random.Range(0, children.Count);
            }
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (selectIndex >= 0)
            {
                return children[selectIndex].Update();
            }
            else
            {
                return State.Failure;
            }
        }
    }
}