using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    [CustomEditor(typeof(CSVRemoteManager))]
    public class CSVRemoteManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            // 기존 인스펙터 그리기 (Contexts 리스트 등)
            DrawDefaultInspector();

            CSVRemoteManager manager = (CSVRemoteManager)target;

            GUILayout.Space(15);
            GUI.enabled = Application.isPlaying == false; // 에디터 멈춘 상태에서만 동작 권장

            if (GUILayout.Button("모든 CSV 원격 동기화 (Sync All)", GUILayout.Height(30)))
            {
                // 비동기 메서드 실행
                _ = manager.SyncAllContexts();
            }

            GUI.enabled = true;
        }
    }
#endif


    [Serializable]
    public class CSVRemoteContext
    {
        public TextAsset csvFile;
        public string remoteAddress;
    }

    // SingletonAsset 구조를 유지합니다. (없다면 ScriptableObject로 대체 가능)
    public class CSVRemoteManager : SingletonAsset<CSVRemoteManager>
    {
        public List<CSVRemoteContext> contexts = new List<CSVRemoteContext>();

        /// <summary>
        /// 특정 텍스트 에셋의 원격 데이터를 다운로드하여 동기화합니다.
        /// </summary>
        public async Task DownloadAndSync(CSVRemoteContext context)
        {
            if (context.csvFile == null || string.IsNullOrEmpty(context.remoteAddress))
            {
                Debug.LogWarning("[CSVRemoteManager] CSV 파일 또는 원격 주소가 비어 있습니다.");
                return;
            }

            using UnityWebRequest webRequest = UnityWebRequest.Get(context.remoteAddress);
            // 리다이렉트가 잦은 구글 URL 특성을 고려해 제한을 넉넉히 둡니다.
            webRequest.redirectLimit = 5; 

            var operation = webRequest.SendWebRequest();

            // [수정 핵심] Task.Yield() 대신 에디터와 런타임 환경을 모두 고려한 대기 루프
            while (!operation.isDone)
            {
#if UNITY_EDITOR
                // 유니티 에디터가 '재생(Play)' 상태가 아닐 때 (즉, 버튼만 눌렀을 때)
                if (!Application.isPlaying)
                {
                    // 메인 스레드를 잠시 멈춰 네트워크 연산이 완료될 시간을 벌어줍니다.
                    System.Threading.Thread.Sleep(10); 
                }
#endif
                // 재생 중이거나 빌드 후 런타임 환경에서는 밀리초 단위로 안전하게 대기합니다.
                await Task.Delay(10);
            }

            // 에러 체크
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[CSVRemoteManager] 다운로드 실패 ({context.csvFile.name}): {webRequest.error}");
                return;
            }

            string csvContent = webRequest.downloadHandler.text;

#if UNITY_EDITOR
            // 에디터 환경인 경우 에셋 파일 자체를 물리적으로 덮어씁니다.
            string assetPath = AssetDatabase.GetAssetPath(context.csvFile);
            if (!string.IsNullOrEmpty(assetPath))
            {
                File.WriteAllText(assetPath, csvContent);
                Debug.Log($"[CSVRemoteManager] 동기화 완료: {context.csvFile.name}");
            }
#else
    Debug.LogWarning("[CSVRemoteManager] 런타임 환경에서는 원격지 파일 덮어쓰기가 지원되지 않습니다.");
#endif
        }

        /// <summary>
        /// 리스트에 등록된 모든 CSV 파일을 순차적으로 동기화합니다.
        /// </summary>
        
        public async Task SyncAllContexts()
        {
            Debug.Log("[CSVRemoteManager] 모든 CSV 파일 동기화 시작...");
            foreach (var context in contexts)
            {
                Debug.Log($"context :  {context.remoteAddress}, {context.csvFile.name}");
                await DownloadAndSync(context);
            }

#if UNITY_EDITOR
            // 모든 파일 수정이 끝나면 유니티 에디터 에셋 데이터베이스를 새로고침합니다.
            AssetDatabase.Refresh();
            Debug.Log("[CSVRemoteManager] 모든 CSV 파일 동기화 및 에셋 새로고침 완료!");
#endif
        }

#if UNITY_EDITOR
        [MenuItem("CSVRemoteManager/Refresh All")]
        public static void RefreshAll()
        {
            GetInstance().SyncAllContexts();
        }
        [MenuItem("Assets/KCoreKit/Create/CSVRemoteManager")]
        public static void Create()
        {
            // TypeExtension.CreateAsset 구조 유지
            TypeExtension.CreateAsset<CSVRemoteManager>("CSVRemoteManager");
        }
#endif
    }
}