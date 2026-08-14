using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace KCoreKit.Scripts.Editor
{
    public class AddToTmpSpriteAssetWindow : UnityEditor.EditorWindow
    {
        private class SpriteRow
        {
            public Sprite Source;
            public string EntryName;
            public bool Include = true;
            public bool OverwriteExisting;
        }

        private static readonly int[] MaxSizeValues = { 512, 1024, 2048, 4096 };
        private static readonly string[] MaxSizeLabels = { "512", "1024", "2048", "4096" };

        private TMP_SpriteAsset _targetAsset;
        private bool _isCreatingNew;
        private string _newAssetPath;

        private bool _showOptions;
        private int _padding = 4;
        private int _maxSizeIndex = 2;

        private List<SpriteRow> _rows = new List<SpriteRow>();
        private Vector2 _scroll;

        private List<string> _resultNames;
        private string _resultAssetName;
        private string _importErrorMessage;

        private GUIStyle _titleStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _importButtonStyle;
        private GUIStyle _rowStyle;

        public static void Open(IEnumerable<Sprite> sprites)
        {
            var window = GetWindow<AddToTmpSpriteAssetWindow>(true, "Add To TMP Sprite Asset");
            window.minSize = new Vector2(480, 480);
            window._rows = sprites.Select(s => new SpriteRow { Source = s, EntryName = s.name }).ToList();
            window._targetAsset = TMP_Settings.defaultSpriteAsset;
            window._isCreatingNew = false;
            window._resultNames = null;
            window._importErrorMessage = null;
            window.Show();
        }

        private void InitStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, margin = new RectOffset(4, 4, 8, 4) };
            _sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            _importButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold, fixedHeight = 34 };
            _rowStyle = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(0, 0, 1, 1), padding = new RectOffset(4, 4, 3, 3) };
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                var icon = EditorGUIUtility.IconContent("Sprite Icon").image;
                if (icon != null)
                {
                    GUILayout.Label(icon, GUILayout.Width(24), GUILayout.Height(24));
                }
                GUILayout.Label("Add To TMP Sprite Asset", _titleStyle);
            }
            EditorGUILayout.Space(2);

            if (_resultNames != null)
            {
                DrawResultSection();
                return;
            }

            DrawTargetSection();
            EditorGUILayout.Space(6);
            DrawOptionsSection();
            EditorGUILayout.Space(6);
            DrawSpriteListSection();
            EditorGUILayout.Space(6);
            DrawImportButton();
        }

        private void DrawTargetSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("대상", _sectionTitleStyle);
                EditorGUILayout.Space(2);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var newTarget = (TMP_SpriteAsset)EditorGUILayout.ObjectField("Sprite Asset", _targetAsset, typeof(TMP_SpriteAsset), false);
                    if (check.changed)
                    {
                        _targetAsset = newTarget;
                        _isCreatingNew = false;
                    }
                }

                if (_targetAsset != null)
                {
                    EditorGUILayout.HelpBox($"현재 {_targetAsset.spriteCharacterTable.Count}개 스프라이트 포함", MessageType.Info);
                }

                EditorGUILayout.Space(2);
                if (GUILayout.Button("새로 만들기..."))
                {
                    var path = EditorUtility.SaveFilePanelInProject("새 TMP Sprite Asset", "NewSpriteAsset", "asset", "저장할 위치를 선택하세요");
                    if (!string.IsNullOrEmpty(path))
                    {
                        _newAssetPath = path;
                        _isCreatingNew = true;
                        _targetAsset = null;
                    }
                }

                if (_isCreatingNew)
                {
                    EditorGUILayout.HelpBox($"새로 생성: {_newAssetPath}", MessageType.None);
                }
                else if (_targetAsset == null)
                {
                    EditorGUILayout.HelpBox("스프라이트를 추가할 Sprite Asset을 선택하거나 새로 만드세요.", MessageType.Info);
                }
            }
        }

        private void DrawOptionsSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _showOptions = EditorGUILayout.Foldout(_showOptions, "옵션", true);
                if (_showOptions)
                {
                    _padding = Mathf.Max(0, EditorGUILayout.IntField("패딩(px)", _padding));
                    _maxSizeIndex = EditorGUILayout.Popup("최대 아틀라스 크기", _maxSizeIndex, MaxSizeLabels);
                }
            }
        }

        private void DrawSpriteListSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                int includedCount = _rows.Count(r => r.Include);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label($"스프라이트 ({includedCount}/{_rows.Count} 선택됨)", _sectionTitleStyle);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("전체 선택", EditorStyles.miniButtonLeft, GUILayout.Width(70)))
                    {
                        SetAllIncluded(true);
                    }
                    if (GUILayout.Button("전체 해제", EditorStyles.miniButtonRight, GUILayout.Width(70)))
                    {
                        SetAllIncluded(false);
                    }
                }

                EditorGUILayout.Space(2);

                var existingNames = KCoreKit.TmpSpriteAtlasPacker.GetExistingEntryNames(_targetAsset);
                var namesSeenSoFar = new HashSet<string>();

                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(140), GUILayout.MaxHeight(320));
                foreach (var row in _rows)
                {
                    DrawSpriteRow(row, existingNames, namesSeenSoFar);
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSpriteRow(SpriteRow row, HashSet<string> existingNames, HashSet<string> namesSeenSoFar)
        {
            using (new EditorGUILayout.VerticalScope(_rowStyle))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    row.Include = EditorGUILayout.Toggle(row.Include, GUILayout.Width(18));

                    var thumbnail = row.Source != null ? AssetPreview.GetMiniThumbnail(row.Source) : null;
                    if (thumbnail != null)
                    {
                        GUILayout.Label(thumbnail, GUILayout.Width(18), GUILayout.Height(18));
                    }

                    using (new EditorGUI.DisabledScope(!row.Include))
                    {
                        EditorGUILayout.ObjectField(row.Source, typeof(Sprite), false, GUILayout.Width(150));
                        row.EntryName = EditorGUILayout.TextField(row.EntryName);
                    }
                }

                if (!row.Include)
                {
                    return;
                }

                bool duplicateInBatch = !string.IsNullOrEmpty(row.EntryName) && namesSeenSoFar.Contains(row.EntryName);
                namesSeenSoFar.Add(row.EntryName);
                bool existsInTarget = !string.IsNullOrEmpty(row.EntryName) && existingNames.Contains(row.EntryName);

                if (duplicateInBatch)
                {
                    EditorGUILayout.HelpBox("선택된 다른 스프라이트와 이름이 중복됩니다. 이름을 바꿔주세요.", MessageType.Warning);
                    row.OverwriteExisting = false;
                }
                else if (existsInTarget)
                {
                    EditorGUILayout.HelpBox("대상에 이미 같은 이름의 항목이 있습니다.", MessageType.Warning);
                    row.OverwriteExisting = EditorGUILayout.ToggleLeft("기존 항목 덮어쓰기", row.OverwriteExisting);
                }
                else
                {
                    row.OverwriteExisting = false;
                }
            }
        }

        private void SetAllIncluded(bool include)
        {
            foreach (var row in _rows)
            {
                row.Include = include;
            }
        }

        private void DrawImportButton()
        {
            string blockReason = GetImportBlockReason();
            using (new EditorGUI.DisabledScope(blockReason != null))
            {
                if (GUILayout.Button("Import", _importButtonStyle))
                {
                    Import();
                }
            }

            if (blockReason != null)
            {
                EditorGUILayout.HelpBox(blockReason, MessageType.Warning);
            }

            if (!string.IsNullOrEmpty(_importErrorMessage))
            {
                EditorGUILayout.HelpBox(_importErrorMessage, MessageType.Error);
            }
        }

        private string GetImportBlockReason()
        {
            if (_targetAsset == null && !_isCreatingNew)
            {
                return "대상 Sprite Asset을 선택하거나 새로 만드세요.";
            }

            var included = _rows.Where(r => r.Include).ToList();
            if (included.Count == 0)
            {
                return "추가할 스프라이트를 하나 이상 선택하세요.";
            }

            if (included.Any(r => string.IsNullOrWhiteSpace(r.EntryName)))
            {
                return "모든 항목의 이름을 입력하세요.";
            }

            var existingNames = KCoreKit.TmpSpriteAtlasPacker.GetExistingEntryNames(_targetAsset);
            var seen = new HashSet<string>();
            foreach (var row in included)
            {
                if (!seen.Add(row.EntryName))
                {
                    return $"'{row.EntryName}' 이름이 중복됩니다.";
                }

                if (existingNames.Contains(row.EntryName) && !row.OverwriteExisting)
                {
                    return $"'{row.EntryName}'는 이미 존재합니다. 덮어쓰기를 선택하거나 이름을 바꾸세요.";
                }
            }

            return null;
        }

        private void Import()
        {
            _importErrorMessage = null;

            TMP_SpriteAsset target = _targetAsset;
            string atlasPath;

            if (_isCreatingNew)
            {
                target = CreateInstance<TMP_SpriteAsset>();
                AssetDatabase.CreateAsset(target, _newAssetPath);
                target.hashCode = TMP_TextUtilities.GetSimpleHashCode(target.name);
                var directory = Path.GetDirectoryName(_newAssetPath);
                atlasPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(_newAssetPath) + ".png").Replace("\\", "/");
            }
            else
            {
                var existingAtlasPath = target.spriteSheet != null ? AssetDatabase.GetAssetPath(target.spriteSheet) : string.Empty;
                if (!string.IsNullOrEmpty(existingAtlasPath))
                {
                    atlasPath = existingAtlasPath;
                }
                else
                {
                    var directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target));
                    atlasPath = Path.Combine(directory, target.name + ".png").Replace("\\", "/");
                }
            }

            var included = _rows.Where(r => r.Include).ToList();
            var entries = included.Select(r => new KCoreKit.TmpSpriteAtlasPacker.PendingEntry
            {
                Source = r.Source,
                EntryName = r.EntryName,
                OverwriteExisting = r.OverwriteExisting
            }).ToList();

            var report = KCoreKit.TmpSpriteAtlasPacker.Repack(target, atlasPath, entries, _padding, MaxSizeValues[_maxSizeIndex]);

            if (!report.Success)
            {
                _importErrorMessage = report.ErrorMessage;
                if (_isCreatingNew)
                {
                    var createdPath = AssetDatabase.GetAssetPath(target);
                    if (!string.IsNullOrEmpty(createdPath))
                    {
                        AssetDatabase.DeleteAsset(createdPath);
                    }
                }
                return;
            }

            _targetAsset = target;
            _isCreatingNew = false;
            _resultAssetName = target.name;
            _resultNames = report.AddedNames;
        }

        private void DrawResultSection()
        {
            EditorGUILayout.HelpBox($"{_resultNames.Count}개 스프라이트를 '{_resultAssetName}'에 추가했습니다.", MessageType.Info);
            EditorGUILayout.Space(4);

            foreach (var name in _resultNames)
            {
                var tag = $"<sprite name=\"{name}\">";
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.SelectableLabel(tag, EditorStyles.textField, GUILayout.Height(20));
                    if (GUILayout.Button("복사", GUILayout.Width(50)))
                    {
                        EditorGUIUtility.systemCopyBuffer = tag;
                    }
                }
            }

            EditorGUILayout.Space(8);
            if (GUILayout.Button("닫기", GUILayout.Height(30)))
            {
                Close();
            }
        }
    }
}
