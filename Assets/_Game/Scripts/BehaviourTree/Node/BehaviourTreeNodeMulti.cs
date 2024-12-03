using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ZSBehaviourTree
{
    public abstract class BehaviourTreeNodeMulti : BehaviourTreeNode
    {
        public List<BehaviourTreeNode> children = new List<BehaviourTreeNode>();

        public override bool Attach(BehaviourTreeNode node)
        {
            if (!this.children.Contains(node))
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

        public override BehaviourTreeNode Clone()
        {
            var node = Instantiate(this);
            node.children = this.children.ConvertAll(child => child.Clone());
            node.children.ForEach(child => child.parent = node);
            return node;
        }
    }
}