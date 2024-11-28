using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public static class Utilities
    {
        public static IEnumerator DelayAction(float delayTime = 1f, Action onComplete = null)
        {
            yield return DelayUtils.Wait(delayTime);
            onComplete?.Invoke();
        }

        public static IEnumerator WaitEndOfFrame(Action onComplete = null)
        {
            yield return DelayUtils.WAIT_END_OF_FRAME;
            onComplete?.Invoke();
        }
    }
}