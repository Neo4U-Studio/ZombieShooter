using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MenuInfo
{
    public MenuType type;
    //public GameObject prefab;
    public string prefabPath;
}

[System.Serializable]
public class PopupInfo
{
    public PopupType type;
    //public GameObject prefab;
    public string prefabPath;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] Camera uiCamera;
    public Camera UICamera { get { return this.uiCamera; } }
    [SerializeField] MenuConfig menuConfigs;
    [SerializeField] PopupConfig popupConfigs;
    [SerializeField] GameObject menuContainer;
    [SerializeField] GameObject popupContainer;
    [SerializeField] GameObject blockInput;

    //private Dictionary<MenuType, GameObject> menuPrefabs;
    private Dictionary<MenuType, string> menuPrefabsPath;
    private MenuScene currentMenu;


    //private Dictionary<PopupType, GameObject> popupPrefabs;
    private Dictionary<PopupType, string> popupPrefabsPath;
    private Dictionary<PopupType, Stack<PopupScene>> popupPool;

    private PopupScene loadingScreen;

    public static UIManager Instance = null;

    private Stack<MenuType> menuStack;
    private List<PopupScene> popupList;
    private int _mainCameraID = -1;
    private CanvasGroup canvasGroup;
    // private bool isInTransition;

    public Canvas UICanvas;
    private MenuScene menuToDestroy;

    private GameObject topPopupContainer;

    private bool canShowForceDownloadPopup;

    private void Awake()
    {
        Instance = this;

        //menuPrefabs = new Dictionary<MenuType, GameObject>();
        this.menuPrefabsPath = new Dictionary<MenuType, string>();
        int num = menuConfigs.menuInfos.Count;
        for (int i = 0; i < num; ++i)
        {
            //menuPrefabs.Add(menuConfigs.menuInfos[i].type, menuConfigs.menuInfos[i].prefab);
            this.menuPrefabsPath.Add(menuConfigs.menuInfos[i].type, menuConfigs.menuInfos[i].prefabPath);
        }

        menuStack = new Stack<MenuType>();

        //popupPrefabs = new Dictionary<PopupType, GameObject>();
        popupPrefabsPath = new Dictionary<PopupType, string>();
        popupPool = new Dictionary<PopupType, Stack<PopupScene>>();
        num = popupConfigs.popupInfos.Count;
        for (int i = 0; i < num; ++i)
        {
            //popupPrefabs.Add(popupConfigs.popupInfos[i].type, popupConfigs.popupInfos[i].prefab);
            popupPrefabsPath.Add(popupConfigs.popupInfos[i].type, popupConfigs.popupInfos[i].prefabPath);
            popupPool.Add(popupConfigs.popupInfos[i].type, new Stack<PopupScene>());
        }

        popupList = new List<PopupScene>();

        // isInTransition = false;
        this.canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        UIEvents.UI_SHOW_MENU += PushMenu;
        UIEvents.UI_POP_MENU += PopMenu;
        UIEvents.UI_SHOW_POPUP += PushPopup;
        UIEvents.UI_HIDE_POPUP += ClosePopup;
        UIEvents.UI_HIDE_TOP_POPUP += Pop_Popup;
        UIEvents.UI_SHOW_LOADING += ShowLoading;
        UIEvents.UI_HIDE_LOADING += HideLoading;
        UIEvents.UI_SET_BLOCK_INPUT += SetBlockInput;
        UIEvents.UI_ATTACH_UI_CAMERA += AttachUICameraToCamera;
        UIEvents.UI_CLEAR_MENU_STACK += ClearMenuStack;
        UIEvents.UI_SHOW_TOP_POPUP += PushTopPopup;
    }

    private void OnDisable()
    {
        UIEvents.UI_SHOW_MENU -= PushMenu;
        UIEvents.UI_POP_MENU -= PopMenu;
        UIEvents.UI_SHOW_POPUP -= PushPopup;
        UIEvents.UI_HIDE_POPUP -= ClosePopup;
        UIEvents.UI_HIDE_TOP_POPUP -= Pop_Popup;
        UIEvents.UI_SHOW_LOADING -= ShowLoading;
        UIEvents.UI_HIDE_LOADING -= HideLoading;
        UIEvents.UI_SET_BLOCK_INPUT -= SetBlockInput;
        UIEvents.UI_ATTACH_UI_CAMERA -= AttachUICameraToCamera;
        UIEvents.UI_CLEAR_MENU_STACK -= ClearMenuStack;
        UIEvents.UI_SHOW_TOP_POPUP -= PushTopPopup;
    }


    public void SetUICamera(Camera uiCamera)
    {
        this.uiCamera = uiCamera;
        this.uiCamera.depth = -10;
        this.UICanvas = GetComponent<Canvas>();
        this.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        this.UICanvas.worldCamera = this.uiCamera;
    }

    public void AttachUICameraToMainCamera()
    {
        if (Camera.main == null || this.uiCamera == null)
            return;
        // Debug.Log("AttachUICameraToMainCamera name " + Camera.main.name);
        _mainCameraID = Camera.main.GetInstanceID();
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        if (cameraData == null)
            return;
        //overlay camera returns null
        if (cameraData.cameraStack == null)
            return;
        if (!cameraData.cameraStack.Contains(this.uiCamera))
            cameraData.cameraStack.Add(this.uiCamera);
    }

    public void AttachUICameraToCamera(Camera camera)
    {
        if (camera == null || this.uiCamera == null)
            return;
        Debug.Log("AttachUICameraToMainCamera name " + camera.name);
        var cameraData = camera.GetUniversalAdditionalCameraData();
        if (cameraData == null)
            return;
        //overlay camera returns null
        if (cameraData.cameraStack == null)
            return;
        if (!cameraData.cameraStack.Contains(this.uiCamera))
            cameraData.cameraStack.Add(this.uiCamera);
    }

    private MenuScene SpawnNewMenu(MenuType type)
    {
        GameObject prefab = Resources.Load(this.menuPrefabsPath[type]) as GameObject;  //menuPrefabs[type];
        GameObject obj = GameObject.Instantiate(prefab, menuContainer.transform);
        obj.name = prefab.name;
        MenuScene menuScene = obj.GetComponent<MenuScene>();

        return menuScene;
    }

    private PopupScene SpawnPopup(PopupType type)
    {
        GameObject obj = null;

        if (popupPool.ContainsKey(type))
        {
            if (popupPool[type].Count > 0)
            {
                obj = popupPool[type].Pop().gameObject;
            }
            else
            {
                GameObject prefab = Resources.Load(this.popupPrefabsPath[type]) as GameObject; //popupPrefabs[type];
                obj = GameObject.Instantiate(prefab, popupContainer.transform);
            }
        }

        obj.SetActive(false);

        PopupScene popupScene = obj.GetComponent<PopupScene>();

        return popupScene;
    }

    public MenuScene GetTopMenu()
    {
        return currentMenu;
    }

    public bool IsMenuInStack(MenuType menuType)
    {
        foreach (MenuType item in menuStack)
        {
            if (item == menuType) return true;
        }
        return false;
    }

    public MenuType GetTopMenuType()
    {
        if (menuStack.Count == 0)
            return MenuType.NONE;

        return menuStack.Peek();
    }
    public PopupScene GetTopPopup()
    {
        if (popupList.Count == 0)
            return null;

        return popupList[popupList.Count - 1];
    }
    public PopupType GetTopPopupType()
    {
        if (popupList.Count == 0)
            return PopupType.NONE;

        return popupList[popupList.Count - 1].GetPopupType();
    }

    public void ShowLoading()
    {
        if (loadingScreen == null || !loadingScreen.IsPopupShow())
            loadingScreen = PushPopup(PopupType.POPUP_LOADING_SCREEN, () =>
            {
                if (loadingScreen != null)
                    loadingScreen.transform.SetAsLastSibling();
            });
    }

    public void HideLoading()
    {
        if (loadingScreen != null && loadingScreen.IsPopupShow())
            ClosePopup(loadingScreen, null);
    }

    public void Update()
    {
        if (Time.frameCount % 60 == 0 && Camera.main != null && this._mainCameraID != Camera.main.GetInstanceID())
        {
            this.AttachUICameraToMainCamera();
        }

        if (this.menuToDestroy != null)
        {
            Debug.Log("Destroy Menu " + this.menuToDestroy.name);
            Destroy(this.menuToDestroy.gameObject);
            this.menuToDestroy = null;
            //Resources.UnloadUnusedAssets();
        }
    }

    public MenuScene PushMenu(MenuType type, System.Action endAction = null)
    {
        // if (isInTransition) return null;
        Debug.Log("UIManager Push menu " + type.ToString());
        MenuScene menuScene;
        if (this.currentMenu != null && this.currentMenu.GetMenuType() == type)
        {
            StartCoroutine(PlayEndActionDelay(endAction));
            this.currentMenu.gameObject.SetActive(false);
            this.currentMenu.gameObject.SetActive(true);
            return this.currentMenu;
        }
        else menuScene = SpawnNewMenu(type);

        StartCoroutine(PushMenu_Couroutine(type, menuScene, endAction));

        return menuScene;
    }

    private IEnumerator PlayEndActionDelay(System.Action endAction = null)
    {
        yield return null;
        endAction?.Invoke();
    }

    private IEnumerator PushMenu_Couroutine(MenuType type, MenuScene menuScene, System.Action endAction)
    {
        // Debug.Log("[UIManager] START Push menu " + menuScene.gameObject.name);
        menuScene.OnCreateMenu();

        yield return menuScene.OpenMenu();

        if (currentMenu != null)
        {
            yield return this.currentMenu.CloseMenu();
            this.currentMenu.SetIsShown(false);
            this.menuToDestroy = this.currentMenu;

            yield return null;
        }

        this.currentMenu = menuScene;

        menuStack.Push(type);

        //yield return null;
        if (endAction != null)
            endAction();

        // Debug.Log("[UIManager] END Push menu " + menuScene.gameObject.name);
    }

    public PopupScene PushPopup(PopupType type, System.Action endAction = null)
    {
        // if (isInTransition) return null;

        PopupScene popupScene = SpawnPopup(type);
        StartCoroutine(PushPopup_Couroutine(type, popupScene, endAction));

        return popupScene;
    }

    public PopupScene PushTopPopup(PopupType type, System.Action endAction = null)
    {
        PopupScene popupScene = SpawnPopup(type);
        if (topPopupContainer == null)
            topPopupContainer = GameManager.Instance.GetTopPopupCanvas();
        if (topPopupContainer != null)
            popupScene.transform.SetParent(topPopupContainer.transform, false);
        StartCoroutine(PushPopup_Couroutine(type, popupScene, endAction));

        return popupScene;
    }

    private IEnumerator PushPopup_Couroutine(PopupType type, PopupScene popupScene, System.Action endAction)
    {
        // isInTransition = true;
        popupScene.transform.SetAsLastSibling();
        popupScene.OnCreatePopup();

        popupList.Add(popupScene);

        yield return popupScene.OpenPopup();

        // isInTransition = false;

        if (endAction != null)
            endAction();

        Debug.Log("[UIManager] Push popup " + popupScene.gameObject.name);
    }

    public void PopMenu(System.Action endAction = null)
    {
        Debug.Log("[UIManager] PopMenu");
        if (menuStack.Count > 1)
            StartCoroutine(PopMenu_Couroutine(endAction));
        else
        {
            Debug.LogWarning("We only have one menu in stack!!!");
            endAction?.Invoke();
        }
    }

    private IEnumerator PopMenu_Couroutine(System.Action endAction)
    {
        if (menuStack.Count > 1)
        {
            MenuType menuType = menuStack.Pop();
            this.currentMenu.SetIsShown(false);
            this.menuToDestroy = this.currentMenu;

            yield return this.currentMenu.CloseMenu();

            yield return null;

            menuType = menuStack.Pop();
            MenuScene menuScene = SpawnNewMenu(menuType);

            yield return PushMenu_Couroutine(menuType, menuScene, null);
        }
        else
        {
            yield return this.currentMenu.CloseMenu();
        }

        yield return null;

        if (endAction != null)
            endAction();

        Debug.Log("[UIManager] Pop menu");
    }

    public void Pop_Popup(System.Action endAction)
    {
        if (popupList.Count > 0)
        {
            StartCoroutine(ClosePopup_Coroutine(popupList[popupList.Count - 1], endAction));
        }
    }

    public void ClosePopup(PopupScene popupScene, System.Action endAction)
    {
        if (popupList.Contains(popupScene))
        {
            StartCoroutine(ClosePopup_Coroutine(popupScene, endAction));
        }
    }

    private IEnumerator ClosePopup_Coroutine(PopupScene popupScene, System.Action endAction)
    {
        popupList.Remove(popupScene);
        yield return popupScene.ClosePopup();

        if (popupPool.ContainsKey(popupScene.GetPopupType()))
        {
            popupPool[popupScene.GetPopupType()].Push(popupScene);
        }

        yield return null;

        if (endAction != null)
            endAction();
    }

    public bool IsMenuOnScreen()
    {
        return (menuStack.Count > 0);
    }

    public void RefreshUI()
    {
        if (this.currentMenu != null)
        {
            this.currentMenu.RefreshUI();
        }
    }

    private void SetBlockInput(bool value)
    {
        blockInput.SetActive(value);
        
        if (this.blockInputCor != null)
            StopCoroutine(this.blockInputCor);
    }

    public bool IsBlockInput()
    {
        return blockInput.activeSelf;
    }


    private Coroutine blockInputCor;
    /// <summary>
    /// This method is used for set the Block Input value for (n) seconds.
    /// </summary>
    public void SetBlockInputIn(bool value, float duration)
    {
        UIEvents.UI_SET_BLOCK_INPUT(value);
        blockInputCor = StartCoroutine(DelaySetUnlockInput(!value, duration));
    }
    private IEnumerator DelaySetUnlockInput(bool value, float duration)
    {
        yield return DelayUtils.Wait(duration);
        UIEvents.UI_SET_BLOCK_INPUT(value);
    }

    public bool IsPopUpOnScreen(PopupType popupType)
    {
        for (int i = 0; i < popupList.Count; i++)
        {
            if (popupList[i].GetPopupType() == popupType)
                return true;
        }

        return false;
    }

    public static bool IsPointerOverGameObject()
    {
        //check mouse
#if UNITY_EDITOR || UNITY_STANDALONE
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return true;
#endif
        //check touch
        if (Input.touchCount > 0)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;

            if (Input.touchCount > 1)
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[1].fingerId))
                    return true;
            }
        }

        return false;
    }

    public void SetCanvasTransparent(float value, float time = 0.5f)
    {
        //this.canvasGroup.alpha = value;
        this.canvasGroup.DOFade(value, time);
    }

    public void SetCanvasRenderMode(RenderMode mode)
    {
        this.GetComponent<Canvas>().renderMode = mode;
    }

    public void RemoveMenuFromStack(MenuType menu)
    {
        Stack<MenuType> tempStack = new Stack<MenuType>();
        while (this.menuStack.Count > 0)
        {
            MenuType curType = this.menuStack.Pop();
            if (curType == menu)
                break;
            else
                tempStack.Push(curType);
        }

        while (tempStack.Count > 0)
        {
            this.menuStack.Push(tempStack.Pop());
        }
    }

    public void ClearMenuStack()
    {
        if (this.menuStack != null && this.menuStack.Count > 1)
        {
            MenuType curMenu = this.menuStack.Pop();
            this.menuStack.Clear();
            this.menuStack.Push(curMenu);
        }
    }

    public void ClearMenuStackAndLeaveBlank()
    {
        PushMenu(MenuType.BLANK);
        ClearMenuStack();
    }

    public bool CanShowForceDownloadPopup()
    {
        return this.canShowForceDownloadPopup;
    }

    public GameObject GetPopupContainer()
    {
        return this.popupContainer;
    }
}
