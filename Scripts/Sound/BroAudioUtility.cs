using System.Linq;
using KCoreKit.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace KCoreKit.Scripts.Sound
{
    public static class BroAudioUtility
    {
        [MenuItem("Assets/KCoreKit/Sound/Add To BroAudio Asset", true)]
        public static bool ValidateAddToBroAudioAsset()
        {
            return Selection.objects.Any(obj => obj is AudioClip);
        }

        [MenuItem("Assets/KCoreKit/Sound/Add To BroAudio Asset", priority = -102)]
        public static void AddToBroAudioAsset()
        {
            var clips = Selection.objects.OfType<AudioClip>().ToList();
            if (clips.Count == 0)
            {
                return;
            }

            AddToBroAudioAssetWindow.Open(clips);
        }
    }
}