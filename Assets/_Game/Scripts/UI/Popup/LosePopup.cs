using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        UIManager.Instance?.ClosePopup(this, () => {
            GameManager.Instance?.LoadMainMenu();
        });
    }
}
