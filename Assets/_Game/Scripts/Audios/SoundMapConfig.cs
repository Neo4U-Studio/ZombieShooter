using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AudioPlayer
{
    [CreateAssetMenu(menuName = "Sound Configs/Sound Map Config")]
    public class SoundMapConfig : ScriptableObject
    {
        public List<SoundMapPath> MapPath;

        [System.Serializable]
        public class SoundMapPath
        {
            public SoundType Type;
            public string Path;
        }

        public void AddNewPath(SoundType type, string path)
        {
            MapPath.Add(new SoundMapPath()
            {
                Type = type,
                Path = path,
            });
        }
    }
}
