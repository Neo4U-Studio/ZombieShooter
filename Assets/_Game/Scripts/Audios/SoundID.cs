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
        MUSIC_THEME_MAINMENU,
        MUSIC_THEME_GAME,
        MUSIC_THEME_BOSS,
        MUSIC_VICTORY,

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
        SFX_ZS_ZOMBIE_MOAN_1,
        SFX_ZS_ZOMBIE_MOAN_2,

        // Environment
        SFX_ZS_ENV_CRICKET = 400,
        SFX_ZS_ENV_WOLF,
        SFX_ZS_ENV_WIND_LIGHT,
        SFX_ZS_ENV_WIND_SPOOKY,
        SFX_ZS_ENV_WIND_HARD,

        // Sfx
        SFX_MENU_BUTTON_CLICK = 500,
        SFX_MENU_BUTTON_OVER,
        SFX_MENU_BUTTON_START,
        SFX_MENU_BUTTON_CANCEL,
    }
}