using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

public class LosePopup : PopupScene
{
    public override PopupType GetPopupType()
    {
        return PopupType.POPUP_LOSE;
    }

    public override void OnStartOpenPopup()
    {
        this.gameObject.SetActive(true);
    }

    public void OnReturnMainMenu()
    {
        SoundManager.Instance?.PauseSFX(SoundID.SFX_MENU_BUTTON_CLICK);
        GameManager.Instance?.LoadMainMenu();
        UIManager.Instance?.ClosePopup(this, null);
    }
}
