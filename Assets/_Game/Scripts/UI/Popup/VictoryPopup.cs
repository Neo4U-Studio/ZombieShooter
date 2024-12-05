using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPopup : PopupScene
{
    public override PopupType GetPopupType()
    {
        return PopupType.POPUP_VICTORY;
    }

    public void OnReturnMainMenu()
    {
        UIManager.Instance?.ClosePopup(this, () => {
            GameManager.Instance?.LoadMainMenu();
        });
    }
}
