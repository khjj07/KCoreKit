
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    public static class DataTableCombineBuilder
    {
        [MenuItem("DataTable/Build All")]
        public static void BuildAll()
        {
            string[] guids = AssetDatabase.FindAssets("t:DataTable");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var dt = AssetDatabase.LoadAssetAtPath<DataTable>(path);

                if (!dt)
                {
                    Debug.LogWarning($"경로에 데이터 테이블이 없습니다: {path}");
                    continue;
                }

                if (!dt.csv)
                {
                    Debug.LogError($"{dt.name}를 읽는 중 오류가 발생했습니다.");
                    continue;
                }

                Debug.Log($"[BuildAll] {dt.name} 데이터 생성 시작");
                dt.UpdateData(dt.csv,null);
                Debug.Log($"[BuildAll] {dt.name} 데이터 생성 완료");
            }
        }
    }
}
#endif