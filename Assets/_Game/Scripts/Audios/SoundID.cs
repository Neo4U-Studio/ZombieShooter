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

        // Player
        SFX_ZS_PLAYER_FOOTSTEP = 200,
        SFX_ZS_PLAYER_JUMP,
        SFX_ZS_PLAYER_LAND,
        SFX_ZS_PLAYER_SHOT,
        SFX_ZS_PLAYER_DEATH,
        SFX_ZS_PLAYER_RELOAD,
        SFX_ZS_PLAYER_EMPTY_AMMO,

        // Items
        SFX_ZS_ITEM_MEDKIT = 250,
        SFX_ZS_ITEM_AMMO,

        // Zombie
        SFX_ZS_ZOMBIE_SPLAT = 300,
    }
}