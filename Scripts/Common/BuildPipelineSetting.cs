#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using KCoreKit;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace KCoreKit
{
    
public class BuildPipelineSetting : SingletonAsset<BuildPipelineSetting> 
{
    [SerializeField]
    private string exeName = "";  
    
    [SerializeField]
    private List<SceneAsset> scenes;
    
    private static string _exeName => GetInstance().exeName;

    [MenuItem("Assets/KCoreKit/Create/BuildPipelineSetting")]
    public static void Create()
    {
        TypeExtension.CreateAsset<BuildPipelineSetting>("BuildPipelineSetting");
    }

    public static void Build()
    {
        try
        {
            Log("====================================");
            Log($"🚀 {_exeName} CI 통합 빌드 시작");
            Log("====================================");

            var target = ParseBuildTargetArgument();
            if (target == null) 
            {
                return;
            }

            Log($"✅ Build Target: {target}");

            if (!BuildAssetBundlesImpl(target.Value))
            {
                Log("❌ AssetBundle 빌드 실패! 전체 빌드 중단.");
                EditorApplication.Exit(1);
                return;
            }
            
            BuildPlayerImpl(target.Value);
            Log("🎉 통합 빌드 최종 완료.");
        }
        catch (Exception ex)
        {
            
            HandleException(ex, "GENERAL_BUILD_FAIL");
        }
    }
    
    public static void BuildAssetBundles()
    {
        try
        {
            Log("====================================");
            Log("📦 AssetBundle 빌드 전용 시작");
            Log("====================================");

            var target = ParseBuildTargetArgument();
            if (target == null) return;

            if (!BuildAssetBundlesImpl(target.Value))
            {
                EditorApplication.Exit(1);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "ASSET_BUNDLES_BUILD_FAIL");
        }
    }
    
    public static void BuildPlayer()
    {
        try
        {
            Log("====================================");
            Log("🎮 Player 빌드 전용 시작");
            Log("====================================");

            var target = ParseBuildTargetArgument();
            if (target == null) return;

            BuildPlayerImpl(target.Value);
        }
        catch (Exception ex)
        {
            HandleException(ex, "PLAYER_BUILD_FAIL");
        }
    }
    
    // ----------------- 내부 빌드 로직 -----------------

    private static bool BuildAssetBundlesImpl(BuildTarget target)
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
            // BuildPipeline.BuildAssetBundles의 주요 예외 포착
            BuildPipeline.BuildAssetBundles(
                assetBundleOutputFolder, 
                BuildAssetBundleOptions.StrictMode, 
                target 
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

    private static void BuildPlayerImpl(BuildTarget target)
    {
        Log("\n🔧 Unity BuildPipeline.BuildPlayer 실행 중...");

        string buildFolder = Path.Combine("Builds", target.ToString());
        string outputPath = Path.Combine(buildFolder, _exeName);

        if (Directory.Exists(buildFolder))
        {
            Log($"🧹 기존 빌드 폴더 삭제 중: {buildFolder}");
            Directory.Delete(buildFolder, true);
        }
        Directory.CreateDirectory(buildFolder);

        Log($"📁 출력 경로: {Path.GetFullPath(outputPath)}");
        var scenePaths = GetInstance().scenes.ConvertAll(AssetDatabase.GetAssetPath).ToArray();
   
        
        var buildOptions = new BuildPlayerOptions
        {
            scenes = scenePaths,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        
        PrintBuildSummary(report);
        
        if (report.summary.result != BuildResult.Succeeded)
        {
            // Player 빌드가 실패하면 TeamCity에 빌드 문제 보고
            ReportTeamCityProblem(
                $"Player 빌드 실패! 결과: {report.summary.result}. Unity 로그를 확인하세요.", 
                "PLAYER_BUILD_FAIL"
            );
            // TeamCity 빌드 실패를 위해 Unity Editor를 0이 아닌 코드로 종료합니다.
            EditorApplication.Exit(1);
        }

        HandleExeNaming(buildFolder, target, outputPath);
    }
    
    private static void Log(string message)
    {
        Console.WriteLine(message);
        Debug.Log(message);
    }

    private static BuildTarget? ParseBuildTargetArgument()
    {
        var args = Environment.GetCommandLineArgs();
        // TeamCity 로그에 인수가 표시되도록 Log 사용
        for (int i = 0; i < args.Length; i++)
            Log($"Arg[{i}]: {args[i]}");

        var targetArgIndex = Array.IndexOf(args, "-buildTarget");

        if (targetArgIndex == -1 || targetArgIndex + 1 >= args.Length)
        {
            //ReportTeamCityProblem("빌드 타겟 인자(-buildTarget)가 없거나 값이 누락되었습니다.", ARGUMENT_PROBLEM_ID);
            Log("❌ ERROR: -buildTarget 인자 문제 발생! 빌드 실패.");
            EditorApplication.Exit(1); // 즉시 종료하여 확실히 실패 처리
            return null;
        }

        string targetValue = args[targetArgIndex + 1];
        if (!Enum.TryParse(targetValue, out BuildTarget target))
        {
            //ReportTeamCityProblem($"잘못된 BuildTarget 값: '{targetValue}'.", ARGUMENT_PROBLEM_ID);
            Log($"❌ ERROR: '{targetValue}' 는 잘못된 BuildTarget입니다. 빌드 실패.");
            EditorApplication.Exit(1);
            return null;
        }

        return target;
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
    
    // 기존의 HandleExeNaming 함수는 그대로 유지
    private static void HandleExeNaming(string buildFolder, BuildTarget target, string finalPath)
    {
        // Standalone 빌드 시 Unity가 임의의 이름을 붙이는 경우를 처리
        string defaultExeName = target.ToString();
        if (target == BuildTarget.StandaloneWindows64)
            defaultExeName = "StandaloneWindows64"; // 일반적인 Windows64 기본 이름

        var generatedExe = Path.Combine(buildFolder, defaultExeName + ".exe");
        
        if (File.Exists(generatedExe) && !File.Exists(finalPath))
        {
            Log($"🔄 Unity 기본 exe 이름 감지됨 ({Path.GetFileName(generatedExe)}). {_exeName}로 변경합니다.");
            File.Move(generatedExe, finalPath);
        }
    }

    // ----------------- TeamCity 보고 함수 -----------------

    /// <summary>
    /// TeamCity 서비스 메시지를 사용하여 빌드 문제를 보고하고 빌드 실패로 표시합니다.
    /// </summary>
    /// <param name="description">빌드 문제 설명</param>
    /// <param name="identity">고유한 문제 ID (TeamCity에서 문제 추적에 사용)</param>
    private static void ReportTeamCityProblem(string description, string identity)
    {
        // TeamCity 빌드 문제 서비스 메시지 출력
        // 이 메시지는 TeamCity Build Problems 탭에 표시되며, 빌드를 실패 상태로 만듭니다.
        // | 와 ' 문자열 이스케이프 필요
        string escapedDescription = description.Replace("|", "||").Replace("'", "|'");
        string tcMessage = $"##teamcity[buildProblem description='{escapedDescription}' identity='{identity}']";
        
        // Debug.LogError를 사용하여 Unity 콘솔 및 TeamCity 로그에 빨간색으로 오류 표시
        Debug.LogError(tcMessage);
        Console.Error.WriteLine(tcMessage); // 혹시 모를 경우를 대비해 Console.Error에도 출력
    }

    /// <summary>
    /// 예외 처리 및 TeamCity 문제 보고를 통합합니다.
    /// </summary>
    private static void HandleException(Exception ex, string identity)
    {
        string errorMessage = ex.ToString();
        ReportTeamCityProblem($"예상치 못한 스크립트 예외 발생: {ex.Message}", identity);
        
        Log("====================================");
        Log("❌ 예외 발생! 빌드 중단");
        Log(errorMessage);
        Log("====================================");

        // 예외가 발생하면 TeamCity 빌드 실패를 위해 0이 아닌 코드로 종료합니다.
        EditorApplication.Exit(1);
    }
}
}
#endif