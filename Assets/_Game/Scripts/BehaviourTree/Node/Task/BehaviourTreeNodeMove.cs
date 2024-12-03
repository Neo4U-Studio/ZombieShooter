using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeMove : BehaviourTreeNodeTask
    {
        public string ObjectKey;
        public Vector3 TargetPosition;
        public float Speed;
        public bool lookAtTarget = true;

        // Normal object
        private GameObject objectToMove = null;

        protected override void OnStart()
        {
            var objectData = GetData(ObjectKey) as GameObject;

            if (objectData != null)
            {
                // Debug.Log("-- data " + objectData);
                objectToMove = objectData;
            }
            base.OnStart();
        }

        protected override void OnStop()
        {
            objectToMove = null;
            base.OnStop();
        }

        protected override State OnUpdate()
        {
            if (objectToMove != null)
            {
                objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, TargetPosition, Speed * Time.deltaTime);
                if (lookAtTarget)
                {
                    objectToMove.transform.LookAt(TargetPosition);
                }
                if (Vector3.Distance(objectToMove.transform.position, TargetPosition) < 0.1f)
                {
                    return State.Success;
                }
                return State.Running;
            }
            return State.Failure;
        }
    }
}