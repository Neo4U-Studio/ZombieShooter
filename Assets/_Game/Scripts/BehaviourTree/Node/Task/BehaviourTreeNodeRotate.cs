using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeRotate : BehaviourTreeNodeTask
    {
        public string ObjectKey;
        public Vector3 TargetRotation;
        public float Time = 0.5f;

        private bool isFinish = false;

        // Normal object
        private GameObject objectToRotate = null;

        protected override void OnStart()
        {
            var objectData = GetData(ObjectKey) as GameObject;
            if (objectData != null)
            {
                // Debug.Log("-- data " + objectData);
                objectToRotate = objectData;
            }
            isFinish = false;
            base.OnStart();
            DoRotate(); // Rotate object
        }
        
        protected override void OnStop()
        {
            objectToRotate = null;
            base.OnStop();
        }

        protected void DoRotate()
        {
            if (objectToRotate != null) {
                objectToRotate.transform.DOKill();
                objectToRotate.transform.DORotate(TargetRotation, Time)
                    .OnComplete(() => isFinish = true);
            }
        }

        protected override State OnUpdate()
        {
            if (objectToRotate != null)
            {
                if (isFinish)
                {
                    return State.Success;
                }
                else
                {
                    return State.Running;
                }
            }
            return State.Failure;
        }
    }
}