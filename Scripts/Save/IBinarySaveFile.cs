using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{
    public abstract class BinarySaveFileBase
    {
        protected string _id;
        protected string _folderPath;
        public abstract void Save();
        public abstract void Load();
        public string GetID()
        {
            return _id;
        }
        public void SetFolderPath(string path)
        {
            _folderPath = path;
        }
        public string GetRelativeFolderPath()
        {
            return _folderPath + "/";
        }
        public string GetAbsoluteFolderPath()
        {
            return Application.persistentDataPath + "/" + _folderPath + "/";
        }
        public string GetPath()
        {
            return Application.persistentDataPath + "/" + _folderPath + "/" + _id;
        }

        public static string[] FindAllData(string folderPath)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + folderPath))
            {
                return Array.Empty<string>();
            }
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/" + folderPath);

            string[] filteredFiles = files
                .Select(file => Path.GetFileName(file)).ToArray();
            return filteredFiles;
        }

        public static void ClearAllData(string folderPath)
        {
            var data = FindAllData(folderPath);

            foreach (var save in data)
            {
                File.Delete(Application.persistentDataPath + "/" + folderPath + "/" + save);
            }
        }
    }
}
