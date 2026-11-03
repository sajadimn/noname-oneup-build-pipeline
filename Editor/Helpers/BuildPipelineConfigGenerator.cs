using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class BuildPipelineConfigGenerator
{
    static BuildPipelineConfigGenerator()
    {
        EditorApplication.delayCall += Generate;
    }

    public static void Generate()
    {
        CreateFolder("Assets", "Settings");
        CreateFolder("Assets/Settings", "BuildPipeline");
        BuildTypesConfig typesConfig = Create<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        Directory.CreateDirectory(BuildPipelinePaths.MarketFolder);
        if (NeedsStoreConfigs(typesConfig))
            CreateStoreConfigs(BuildPipelinePaths.Folder, typesConfig);
    }

    public static bool NeedsStoreConfigs(BuildTypesConfig typesConfig)
    {
        return typesConfig.buildTypesConfig.Count == 0;
    }

    public static void CreateStoreConfigs(string folder, BuildTypesConfig typesConfig)
    {
        bool hadMultiBuild = typesConfig.multiBuildTypeConfig.Count > 0;
        foreach (BuildStoreConfig store in BuildStoreConfigs.All)
        {
            string path = $"{folder}/{store.name}.asset";
            BuildConfig config = AssetDatabase.LoadAssetAtPath<BuildConfig>(path);
            if (config == null)
            {
                config = store.IsIos ? CreateIos(store) : CreateAndroid(store);
                AssetDatabase.CreateAsset(config, path);
            }

            BuildTypeConfig row = typesConfig.buildTypesConfig.FirstOrDefault(item => item.buildType == store.buildType);
            if (row == null)
                typesConfig.buildTypesConfig.Add(new BuildTypeConfig { buildType = store.buildType, buildConfig = config });
            else if (row.buildConfig == null)
                row.buildConfig = config;
        }

        if (!hadMultiBuild)
            typesConfig.multiBuildTypeConfig.AddRange(
                BuildStoreConfigs.MultiBuild.Select(buildType => new MultiBuildTypeConfig { buildType = buildType }));

        EditorUtility.SetDirty(typesConfig);
        AssetDatabase.SaveAssets();
    }

    private static AndroidBuildConfig CreateAndroid(BuildStoreConfig store)
    {
        AndroidBuildConfig config = ScriptableObject.CreateInstance<AndroidBuildConfig>();
        Apply(config, store, BuildTargetGroup.Android, BuildTarget.Android);
        config.minSdkVersion = BuildStoreConfigs.MinSdkVersion;
        config.targetSdkVersion = BuildStoreConfigs.TargetSdkVersion;
        config.scriptingImplementation = BuildStoreConfigs.AndroidScripting;
        config.targetArchitectures = BuildStoreConfigs.AndroidArchitectures;
        config.bundleSignerState = store.bundleSignerState;
        config.bundleSignerJarFile = "";
        return config;
    }

    private static iOSBuildConfig CreateIos(BuildStoreConfig store)
    {
        iOSBuildConfig config = ScriptableObject.CreateInstance<iOSBuildConfig>();
        Apply(config, store, BuildTargetGroup.iOS, BuildTarget.iOS);
        config.targetMinimumIOSVersion = BuildStoreConfigs.MinimumIosVersion;
        config.compressionMethod = store.isDevelopVersion
            ? BuildStoreConfigs.IosDevelopCompression
            : BuildStoreConfigs.IosCompression;
        config.signingTeamId = "";
        return config;
    }

    private static void Apply(BuildConfig config, BuildStoreConfig store, BuildTargetGroup group, BuildTarget target)
    {
        config.name = store.name;
        config.buildTargetGroup = group;
        config.buildTarget = target;
        config.buildType = store.buildType;
        config.exportType = store.exportType;
        config.isDevelopVersion = store.isDevelopVersion;
        config.extraScriptingDefines = store.extraScriptingDefines.ToArray();
        config.scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
    }

    public static T Create<T>(string path) where T : ScriptableObject
    {
        T config = AssetDatabase.LoadAssetAtPath<T>(path);
        if (config != null)
            return config;

        config = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        return config;
    }

    private static void CreateFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
            AssetDatabase.CreateFolder(parent, name);
    }
}
