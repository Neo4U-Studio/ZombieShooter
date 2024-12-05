using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}

