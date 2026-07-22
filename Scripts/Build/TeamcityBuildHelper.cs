
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;


namespace KCoreKit
{
    public static class TeamcityBuildHelper
    {
#if UNITY_EDITOR
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
               
                var assetPath = TeamcityBuildSetting.GetInstance().buildPipelineSettingFolder.GetAbsolutePath();
                
               var settings = AssetDatabase.LoadAssetAtPath<TeamcityBuildPipelineSetting>($"{assetPath}/{idStr}.asset");

                if (settings == null)
                {
                    Debug.LogError($"[-] Build settings asset not found at path: {assetPath}");
                    EditorApplication.Exit(1);
                    return;
                }
                
                AssetDatabase.SaveAssets();
                
                Log("====================================");
                Log($"🚀 {settings.name} CI 통합 빌드 시작");
                Log("====================================");
                

                Log($"✅ Build Target: {settings.targetPlatform}");

                if (!BuildAssetBundlesImpl(settings))
                {
                    Log("❌ AssetBundle 빌드 실패! 전체 빌드 중단.");
                    EditorApplication.Exit(1);
                    return;
                }

                BuildPlayerImpl(settings);
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

        private static void BuildPlayerImpl(TeamcityBuildPipelineSetting setting)
        {
            Log("\n🔧 Unity BuildPipeline.BuildPlayer 실행 중...");

            string buildFolder = Path.Combine(setting.outputDirectory, setting.targetPlatform.ToString());
            string locationPathName;

            if (setting.targetPlatform == BuildTarget.WebGL)
            {
                locationPathName = buildFolder;
            }
            else
            {
                string executableName = setting.outputName;
                switch (setting.targetPlatform)
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
            var scenePaths = setting.scenes.ConvertAll(AssetDatabase.GetAssetPath).ToArray();


            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = locationPathName,
                target = setting.targetPlatform,
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
            Debug.Log(message);
        }
#endif
    }
}
