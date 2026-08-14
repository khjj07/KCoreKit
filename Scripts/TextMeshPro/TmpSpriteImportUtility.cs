using System.Collections.Generic;
using System.Linq;
using KCoreKit.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace KCoreKit.Scripts.TextMeshPro
{
    public static class TmpSpriteImportUtility
    {
        [MenuItem("Assets/KCoreKit/TextMeshPro/Add To Sprite Asset", true)]
        public static bool ValidateAddToSpriteAsset()
        {
            return Selection.objects.Any(obj => obj is Sprite || obj is Texture2D);
        }

        [MenuItem("Assets/KCoreKit/TextMeshPro/Add To Sprite Asset", priority = 30)]
        public static void AddToSpriteAsset()
        {
            var sprites = ResolveSelectionToSprites(Selection.objects);
            if (sprites.Count == 0)
            {
                return;
            }

            AddToTmpSpriteAssetWindow.Open(sprites);
        }

        private static List<Sprite> ResolveSelectionToSprites(Object[] selection)
        {
            var result = new List<Sprite>();
            var seen = new HashSet<Sprite>();

            foreach (var obj in selection)
            {
                if (obj is Sprite sprite)
                {
                    if (seen.Add(sprite))
                    {
                        result.Add(sprite);
                    }

                    continue;
                }

                if (obj is Texture2D texture)
                {
                    var path = AssetDatabase.GetAssetPath(texture);
                    var subSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();
                    foreach (var sub in subSprites)
                    {
                        if (seen.Add(sub))
                        {
                            result.Add(sub);
                        }
                    }
                }
            }

            return result;
        }
    }
}
