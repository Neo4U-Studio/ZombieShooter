using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Text;

namespace AudioPlayer
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] GameObject sfxPrefab;
        [SerializeField] SoundMapConfig soundMapConfig;
        [SerializeField] int numberOfDefaultSFX = 8;

        public float CurrentSfxVolume { get; private set; }
        public float CurrentMusicVolume { get; private set; }

        public float GlobalSfxVolume { get; private set; }
        public float GlobalMusicVolume { get; private set; }


        [Space(10)]
        [SerializeField] AudioMixer currentMixer;
        [SerializeField] AudioMixerGroup SFX_MixerGroup;
        [SerializeField] AudioMixerGroup Music_MixerGroup;

        private List<AudioSourceController> sfxAudioSrcChannel;
        private Dictionary<SoundID, AudioSource> aloneAudioSourceChannel;
        private Dictionary<SoundID, IEnumerator> aloneAudioSourceFadeCommand;
        private Dictionary<SoundID, int> continuousTimeSampleDict;
        private Dictionary<SoundType, SoundMap> loadedSoundMapDict;
        private List<SoundID> m_soundIDPlayedInCurrentFrame;

        private const string MASTER_VOLUME = "MasterVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SFXVolume";
        private const string GLOBAL_MUSIC_VOLUME = "GlobalMusicVolume";
        private const string GLOBAL_SFX_VOLUME = "GlobalSfxVolume";

        private bool isDisableMusic = false;


        private List<SoundMap> soundMaps;
        private void Awake()
        {
            sfxAudioSrcChannel = new List<AudioSourceController>();
            aloneAudioSourceChannel = new Dictionary<SoundID, AudioSource>();
            aloneAudioSourceFadeCommand = new Dictionary<SoundID, IEnumerator>();
            continuousTimeSampleDict = new Dictionary<SoundID, int>();
            m_soundIDPlayedInCurrentFrame = new List<SoundID>();
            loadedSoundMapDict = new();
            this.enabled = false;

            this.soundMaps = new List<SoundMap>();
            
            LoadCommonSoundMaps();
            StartCoroutine(Init());
        }

        public IEnumerator Init()
        {
            for (int i = 0; i < numberOfDefaultSFX; i++)
            {
                AudioSourceController newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSourceController>();
                sfxAudioSrcChannel.Add(newSFX);
            }
            yield return null;
            this.enabled = true;
            LoadSave();

            CurrentSfxVolume = 1f;
            CurrentMusicVolume = 1f;

            SetSfxVolume(CurrentSfxVolume);
            SetMusicVolume(CurrentMusicVolume);
        }

        void TriggerDisableMusic()
        {
            isDisableMusic = !isDisableMusic;
            SetMusicMute(isDisableMusic);
        }

        void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime;
            if (Time.frameCount % 10 == 0)
            {
                for (int i = 0; i < sfxAudioSrcChannel.Count; i++)
                {
                    AudioSourceController item = sfxAudioSrcChannel[i];
                    if (!item.audioSource.isPlaying && item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }

            m_soundIDPlayedInCurrentFrame.Clear();
        }

        private AudioSource GetFreeSFXChannel(bool requestLowPriority = false)
        {
            AudioSourceController freeAS = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);
            if (freeAS == null)
            {
                if (requestLowPriority)
                {
                    freeAS = sfxAudioSrcChannel.Find(x => x.IsLowPriority);
                    if (freeAS == null)
                    {
                        return null;
                    }
                }
                else
                {
                    int index = Random.Range(0, sfxAudioSrcChannel.Count);
                    freeAS = sfxAudioSrcChannel[index];
                }
            }
            return freeAS.audioSource;
        }

        private AudioSource GetAloneAudioSource(SoundID soundID)
        {
            AudioSource audioSource;
            if (!aloneAudioSourceChannel.TryGetValue(soundID, out audioSource))
            {
                AudioSourceController controller = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);

                if (controller == null)
                    audioSource = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
                else
                {
                    sfxAudioSrcChannel.Remove(controller);
                    audioSource = controller.audioSource;
                }
                aloneAudioSourceChannel.Add(soundID, audioSource);
            }
            return audioSource;
        }

        private AudioSource GetNewSFXChannel()
        {
            return Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
        }

        public AudioSource PlaySound(SoundClipData sfxData)
        {
            return PlaySound(sfxData, 1.0f);
        }
        
        public AudioSource PlaySound(SoundClipData sfxData, float volume)
        {
            AudioSource sfxAudioSrc;

            sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);
            if (sfxAudioSrc == null)
                return null;

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = sfxData.IsLoop;

            sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;

            if (sfxData.IsPitch)
                sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();

            //this.Log(string.Format(this._logSoundClipDataFormat, Time.frameCount, sfxData.name, sfxAudioSrc.clip.name));

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
            {
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            }
            return sfxAudioSrc;
        }

        public AudioSource PlaySound(AudioClip audioClip, float volume = 1f)
        {
            AudioSource sfxAudioSrc;

            sfxAudioSrc = GetFreeSFXChannel();
            if (sfxAudioSrc == null)
                return null;

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = false;

            sfxAudioSrc.outputAudioMixerGroup = this.SFX_MixerGroup;

            sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = audioClip;
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();

            //this.Log(string.Format(this._logAudioClipFormat, Time.frameCount, Time.frameCount, sfxAudioSrc.clip.name));
            return sfxAudioSrc;
        }

        public AudioSource PlaySound(SoundID soundID, float volume = 1f)
        {
            if (soundID == SoundID.NONE) return null;

            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }

                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                string errorLog = string.Format("SoundMgr {0} missing mapping !", soundID.ToString());
                Debug.LogError(errorLog);
                return null;
            }

            for (int i = 0; i < m_soundIDPlayedInCurrentFrame.Count; i++)
            {
                if (m_soundIDPlayedInCurrentFrame[i] == soundID)
                {
                    string warningLog = string.Format("SoundMgr {0} already played in this frame!", soundID.ToString());
                    Debug.LogWarning(warningLog);
                    return null;
                }
            }
            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            AudioSource sfxAudioSrc;
            if (sfxData.IsAlone || sfxData.IsContinues)
            {
                sfxAudioSrc = GetAloneAudioSource(soundID);
            }
            else
                sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);

            if (sfxAudioSrc == null) return null;

            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return null;
            }

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = sfxData.IsLoop;

            sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;

            if (sfxData.IsPitch)
                sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();
            m_soundIDPlayedInCurrentFrame.Add(soundID);

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            return sfxAudioSrc;
        }

        public AudioSource PlayMusic(SoundID soundID, float volume = 1f)
        {
            var audioSource = this.PlaySound(soundID, volume);
            if (audioSource != null)
            {
                audioSource.outputAudioMixerGroup = this.Music_MixerGroup;
            }
            return audioSource;
        }

        public AudioSource PlayMusic(AudioClip audioClip, float volume = 1f)
        {
            var audioSource = this.PlaySound(audioClip, volume);
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = this.Music_MixerGroup;

            return audioSource;
        }

        public void UnPauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            if (sfxData.IsAlone)
            {
                AudioSource sfxAudioSrc;
                if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;

                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }
                    sfxAudioSrc.UnPause();

                    coroutine = Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null);
                    StartCoroutine(coroutine);
                }
                else
                    PlaySound(soundID);
            }
            else if (sfxData.IsContinues)
            {
                AudioSource sfxAudioSrc;
                if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;
                }
                sfxAudioSrc = PlaySound(soundID);
                sfxAudioSrc.timeSamples = continuousTimeSampleDict[soundID];
            }
        }

        public IEnumerator Coroutine_FadeInSFX(AudioSource aSrc, float time, System.Action action = null)
        {
            float timeOut = 0f;
            float originalVol = aSrc.volume;
            while (timeOut < time)
            {
                timeOut += Time.unscaledDeltaTime;
                float volume = timeOut / time * originalVol;
                if (aSrc == null) yield break;
                aSrc.volume = volume;
                yield return null;
            }

            if (aSrc == null) yield break;
            aSrc.volume = 1f;

            if (action != null) action();
        }

        public IEnumerator Coroutine_FadeOutSFX(AudioSource aSrc, float time, System.Action action = null)
        {
            float timeOut = time;
            float originalVol = aSrc.volume;
            while (timeOut > 0f)
            {
                timeOut -= Time.unscaledDeltaTime;
                float volume = timeOut / time * originalVol;
                aSrc.volume = volume;
                yield return null;
            }

            aSrc.volume = 0f;

            if (action != null) action();
        }


        public void PauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;
            AudioSource sfxAudioSrc;
            if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc) && sfxAudioSrc.isPlaying)
            {
                if (sfxData.IsAlone)
                {
                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }

                    coroutine = Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () => sfxAudioSrc.Pause());
                    aloneAudioSourceFadeCommand.Add(soundID, coroutine);
                    StartCoroutine(coroutine);
                }

                if (sfxData.IsContinues)
                {
                    continuousTimeSampleDict[soundID] = sfxAudioSrc.timeSamples;

                    aloneAudioSourceChannel.Remove(soundID);
                    sfxAudioSrcChannel.Add(sfxAudioSrc.GetComponent<AudioSourceController>());

                    StartCoroutine(Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () =>
                    {
                        sfxAudioSrc.Stop();
                    }));
                }
            }
        }


        private void SetVolume(string name, float value)
        {
            float db = 20.0f * (value < 0.001f ? -4.0f : Mathf.Log10(value));
            currentMixer.SetFloat(name, db);
        }

        public void SetSfxVolume(float volumeValue)
        {
            CurrentSfxVolume = volumeValue;
            SetVolume(SFX_VOLUME, volumeValue * GlobalSfxVolume);
        }

        public float GetSFXVolume()
        {
            return this.CurrentSfxVolume;
        }

        public void SetMusicVolume(float targetVolume, float fadeTime = 0f)
        {
            if (fadeTime <= 0f)
            {
                CurrentMusicVolume = targetVolume;
                SetVolume(MUSIC_VOLUME, targetVolume * GlobalMusicVolume);
                return;
            }

            musicFadeCor = StartCoroutine(_FadeMusic(targetVolume, fadeTime));
        }

        private Coroutine musicFadeCor;
        public IEnumerator _FadeMusic(float targetVol, float fadeTime)
        {
            float timeOut = 0f;
            float originalVol = CurrentMusicVolume;
            float delta = targetVol - originalVol; // delta > 0: fade up, otherwise fade down
            while (timeOut < fadeTime)
            {
                timeOut += Time.deltaTime;
                float vol = originalVol + (timeOut / fadeTime * delta);

                CurrentMusicVolume = vol;
                SetMusicVolume(CurrentMusicVolume);
                yield return null;
            }

            CurrentMusicVolume = targetVol;
            SetMusicVolume(CurrentMusicVolume);
        }


        public void SetMusicVolumeBack(float targetVolume = 1.0f, float delaytime = 0f, float fadeTime = 0f)
        {
            StartCoroutine(Coroutine_SetMusicVolumeBack(targetVolume, delaytime, fadeTime));
        }

        public IEnumerator Coroutine_SetMusicVolumeBack(float targetVolume = 1.0f, float delaytime = 0f, float fadeTime = 0f)
        {
            yield return new WaitForSeconds(delaytime);
            SetMusicVolume(targetVolume, fadeTime);
        }

        public void SetMuteMaster(bool isMute = true)
        {
            SetVolume(MASTER_VOLUME, isMute ? 0.0f : 1.0f);
        }

        public void SetSfxMute(bool isMute = true)
        {
            GlobalSfxVolume = isMute ? 0.0f : 1.0f;
            SetSfxVolume(CurrentSfxVolume);
            Save();
        }

        public void SetMusicMute(bool isMute = true)
        {
            GlobalMusicVolume = isMute ? 0.0f : 1.0f;
            SetMusicVolume(CurrentMusicVolume);
            Save();
        }

        private void LoadSave()
        {
            GlobalSfxVolume = PlayerPrefs.GetFloat(GLOBAL_SFX_VOLUME, 1.0f);
            GlobalMusicVolume = PlayerPrefs.GetFloat(GLOBAL_MUSIC_VOLUME, 1.0f);
        }

        void Save()
        {
            PlayerPrefs.SetFloat(GLOBAL_SFX_VOLUME, GlobalSfxVolume);
            PlayerPrefs.SetFloat(GLOBAL_MUSIC_VOLUME, GlobalMusicVolume);
            PlayerPrefs.Save();
        }

        private  void LoadCommonSoundMaps()
        {
            LoadSoundMap(SoundType.COMMON);
            LoadSoundMap(SoundType.MUSIC);
            LoadSoundMap(SoundType.SFX);
        }

        public bool IsSoundMapLoaded(SoundType soundType)
        {
            return loadedSoundMapDict.ContainsKey(soundType);
        }

        public void LoadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                    return; // Already loaded
            }

            for (int i = 0; i < soundMapConfig.MapPath.Count; i++)
            {
                if (soundMapConfig.MapPath[i].Type == soundType)
                {
                    SoundMap soundMap = Resources.Load<SoundMap>(soundMapConfig.MapPath[i].Path);
                    if (soundMap == null)
                    {
                        Debug.LogError("[SoundManager] Soundmap is null! Cannot be loaded: " + soundType);
                        return;
                    }
                    soundMap.LoadSoundData();
                    this.soundMaps.Add(soundMap);
                    loadedSoundMapDict.TryAdd(soundType, soundMap);
                    return;
                }
            }
        }

        public void UnloadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                {
                    Resources.UnloadAsset(this.soundMaps[i]);
                    this.soundMaps.RemoveAt(i);
                    loadedSoundMapDict.Remove(soundType);
                    //Resources.UnloadUnusedAssets();
                    return;
                }
            }
        }

        public bool IsSoundLoaded(SoundID soundID)
        {
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public SoundClipData GetSoundClipData(SoundID soundID)
        {
            if (soundID == SoundID.NONE) return null;

            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        return sound.Data;
                    }
                }
            }

            return null;
        }

        public SoundClipData GetSoundClipData(SoundType soundMapType, SoundID soundID)
        {
            if (soundID == SoundID.NONE || !loadedSoundMapDict.ContainsKey(soundMapType))
                return null;

            var soundMap = loadedSoundMapDict[soundMapType];
            foreach (SoundMapping sound in soundMap.SoundMappingList)
            {
                if (sound.Id == soundID)
                {
                    return sound.Data;
                }
            }
            return null;
        }
    }
}