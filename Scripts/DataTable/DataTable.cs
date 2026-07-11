using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

using UnityEngine;
using Object = UnityEngine.Object;

namespace KCoreKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DataTable), true)]
    public class DataTableInspector : Editor
    {
        private SerializedProperty scriptProp;
        private int pickerControlID;

        public override void OnInspectorGUI()
        {
            DataTable dataTable = (DataTable)target;
            scriptProp = serializedObject.FindProperty("rowScript");
            if (dataTable.csv)
            {
                base.OnInspectorGUI();

                SyncCSV(dataTable);
                GUILayout.Space(10);
                DrawTableFields(dataTable);
                GUILayout.Space(10);
                
                if (GUILayout.Button("Refresh", GUILayout.Height(30)))
                {
                    // [수정됨] 비동기 작업 대기를 위해 별도 메서드로 분리
                    RefreshDataAsync(dataTable);
                }
                
                if (GUILayout.Button("Clear", GUILayout.Height(30)))
                {
                    dataTable.Clear();
                }
            }
            else
            {
                if (!dataTable.rowScript)
                {
                    if (GUILayout.Button("Choose DataTableRow Script", GUILayout.Height(25)))
                    {
                        Rect rect = EditorGUILayout.GetControlRect(false, 0);
                        var dropdown = new MonoScriptSelectorDropdown(new AdvancedDropdownState(), (selectedScript) => 
                        {
                            Undo.RecordObject(dataTable, "Select Row Script");
                            dataTable.rowScript = selectedScript;
                            dataTable.rowTypeName = selectedScript.GetClass().AssemblyQualifiedName;
                            EditorUtility.SetDirty(dataTable);
            
                            serializedObject.Update(); 
                        });
                        dropdown.Setup(typeof(DataTableRowBase));
        
                        dropdown.Show(rect);
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("monoScript", dataTable.rowScript, typeof(MonoScript), false);
                    GUI.enabled = true;
                    if (GUILayout.Button("Generate Managing Table"))
                    {
                        dataTable.csv = dataTable.GenerateCsv();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == pickerControlID)
                {
                    dataTable.rowScript = EditorGUIUtility.GetObjectPickerObject() as MonoScript;
                    dataTable.rowTypeName = dataTable.GetRowType().AssemblyQualifiedName;
                    Event.current.Use();
                    GUI.changed = true;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        // [추가됨] Task를 정상적으로 await 하여 데이터 누락을 방지하는 비동기 래퍼 메서드
        private async void RefreshDataAsync(DataTable dataTable)
        {
            dataTable.Clear();
            await dataTable.UpdateData(dataTable.csv, null);
            
            // 안전을 위해 UpdateData 완료 후 최종 저장 및 리프레시 명시
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }

        private void DrawTableFields(DataTable dataTable)
        {
            Type targetType = dataTable.rowScript.GetClass();
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField($"{targetType.Name} 클래스 필드 ({targetType.GetFields().Length}개)");

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("필드 이름", headerStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f));
            GUILayout.Label("필드 타입", headerStyle);
            EditorGUILayout.EndHorizontal();
            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            GUIStyle evenStyle = new GUIStyle(EditorStyles.label);
            GUIStyle oddStyle = new GUIStyle(EditorStyles.label);
            Color baseColor = EditorGUIUtility.isProSkin
                ? new Color(0.25f, 0.25f, 0.25f)
                : new Color(0.35f, 0.35f, 0.35f);
            evenStyle.normal.background = Texture2D.blackTexture;
            oddStyle.normal.background = CreateColorTexture(baseColor);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                GUIStyle currentStyle = (i % 2 == 0) ? evenStyle : oddStyle;
                EditorGUILayout.BeginHorizontal(currentStyle);
                GUILayout.Label(field.Name, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f));
                GUILayout.Label(GetTypeName(field.FieldType));
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void SyncCSV(DataTable dataTable)
        {
            if (dataTable.name != dataTable.csv.name)
            {
                var path = AssetDatabase.GetAssetPath(dataTable.csv);
                AssetDatabase.RenameAsset(path, dataTable.name);
                EditorUtility.SetDirty(dataTable);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private string GetTypeName(Type type)
        {
            if (type == typeof(string)) return "string";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(float)) return "float";
            else if (type == typeof(bool)) return "bool";
            else if (type == typeof(double)) return "double";
            else if (type.IsEnum) return "enum";

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                Type[] typeArgs = type.GetGenericArguments();

                if (genericType == typeof(System.Collections.Generic.List<>))
                {
                    return $"List<{GetTypeName(typeArgs[0])}>";
                }
            }

            if (type.IsArray)
            {
                return $"{GetTypeName(type.GetElementType())}[]";
            }

            return type.Name;
        }

        private Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        [MenuItem("Assets/KCoreKit/Copy DataTable", priority = -101)]
        public static void CopyAsset()
        {
            var asset = Selection.activeObject as DataTable;
            if (asset != null)
            {
                asset.CopyAsset();
            }
        }

        [MenuItem("Assets/KCoreKit/Copy DataTable", true)]
        public static bool ValidateCopyAsset()
        {
            return Selection.activeObject is DataTable;
        }
    }
#endif

    public class DataTable : ScriptableObject
    {
#if UNITY_EDITOR
        [ReadOnly] public MonoScript rowScript;
#endif
        [ReadOnly] public TextAsset csv;
        [SerializeField] [HideInInspector] public List<DataTableRowBase> dataList = new List<DataTableRowBase>();
        [ReadOnly] public string rowTypeName;
        
#if UNITY_EDITOR
        
        [MenuItem("Assets/KCoreKit/Create/DataTable")]
        public static void Create()
        {
            TypeExtension.CreateAsset<DataTable>("DataTable");
        }

        public void CopyAsset()
        {
            var originPaths = new string[]
            {
                AssetDatabase.GetAssetPath(this),
                AssetDatabase.GetAssetPath(csv)
            };

            var copyPaths = new string[]
            {
                AssetDatabase.GetAssetPath(this).Replace(".asset", "_copy.asset"),
                AssetDatabase.GetAssetPath(csv).Replace(".csv", "_copy.csv"),
            };
            AssetDatabase.CopyAsset(originPaths[0], copyPaths[0]);
            AssetDatabase.CopyAsset(originPaths[1], copyPaths[1]);

            var asset = AssetDatabase.LoadAssetAtPath<DataTable>(copyPaths[0]);
            var copyCsv = AssetDatabase.LoadAssetAtPath<TextAsset>(copyPaths[1]);
            asset.csv = copyCsv;
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
#if UNITY_EDITOR
        public Type GetRowType()
        {
            return rowScript.GetClass();
        }
#endif
        public List<T> Get<T>() where T : DataTableRowBase
        {
            return dataList.ConvertAll<T>(x => x as T);
        }

        public T Find<T>(string id) where T : DataTableRowBase
        {
            return Get<T>().Find(x => x.id == id);
        }

        public T Find<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            return Get<T>().Find(predicate);
        }

        public List<T> FindAll<T>() where T : DataTableRowBase
        {
            return Get<T>();
        }

        public List<T> FindAll<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            return Get<T>().FindAll(predicate);
        }

#if UNITY_EDITOR
        public TextAsset GenerateCsv()
        {
            Type dataType = rowScript.GetClass();

            FieldInfo[] allFields =
                dataType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            List<string> headers = new List<string>();
            
            headers.Add("id");

            foreach (FieldInfo field in allFields)
            {
                if (field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                headers.Add(field.Name);
            }

            string csvHeader = string.Join(",", headers);
            string assetPath = AssetDatabase.GetAssetPath(this);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("현재 DataTable 에셋의 경로를 찾을 수 없습니다.");
                return null;
            }

            string directoryPath = Path.GetDirectoryName(assetPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);

            string finalPath = Path.Combine(directoryPath, fileNameWithoutExtension + ".csv");
            System.Text.Encoding encoder = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

            try
            {
                File.WriteAllText(finalPath, csvHeader, System.Text.Encoding.UTF8);
                AssetDatabase.ImportAsset(finalPath); 
    
                var newAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(finalPath);

                if (newAsset != null)
                {
                    EditorGUIUtility.PingObject(newAsset);
                }

                Debug.Log($"✅ CSV 템플릿 생성 성공: {finalPath}");

                return newAsset;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ CSV 파일 저장 중 오류 발생: {e.Message}");
            }

            return null;
        }
#endif
    
        public async Task UpdateData(TextAsset csvAsset, Action<object, Dictionary<string, string>> customAction)
        {
            rowTypeName = GetRowType().AssemblyQualifiedName;
            var newList = new List<DataTableRowBase>();
            List<Dictionary<string, string>> csv = CSVReader.Read(csvAsset);
            var tasks = new List<Task>();

            int rowCount = 1;
            foreach (var row in csv)
            {
                if (bool.Parse(row["isEnable"]) != true)
                {
                    continue;
                }

                DataTableRowBase asset = dataList.Find(x => x.id == row["id"]);

                if (!asset)
                {
                    asset = CreateInstance(rowScript.GetClass()) as DataTableRowBase;
                    AssetDatabase.AddObjectToAsset(asset, this);
                }
                asset.SetRawData(row);
                Type type = rowScript.GetClass();
                FieldInfo[] allFields =
                    type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (FieldInfo field in allFields)
                {
                    if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                        continue;

                    string key = row.Keys.FirstOrDefault(k => k.Equals(field.Name, StringComparison.OrdinalIgnoreCase));
                    if (key == null) continue;

                    string rawValue = row[key];
                    Type fieldType = field.FieldType;

                    try
                    {
                        if (string.IsNullOrEmpty(rawValue))
                        {
                            field.SetValue(asset, null);
                        }
                        else if (fieldType.IsGenericType)
                        {
                            // [수정됨] 백그라운드 스레드에서 SetValue 실행을 방지하기 위해 로컬 async 래퍼 사용
                            async Task AssignGenericTypeAsync()
                            {
                                var result = await ProcessGenericType(fieldType, rawValue);
                                field.SetValue(asset, result);
#if UNITY_EDITOR
                                EditorUtility.SetDirty(asset);
#endif
                            }
                            tasks.Add(AssignGenericTypeAsync());
                        }
                        else
                        {
                            if (fieldType == typeof(string))
                            {
                                field.SetValue(asset, rawValue);
                            }
                            else if (fieldType == typeof(int))
                            {
                                field.SetValue(asset, int.Parse(rawValue));
                            }
                            else if (fieldType == typeof(float))
                            {
                                field.SetValue(asset, float.Parse(rawValue));
                            }
                            else if (fieldType == typeof(bool))
                            {
                                field.SetValue(asset, bool.Parse(rawValue));
                            }
                            else if (fieldType.IsEnum)
                            {
                                field.SetValue(asset, Enum.Parse(fieldType, rawValue));
                            }
                            else if (typeof(MonoBehaviour).IsAssignableFrom(fieldType))
                            {
                                // Addressable 방식 유지
                                tasks.Add(AddressableExtension.LoadAsset<GameObject>(rawValue, x =>
                                {
                                    if (x != null)
                                    {
                                        field.SetValue(asset, x.GetComponent(fieldType));
#if UNITY_EDITOR
                                        // [수정됨] 콜백 완료 시점에 다시 한 번 Dirty 마킹 (매우 중요)
                                        EditorUtility.SetDirty(asset); 
#endif
                                    }
                                }));
                            }
                            else if (typeof(Object).IsAssignableFrom(fieldType))
                            {
                                // Addressable 방식 유지
                                tasks.Add(AddressableExtension.LoadAsset<Object>(rawValue, x =>
                                {
                                    field.SetValue(asset, x);
#if UNITY_EDITOR
                                    // [수정됨] 콜백 완료 시점에 다시 한 번 Dirty 마킹 (매우 중요)
                                    EditorUtility.SetDirty(asset);
#endif
                                }));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌[{name}]-[Column : {field.Name}, Row : {rowCount}] 처리 중 오류 발생: {e.Message}");
                    }

                    customAction?.Invoke(asset, row);
                    asset.name = asset.id;
                }
                
                EditorUtility.SetDirty(asset);
                newList.Add(asset);
                rowCount++;
            }

            await Task.WhenAll(tasks);

            foreach (var item in newList)
            {
                EditorUtility.SetDirty(item);
            }

            dataList = newList;
            EditorUtility.SetDirty(this);
            
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private static async Task<object> ProcessGenericType(Type fieldType, string rawValue)
        {
            Type elementType = fieldType.GetGenericArguments()[0];

            var list = rawValue.ParseStringList();
            object result = null;
            if (elementType == typeof(string))
            {
                result = new List<string>(list.ConvertAll(x => x.ToString()));
            }
            else if (elementType == typeof(float))
                result = list.ConvertAll(float.Parse);
            else if (elementType == typeof(int))
                result = list.ConvertAll(int.Parse);
            else if (elementType == typeof(bool))
                result = list.ConvertAll(bool.Parse);
            else if (elementType.IsEnum)
            {
                Type targetListType = typeof(List<>).MakeGenericType(elementType);
                var targetList = Activator.CreateInstance(targetListType);
                MethodInfo addMethod = targetListType.GetMethod("Add");
                foreach (string item in list)
                {
                    addMethod.Invoke(targetList, new object[] { Enum.Parse(elementType, item) });
                }

                result = targetList;
            }
            else if (typeof(MonoBehaviour).IsAssignableFrom(elementType))
            {
                Type targetListType = typeof(List<>).MakeGenericType(elementType);
                var targetList = Activator.CreateInstance(targetListType);
                MethodInfo addMethod = targetListType.GetMethod("Add");
                foreach (string item in list)
                {
                    // Addressable 방식 유지
                    await AddressableExtension.LoadAsset<GameObject>(item, 
                        x => { 
                            if(x != null) addMethod.Invoke(targetList, new object[] { x.GetComponent(elementType) }); 
                        });
                }
                
                result = targetList;
            }
            else if (typeof(Object).IsAssignableFrom(elementType))
            {
                Type targetListType = typeof(List<>).MakeGenericType(elementType);
                var targetList = Activator.CreateInstance(targetListType);
                MethodInfo addMethod = targetListType.GetMethod("Add");
                foreach (string item in list)
                {
                    // Addressable 방식 유지
                    await AddressableExtension.LoadAsset<Object>(item,
                        x => { 
                            if(x != null) addMethod.Invoke(targetList, new object[] { x }); 
                        });
                }

                result = targetList;
            }

            return result;
        }

#if UNITY_EDITOR
        public void Clear()
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("메인 에셋의 경로를 찾을 수 없습니다.");
                return;
            }

            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            for (int i = subAssets.Length - 1; i >= 0; i--)
            {
                Object subAsset = subAssets[i];
                if (subAsset != null)
                {
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    DestroyImmediate(subAsset, true);
                }
            }

            dataList?.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"{subAssets.Length}개의 하위 에셋을 모두 찾아서 삭제했습니다.");
        }
#endif
        public string GetRowTypeName()
        {
            return rowTypeName;
        }
    }
}