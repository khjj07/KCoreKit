using System.IO;
using System.Text;
using UnityEngine;

namespace KCoreKit
{
    public class BinarySaveFile<T> : BinarySaveFileBase where T : ISaveData, new() 
    {
        private T _value;
        public BinarySaveFile(string id)
        {
            _value = new T();
            _id = id;
        }
        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            _value = value;
        }
        public override void Save()
        {
            var jsonString = JsonUtility.ToJson(_value, true);
            using FileStream fileStream = File.Open(GetPath(), FileMode.OpenOrCreate);
            using BinaryWriter binaryWriter = new(fileStream, Encoding.UTF8);
            binaryWriter.Write(jsonString);
            binaryWriter.Close();
            fileStream.Close();
#if UNITY_EDITOR
            Debug.Log(_id + " is saved!");
            Debug.Log(GetPath());
#endif
        }
        public override void Load()
        {
            if (File.Exists(GetPath()))
            {
                using FileStream fileStream = File.Open(GetPath(), FileMode.Open);
                using BinaryReader binaryReader = new(fileStream);
                var jsonString = binaryReader.ReadString();
                _value = JsonUtility.FromJson<T>(jsonString);
                binaryReader.Close();
                fileStream.Close();
#if UNITY_EDITOR
                Debug.Log(_id + " is loaded!");
                Debug.Log(GetPath());
#endif
            }
        }

        public void Remove()
        {
            if (File.Exists(GetPath()))
            {
                File.Delete(GetPath());
#if UNITY_EDITOR
                Debug.Log(_id + " is removed!");
                Debug.Log(GetPath());
#endif
            }
        }

        public bool Exist()
        {
            return File.Exists(GetPath());
        }

     
    }
}