using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI Configs/PopupConfig")]
public class PopupConfig : ScriptableObject
{
    [SerializeField]
    public List<PopupInfo> popupInfos;

    [NaughtyAttributes.Button]
    public void CheckPopupPrefabPaths()
    {
        for (int i = popupInfos.Count - 1; i >= 0; i--)
        {
            string filePath = popupInfos[i].prefabPath;
            var popup = Resources.Load<GameObject>(filePath);
            if (popup == null)
            {
                Debug.LogError($"{i}_Can not load resource {filePath}");
                popupInfos.RemoveAt(i);
                continue;
            }

            Debug.Log($"{i}_Done! {popup.name}: {filePath}");
        }
    }
}