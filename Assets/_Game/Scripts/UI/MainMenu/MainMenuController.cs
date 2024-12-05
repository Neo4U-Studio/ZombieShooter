using System.Collections;
using System.Collections.Generic;
using AudioPlayer;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private AudioSource themeMusic = null;
    void Start()
    {
        themeMusic = SoundManager.Instance?.PlayMusic(SoundID.MUSIC_THEME_MAINMENU);
        UIManager.Instance?.PushMenu(MenuType.MAIN_MENU);
        UIEvents.UI_SET_BLOCK_INPUT?.Invoke(false);
    }

    private void OnDestroy() {
        if (themeMusic)
        {
            themeMusic.Stop();
            themeMusic = null;
        }
    }
}
