using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

public enum SoundType
{
    COMMON = 0,
    MUSIC,
    SFX,
}

namespace AudioPlayer
{
    [CreateAssetMenu(menuName = "Sound Configs/Sound Map")]
    public class SoundMap : ScriptableObject
    {
        public SoundType soundType;
        [SerializeField] string path = "SoundData";
        [ReadOnly]
        public List<SoundMapping> SoundMappingList;


        [Button]
        public void LoadSoundDataButton()
        {
            LoadSoundData();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        public void LoadSoundData()
        {
            var gos = Resources.LoadAll(path);
            if (gos == null || gos.Length == 0) return;
            SoundMappingList = new List<SoundMapping>();
            foreach (var go in gos)
            {
                SoundClipData data = (SoundClipData)go;
                SoundMappingList.Add(new SoundMapping(data.Id, data));
                SoundMappingList.Sort();
            }
        }

        public void UpdatePath(string newPath)
        {
            path = newPath;
        }

        [Button]
        public void ClearSoundData()
        {
            SoundMappingList.Clear();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        [Button]
        public void EditSoundDefine()
        {
#if UNITY_EDITOR
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Audios/SoundID.cs"));
#endif
        }
    }


    [Serializable]
    public class SoundMapping : IComparable
    {
        [HideInInspector] public string name;
        [ReadOnly] public SoundID Id;
        [ReadOnly] public SoundClipData Data;

        public SoundMapping(SoundID id, SoundClipData data)
        {
            this.name = id.ToString();
            this.Id = id;
            this.Data = data;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;
            SoundMapping s = (SoundMapping)obj;

            if (this.Id < s.Id)
                return -1;
            else if (this.Id > s.Id)
                return 1;
            return 0;
        }
    }
}


