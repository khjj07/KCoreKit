using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{ 
    public class DataTableSystem : GameSubSystemBase
    {
        [SerializeField]
        private List<DataTable> dataTables = new List<DataTable>();
        private static Dictionary<Type, List<DataTable>> _dataTableDictionary;
        public T FindRow<T>(string id) where T : DataTableRowBase
        {
            var typeList = GetDerivedTypes(typeof(T));
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

        public DataTable FindTable<T>(string tableName)
        {
            _dataTableDictionary.TryGetValue(typeof(T), out List<DataTable> tableList);
            return tableList!.Find(x=>x.name == tableName);
        }
        
        public T FindRow<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            var typeList = GetDerivedTypes(typeof(T));

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

        public List<T> FindAllRows<T>() where T : DataTableRowBase
        {
            var result = new List<T>();
            var typeList = GetDerivedTypes(typeof(T));
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

        public List<T> FindAllRows<T>(Predicate<T> predicate) where T : DataTableRowBase
        {
            var result = new List<T>();
            var typeList = GetDerivedTypes(typeof(T));

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

        public override IEnumerator OnInitialize()
        {
            _dataTableDictionary = new Dictionary<Type, List<DataTable>>();
            foreach (var asset in dataTables)
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

            yield return null;
        }
        
    }
}