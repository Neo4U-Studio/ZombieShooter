using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance?.PushMenu(MenuType.MAIN_MENU);
        UIEvents.UI_SET_BLOCK_INPUT?.Invoke(false);
    }
}
