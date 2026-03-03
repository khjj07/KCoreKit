using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
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
                if (GUILayout.Button("테이블 업데이트", GUILayout.Height(30)))
                {
                    dataTable.UpdateData(dataTable.csv, null);
                }
            }
            else
            {
                if (!dataTable.rowScript)
                {
                    if (GUILayout.Button("데이터 테이블 스크립트 선택", GUILayout.Height(25)))
                    {
                        // 버튼의 위치를 기준으로 드롭다운 출력
                        Rect rect = EditorGUILayout.GetControlRect(false, 0);
                        var dropdown = new RowScriptSelectorDropdown(new AdvancedDropdownState(), (selectedScript) => 
                        {
                            // 선택 시 실행될 콜백
                            Undo.RecordObject(dataTable, "Select Row Script");
                            dataTable.rowScript = selectedScript;
                            dataTable.rowTypeName = selectedScript.GetClass().AssemblyQualifiedName;
                            EditorUtility.SetDirty(dataTable);
            
                            // 시리얼라이즈드 프로퍼티 업데이트가 필요한 경우
                            serializedObject.Update(); 
                        });
        
                        dropdown.Show(rect);
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("monoScript", dataTable.rowScript, typeof(MonoScript), false);
                    GUI.enabled = true;
                    if (GUILayout.Button("테이블 생성 및 연동"))
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
            // C# 기본 타입 별칭(Alias) 매핑
            if (type == typeof(string))
                return "string";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(double))
                return "double";
            else if (type.IsEnum)
                return "enum";

            // 제네릭 타입 처리 (List<T> 등)
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

        [MenuItem("Assets/Default/Copy DataTable", priority = -101)]
        public static void CopyAsset()
        {
            var asset = Selection.activeObject as DataTable;
            if (asset != null) // null 체크는 항상 좋은 습관입니다.
            {
                asset.CopyAsset();
            }
        }

        [MenuItem("Assets/Default/Copy DataTable", true)]
        public static bool ValidateCopyAsset()
        {
            return Selection.activeObject is DataTable;
        }
    }

    [CustomEditor(typeof(DataTableRowBase), true)]
    public class DataTableRowBaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
    
#endif


    public abstract class DataTableRowBase : ScriptableObject
    {
        public string id;
        public bool isEnable;
    }

    public class DataTable : ScriptableObject
    {
#if UNITY_EDITOR
        [ReadOnly] public MonoScript rowScript;
#endif
        [ReadOnly] public TextAsset csv;
        [SerializeField] [HideInInspector] public List<DataTableRowBase> dataList = new List<DataTableRowBase>();
        [ReadOnly] public string rowTypeName;
#if UNITY_EDITOR
        [MenuItem("Assets/Default/Create/DataTable")]
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

            // GenerateData 로직에 따라 첫 번째 컬럼은 "id"라고 가정하고 추가
            headers.Add("id");

            foreach (FieldInfo field in allFields)
            {
                // 이미 추가된 "id" 필드는 제외
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
                // 명시적 바이트 변환 대신 문자열로 직접 저장 (유니티 표준 방식)
                File.WriteAllText(finalPath, csvHeader, System.Text.Encoding.UTF8);
                AssetDatabase.ImportAsset(finalPath); // Refresh보다 더 정확한 개별 임포트
    
                var newAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(finalPath);

                if (newAsset != null)
                {
                    EditorGUIUtility.PingObject(newAsset);
                }

                Debug.Log($"✅ CSV 템플릿이 성공적으로 저장되었습니다: {finalPath}");

                return newAsset;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ CSV 파일 저장 중 오류 발생: {e.Message}");
            }

            return null;
        }

        public async Task UpdateData(TextAsset csvAsset, Action<object, Dictionary<string, string>> customAction)
        {
            rowTypeName = GetRowType().AssemblyQualifiedName;
            var newList = new List<DataTableRowBase>();
            List<Dictionary<string, string>> csv = CSVReader.Read(csvAsset);

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
                    EditorUtility.SetDirty(this);
                }

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
                        object parsedValue = null;
                        // 기본 타입 처리

                        if (fieldType.IsGenericType)
                        {
                            parsedValue = await ProcessGenericType(fieldType, rawValue);
                            field.SetValue(asset, parsedValue);
                        }
                        else
                        {
                            if (fieldType == typeof(string))
                            {
                                parsedValue = rawValue;
                                field.SetValue(asset, parsedValue);
                            }
                            else if (fieldType == typeof(int))
                            {
                                parsedValue = int.Parse(rawValue);
                                field.SetValue(asset, parsedValue);
                            }
                            else if (fieldType == typeof(float))
                            {
                                parsedValue = float.Parse(rawValue);
                                field.SetValue(asset, parsedValue);
                            }
                            else if (fieldType == typeof(bool))
                            {
                                parsedValue = bool.Parse(rawValue);
                                field.SetValue(asset, parsedValue);
                            }
                            else if (fieldType.IsEnum)
                            {
                                parsedValue = Enum.Parse(fieldType, rawValue);
                                field.SetValue(asset, parsedValue);
                            }
                            else if (typeof(MonoBehaviour).IsAssignableFrom(fieldType))
                            {
                                await AddressableExtension.LoadAsset<GameObject>(rawValue,
                                    x =>
                                    {
                                        parsedValue = x.GetComponent(fieldType);
                                        field.SetValue(asset, parsedValue);
                                    });
                            }
                            else if (typeof(Object).IsAssignableFrom(fieldType))
                            {
                                await AddressableExtension.LoadAsset<Object>(rawValue, x =>
                                {
                                    field.SetValue(asset, x);
                                });
                            }
                        }


                        Debug.Log(rawValue);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌[{name}]-[Column : {field.Name}, Row : {rowCount}] 처리 중 오류 발생: {e.Message}");
                    }

                    customAction?.Invoke(asset, row);
                    asset.name = asset.id;
                }

                EditorUtility.SetDirty(asset);
                EditorUtility.SetDirty(this);
                newList.Add(asset);
                rowCount++;
            }

            // 기존에 존재했으나 CSV에 없는 데이터는 제거
            foreach (DataTableRowBase dataAsset in dataList)
            {
                if (!newList.Exists(x => x.id == dataAsset.id))
                {
                    DestroyImmediate(dataAsset, true);
                }
                else if (dataAsset.name == "" || dataAsset.id == "")
                {
                    DestroyImmediate(dataAsset, true);
                }
            }

            //쓰레기 청소
            var path = AssetDatabase.GetAssetPath(this);
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in allAssets)
            {
                if (asset.name == "")
                {
                    DestroyImmediate(asset, true);
                }
            }

            dataList = newList;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static async Task<object> ProcessGenericType(Type fieldType, string rawValue)
        {
            Type elementType = fieldType.GetGenericArguments()[0];

            var list = rawValue.ParseStringList();
            object result = null;
            if (elementType == typeof(string))
            {
                result = new ReadOnlyList<string>(list.ConvertAll(x => x.ToString()));
            }
            else if (elementType == typeof(float))
                result = list.ConvertAll(float.Parse);
            else if (elementType == typeof(int))
                result = list.ConvertAll(int.Parse);
            else if (elementType == typeof(bool))
                result = list.ConvertAll(bool.Parse);
            else if (elementType.IsEnum)
            {
                Type targetListType = typeof(ReadOnlyList<>).MakeGenericType(elementType);
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
                Type targetListType = typeof(ReadOnlyList<>).MakeGenericType(elementType);
                var targetList = Activator.CreateInstance(targetListType);
                MethodInfo addMethod = targetListType.GetMethod("Add");
                foreach (string item in list)
                {
                    await AddressableExtension.LoadAsset<GameObject>(item, 
                        x => { addMethod.Invoke(targetList, new object[] { x.GetComponent(fieldType) }); });
                }
                
                result = targetList;
            }
            else if (typeof(Object).IsAssignableFrom(elementType))
            {
                Type targetListType = typeof(ReadOnlyList<>).MakeGenericType(elementType);
                var targetList = Activator.CreateInstance(targetListType);
                MethodInfo addMethod = targetListType.GetMethod("Add");
                foreach (string item in list)
                {
                    await AddressableExtension.LoadAsset<Object>(item,
                        x => { addMethod.Invoke(targetList, new object[] { x }); });
                }

                result = targetList;
            }

            return result;
        }

#endif
        public string GetRowTypeName()
        {
            return rowTypeName;
        }
    }
}