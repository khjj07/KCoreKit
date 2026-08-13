using System.Collections.Generic;
using System.Linq;
using Ami.BroAudio;
using Ami.BroAudio.Data;
using Ami.BroAudio.Editor;
using UnityEditor;
using UnityEngine;

namespace KCoreKit.Scripts.Editor
{
    public class AddToBroAudioAssetWindow : UnityEditor.EditorWindow
    {
        private enum ImportMode
        {
            OnePerClip,
            SingleMultiClip,
        }

        private class ClipEntry
        {
            public AudioClip Clip;
            public string EntityName;
            public bool Include = true;
        }

        private static readonly string[] ModeToolbarLabels = { "클립마다 개별 엔티티", "하나의 멀티클립 엔티티" };

        private AudioAsset _targetAsset;
        private BroAudioType _audioType = BroAudioType.SFX;
        private ImportMode _mode = ImportMode.OnePerClip;
        private string _multiClipEntityName = "New Sound";
        private List<ClipEntry> _clips = new List<ClipEntry>();
        private Vector2 _scroll;

        private GUIStyle _titleStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _importButtonStyle;
        private GUIStyle _rowStyle;

        public static void Open(IEnumerable<AudioClip> clips)
        {
            var window = GetWindow<AddToBroAudioAssetWindow>(true, "Add To BroAudio Asset");
            window.minSize = new Vector2(460, 420);
            window._clips = clips.Select(c => new ClipEntry { Clip = c, EntityName = c.name }).ToList();
            if (window._clips.Count > 0)
            {
                window._multiClipEntityName = window._clips[0].EntityName;
            }
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
                var icon = EditorGUIUtility.IconContent("AudioClip Icon").image;
                if (icon != null)
                {
                    GUILayout.Label(icon, GUILayout.Width(24), GUILayout.Height(24));
                }
                GUILayout.Label("Add To BroAudio Asset", _titleStyle);
            }
            EditorGUILayout.Space(2);

            DrawTargetSection();
            EditorGUILayout.Space(6);
            DrawImportModeSection();
            EditorGUILayout.Space(6);
            DrawClipListSection();
            EditorGUILayout.Space(6);
            DrawImportButton();
        }

        private void DrawTargetSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("대상", _sectionTitleStyle);
                EditorGUILayout.Space(2);

                _targetAsset = (AudioAsset)EditorGUILayout.ObjectField("Audio Asset", _targetAsset, typeof(AudioAsset), false);
                _audioType = (BroAudioType)EditorGUILayout.EnumPopup("Audio Type", _audioType);

                if (_targetAsset == null)
                {
                    EditorGUILayout.HelpBox("클립을 추가할 Audio Asset을 선택하세요.", MessageType.Info);
                }
            }
        }

        private void DrawImportModeSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("가져오기 방식", _sectionTitleStyle);
                EditorGUILayout.Space(2);

                _mode = (ImportMode)GUILayout.Toolbar((int)_mode, ModeToolbarLabels);

                EditorGUILayout.HelpBox(
                    _mode == ImportMode.OnePerClip
                        ? "선택한 클립마다 개별 Audio Entity를 생성합니다."
                        : "선택한 클립을 모두 하나의 Audio Entity(멀티클립)로 묶습니다.",
                    MessageType.None);

                if (_mode == ImportMode.SingleMultiClip)
                {
                    _multiClipEntityName = EditorGUILayout.TextField("Entity Name", _multiClipEntityName);
                }
            }
        }

        private void DrawClipListSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                int includedCount = _clips.Count(c => c.Include);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label($"클립 ({includedCount}/{_clips.Count} 선택됨)", _sectionTitleStyle);
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

                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(120), GUILayout.MaxHeight(280));
                foreach (var entry in _clips)
                {
                    DrawClipRow(entry);
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawClipRow(ClipEntry entry)
        {
            using (new EditorGUILayout.HorizontalScope(_rowStyle))
            {
                entry.Include = EditorGUILayout.Toggle(entry.Include, GUILayout.Width(18));

                var thumbnail = entry.Clip != null ? AssetPreview.GetMiniThumbnail(entry.Clip) : null;
                if (thumbnail != null)
                {
                    GUILayout.Label(thumbnail, GUILayout.Width(18), GUILayout.Height(18));
                }

                using (new EditorGUI.DisabledScope(!entry.Include))
                {
                    EditorGUILayout.ObjectField(entry.Clip, typeof(AudioClip), false, GUILayout.Width(150));

                    GUILayout.Label(FormatLength(entry.Clip), EditorStyles.miniLabel, GUILayout.Width(45));

                    using (new EditorGUI.DisabledScope(_mode != ImportMode.OnePerClip))
                    {
                        entry.EntityName = EditorGUILayout.TextField(entry.EntityName);
                    }
                }
            }
        }

        private static string FormatLength(AudioClip clip)
        {
            if (clip == null)
            {
                return string.Empty;
            }

            var time = System.TimeSpan.FromSeconds(clip.length);
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }

        private void SetAllIncluded(bool include)
        {
            foreach (var entry in _clips)
            {
                entry.Include = include;
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
        }

        private string GetImportBlockReason()
        {
            if (_targetAsset == null)
            {
                return "Audio Asset을 선택하세요.";
            }

            var included = _clips.Where(c => c.Include).ToList();
            if (included.Count == 0)
            {
                return "가져올 클립을 하나 이상 선택하세요.";
            }

            if (_mode == ImportMode.OnePerClip && included.Any(c => string.IsNullOrWhiteSpace(c.EntityName)))
            {
                return "모든 클립의 Entity Name을 입력하세요.";
            }

            if (_mode == ImportMode.SingleMultiClip && string.IsNullOrWhiteSpace(_multiClipEntityName))
            {
                return "Entity Name을 입력하세요.";
            }

            return null;
        }

        private void Import()
        {
            var included = _clips.Where(c => c.Include).ToList();
            if (_targetAsset == null || included.Count == 0)
            {
                return;
            }

            var assetEditor = (AudioAssetEditor)UnityEditor.Editor.CreateEditor(_targetAsset, typeof(AudioAssetEditor));
            var createdEntityEditors = new List<AudioEntityEditor>();

            try
            {
                if (_mode == ImportMode.OnePerClip)
                {
                    foreach (var entry in included)
                    {
                        createdEntityEditors.Add(CreateEntity(assetEditor, entry.EntityName, new List<AudioClip> { entry.Clip }));
                    }
                }
                else
                {
                    createdEntityEditors.Add(CreateEntity(assetEditor, _multiClipEntityName, included.Select(c => c.Clip).ToList()));
                }

                assetEditor.Verify();
                assetEditor.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
            finally
            {
                foreach (var entityEditor in createdEntityEditors)
                {
                    DestroyImmediate(entityEditor);
                }

                DestroyImmediate(assetEditor);
            }

            Debug.Log($"[BroAudio] {included.Count}개 클립을 '{_targetAsset.AssetName}' 에 추가했습니다.");
            Close();
        }

        private AudioEntityEditor CreateEntity(AudioAssetEditor assetEditor, string entityName, List<AudioClip> clips)
        {
            var (_, entityEditor) = assetEditor.CreateNewEntity(entityName, _audioType);
            var clipListProp = entityEditor.serializedObject.FindProperty(nameof(AudioEntity.Clips));

            for (int i = 0; i < clips.Count; i++)
            {
                assetEditor.SetClipList(clipListProp, i, clips[i]);
            }

            entityEditor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            entityEditor.serializedObject.Update();
            return entityEditor;
        }
    }
}
