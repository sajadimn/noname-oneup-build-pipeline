using System.IO;
using UnityEditor;
using UnityEngine;
#if ONEUP_GOOGLE_PLAY_GAMES
using GooglePlayGames.Editor;
#endif

public static class GooglePlayGamesBuildStep
{
    private const string PluginManifestPath = "/Plugins/Android/GooglePlayGamesManifest.androidlib";
    private const string ManifestPath = "Packages/manifest.json";

#if ONEUP_GOOGLE_PLAY_GAMES
    public const bool IsInstalled = true;
#else
    public const bool IsInstalled = false;
#endif

    public static void Run(AndroidBuildConfig config)
    {
#if ONEUP_GOOGLE_PLAY_GAMES
        if (!config.hasGooglePlayGames)
            return;

        if (GPGSAndroidSetupUI.PerformSetup(config.clientId, config.constantDirectory, config.className,
                config.configData, null))
        {
            GPGSAndroidSetupUI.CheckBundleId();
            GPGSProjectSettings.Instance.Set(GPGSUtil.ANDROIDSETUPDONEKEY, true);
        }
        else
        {
            Debug.LogError("GooglePlayGamesBuildStep - Setup failed, check clientId and configData in the build config");
        }

        if (string.IsNullOrEmpty(config.googlePlayGamesManifest))
            return;

        string target = Application.dataPath + PluginManifestPath;
        if (Directory.Exists(target))
            FileUtil.DeleteFileOrDirectory(target);
        FileUtil.ReplaceDirectory(config.googlePlayGamesManifest, target);
#endif
    }

    [MenuItem("OneUp/Build/Install Google Play Games", false, 20)]
    private static void Install()
    {
        BuildTypesConfig config = AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        string version = config != null ? config.googlePlayGamesVersion : new BuildVersionDefaults().GooglePlayGames;
        File.WriteAllText(ManifestPath, BuildManifestEditor.AddGooglePlayGames(File.ReadAllText(ManifestPath), version));
        UnityEditor.PackageManager.Client.Resolve();
    }

    private class BuildVersionDefaults
    {
        public readonly string GooglePlayGames = "2.3.0";
    }
}
