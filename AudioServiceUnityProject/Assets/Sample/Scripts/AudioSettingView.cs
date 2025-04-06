using AppCore.Runtime;
using AppService.Runtime;
using AudioService.Simple;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sample
{
    public class AudioSettingView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomSlider masterVolumeSlider;
        [SerializeField] private CustomSlider bgmVolumeSlider;
        [SerializeField] private CustomSlider seVolumeSlider;
        [SerializeField] private CustomButton closeButton;
        [SerializeField] private CanvasGroup canvasGroup;

        public Canvas Canvas => canvas;
        private ModalScreen ModalScreen => ServiceLocator.Get<ModalScreen>();
        private SimpleAudioService AudioService => ServiceLocator.Get<SimpleAudioService>();    
         
        public static async UniTask<AudioSettingView> CreateAsync()
        {
            var go = await Addressables.InstantiateAsync("Assets/Sample/Prefabs/AudioSettingView.prefab");
            var view = go.GetComponent<AudioSettingView>();
            view.gameObject.SetActive(false);
            view.Initialize();
            return view;
        }
        
        public void Push()
        {
            ModalScreen.Push(this);
        }
        
        public void Pop()
        {
            ModalScreen.Pop(this);
        }
        
        public void Open()
        {
            gameObject.SetActive(true);
        }
        
        public void Close()
        {
            Destroy(gameObject);
        }
        
        private void Initialize()
        {
            var audioService = ServiceLocator.Get<SimpleAudioService>();
            var volume = audioService.GetVolumeByPlayerPrefs();
            masterVolumeSlider.SetValueWithNotifySafe(volume.MasterVolume);
            bgmVolumeSlider.SetValueWithNotifySafe(volume.BgmVolume);
            seVolumeSlider.SetValueWithNotifySafe(volume.SeVolume);

            closeButton.SubscribeToClick(() =>
                {
                    Debug.Log("Close Button Clicked");
                    AudioService.PlaySe("SE_Cancel");
                    Pop();
                }, canvasGroup
            );
            
            masterVolumeSlider.OnValueChangedObservable.Subscribe(value =>
            {
                AudioService.ChangeAndWriteMasterVolume(value);
            }).RegisterTo(destroyCancellationToken);
            
            bgmVolumeSlider.OnValueChangedObservable.Subscribe(value =>
            {
                AudioService.ChangeBgmVolume(value);
            }).RegisterTo(destroyCancellationToken);
            
            seVolumeSlider.OnValueChangedObservable.Subscribe(value =>
            {
                AudioService.ChangeSeVolume(value);
            }).RegisterTo(destroyCancellationToken);
            
            seVolumeSlider.OnPointerUpObservable.Subscribe(_ =>
            {
                AudioService.PlaySe("SE_Ok");
            }).RegisterTo(destroyCancellationToken);
        }
    }
}