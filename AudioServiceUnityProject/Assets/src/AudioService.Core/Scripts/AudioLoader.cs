using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace AudioService.Core
{
    public class AudioLoader : IDisposable
    {
        private readonly string assetKey;
        private readonly Dictionary<string, AudioClip> clipMaps = new();
        private AsyncOperationHandle<IList<IResourceLocation>> clipOpHandle;

        public AudioLoader(string assetKey)
        {
            this.assetKey = assetKey;
        }

        public AudioClip GetAudioClip(string audioClipName)
        {
            if (clipMaps.TryGetValue(audioClipName, out var audioClip))
            {
                return audioClip;
            }

            Debug.LogWarning($"AudioClip not found: {audioClipName}");
            return null;
        }

        public async UniTask LoadAsync()
        {
            try
            {
                clipOpHandle = Addressables.LoadResourceLocationsAsync(assetKey);
                await clipOpHandle.ToUniTask();

                var loadTasks = Enumerable
                    .Select(clipOpHandle.Result.Select(Addressables.LoadAssetAsync<AudioClip>),
                        asyncOperationHandle => asyncOperationHandle.ToUniTask()
                    ).ToList();

                try
                {
                    var audioClipResults = await UniTask.WhenAll(loadTasks);
                    foreach (var clip in audioClipResults)
                    {
                        if (clip != null)
                        {
                            if (!clipMaps.TryAdd(clip.name, clip))
                            {
                                Debug.LogWarning($"Duplicate key detected: {clip.name}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"Failed to load AudioClip: {clip.name}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"An error occurred during loading assets: {e.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load audio resources: {e.Message}");
            }
        }

        public void Dispose()
        {
            foreach (var clip in clipMaps.Values)
            {
                Addressables.Release(clip);
            }
            clipMaps.Clear();
            if (clipOpHandle.IsValid())
            {
                Addressables.Release(clipOpHandle);
            }
        }
    }
}