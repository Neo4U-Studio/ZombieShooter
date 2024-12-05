using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MenuScene
{
    public override MenuType GetMenuType()
    {
        return MenuType.MAIN_MENU;
    }

    public void OnClickStart()
    {
        GameManager.Instance?.LoadGame();
    }

    public void OnClickOption()
    {
        // Do something
    }

    public void OnClickQuit()
    {
        Debug.Log("[Main Menu] Quit Game");
        Application.Quit();
    }
}
