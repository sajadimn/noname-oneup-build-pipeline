using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AndroidBuildPipelineEditor : ProjectBuildPipeline
{
    private static AndroidBuildConfig androidBuildConfig;

    public override BuildPlayerOptions SetBuildOptions(BuildConfigData _buildConfigData)
    {
        buildOptions = new BuildPlayerOptions();
        if (_buildConfigData == null)
            return buildOptions;

        androidBuildConfig = _buildConfigData.buildConfig as AndroidBuildConfig;
        if (androidBuildConfig == null)
            return buildOptions;

        if (BuildRules.IsGooglePlayGamesMissing(androidBuildConfig.hasGooglePlayGames, GooglePlayGamesBuildStep.IsInstalled))
        {
            Debug.LogError("AndroidBuildPipelineEditor - This config needs Google Play Games: use OneUp/Build/Install Google Play Games");
            return buildOptions;
        }

        buildOptions.targetGroup = BuildTargetGroup.Android;
        buildOptions.options = BuildOptions.None;

        base.SetBuildOptions(_buildConfigData);
        return buildOptions;
    }

    protected override void SetPlayerSettings(BuildConfigData _buildConfigData)
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, androidBuildConfig.packageName);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, androidBuildConfig.scriptingImplementation);
        PlayerSettings.Android.useCustomKeystore = androidBuildConfig.hasCustomKeystore;
        if (androidBuildConfig.hasCustomKeystore)
        {
            PlayerSettings.Android.keystoreName = androidBuildConfig.keyStorePath;
            PlayerSettings.Android.keystorePass = _buildConfigData.keystorePass;
            PlayerSettings.Android.keyaliasName = androidBuildConfig.keyaliasName;
            PlayerSettings.Android.keyaliasPass = _buildConfigData.keyaliasPass;
        }
        PlayerSettings.Android.minSdkVersion = androidBuildConfig.minSdkVersion;
        PlayerSettings.Android.targetSdkVersion = androidBuildConfig.targetSdkVersion;
        PlayerSettings.Android.targetArchitectures = androidBuildConfig.targetArchitectures;
        EditorUserBuildSettings.buildAppBundle = androidBuildConfig.exportType == ApkType.aab;
        EditorUserBuildSettings.androidCreateSymbolsZip = androidBuildConfig.createSymbolsZip;
        base.SetPlayerSettings(_buildConfigData);
    }

    protected override void SetBuildConfigs(BuildConfigData _buildConfigData)
    {
        androidBuildConfig = _buildConfigData.buildConfig as AndroidBuildConfig;
        if (androidBuildConfig == null)
            return;

        SetResDirectory(_buildConfigData);
        SetManifest(_buildConfigData);
        SetFirebase(_buildConfigData);
        GooglePlayGamesBuildStep.Run(androidBuildConfig);
        base.SetBuildConfigs(_buildConfigData);
    }

    private void SetResDirectory(BuildConfigData _buildConfigData)
    {
        if (!androidBuildConfig.hasRes)
            return;

        string oldProjectNamePath = Application.dataPath + "/Plugins/Android/res";
        if (File.Exists(oldProjectNamePath))
            FileUtil.DeleteFileOrDirectory(oldProjectNamePath);
        FileUtil.ReplaceDirectory(androidBuildConfig.resPath, oldProjectNamePath);
    }

    private void SetManifest(BuildConfigData _buildConfigData)
    {
        if (!androidBuildConfig.hasManifest)
            return;

        string oldManifestPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        if (File.Exists(oldManifestPath))
            FileUtil.DeleteFileOrDirectory(oldManifestPath);
        FileUtil.CopyFileOrDirectory(androidBuildConfig.manifestPath, oldManifestPath);
    }

    private void SetFirebase(BuildConfigData _buildConfigData)
    {
        if (!androidBuildConfig.hasFirebase)
            return;

        string oldFirebasePath = Application.dataPath + "/google-services.json";
        if (File.Exists(oldFirebasePath))
            FileUtil.DeleteFileOrDirectory(oldFirebasePath);
        FileUtil.CopyFileOrDirectory(androidBuildConfig.firebaseJsonPath, oldFirebasePath);

        string oldFirebaseManifestPath =
            Application.dataPath + "/Plugins/Android/FirebaseApp.androidlib";
        if (Directory.Exists(oldFirebaseManifestPath))
            FileUtil.DeleteFileOrDirectory(oldFirebaseManifestPath);
        FileUtil.ReplaceDirectory(androidBuildConfig.firebaseManifestPath, oldFirebaseManifestPath);
    }

    public override List<string> GetFilesToRestore(BuildConfigData _buildConfigData)
    {
        List<string> files = base.GetFilesToRestore(_buildConfigData);
        if (!(_buildConfigData.buildConfig is AndroidBuildConfig config))
            return files;

        if (config.hasManifest)
            files.Add(Application.dataPath + "/Plugins/Android/AndroidManifest.xml");
        if (config.hasRes)
            files.Add(Application.dataPath + "/Plugins/Android/res");
        if (config.hasFirebase)
        {
            files.Add(Application.dataPath + "/google-services.json");
            files.Add(Application.dataPath + "/Plugins/Android/FirebaseApp.androidlib");
        }

        if (config.hasGooglePlayGames)
            files.Add(Application.dataPath + "/Plugins/Android/GooglePlayGamesManifest.androidlib");
        return files;
    }

    public override List<string> GetRequiredPaths(BuildConfigData _buildConfigData)
    {
        List<string> paths = base.GetRequiredPaths(_buildConfigData);
        if (!(_buildConfigData.buildConfig is AndroidBuildConfig config))
            return paths;

        if (config.hasManifest)
            BuildRules.AddRequired(paths, config.manifestPath, nameof(config.manifestPath));
        if (config.hasRes)
            BuildRules.AddRequired(paths, config.resPath, nameof(config.resPath));
        if (config.hasFirebase)
        {
            BuildRules.AddRequired(paths, config.firebaseJsonPath, nameof(config.firebaseJsonPath));
            BuildRules.AddRequired(paths, config.firebaseManifestPath, nameof(config.firebaseManifestPath));
        }

        if (config.hasCustomKeystore)
            BuildRules.AddRequired(paths, config.keyStorePath, nameof(config.keyStorePath));

        if (config.hasGooglePlayGames && !string.IsNullOrEmpty(config.googlePlayGamesManifest))
            paths.Add(config.googlePlayGamesManifest);
        return paths;
    }

    protected override string GetPathName(BuildConfigData _buildConfigData)
    {
        string locationPath = "";
        string projectName = $"{_buildConfigData.buildConfig.projectName}_{_buildConfigData.buildConfig.buildType}";
        if (_buildConfigData.buildConfig.isDevelopVersion)
        {
            locationPath = "Build/" +
                           projectName +
                           "_V" + _buildConfigData.version +
                           "_B" + _buildConfigData.buildNumber +
                           "_" + _buildConfigData.exportTaker +
                           DateTime.Now.ToString("_D yyyy-MM-dd_T HH-mm") +
                           "." + _buildConfigData.buildConfig.exportType.ToString();
        }
        else
        {
            locationPath = "Build/" +
                           projectName +
                           "_V" + _buildConfigData.version +
                           "_B" + _buildConfigData.buildNumber +
                           "." + _buildConfigData.buildConfig.exportType.ToString();
        }

        return locationPath;
    }

    public override async Task<bool> DoneBuildFile(BuildConfigData _buildConfigData, BuildPlayerOptions _buildOptions)
    {
        if (androidBuildConfig == null)
            return false;

        bool symbolFileState = await CreateSymbolFile(_buildConfigData, _buildOptions);
        if (symbolFileState == false)
            return false;

        bool bundleSignerState = await CreateBundleSignerFile(_buildConfigData, _buildOptions);
        if (bundleSignerState == false)
            return false;

        return true;
    }

    private async Task<bool> CreateSymbolFile(BuildConfigData _buildConfigData, BuildPlayerOptions _buildOptions)
    {
        if (androidBuildConfig == null)
            return false;

        if (!androidBuildConfig.createSymbolsZip)
            return true;

        string symbolFileName = $"-{_buildConfigData.version}-v{_buildConfigData.buildNumber}.symbols.zip";
        string symbolFileLocation =
            _buildOptions.locationPathName.Replace($".{androidBuildConfig.exportType.ToString()}", symbolFileName);
        string symbolLocation = Application.dataPath.Replace("Assets", symbolFileLocation);
        AndroidSymbolShrinker.ShrinkSymbolFile(symbolLocation);
        return true;
    }

    private async Task<bool> CreateBundleSignerFile(BuildConfigData _buildConfigData,
        BuildPlayerOptions _buildOptions)
    {
        if (androidBuildConfig == null)
            return false;

        if (!androidBuildConfig.bundleSignerState)
            return true;

        string signerJar = BuildRules.GetBundleSignerJar(androidBuildConfig.bundleSignerJarFile,
            System.IO.Path.GetFullPath(BuildPipelinePaths.BundleSignerJar));
        try
        {
            string outputPath = _buildOptions.locationPathName;
            string aabPath = Application.dataPath.Replace("Assets", outputPath);
            string binFileName = outputPath.Replace("." + _buildConfigData.buildConfig.exportType.ToString(), ".bin");
            string binFilePath = Application.dataPath.Replace("Assets", binFileName);
            string binPath = Application.dataPath.Replace("Assets", "Build/BundleSigner");
            if (Directory.Exists(binPath))
                Directory.Delete(binPath, true);

            Directory.CreateDirectory(binPath);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = BuildJava.GetPath();
            startInfo.Arguments =
                $" --enable-native-access=ALL-UNNAMED -jar \"{signerJar}\" genbin --bundle \"{aabPath}\" --bin \"{binPath}\" --v2-signing-enabled true --v3-signing-enabled false --ks " +
                $"\"{androidBuildConfig.keyStorePath}\" --ks-key-alias {androidBuildConfig.keyaliasName} --ks-pass pass:{_buildConfigData.keystorePass} --key-pass pass:{_buildConfigData.keyaliasPass} -v";
            process.StartInfo = startInfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Debug.Log("AndroidBuildPipelineEditor - CreateBundleSignerFile - Output: " + output);
            if (process.ExitCode != 0)
            {
                Debug.LogError("AndroidBuildPipelineEditor - CreateBundleSignerFile - Error: " + error);
                return false;
            }

            File.Move(Directory.GetFiles(binPath)[0], binFilePath);
            Directory.Delete(binPath, true);
        }
        catch (Exception ex)
        {
            Debug.LogError("AndroidBuildPipelineEditor - CreateBundleSignerFile - Exception: " + ex.Message);
            return false;
        }

        return true;
    }
}