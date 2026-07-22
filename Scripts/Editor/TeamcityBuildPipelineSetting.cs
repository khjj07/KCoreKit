using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace KCoreKit
{
    public class TeamcityBuildPipelineSetting : ScriptableObject
    {
        
        [Header("Target Platform")]
        public BuildTarget targetPlatform;
        public BuildTargetGroup targetGroup;

      

        [Header("Output")]
        public string outputDirectory = "Builds";
        public string outputName = "Game";


        [Header("Scenes")]
        [SerializeField] public List<SceneAsset> scenes;
        
        [Header("WebGL Specific Options")]
        public bool enableCompression = true;
        
        [MenuItem("Assets/KCoreKit/Create/TeamcityBuildPipelineSetting")]
        public static void Create()
        {
            TypeExtension.CreateAsset<TeamcityBuildPipelineSetting>("TeamcityBuildPipelineSetting");
        }
    }
}