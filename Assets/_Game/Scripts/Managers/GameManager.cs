using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject managersContainerPrefab;
    [SerializeField] GameObject eventSystemPrefab;
    [SerializeField] GameObject canvasPrefab;
    [SerializeField] GameObject topPopupPrefab;
    [SerializeField] Camera uiCamera;

    [SerializeField] ManagerConfig managerConfig;

    public bool IsInitialized { get { return isInitialized; } }
    private bool isInitialized;

    private GameObject managersContainer;
    private GameObject eventSystem;
    private GameObject mainCanvas;
    private GameObject topPopupCanvas;

    private void Awake()
    {
#if ENABLE_CHEAT || UNITY_EDITOR
        Debug.unityLogger.filterLogType = LogType.Log;
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.filterLogType = LogType.Error | LogType.Exception;
        Debug.unityLogger.logEnabled = false;
#endif

        isInitialized = false;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        managersContainer = Instantiate(managersContainerPrefab);
        transform.parent = managersContainer.transform;
        DontDestroyOnLoad(managersContainer);

        eventSystem = Instantiate(eventSystemPrefab);
        DontDestroyOnLoad(eventSystem);

        mainCanvas = Instantiate(canvasPrefab);
        DontDestroyOnLoad(mainCanvas);

        topPopupCanvas = Instantiate(this.topPopupPrefab);
        DontDestroyOnLoad(topPopupCanvas);

        UIManager.Instance.SetUICamera(this.uiCamera);

        CreateManagers();

#if UNITY_STANDALONE
        float ratio = (float)Screen.width/(float)Screen.height;
        float saveRatio = PlayerPrefs.GetFloat("ScreenRatio", ratio);
        PlayerPrefs.SetFloat("ScreenRatio", saveRatio);
#endif
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    protected void CreateManagers()
    {
        for (int i = 0; i < this.managerConfig.managerList.Count; i++)
        {
            if (this.managerConfig.managerList[i] == null) 
                continue; 
            GameObject newManager = Instantiate(this.managerConfig.managerList[i]);
            newManager.transform.parent = managersContainer.transform;
        }
    }

    public GameObject GetTopPopupCanvas()
    {
        return this.topPopupCanvas;
    }
}
