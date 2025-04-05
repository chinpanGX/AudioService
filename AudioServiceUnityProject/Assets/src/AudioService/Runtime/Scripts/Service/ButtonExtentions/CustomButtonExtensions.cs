#nullable enable
using System;
using AppService.Runtime;
using UnityEngine;

namespace AudioService.Simple
{
    public class AudioOptions
    {
        public SimpleAudioService audioService;
        public string clipName;

        public AudioOptions(SimpleAudioService audioService, string clipName)
        {
            this.audioService = audioService;
            this.clipName = clipName;
        }
    }

    public static class CustomButtonExtensions
    {
        public static void SubscribeToClickAndPlaySe(this CustomButton customButton,
            Action action,
            AudioOptions? audioOptions,
            CanvasGroup? canvasGroup = null)
        {
            customButton.SubscribeToClick(() =>
            {
                action.Invoke();
                audioOptions?.audioService.PlaySe(audioOptions.clipName);
            }, canvasGroup);
        }

    }
}