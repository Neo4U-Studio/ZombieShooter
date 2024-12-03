using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeTrueFalse : BehaviourTreeNodeMulti // Only 2 connect children for this node type
    {
        public bool Result = true;

        public override bool Attach(BehaviourTreeNode node)
        {
            if (!this.children.Contains(node) && this.children.Count < 2)
            {
                node.parent = this;
                this.children.Add(node);
                return true;
            }
            return false;
        }

        public override bool Detach(BehaviourTreeNode node)
        {
            if (this.children.Contains(node))
            {
                node.parent = null;
                this.children.Remove(node);
                return true;
            }
            return false;
        }

        protected virtual void CheckResult()
        {
            // Init result in subclass
            Result = true;
        }

        protected override State OnUpdate()
        {
            CheckResult();
            switch (children.Count)
            {
                case 1:
                    return children[0].Update();
                case 2:
                    if (Result)
                    {
                        return children[0].Update();
                    }
                    else
                    {
                        return children[1].Update();
                    }
                default:
                    return State.Failure;
            }
        }
    }
}