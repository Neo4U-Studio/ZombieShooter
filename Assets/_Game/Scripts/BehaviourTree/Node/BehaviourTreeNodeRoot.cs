using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeRoot : BehaviourTreeNode
    {
        public BehaviourTreeNode child = null;

        public override bool Attach(BehaviourTreeNode node)
        {
            if (this.child != node)
            {
                if (this.child != null)
                {
                    Detach(this.child);
                }
                node.parent = this;
                this.child = node;
                return true;
            }
            return false;
        }

        public override bool Detach(BehaviourTreeNode node)
        {
            if (this.child == node)
            {
                node.parent = null;
                this.child = null;
                return true;
            }
            return false;
        }

        protected override State OnUpdate()
        {
            if (child != null)
            {
                return child.Update();
            }
            else
            {
                return State.Failure;
            }
        }

        public override BehaviourTreeNode Clone()
        {
            var node = Instantiate(this);
            if (this.child != null)
            {
                node.child = this.child.Clone();
                node.child.parent = node;
            }
            return node;
        }
    }
}