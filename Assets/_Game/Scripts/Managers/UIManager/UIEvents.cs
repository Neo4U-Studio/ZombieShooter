using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UIEvents
{
    public static Func<MenuType, Action, MenuScene> UI_SHOW_MENU;
    public static Action<Action> UI_POP_MENU;
    public static Func<PopupType, Action, PopupScene> UI_SHOW_POPUP;
    public static Func<PopupType, Action, PopupScene> UI_SHOW_TOP_POPUP;
    public static Action<Action> UI_HIDE_TOP_POPUP;
    public static Action<PopupScene, Action> UI_HIDE_POPUP;
    public static Action UI_SHOW_LOADING;
    public static Action UI_HIDE_LOADING;
    public static Action<bool> UI_SET_BLOCK_INPUT;
    public static Action<Camera> UI_ATTACH_UI_CAMERA;
    public static Action UI_CLEAR_MENU_STACK;
}