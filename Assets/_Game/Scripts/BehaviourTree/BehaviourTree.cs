using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ZSBehaviourTree
{
    public class BehaviourTree : ScriptableObject
    {
        [HideInInspector] public BehaviourTreeNode RootNode;
        [HideInInspector] public BehaviourTreeNode.State TreeState = BehaviourTreeNode.State.Running;

        public static Action<BehaviourTreeNode> ON_NODE_START;
        public static Action<BehaviourTreeNode> ON_NODE_STOP;

        public Action<BehaviourTreeNode> OnNodeStart;
        public Action<BehaviourTreeNode> OnNodeStop;

        public List<BehaviourTreeNode> Nodes = new List<BehaviourTreeNode>();

        public void StartEvent()
        {
            if (RootNode != null)
            {
                TreeState = RootNode.Update();
            }
        }

        public BehaviourTreeNode.State Update()
        {
            if (RootNode.state == BehaviourTreeNode.State.Running)
            {
                TreeState = RootNode.Update();
            }
            else
            {
                TreeState = RootNode.state;
            }
            return TreeState;
        }
#if UNITY_EDITOR
        public BehaviourTreeNode CreateNode(System.Type type)
        {
            BehaviourTreeNode newNode = ScriptableObject.CreateInstance(type) as BehaviourTreeNode;
            newNode.name = type.Name;
            newNode.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "BehaviourTree (CreateNode)");
            Nodes.Add(newNode);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(newNode, this);
            }
            
            Undo.RegisterCreatedObjectUndo(newNode, "BehaviourTree (CreateNode)");
            
            AssetDatabase.SaveAssets();
            return newNode;
        }

        public void DeleteNode(BehaviourTreeNode nodeToRemove)
        {
            if (Nodes != null && Nodes.Contains(nodeToRemove))
            {
                Undo.RecordObject(this, "BehaviourTree (DeleteNode)");
                Nodes.Remove(nodeToRemove);

                // AssetDatabase.RemoveObjectFromAsset(nodeToRemove);
                Undo.DestroyObjectImmediate(nodeToRemove);

                AssetDatabase.SaveAssets();
            }
        }

        public bool AddChild(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            bool result;
            Undo.RecordObject(parent, "BehaviourTree (AddChild)");
            result = parent.Attach(child);
            EditorUtility.SetDirty(parent);
            return result;
        }

        public bool RemoveChild(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            bool result;
            Undo.RecordObject(parent, "BehaviourTree (RemoveChild)");
            result = parent.Detach(child);
            EditorUtility.SetDirty(parent);
            return result;
        }
#endif

        public static List<BehaviourTreeNode> GetChildren(BehaviourTreeNode parent)
        {
            List<BehaviourTreeNode> result = new List<BehaviourTreeNode>();

            BehaviourTreeNodeRoot root = parent as BehaviourTreeNodeRoot;
            if (root && root.child != null)
            {
                result.Add(root.child);
            }

            BehaviourTreeNodeSingle single = parent as BehaviourTreeNodeSingle;
            if (single && single.child != null)
            {
                result.Add(single.child);
            }

            BehaviourTreeNodeMulti multi = parent as BehaviourTreeNodeMulti;
            if (multi)
            {
                result.AddRange(multi.children);
            }

            return result;
        }

        public static void Traverse(BehaviourTreeNode node, System.Action<BehaviourTreeNode> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((child) => Traverse(child, visitor));
            }
        }

        public void BindData(string key, object value)
        {
            Traverse(RootNode, node => node.SetData(key, value));
        }

        public void UnbindData(string key)
        {
            Traverse(RootNode, node => node.RemoveData(key));
        }

        public object GetData(string key)
        {
            return RootNode.GetData(key);
        }

        public BehaviourTree Clone()
        {
            BehaviourTree cloneTree = Instantiate(this);
            cloneTree.RootNode = cloneTree.RootNode.Clone();
            cloneTree.Nodes = new List<BehaviourTreeNode>();
            Traverse(cloneTree.RootNode, (node) => {
                cloneTree.Nodes.Add(node);
            });
            return cloneTree;
        }
    }
}