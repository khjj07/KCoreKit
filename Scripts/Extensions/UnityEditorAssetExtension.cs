using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace KCoreKit
{
    public static class UnityEditorAssetExtensions
    {
#if UNITY_EDITOR
        public static string GetSelectedDirectoryPath()
        {
            Object selectedObject = Selection.activeObject;

            if (selectedObject == null)
            {
                return null;
            }
            
            string path = AssetDatabase.GetAssetPath(selectedObject);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            
            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }
            else
            {
                return Path.GetDirectoryName(path);
            }
        }

        public static bool IsInPrefabStage()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null;
        }

        public static string GetLocalPath(this DefaultAsset @this)
        {
            var success =
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(@this, out var guid, out long _);

            if (success)
                return AssetDatabase.GUIDToAssetPath(guid);
            return null;
        }

        public static string GetAbsolutePath(this DefaultAsset @this)
        {
            var path = GetLocalPath(@this);
            if (path == null)
                return null;

            path = path.Substring(path.IndexOf('/') + 1);
            return Application.dataPath + "/" + path;
        }

        public static DirectoryInfo GetDirectoryInfo(this DefaultAsset @this)
        {
            var absPath = GetAbsolutePath(@this);
            return absPath != null ? new DirectoryInfo(absPath) : null;
        }

        public static List<T> LoadAllObjectsInFolder<T>(this DefaultAsset @this) where T : class
        {
            var assets = new List<T>();
            // ???? ?? ??? ????? GUID ???? ??????
            var assetGUIDs = AssetDatabase.FindAssets("", new[] { GetLocalPath(@this) });

            foreach (var guid in assetGUIDs)
            {
                // GUID?? ???? ???
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // ????? Object ??????? ???
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) is T tmp) assets.Add(tmp);
            }

            return assets;
        }
#endif
        public static void SaveAsPNG(this Texture2D @this, string path) //metodo que exporta como png
        {
            byte[] bytes = @this.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
         
        }
        
        public static Sprite ToSprite(this Texture2D @this)
        {
          return Sprite.Create(@this, new Rect(0, 0, @this.width, @this.height), new Vector2(0.5f, 0.5f));
        }
        
        
        public static Texture2D ToTexture2D(this Sprite @this)
        {
            var rect = @this.textureRect;
            var pixels = @this.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

            Texture2D result = new Texture2D((int)rect.width, (int)rect.height);
            result.SetPixels(pixels);
            result.Apply();
        
            return result;
        }
        
        public static Texture2D TrimTexture(this Texture2D @this)
        {
            Color32[] pixels = @this.GetPixels32();
            int width = @this.width;
            int height = @this.height;

            // 경계값 초기화
            int minX = width, minY = height, maxX = 0, maxY = 0;
            bool hasContent = false;

            // 1. 투명하지 않은 영역의 경계 찾기
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // alpha가 0보다 크면 내용이 있는 것으로 간주 (임계값 조절 가능)
                    if (pixels[y * width + x].a > 0)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                        hasContent = true;
                    }
                }
            }

            // 만약 완전히 투명한 이미지라면 원본 반환 또는 null
            if (!hasContent) return null;

            // 2. 잘라낼 영역의 크기 계산
            int newWidth = maxX - minX + 1;
            int newHeight = maxY - minY + 1;

            // 3. 새로운 텍스처 생성 및 픽셀 복사
            Texture2D croppedTexture = new Texture2D(newWidth, newHeight);
            Color[] newPixels = @this.GetPixels(minX, minY, newWidth, newHeight);
        
            croppedTexture.SetPixels(newPixels);
            croppedTexture.Apply();

            return croppedTexture;
        }
    }
}