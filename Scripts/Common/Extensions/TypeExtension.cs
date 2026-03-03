using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace KCoreKit
{
    public static class TypeExtension
    {
        // concrete 타입 찾기
        public static List<Type> GetAllConcreteSubclasses<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType)
                .ToList();
        }
#if UNITY_EDITOR
        public static T CreateAsset<T>(string assetName) where T : ScriptableObject
        {
            string path = UnityEditorAssetExtensions.GetSelectedDirectoryPath();

            if (string.IsNullOrEmpty(path))
            {
                // 폴더 선택이 유효하지 않은 경우, Assets 루트 폴더를 기본값으로 사용
                path = "Assets";
            }

            // 2. 에셋 이름 결정 및 유니크한 경로 생성
            string fullPath = Path.Combine(path, assetName + ".asset");
        
            // 프로젝트 내에서 중복되지 않는 유니크한 경로를 생성합니다.
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            // 3. ScriptableObject 인스턴스 생성
            var asset = ScriptableObject.CreateInstance<T>();
        
            if (asset == null)
            {
                Debug.LogError($"{typeof(T).Name} 인스턴스 생성에 실패했습니다. 클래스 정의를 확인하세요.");
                return asset;
            }
        
            // 4. 에셋 저장
            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 5. 생성된 에셋을 하이라이트하고 선택
            EditorUtility.FocusProjectWindow();
        
            Debug.Log($"✅{typeof(T).Name}이 성공적으로 생성되었습니다: {uniquePath}");
            return asset;
        }
        #endif
    }
}