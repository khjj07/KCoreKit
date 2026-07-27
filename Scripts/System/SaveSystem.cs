using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KCoreKit
{
    public static class SaveSystem
    {
        public static void Save<T>(T data, string fileName, string directory, bool prettyPrint = false) where T : ISerializeData
        {
            var savePath = Application.persistentDataPath + "/" + directory + "/" + fileName;
            var jsonString = JsonUtility.ToJson(data, prettyPrint);
            using FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
            using BinaryWriter binaryWriter = new(fileStream, Encoding.UTF8);
            binaryWriter.Write(jsonString);
            binaryWriter.Close();
            fileStream.Close();
#if UNITY_EDITOR
            Debug.Log(fileName + " is saved!");
            Debug.Log(savePath);
#endif
        }

        public static void Load<T>(string fileName, string directory, out T data) where T : ISerializeData
        {
            var savePath = Application.persistentDataPath + "/" + directory + "/" + fileName;
            if (File.Exists(savePath))
            {
                using FileStream fileStream = File.Open(savePath, FileMode.Open);
                using BinaryReader binaryReader = new(fileStream);
                var jsonString = binaryReader.ReadString();
                data = JsonUtility.FromJson<T>(jsonString);
                binaryReader.Close();
                fileStream.Close();
#if UNITY_EDITOR
                Debug.Log(fileName + " is loaded!");
                Debug.Log(savePath);
#endif
            }
            else
            {
                throw new FileNotFoundException($"No save files found : {savePath}");
            }
        }

        public static void LoadAll<T>(string directory, out List<T> dataList) where T : ISerializeData
        {
            var folderPath = Path.Combine(Application.persistentDataPath, directory);
            dataList = new List<T>();

            // 1. 디렉토리가 존재하는지 확인
            if (Directory.Exists(folderPath))
            {
                // 2. 디렉토리 내의 모든 파일 경로 가져오기
                string[] filePaths = Directory.GetFiles(folderPath);

                if (filePaths.Length == 0)
                {
                    // 파일이 하나도 없을 경우 예외 발생 (기존 Load의 정책 유지)
                    throw new FileNotFoundException($"No save files found in directory: {directory}");
                }

                foreach (var path in filePaths)
                {
                    try
                    {
                        using FileStream fileStream = File.Open(path, FileMode.Open);
                        using BinaryReader binaryReader = new(fileStream, Encoding.UTF8);
                
                        var jsonString = binaryReader.ReadString();
                        T data = JsonUtility.FromJson<T>(jsonString);
                
                        if (data != null)
                        {
                            dataList.Add(data);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // 특정 파일이 손상되었을 경우를 대비해 로그를 남기고 계속 진행하거나, 중단할 수 있음
                        Debug.LogError($"Failed to load file at {path}: {ex.Message}");
                    }
                }

#if UNITY_EDITOR
                Debug.Log($"{dataList.Count} files loaded from {directory}");
#endif
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory doesn't exist: {directory}");
            }
        }
        
        public static void Remove(string fileName, string directory)
        {
            var savePath = Application.persistentDataPath + "/" + directory + "/" + fileName;
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
#if UNITY_EDITOR
                Debug.Log(fileName + " is removed!");
                Debug.Log(savePath);
#endif
            }
        }
        
        public static bool Exist(string fileName, string directory)
        {
            return File.Exists(Application.persistentDataPath + "/" + directory + "/" + fileName);
        }
    }
}