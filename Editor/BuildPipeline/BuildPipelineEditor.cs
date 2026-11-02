using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPipelineEditor
{
    public static BuildPipelineEditor Instance;
    private int buildIndex;

    public static void Init()
    {
        if (Instance != null)
            return;
        Instance = new BuildPipelineEditor();
    }

    public async Task<bool> Build(BuildConfigData buildConfigData)
    {
        ProjectBuildPipeline.Init(buildConfigData.buildConfig.buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildConfigData.buildConfig.buildTargetGroup,
            buildConfigData.buildConfig.buildTarget);
        BuildNumberEditor.SetBuildNumber(buildConfigData.buildConfig.buildTarget, buildConfigData.buildNumber);

        bool addressableState = await SetAddressableAssets(buildConfigData);
        if (addressableState == false)
            return false;

        List<string> emptySettings = BuildRules.FindEmptySettings(buildConfigData.buildConfig);
        if (emptySettings.Count > 0)
        {
            Debug.LogError("BuildPipelineEditor - Fill these in the build config:\n" + string.Join("\n", emptySettings));
            return false;
        }

        List<string> missing = BuildRules.FindMissing(ProjectBuildPipeline.Instance.GetRequiredPaths(buildConfigData),
            path => File.Exists(path) || Directory.Exists(path));
        if (missing.Count > 0)
        {
            Debug.LogError("BuildPipelineEditor - Files this config needs are missing:\n" + string.Join("\n", missing));
            return false;
        }

        BuildTypesConfig typesConfig = AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        BuildFileBackup backup = new BuildFileBackup();
        BuildPlayerSettingsBackup settingsBackup = new BuildPlayerSettingsBackup();
        if (typesConfig == null || typesConfig.restoreFilesAfterBuild)
        {
            List<string> paths = ProjectBuildPipeline.Instance.GetFilesToRestore(buildConfigData);
            if (typesConfig != null)
                paths.AddRange(typesConfig.restorePaths);
            backup.Save(paths);
            settingsBackup.Save();
        }

        try
        {
            BuildPlayerOptions buildOptions = ProjectBuildPipeline.Instance.SetBuildOptions(buildConfigData);
            if (String.IsNullOrEmpty(buildOptions.locationPathName))
                return false;

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            LogBuildInfo(buildConfigData.buildConfig.projectName, report.summary,
                buildConfigData.buildConfig.buildType.ToString());
            if (!BuildRules.ShouldContinue(report.summary.result))
                return false;

            bool platformState = await ProjectBuildPipeline.Instance.DoneBuildFile(buildConfigData, buildOptions);
            if (platformState == false)
                return false;

            EditorUtility.RevealInFinder(buildOptions.locationPathName);
            return true;
        }
        finally
        {
            backup.Restore();
            settingsBackup.Restore();
            AssetDatabase.Refresh();
        }
    }

    public void Build(List<BuildConfigData> buildsConfigData)
    {
        buildIndex = 0;
        SetMultiBuild(buildsConfigData);
    }

    private async void SetMultiBuild(List<BuildConfigData> buildsConfigData)
    {
        if (buildIndex >= buildsConfigData.Count)
            return;

        bool state = await Build(buildsConfigData[buildIndex]);
        if (state == false)
            return;

        buildIndex++;
        SetMultiBuild(buildsConfigData);
    }

    private void LogBuildInfo(string projectName, BuildSummary _summary, string _buildType)
    {
        if (_summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"{projectName}{_buildType} Build Succeeded: {_summary.totalSize} bytes");
        }

        if (_summary.result == BuildResult.Failed)
        {
            Debug.Log($"{projectName}{_buildType} Build Failed");
        }
    }

    private async Task<bool> SetAddressableAssets(BuildConfigData buildConfigData)
    {
        if (!buildConfigData.buildConfig.addressableUpdateBuildState)
            return true;

#if ONEUP_ADDRESSABLES
        bool state = await AddressableBuildWindow.SetAddressableProfile(
            buildConfigData.buildConfig.addressableProfile, true, buildConfigData.personalAddressableName);
        return state;
#else
        Debug.LogError("BuildPipelineEditor - This config needs Addressables, but the package is not installed");
        await Task.CompletedTask;
        return false;
#endif
    }
}