using System;
using System.Collections.Generic;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using AudioService.Core;

namespace AudioService.Simple
{
    public class SimpleAudioService : MonoBehaviour, IDisposable
    {
        private static readonly string AssetKey = "Audio";

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource seAudioSource;

        /// <summary>
        ///  AudioMixerのExposedパラメータ名
        /// </summary>
        private readonly string masterExposedName = "Master";
        private readonly string bgmExposedName = "Bgm";
        private readonly string seExposedName = "Se";
        
        /// <summary>
        /// PlayerPrefsのキー名
        /// </summary>
        private readonly string masterVolumeKey = "MasterVolume";
        private readonly string bgmVolumeKey = "BgmVolume";
        private readonly string seVolumeKey = "SeVolume";

        private AudioLoader audioLoader;
        private bool isLoaded = false;

        /// <summary>
        /// Audioのアセットをすべてロードします。
        /// </summary>
        public async UniTask LoadAllAsyncIfNeed()
        {
            if (isLoaded)
            {
                Debug.LogError("AudioService is already loaded.");
                return;
            }
            audioLoader = new AudioLoader(AssetKey);
            await UniTask.WhenAll(audioLoader.LoadAsync());

            audioMixer.SetFloat(masterExposedName,
                VolumeConverter.ToDecibel(PlayerPrefs.GetFloat(masterVolumeKey, 1f))
            );
            audioMixer.SetFloat(bgmExposedName, VolumeConverter.ToDecibel(PlayerPrefs.GetFloat(bgmVolumeKey, 1f)));
            audioMixer.SetFloat(seExposedName, VolumeConverter.ToDecibel(PlayerPrefs.GetFloat(seVolumeKey, 1f)));
            
            isLoaded = true;
        }

        /// <summary>
        /// BGMを再生する。
        /// 再生中のBGMがあれば、停止して、AudioClipを切り替えて再生をします。
        /// BGMの再生ができない場合は、エラーログを出力します。
        /// </summary>
        /// <param name="audioClipName"> 再生するAudioClipの名前 </param>
        public void PlayBgm(string audioClipName)
        {
            if (!isLoaded)
            {
                Debug.LogError("Audio Assets is not loaded.");
                return;
            }
            
            var audioClip = audioLoader.GetAudioClip(audioClipName);
            if (audioClip == null)
            {
                Debug.LogError($"AudioClip {audioClipName} is Not Found");
                return;
            }
            
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
            bgmAudioSource.clip = audioClip;
            bgmAudioSource.Play();
        }
        
        /// <summary>
        /// 再生中のBGMがあれば、停止します。
        /// 再生中のBGMがない場合は、何もしません。
        /// </summary>
        public void StopBgm()
        {
            if (!isLoaded)
            {
                Debug.LogError("Audio Assets is not loaded.");
                return;
            }
            
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
        }

        /// <summary>
        /// SEを再生します。
        /// 再生ができない場合は、エラーログを出力します。
        /// </summary>
        /// <param name="audioClipName"> 再生するAudioClipの名前 </param>
        public void PlaySe(string audioClipName)
        {
            if (!isLoaded)
            {
                Debug.LogError("Audio Assets is not loaded.");
                return;
            }
            
            var audioClip = audioLoader.GetAudioClip(audioClipName);
            if (audioClip == null)
            {
                Debug.LogError($"AudioClip {audioClipName} is Not Found");
                return;
            }

            seAudioSource.PlayOneShot(audioClip);
        }
        
        /// <summary>
        /// Masterボリュームの変更を行い, PlayerPrefsに保存します。
        /// </summary>
        /// <param name="masterVolume"></param>
        public void ChangeAndWriteMasterVolume(float masterVolume)
        {
            audioMixer.SetFloat(masterExposedName, VolumeConverter.ToDecibel(masterVolume));
            PlayerPrefs.SetFloat(masterVolumeKey, masterVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// BGMボリュームの変更を行い, PlayerPrefsに保存します。
        /// </summary>
        /// <param name="bgmVolume"></param>
        public void ChangeBgmVolume(float bgmVolume)
        {
            audioMixer.SetFloat(bgmExposedName, VolumeConverter.ToDecibel(bgmVolume));
            PlayerPrefs.SetFloat(bgmVolumeKey, bgmVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// SEのボリュームの変更を行い, PlayerPrefsに保存します。
        /// </summary>
        /// <param name="seVolume"></param>
        public void ChangeSeVolume(float seVolume)
        {
            audioMixer.SetFloat(seExposedName, VolumeConverter.ToDecibel(seVolume));
            PlayerPrefs.SetFloat(seVolumeKey, seVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// PlayerPrefsからボリュームの取得を行います。
        /// </summary>
        /// <returns></returns>
        public (float MasterVolume, float BgmVolume, float SeVolume) GetVolumeByPlayerPrefs()
        {
            var masterVolume = PlayerPrefs.GetFloat(masterVolumeKey, 1f);
            var bgmVolume = PlayerPrefs.GetFloat(bgmVolumeKey, 1f);
            var seVolume = PlayerPrefs.GetFloat(seVolumeKey, 1f);
            return (masterVolume, bgmVolume, seVolume);
        }

        /// <summary>
        /// 後始末処理を行います。
        /// MonoBehaviourのOnDestroy時に強制的に呼ばれる
        /// </summary>
        public void Dispose()
        {
            audioLoader?.Dispose();
        }

        #region unityイベント

        private void Start()
        {
            ServiceLocator.TryAddCache(this);
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion Unityイベント

    }
}