using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayUtils
{
    public static class Scaled
    {
        public static readonly WaitForSeconds WAIT_ONE_TENTH_SEC = new WaitForSeconds(0.1f);
        public static readonly WaitForSeconds WAIT_ONE_FIFTH_SEC = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds WAIT_ONE_FOURTH_SEC = new WaitForSeconds(0.25f);
        public static readonly WaitForSeconds WAIT_ONE_THIRD_SEC = new WaitForSeconds(0.3333f);
        public static readonly WaitForSeconds WAIT_HALF_SEC = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds WAIT_1_SEC = new WaitForSeconds(1f);
        public static readonly WaitForSeconds WAIT_2_SEC = new WaitForSeconds(2f);
        public static readonly WaitForSeconds WAIT_3_SEC = new WaitForSeconds(3f);
        public static readonly WaitForSeconds WAIT_4_SEC = new WaitForSeconds(4f);
        public static readonly WaitForSeconds WAIT_5_SEC = new WaitForSeconds(5f);
        public static readonly WaitForSeconds WAIT_6_SEC = new WaitForSeconds(6f);
        public static readonly WaitForSeconds WAIT_7_SEC = new WaitForSeconds(7f);
        public static readonly WaitForSeconds WAIT_8_SEC = new WaitForSeconds(8f);
        public static readonly WaitForSeconds WAIT_9_SEC = new WaitForSeconds(9f);
        public static readonly WaitForSeconds WAIT_10_SEC = new WaitForSeconds(10f);

    }

    public static readonly WaitForEndOfFrame WAIT_END_OF_FRAME = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WAIT_FIXED_UPDATE = new WaitForFixedUpdate();

    private static Dictionary<float, WaitForSeconds> cachedWfs = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Wait(float duration)
    {
        if (cachedWfs.ContainsKey(duration))
        {
            return cachedWfs[duration];
        }
        else
        {
            WaitForSeconds wfs = new WaitForSeconds(duration);
            cachedWfs.Add(duration, wfs);
            return wfs;
        }
    }

    public static IEnumerator WaitNull(int times)
    {
        while (times > 0)
        {
            yield return null;
            times--;
        }
    }
}
