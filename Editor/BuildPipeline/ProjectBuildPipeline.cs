using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class ProjectBuildPipeline
{
    public static ProjectBuildPipeline Instance;
    protected BuildPlayerOptions buildOptions;

    public static void Init(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                ProjectBuildPipeline.Instance = new AndroidBuildPipelineEditor();
                break;
            case BuildTarget.iOS:
                ProjectBuildPipeline.Instance = new iOSBuildPipelineEditor();
                break;
        }
    }

    public virtual BuildPlayerOptions SetBuildOptions(BuildConfigData _buildConfigData)
    {
        buildOptions.scenes = _buildConfigData.buildConfig.scenes;
        buildOptions.target = _buildConfigData.buildConfig.buildTarget;
        buildOptions.extraScriptingDefines = _buildConfigData.buildConfig.extraScriptingDefines;
        buildOptions.locationPathName = ProjectBuildPipeline.Instance.GetPathName(_buildConfigData);
        ProjectBuildPipeline.Instance.SetPlayerSettings(_buildConfigData);
        ProjectBuildPipeline.Instance.SetBuildConfigs(_buildConfigData);
        return buildOptions;
    }

    protected virtual string GetPathName(BuildConfigData _buildConfigData)
    {
        return "";
    }

    protected virtual void SetPlayerSettings(BuildConfigData _buildConfigData)
    {
        BuildNumberEditor.SetBuildVersion(_buildConfigData.version);
        PlayerSettings.companyName = _buildConfigData.buildConfig.companyName;
        PlayerSettings.productName = _buildConfigData.buildConfig.productName;
        EditorUserBuildSettings.development = _buildConfigData.developmentBuildState;
        EditorUserBuildSettings.buildWithDeepProfilingSupport = _buildConfigData.developmentBuildState;
        EditorUserBuildSettings.allowDebugging = _buildConfigData.developmentBuildState;
    }

    protected virtual void SetBuildConfigs(BuildConfigData _buildConfigData)
    {
        SetMarketData(_buildConfigData);
        SetGameAnalyticsData(_buildConfigData);
        SetExtraFile(_buildConfigData);
        SetExtraDirectory(_buildConfigData);
        AssetDatabase.Refresh();
    }

    private void SetMarketData(BuildConfigData _buildConfigData)
    {
        string oldMarketPath = Application.dataPath + "/Plugins/Android/Market";
        if (Directory.Exists(oldMarketPath))
            FileUtil.DeleteFileOrDirectory(oldMarketPath);

        if (!_buildConfigData.buildConfig.hasMarketFile)
            return;

        string marketFilesPath = System.IO.Path.GetFullPath(_buildConfigData.buildConfig.marketFilesPath);
        FileUtil.ReplaceDirectory(marketFilesPath, oldMarketPath);
    }

    private void SetGameAnalyticsData(BuildConfigData _buildConfigData)
    {
        if (!_buildConfigData.buildConfig.hasGameAnalytics)
            return;

        string oldGameAnalyticsPath = Application.dataPath + "/Resources/GameAnalytics";
        if (Directory.Exists(oldGameAnalyticsPath))
            FileUtil.DeleteFileOrDirectory(oldGameAnalyticsPath);
        FileUtil.ReplaceDirectory(_buildConfigData.buildConfig.gameAnalyticsPath, oldGameAnalyticsPath);
    }

    private void SetExtraFile(BuildConfigData _buildConfigData)
    {
        if (_buildConfigData.buildConfig.replaceExtraFile.Count == 0)
            return;

        foreach (var file in _buildConfigData.buildConfig.replaceExtraFile)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file.replaceFileTo));
            if (File.Exists(file.replaceFileTo))
                FileUtil.DeleteFileOrDirectory(file.replaceFileTo);
            FileUtil.CopyFileOrDirectory(file.replaceFileFrom, file.replaceFileTo);
        }
    }

    private void SetExtraDirectory(BuildConfigData _buildConfigData)
    {
        if (_buildConfigData.buildConfig.replaceExtraDirectory.Count == 0)
            return;

        foreach (var file in _buildConfigData.buildConfig.replaceExtraDirectory)
        {
            if (File.Exists(file.replaceDirectoryTo))
                FileUtil.DeleteFileOrDirectory(file.replaceDirectoryTo);
            FileUtil.ReplaceDirectory(file.replaceDirectoryFrom, file.replaceDirectoryTo);
        }
    }

    public virtual List<string> GetFilesToRestore(BuildConfigData _buildConfigData)
    {
        BuildConfig config = _buildConfigData.buildConfig;
        List<string> paths = new List<string> { Application.dataPath + "/Plugins/Android/Market" };
        foreach (FilePathAddress file in config.replaceExtraFile)
            paths.Add(file.replaceFileTo);
        foreach (DirectoryPathAddress folder in config.replaceExtraDirectory)
            paths.Add(folder.replaceDirectoryTo);
        if (config.hasGameAnalytics)
            paths.Add(Application.dataPath + "/Resources/GameAnalytics");
        return paths;
    }

    private static string GetFullPath(string path)
    {
        return string.IsNullOrEmpty(path) ? path : System.IO.Path.GetFullPath(path);
    }

    public virtual List<string> GetRequiredPaths(BuildConfigData _buildConfigData)
    {
        BuildConfig config = _buildConfigData.buildConfig;
        List<string> paths = new List<string>();
        foreach (FilePathAddress file in config.replaceExtraFile)
            paths.Add(file.replaceFileFrom);
        foreach (DirectoryPathAddress folder in config.replaceExtraDirectory)
            paths.Add(folder.replaceDirectoryFrom);
        if (config.hasMarketFile)
            BuildRules.AddRequired(paths, GetFullPath(config.marketFilesPath), nameof(config.marketFilesPath));
        if (config.hasGameAnalytics)
            BuildRules.AddRequired(paths, config.gameAnalyticsPath, nameof(config.gameAnalyticsPath));
        return paths;
    }

    public virtual async Task<bool> DoneBuildFile(BuildConfigData _buildConfigData, BuildPlayerOptions _buildOptions)
    {
        return true;
    }
}