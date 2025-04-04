#nullable enable
using System;
using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sample
{
    public class SampleTopView : MonoBehaviour, IView
    {
        private static readonly string AssetKey = "Assets/Sample/Prefabs/SampleTopView.prefab";
        
        [SerializeField] private Canvas? canvas;
        [SerializeField] private CustomButton? playBgmButton;
        [SerializeField] private CustomButton? stopBgmButton;
        [SerializeField] private CustomButton? openAudioSettingsButton;
        [SerializeField] private CanvasGroup? canvasGroup;

        public Canvas? Canvas => canvas;
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();
        private ViewScreen ViewScreen => ServiceLocator.Get<ViewScreen>();
        
        private readonly Subject<Unit> onClickedOpenAudioSettings = new();
        public Observable<Unit> OnClickedOpenAudioSettings => onClickedOpenAudioSettings;

        public static async UniTask<SampleTopView> CreateAsync()
        {
            var go = await Addressables.InstantiateAsync(AssetKey);
            go.SetActive(false);
            var view = go.GetComponent<SampleTopView>();
            return view;
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public　async UniTask InitializeAsync()
        {
            await AudioService.LoadAllAsyncIfNeed();
            
            if (playBgmButton != null)
            {
                playBgmButton.SubscribeToClickAndPlaySe(() =>
                {
                    Debug.Log("Play BGM Button Clicked");
                    AudioService.PlayBgm("BGM_Sample");
                }, new AudioOptions(AudioService, "SE_Ok"), canvasGroup);
            }
            
            if (stopBgmButton != null)
            {
                stopBgmButton.SubscribeToClickAndPlaySe(() =>
                    {
                        Debug.Log("Stop BGM Button Clicked");
                        AudioService.StopBgm();
                    }, new AudioOptions(AudioService, "SE_Cancel"), canvasGroup
                );
            }
            
            if (openAudioSettingsButton != null)
            {
                openAudioSettingsButton.SubscribeToClickAndPlaySe(
                    () => onClickedOpenAudioSettings.OnNext(Unit.Default),
                    new AudioOptions(AudioService, "SE_Ok"), 
                    canvasGroup
                );
            }
        }
        
        public void Push()
        {
            ViewScreen.Push(this);
        }
        
        public void Pop()
        {
            ViewScreen.Pop();
        }
        
        public void Open()
        {
            gameObject.SetActive(true);
        }
        
        public void Close()
        {
            Destroy(gameObject);
        }
    }

}