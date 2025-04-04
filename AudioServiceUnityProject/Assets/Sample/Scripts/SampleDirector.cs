#nullable enable
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Sample
{
    public class SampleDirector : MonoBehaviour
    {
        private void Start()
        {
            CreateSampleTopView().Forget();
        }
        
        private async UniTaskVoid CreateSampleTopView()
        {
            var sampleTopView = await SampleTopView.CreateAsync();
            await sampleTopView.InitializeAsync();
            sampleTopView.OnClickedOpenAudioSettings.SubscribeAwait(async (_, _) =>
                {
                    await OpenAudioSettings();
                }
            ).RegisterTo(destroyCancellationToken);
            
            sampleTopView.Push();
            sampleTopView.Open();
        }
        
        private async UniTask OpenAudioSettings()
        {
            var audioSettingsView = await AudioSettingView.CreateAsync();
            audioSettingsView.Push();
            audioSettingsView.Open();
        }
    }
}