using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public class DontDestroyAndDistinct : MonoBehaviour
    {
        private static Dictionary<string,DontDestroyAndDistinct> _instance = new Dictionary<string, DontDestroyAndDistinct>();

        // Domain Reload를 끈 상태에선 static이 Play 세션을 넘어 살아남아, 이전 세션의 (이미 파괴된)
        // 인스턴스 때문에 새 인스턴스가 중복으로 판정돼 파괴된다. Play 진입 시마다 비워준다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlayMode()
        {
            _instance.Clear();
        }

        private void Awake()
        {
            if (_instance.TryGetValue(gameObject.name, out var existing) && existing != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance[gameObject.name] = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
