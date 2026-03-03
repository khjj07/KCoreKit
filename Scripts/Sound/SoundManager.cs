using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sm = (SoundManager)target;
            sm.channels = sm.GetComponentsInChildren<Channel>();
        }
    }
#endif

    public class SoundManager : Singleton<SoundManager>
    {
        public SoundListAsset asset;
        [HideInInspector]
        public Channel[] channels;

        public float mainVolume;

        public void Awake()
        {
            channels = GetComponentsInChildren<Channel>();
        }
        public static void Play(string name,int channel)
        {
            GetInstance().channels[channel].Play(GetInstance().asset.GetSoundByName(name).clip);
        }

        public static void Play(string name)
        {
            GetInstance().channels[0].Play(GetInstance().asset.GetSoundByName(name).clip);
        }
        public static void PlayOneShot(string name, int channel)
        {
            GetInstance().channels[channel].PlayOneShot(GetInstance().asset.GetSoundByName(name).clip);
        }
        public static void PlayOneShot(string name)
        {
            GetInstance().channels[0].PlayOneShot(GetInstance().asset.GetSoundByName(name).clip);
        }
        public static void Stop(int channel)
        {
            GetInstance().channels[channel].Stop();
        }

        public static void Pause(int channel)
        {
            GetInstance().channels[channel].Pause();
        }

        public static void SetVolume(float volume,int channel)
        {
            GetInstance().channels[channel].SetVolume(volume);
        }
        public static void SetMainVolume(float volume)
        {
            GetInstance().mainVolume = volume;
            foreach (var channel in GetInstance().channels)
            {
                channel.SetVolume(volume);
            }
        }
        public static float GetMainVolume()
        {
            return GetInstance().mainVolume;
        }

        public static void StopAll()
        {
            foreach (var channel in GetInstance().channels)
            {
                channel.Stop();
            }
        }
    }
}
