using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public abstract class BehaviourTreeNode : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [HideInInspector] public bool isStarted = false;
        [HideInInspector] public State state = State.Running;

        [HideInInspector] public string guid; // For Editor
        [HideInInspector] public Vector2 positionView; // For Editor
        [TextArea] public string description; // For Editor

        public BehaviourTreeNode parent = null;

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

#region Node Behaviour
        public State Update() // For the first node
        {
            if (!isStarted)
            {
                isStarted = true;
                this.state = State.Running;
                OnStart();
            }

            state = OnUpdate();

            if (state == State.Failure || state == State.Success)
            {
                isStarted = false;
                OnStop();
            }

            return state;
        }

        protected virtual void OnStart() {
            // Debug.Log($"--[ZS] Event Node Start {this.name}");
        }

        // Remove this later
        protected virtual void OnStop() {
            // Debug.Log($"--[ZS] Event Node Stop {this.name}");
        }

        protected virtual State OnUpdate() { return State.Failure; }

        public virtual bool Attach(BehaviourTreeNode node) { return true; }
        public virtual bool Detach(BehaviourTreeNode node) { return true; }

        public virtual BehaviourTreeNode Clone()
        {
            var node = Instantiate(this);
            return node;
        }

        public void Abort() {
            BehaviourTree.Traverse(this, (node) => {
                node.isStarted = false;
                node.state = State.Running;
                node.OnStop();
            });
        }
#endregion

#region Node Data
        public void SetData(string key, object value)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext[key] = value;
            }
            else
            {
                dataContext.Add(key, value);
            }
        }

        public object GetData(string key)
        {
            if (dataContext.TryGetValue(key, out object result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public void RemoveData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
            }
        }
#endregion
    }
}