using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioPlayer
{
    [Serializable]
    public enum SoundID
    {
        NONE = 0,

        // Shooting Game
        SFX_ZS_FOOTSTEP = 200,
        SFX_ZS_JUMP,
        SFX_ZS_LAND,
        SFX_ZS_SHOT,

        SFX_ZS_ITEM_MEDKIT = 250,
        SFX_ZS_ITEM_AMMO,
    }
}