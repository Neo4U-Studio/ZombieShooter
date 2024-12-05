using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

public class VictoryPopup : PopupScene
{
    public override PopupType GetPopupType()
    {
        return PopupType.POPUP_VICTORY;
    }

    public override void OnStartOpenPopup()
    {
        this.gameObject.SetActive(true);
    }

    public void OnReturnMainMenu()
    {
        SoundManager.Instance?.PauseSFX(SoundID.SFX_MENU_BUTTON_CLICK);
        UIManager.Instance?.ClosePopup(this, null);
        GameManager.Instance?.LoadMainMenu();
    }
}
