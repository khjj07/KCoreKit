#if  UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace KCoreKit
{
    public class TeamcityBuildHelper
    {
        private static string GetArgument(string[] args,string name)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
        public static void Build()
        {
          
            try
            {
                var args = Environment.GetCommandLineArgs();
                string idStr = GetArgument(args, "-id");
               
                var assetPath = TeamcityBuildSetting.GetInstance().buildPipelineSettingFolder.GetLocalPath();
                
               var settings = AssetDatabase.LoadAssetAtPath<TeamcityBuildPipelineSetting>($"{assetPath}/{idStr}.asset");

                if (settings == null)
                {
                    Debug.LogError($"[-] Build settings asset not found at path: {assetPath}");
                    EditorApplication.Exit(1);
                    return;
                }
                
                Log("====================================");
                Log($"🚀 {settings.name} CI 통합 빌드 시작");
                Log("====================================");
                
                Log("\n🔧 Unity BuildPipeline.BuildPlayer 실행 중...");

                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(settings.targetPlatform);
                if (EditorUserBuildSettings.activeBuildTarget != settings.targetPlatform)
                {
                    Log($"🔄 빌드 타겟을 {settings.targetPlatform}(으)로 전환 중...");
                    bool switchSuccess = EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, settings.targetPlatform);
                    if (!switchSuccess)
                    {
                        ReportTeamCityProblem($"빌드 타겟을 {settings.targetPlatform}으로 전환하는 데 실패했습니다.", "TARGET_SWITCH_FAIL");
                        EditorApplication.Exit(1);
                        return;
                    }
                }
                
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
                
                
                Log($"✅ Build Target: {settings.targetPlatform}");

                var platform = settings.targetPlatform;
                var name = settings.name;
                var outputName = settings.outputName;
                var scenePaths = settings.scenes.ConvertAll(AssetDatabase.GetAssetPath).ToArray();

                if (!BuildAssetBundlesImpl(settings))
                {
                    Log("❌ AssetBundle 빌드 실패! 전체 빌드 중단.");
                    EditorApplication.Exit(1);
                    return;
                }

                BuildPlayerImpl(name,platform,outputName,scenePaths);
                Log("🎉 통합 빌드 최종 완료.");
            }
            catch (Exception ex)
            {
                HandleException(ex, "GENERAL_BUILD_FAIL");
            }
        }
        
        
        private static bool BuildAssetBundlesImpl(TeamcityBuildPipelineSetting setting)
        {
            Log("\n🔧 AssetBundle 빌드 실행 중...");

            string assetBundleOutputFolder = Path.Combine(Application.dataPath, "StreamingAssets", "AssetBundles");

            if (Directory.Exists(assetBundleOutputFolder))
            {
                Log($"🧹 기존 AssetBundle 폴더 삭제 중: {assetBundleOutputFolder}");
                Directory.Delete(assetBundleOutputFolder, true);
            }

            Directory.CreateDirectory(assetBundleOutputFolder);

            Log($"📦 AssetBundle 출력 경로: {assetBundleOutputFolder}");

            try
            {
                BuildPipeline.BuildAssetBundles(
                    assetBundleOutputFolder,
                    BuildAssetBundleOptions.None,
                    setting.targetPlatform
                );

                Log("✅ AssetBundle 빌드 완료.");
                return true;
            }
            catch (Exception ex)
            {
                ReportTeamCityProblem(
                    $"AssetBundle 빌드 중 예외 발생: {ex.Message}",
                    "ASSET_BUNDLES_BUILD_FAIL"
                );
                Log($"❌ AssetBundle 빌드 중 예외 발생: {ex.Message}");
                return false;
            }
        }

        private static void BuildPlayerImpl(string settingName, BuildTarget targetPlatform, string outputName, string[] scenePaths)
        {
            
            
            string buildFolder = Path.Combine("Builds", settingName);
            string locationPathName;

            if (targetPlatform == BuildTarget.WebGL)
            {
                locationPathName = buildFolder;
            }
            else
            {
                string executableName = outputName;
                switch (targetPlatform)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        executableName += ".exe";
                        break;
                    case BuildTarget.Android:
                        executableName += EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                        break;
                    case BuildTarget.StandaloneOSX:
                        executableName += ".app";
                        break;
                }
                locationPathName = Path.Combine(buildFolder, executableName);
            }

            if (Directory.Exists(buildFolder))
            {
                Log($"🧹 기존 빌드 폴더 삭제 중: {buildFolder}");
                Directory.Delete(buildFolder, true);
            }

            Directory.CreateDirectory(buildFolder);

            Log($"📁 출력 경로: {Path.GetFullPath(locationPathName)}");

            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = locationPathName,
                target = targetPlatform,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            PrintBuildSummary(report);

            if (report.summary.result != BuildResult.Succeeded)
            {
                ReportTeamCityProblem(
                    $"Player 빌드 실패! 결과: {report.summary.result}. Unity 로그를 확인하세요.",
                    "PLAYER_BUILD_FAIL"
                );
                EditorApplication.Exit(1);
            }
        }
        
        private static void HandleException(Exception ex, string identity)
        {
            string errorMessage = ex.ToString();
            ReportTeamCityProblem($"예상치 못한 스크립트 예외 발생: {ex.Message}", identity);
            Log("====================================");
            Log("❌ 예외 발생! 빌드 중단");
            Log(errorMessage);
            Log("====================================");
            EditorApplication.Exit(1);
        }
        
        private static void ReportTeamCityProblem(string description, string identity)
        {
            string escapedDescription = description.Replace("|", "||").Replace("'", "|'");
            string tcMessage = $"##teamcity[buildProblem description='{escapedDescription}' identity='{identity}']";
            Debug.LogError(tcMessage);
            Console.Error.WriteLine(tcMessage);
        }
        
        
        private static void PrintBuildSummary(BuildReport report)
        {
            Log("====================================");
            Log($"📦 Build Result: {report.summary.result}");
            Log($"🕓 Duration: {report.summary.totalTime}");
            Log($"⚙️ Output Path: {report.summary.outputPath}");
            Log($"⚠️ Warnings: {report.summary.totalWarnings} | ❌ Errors: {report.summary.totalErrors}");

            if (report.summary.result == BuildResult.Succeeded)
                Log("✅ 빌드 성공!");
            else
                Log("❌ 빌드 실패! Unity 로그를 확인하세요.");
            Log("====================================");
        }
        
        private static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}

#endif
