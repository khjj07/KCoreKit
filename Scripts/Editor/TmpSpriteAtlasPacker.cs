using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace KCoreKit
{
    public static class TmpSpriteAtlasPacker
    {
        public class PendingEntry
        {
            public Sprite Source;
            public string EntryName;
            public bool OverwriteExisting;
        }

        public class PackReport
        {
            public bool Success;
            public string ErrorMessage;
            public List<string> AddedNames = new List<string>();
        }

        private class PixelEntry
        {
            public string Name;
            public Texture2D Pixels;
            public Vector2 OriginalSize;
            public Vector2 OriginalPivot;
        }

        public static HashSet<string> GetExistingEntryNames(TMP_SpriteAsset target)
        {
            var set = new HashSet<string>();
            if (target == null)
            {
                return set;
            }

            foreach (var character in target.spriteCharacterTable)
            {
                set.Add(character.name);
            }

            return set;
        }

        /// <summary>
        /// Fully repacks the target sprite asset's atlas from its existing entries plus the pending new/overwritten
        /// entries, then rebuilds its glyph/character tables. atlasPath is where the resulting PNG is written -
        /// pass the target's current spriteSheet path to update in place, or a new path when creating a fresh asset.
        /// </summary>
        public static PackReport Repack(TMP_SpriteAsset target, string atlasPath, List<PendingEntry> newEntries, int padding, int maxSize)
        {
            var report = new PackReport();
            var restoreReadable = new Dictionary<string, bool>();

            try
            {
                if (target != null && string.IsNullOrEmpty(target.version))
                {
                    EnsureVersionSet(target);
                }

                var pixelEntries = new List<PixelEntry>();
                var overwriteNames = new HashSet<string>(newEntries.Where(e => e.OverwriteExisting).Select(e => e.EntryName));

                if (target != null && target.spriteSheet is Texture2D existingAtlas)
                {
                    EnsureReadable(existingAtlas, restoreReadable);

                    foreach (var character in target.spriteCharacterTable)
                    {
                        if (overwriteNames.Contains(character.name))
                        {
                            continue;
                        }

                        var glyph = target.spriteGlyphTable.FirstOrDefault(g => g.index == character.glyphIndex);
                        if (glyph == null)
                        {
                            continue;
                        }

                        var rect = glyph.glyphRect;
                        var pivot = glyph.sprite != null ? glyph.sprite.pivot : new Vector2(rect.width * 0.5f, rect.height * 0.5f);

                        pixelEntries.Add(new PixelEntry
                        {
                            Name = character.name,
                            Pixels = ExtractPixels(existingAtlas, rect.x, rect.y, rect.width, rect.height),
                            OriginalSize = new Vector2(rect.width, rect.height),
                            OriginalPivot = pivot
                        });
                    }
                }

                foreach (var entry in newEntries)
                {
                    var srcTex = entry.Source.texture;
                    EnsureReadable(srcTex, restoreReadable);
                    var r = entry.Source.rect;

                    pixelEntries.Add(new PixelEntry
                    {
                        Name = entry.EntryName,
                        Pixels = ExtractPixels(srcTex, r.x, r.y, r.width, r.height),
                        OriginalSize = new Vector2(r.width, r.height),
                        OriginalPivot = entry.Source.pivot
                    });
                }

                if (pixelEntries.Count == 0)
                {
                    report.Success = false;
                    report.ErrorMessage = "패킹할 스프라이트가 없습니다.";
                    return report;
                }

                var atlas = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                var normalizedRects = atlas.PackTextures(pixelEntries.Select(e => e.Pixels).ToArray(), padding, maxSize);

                foreach (var e in pixelEntries)
                {
                    Object.DestroyImmediate(e.Pixels);
                }

                if (normalizedRects == null)
                {
                    Object.DestroyImmediate(atlas);
                    report.Success = false;
                    report.ErrorMessage = $"스프라이트를 {maxSize}px 아틀라스에 담을 수 없습니다. 최대 크기를 늘리거나 스프라이트 수를 줄이세요.";
                    return report;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(atlasPath));
                File.WriteAllBytes(atlasPath, atlas.EncodeToPNG());
                Object.DestroyImmediate(atlas);
                AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);

                var importer = (TextureImporter)AssetImporter.GetAtPath(atlasPath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;

                var newTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
                var pixelRects = new Rect[pixelEntries.Count];

#pragma warning disable 0618
                var metas = new SpriteMetaData[pixelEntries.Count];
                for (int i = 0; i < pixelEntries.Count; i++)
                {
                    var e = pixelEntries[i];
                    var nRect = normalizedRects[i];
                    var pixelRect = new Rect(
                        Mathf.Round(nRect.x * newTexture.width),
                        Mathf.Round(nRect.y * newTexture.height),
                        Mathf.Round(nRect.width * newTexture.width),
                        Mathf.Round(nRect.height * newTexture.height));
                    pixelRects[i] = pixelRect;

                    metas[i] = new SpriteMetaData
                    {
                        name = e.Name,
                        rect = pixelRect,
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(e.OriginalPivot.x / e.OriginalSize.x, e.OriginalPivot.y / e.OriginalSize.y)
                    };
                }

                importer.spritesheet = metas;
#pragma warning restore 0618
                importer.SaveAndReimport();

                var slicedSprites = AssetDatabase.LoadAllAssetsAtPath(atlasPath)
                    .OfType<Sprite>()
                    .ToDictionary(s => s.name);

                var spriteAsset = target;
                spriteAsset.spriteSheet = newTexture;

                spriteAsset.spriteCharacterTable.Clear();
                spriteAsset.spriteGlyphTable.Clear();

                for (int i = 0; i < pixelEntries.Count; i++)
                {
                    var e = pixelEntries[i];
                    if (!slicedSprites.TryGetValue(e.Name, out var slicedSprite))
                    {
                        report.Success = false;
                        report.ErrorMessage = $"'{e.Name}' 슬라이싱에 실패했습니다.";
                        return report;
                    }

                    var glyph = new TMP_SpriteGlyph
                    {
                        index = (uint)i,
                        metrics = new GlyphMetrics(e.OriginalSize.x, e.OriginalSize.y, -e.OriginalPivot.x, e.OriginalSize.y - e.OriginalPivot.y, e.OriginalSize.x),
                        glyphRect = new GlyphRect(pixelRects[i]),
                        scale = 1.0f,
                        sprite = slicedSprite
                    };

                    var character = new TMP_SpriteCharacter(0xFFFE, glyph)
                    {
                        name = e.Name,
                        scale = 1.0f
                    };

                    spriteAsset.spriteGlyphTable.Add(glyph);
                    spriteAsset.spriteCharacterTable.Add(character);
                }

                spriteAsset.SortGlyphTable();
                spriteAsset.UpdateLookupTables();

                if (spriteAsset.material == null)
                {
                    var shader = Shader.Find("TextMeshPro/Sprite");
                    var material = new Material(shader) { name = spriteAsset.name + " Material" };
                    material.SetTexture(ShaderUtilities.ID_MainTex, newTexture);
                    spriteAsset.material = material;
                    AssetDatabase.AddObjectToAsset(material, spriteAsset);
                }
                else
                {
                    spriteAsset.material.SetTexture(ShaderUtilities.ID_MainTex, newTexture);
                    EditorUtility.SetDirty(spriteAsset.material);
                }

                EditorUtility.SetDirty(spriteAsset);
                AssetDatabase.SaveAssets();

                report.Success = true;
                report.AddedNames = newEntries.Select(e => e.EntryName).ToList();
                return report;
            }
            finally
            {
                RestoreReadable(restoreReadable);
            }
        }

        /// <summary>
        /// TMP_Asset.version's setter is internal, so a freshly-created TMP_SpriteAsset never gets it assigned.
        /// TMP_SpriteAsset.Awake()/UpdateLookupTables() treat an empty version as a legacy pre-1.1.0 asset and
        /// run UpgradeSpriteAsset(), which clears the glyph/character tables and rebuilds them from the old
        /// spriteInfoList format (empty for a new asset) - silently wiping and re-saving an empty table on the
        /// next load. SerializedObject can still write the serialized backing field directly.
        /// </summary>
        private static void EnsureVersionSet(TMP_SpriteAsset target)
        {
            var serializedObject = new SerializedObject(target);
            var versionProperty = serializedObject.FindProperty("m_Version");
            versionProperty.stringValue = "1.1.0";
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Texture2D ExtractPixels(Texture2D source, float x, float y, float width, float height)
        {
            int ix = Mathf.RoundToInt(x);
            int iy = Mathf.RoundToInt(y);
            int iw = Mathf.Max(1, Mathf.RoundToInt(width));
            int ih = Mathf.Max(1, Mathf.RoundToInt(height));

            var pixels = source.GetPixels(ix, iy, iw, ih);
            var tex = new Texture2D(iw, ih, TextureFormat.RGBA32, false);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static void EnsureReadable(Texture2D tex, Dictionary<string, bool> restoreMap)
        {
            var path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path) || restoreMap.ContainsKey(path))
            {
                return;
            }

            if (AssetImporter.GetAtPath(path) is not TextureImporter importer)
            {
                return;
            }

            restoreMap[path] = importer.isReadable;
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
        }

        private static void RestoreReadable(Dictionary<string, bool> restoreMap)
        {
            foreach (var kvp in restoreMap)
            {
                if (kvp.Value)
                {
                    continue;
                }

                if (AssetImporter.GetAtPath(kvp.Key) is not TextureImporter importer)
                {
                    continue;
                }

                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }
    }
}
