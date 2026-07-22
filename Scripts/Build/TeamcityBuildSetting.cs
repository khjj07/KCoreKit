#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KCoreKit
{
    public class TeamcityBuildSetting : SingletonAsset<TeamcityBuildSetting>
    {
#if UNITY_EDITOR
        public DefaultAsset buildPipelineSettingFolder;
        [MenuItem("Assets/KCoreKit/Create/TeamcityBuildSetting")]
        public static void Create()
        {
            TypeExtension.CreateAsset<TeamcityBuildSetting>("TeamcityBuildSetting");
        }
#endif
    }
}