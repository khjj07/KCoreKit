#if  UNITY_EDITOR

using UnityEditor;

namespace KCoreKit
{
    public class TeamcityBuildSetting : SingletonAsset<TeamcityBuildSetting>
    {
        public DefaultAsset buildPipelineSettingFolder;
        
        [MenuItem("Assets/KCoreKit/Create/TeamcityBuildSetting")]
        public static void Create()
        {
            TypeExtension.CreateAsset<TeamcityBuildSetting>("TeamcityBuildSetting");
        }
    }
}
#endif