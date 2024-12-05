using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

public class MainMenuUI : MenuScene
{
    public override MenuType GetMenuType()
    {
        return MenuType.MAIN_MENU;
    }

    public void OnClickStart()
    {
        SoundManager.Instance?.PauseSFX(SoundID.SFX_MENU_BUTTON_START);
        GameManager.Instance?.LoadGame();
    }

    public void OnClickOption()
    {
        SoundManager.Instance?.PauseSFX(SoundID.SFX_MENU_BUTTON_CLICK);
    }

    public void OnClickQuit()
    {
        Debug.Log("[Main Menu] Quit Game");
        SoundManager.Instance?.PauseSFX(SoundID.SFX_MENU_BUTTON_CANCEL);
        Application.Quit();
    }
}
