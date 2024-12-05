using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZSBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] public BehaviourTree tree;

        private bool isTreeStarted = false;

        public void RunTree(Action<BehaviourTree> runningTreeCallback = null)
        {
            if (tree != null && !isTreeStarted)
            {
                tree = tree.Clone();
                runningTreeCallback?.Invoke(tree);

                tree.StartEvent();
                isTreeStarted = true;
            }
        }

        void Update()
        {
            if (tree != null && isTreeStarted)
            {
                if (tree.TreeState == BehaviourTreeNode.State.Running)
                {
                    tree.Update();
                }
                else
                {
                    StopTree();
                }
            }
        }

        private void StopTree()
        {
            if (tree != null && isTreeStarted)
            {
                isTreeStarted = false;
            }
        }
    }
}