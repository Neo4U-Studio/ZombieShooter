using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI Configs/MenuConfig")]
public class MenuConfig : ScriptableObject
{
    [SerializeField] public List<MenuInfo> menuInfos;

    [NaughtyAttributes.Button]
    public void CheckPopupPrefabPaths()
    {
        for (int i = menuInfos.Count - 1; i >= 0; i--)
        {
            string filePath = menuInfos[i].prefabPath;
            var popup = Resources.Load<GameObject>(filePath);
            if (popup == null)
            {
                Debug.LogError($"{i}_Can not load resource {filePath}");
                menuInfos.RemoveAt(i);
                continue;
            }

            Debug.Log($"{i}_Done! {popup.name}: {filePath}");
        }
    }
}