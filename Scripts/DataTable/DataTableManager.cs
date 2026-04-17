using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using UnityEngine;

namespace KCoreKit
{ 
    public class DataTableManager : Singleton<DataTableManager>
    {
        private DataTable[] _dataTables;
        private static bool _isLoaded;
        public static Action onLoad;
        private static Dictionary<Type, List<DataTable>> _dataTableDictionary;

        public void Start()
        {
            _dataTables = Resources.LoadAll<DataTable>("");
            _dataTableDictionary = new Dictionary<Type, List<DataTable>>();
            foreach (var asset in _dataTables)
            {
                var type = asset.rowTypeName;
                var key = Type.GetType(type);
                if (key != null && _dataTableDictionary.ContainsKey(key))
                {
                    _dataTableDictionary[key].Add(asset);
                }
                else
                {
                    _dataTableDictionary.TryAdd(Type.GetType(type), new List<DataTable>() { asset });
                }
            }
            onLoad?.Invoke();
            _isLoaded = true;
        }
        
        public static T FindRow<T>(string id) where T : DataTableRowBase
        {
            var typeList = GetInstance().GetDerivedTypes(typeof(T));
            T result = null;

            foreach (var type in typeList)
            {
                var dtList = _dataTableDictionary[type];
                if (dtList != null)
                {
                    foreach (var dt in dtList)
                    {
                        result = dt.Find<T>(id);
                        if (result)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        private List<Type> GetDerivedTypes(Type type)
        {
            var typeList = _dataTableDictionary.Keys.ToList();
            var derivedTypeList = typeList.FindAll(type.IsAssignableFrom);
            return derivedTypeList;
        }

        public static DataTable FindTable<T>(string tableName)
        {
            _dataTableDictionary.TryGetValue(typeof(T), out List<DataTable> tableList);
            return tableList!.Find(x=>x.name == tableName);
        }
        public static T FindRow<T>() where T : DataTableRowBase
        {
            var typeList = GetInstance().GetDerivedTypes(typeof(T));

            foreach (var type in typeList)
            {
                var dtList = _dataTableDictionary[type];
                foreach (var dt in dtList)
                {
                    return dt.Get<T>()[0];
                }
            }

            return null;
        }
        
        public static T FindRow<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            var typeList = GetInstance().GetDerivedTypes(typeof(T));

            foreach (var type in typeList)
            {
                var dtList = _dataTableDictionary[type];
                foreach (var dt in dtList)
                {
                    return dt.Find<T>(predicate);
                }
            }

            return null;
        }

        public static List<T> FindAllRows<T>() where T : DataTableRowBase
        {
            var result = new List<T>();
            var typeList = GetInstance().GetDerivedTypes(typeof(T));
            foreach (var type in typeList)
            {
                var dtList = _dataTableDictionary[type];
                foreach (var dt in dtList)
                {
                    result.AddRange(dt.FindAll<T>()); 
                }
            }

            return result;
        }

        public static List<T> FindAllRows<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            var result = new List<T>();
            var typeList = GetInstance().GetDerivedTypes(typeof(T));

            foreach (var type in typeList)
            {
                var dtList = _dataTableDictionary[type];
                foreach (var dt in dtList)
                {
                    result.AddRange(dt.FindAll<T>(predicate)); 
                }
            }

            return result;
        }

        public static void AddOnLoadAction(Action action)
        {
            if (_isLoaded)
            {
                action.Invoke();
            }
            else
            {
                onLoad += action.Invoke;
            }
        }
    }
}