using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BuildWindowEditor : EditorWindow
{
    private string sdkVersion = "Unity SDK: v2.1.1";
    private static BuildTypesConfig buildTypesConfig;
    private static List<ProjectKeystoreConfig> buildKeystores = new List<ProjectKeystoreConfig>();
    private static List<MultiBuildConfigState> multiBuildConfigsState = new List<MultiBuildConfigState>();
    private static BuildTypes buildType = BuildTypes.Dev_Bazaar;
    private static bool multiBuildState = false;
    private static bool developmentBuildState = false;
    private static int buildNumber;
    private static string version;
    public static string exportTaker;
    private static bool personalAddressableState = false;
    private static BuildTarget lastBuildTarget = BuildTarget.NoTarget;
    private int separator = 10;
    private int space = 2;
    private static Single windowWidthSize = 430;
    private static Single windowHeightSize = 635;
    private static Single buttonWidthSize = 220;
    private static Single buttonHeightSize = 40;
    private static Single exportTakerTextFieldSize = 410;
    private static Single distanceBetweenMultiBuildStateAndConfig = 20;

    [MenuItem("OneUp/Build/Create Build Types Config", false, 2)]
    public static void CreateBuildTypesConfig()
    {
        BuildPipelineConfigGenerator.Generate();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
    }

    [MenuItem("OneUp/Build/Create Store Build Configs", false, 3)]
    public static void CreateStoreBuildConfigs()
    {
        BuildPipelineConfigGenerator.Generate();
        BuildTypesConfig typesConfig = AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        BuildPipelineConfigGenerator.CreateStoreConfigs(BuildPipelinePaths.Folder, typesConfig);
        Selection.activeObject = typesConfig;
    }

    [MenuItem("OneUp/Build/Create Android Build Config", false, 4)]
    public static void CreateAndroidBuildConfig()
    {
        BuildPipelineConfigGenerator.Generate();
        Selection.activeObject = BuildPipelineConfigGenerator.Create<AndroidBuildConfig>(BuildPipelinePaths.AndroidBuildConfig);
    }

    [MenuItem("OneUp/Build/Create iOS Build Config", false, 5)]
    public static void CreateiOSBuildConfig()
    {
        BuildPipelineConfigGenerator.Generate();
        Selection.activeObject = BuildPipelineConfigGenerator.Create<iOSBuildConfig>(BuildPipelinePaths.IosBuildConfig);
    }

    [MenuItem("OneUp/Build/Build Setting", false, 1)]
    public static void BuildSetting()
    {
        BuildPipelineEditor.Init();
        var buildWindow = GetWindow(typeof(BuildWindowEditor));
        buildWindow.titleContent = new GUIContent("Build Pipeline Window");
        buildWindow.minSize = new Vector2(windowWidthSize, windowHeightSize);
        buildWindow.maxSize = new Vector2(windowWidthSize, windowHeightSize);
        personalAddressableState = false;
        lastBuildTarget = BuildTarget.NoTarget;
        IncreaseBuildNumber(BuildTarget.Android);
        version = PlayerSettings.bundleVersion;
        buildTypesConfig = null;
        BuildPipelineConfigGenerator.Generate();
        buildTypesConfig = AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        if (buildTypesConfig == null)
            return;

        buildKeystores.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(ProjectVersion)).Length; i++)
        {
            buildKeystores.Add(new ProjectKeystoreConfig());
        }

        multiBuildConfigsState.Clear();
        for (int i = 0; i < buildTypesConfig.multiBuildTypeConfig.Count; i++)
        {
            multiBuildConfigsState.Add(new MultiBuildConfigState());
        }
    }

    private void OnGUI()
    {
        GUI.backgroundColor = Color.cyan;
        EditorGUILayout.Space(separator);
        BuildWindowGUI.BeginBox();
        BuildWindowGUI.Title("Build Project Versions", "Android & iOS", TextAnchor.MiddleCenter);
        BuildWindowGUI.EndBox();
        GUI.backgroundColor = Color.white;

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        version = EditorGUILayout.TextField("Build Version: ", version, GetSizeOption());
        buildNumber = EditorGUILayout.IntField("Build Number: ", buildNumber, GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        if (buildTypesConfig == null)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("First Click On CreateBuildTypesConfig In Build Tab To Create It", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        if (buildKeystores.Count == 0)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("ProjectVersion Enum In BuildTypesConfig Script Cant Be Empty", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        if (string.IsNullOrEmpty(version) || buildNumber.ToString() == "" || buildNumber <= 0)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Version And BuildNumber Cant Be Empty Or Negative", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        multiBuildState = EditorGUILayout.Toggle("Multi Build State: ", multiBuildState, GetSizeOption());
        developmentBuildState =
            EditorGUILayout.Toggle("Development Profiling: ", developmentBuildState, GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        exportTaker =
            EditorGUILayout.TextField("Export Taker Name: ", exportTaker, GetSizeOption());
        personalAddressableState =
            EditorGUILayout.Toggle("Personal Addressable: ", personalAddressableState, GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        if (string.IsNullOrEmpty(exportTaker))
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Export Taker Name Can't Be Empty", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        if (!multiBuildState)
        {
            SetCurrentBuild();
        }
        else
        {
            SetMultiBuild();
        }
    }

    private void SetCurrentBuild()
    {
        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        buildType = (BuildTypes)EditorGUILayout.EnumPopup("Current Build Type: ", buildType);
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildTypeConfig buildTypeConfig = buildTypesConfig.buildTypesConfig.Find(x => x.buildType == buildType);
        if (buildTypeConfig == null)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Cant Find Current Build Type In BuildTypesConfig ScriptableObject",
                "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        Object buildConfig = buildTypeConfig.buildConfig;
        if (buildConfig == null)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Click On CreateBuildConfig In Build Tab To Create New BuildConfig", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Change Name Of ScriptableObject", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Add Current BuildConfig In BuildTypesConfig ScriptableObject", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        if (((BuildConfig)buildConfig).buildTarget == BuildTarget.Android &&
            ((AndroidBuildConfig)buildConfig)?.hasCustomKeystore == true)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.BeginBox();
            BuildWindowGUI.Title(
                Enum.GetValues(typeof(ProjectVersion)).GetValue((int)((BuildConfig)buildConfig).projectVersion)
                    .ToString() + " Password", "", TextAnchor.MiddleCenter);
            EditorGUILayout.BeginHorizontal();
            string keystoreTitle =
                EditorGUILayout.TextArea("Keystore Pass: ", EditorStyles.largeLabel, GetSizeOption());
            string keyaliasTitle =
                EditorGUILayout.TextArea("Keyalias Pass: ", EditorStyles.largeLabel, GetSizeOption());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            buildKeystores[0].keystorePass =
                EditorGUILayout.PasswordField(buildKeystores[0].keystorePass, GetSizeOption());
            buildKeystores[0].keyaliasPass =
                EditorGUILayout.PasswordField(buildKeystores[0].keyaliasPass, GetSizeOption());
            EditorGUILayout.EndHorizontal();
            BuildWindowGUI.EndBox();
        }

        EditorGUILayout.Separator();
        BuildWindowGUI.Title("It Will Build The Following Config", "", TextAnchor.MiddleCenter);

        EditorGUILayout.Separator();
        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        buildConfig = EditorGUILayout.ObjectField((Object)buildConfig, typeof(Object), true);
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        if (buildConfig == null)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Add Current BuildConfig In BuildTypesConfig ScriptableObject", "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        IncreaseBuildNumber(((BuildConfig)buildConfig).buildTarget);
        CurrentBuildDescription((BuildConfig)buildConfig);

        if (((BuildConfig)buildConfig).buildTarget == BuildTarget.Android &&
            ((AndroidBuildConfig)buildConfig)?.hasCustomKeystore == true)
        {
            if (string.IsNullOrEmpty(buildKeystores[0].keystorePass) ||
                string.IsNullOrEmpty(buildKeystores[0].keyaliasPass))
            {
                EditorGUILayout.Separator();
                BuildWindowGUI.Title("Enter Keystore Passwords To Build", "Note: Warning", TextAnchor.MiddleCenter);
                EditorGUILayout.Separator();
                SDKVersion();
                return;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Build", GUILayout.Height(buttonHeightSize), GUILayout.Width(buttonWidthSize)))
        {
            BuildConfigData buildConfigData = new BuildConfigData();
            buildConfigData.buildConfig = (BuildConfig)buildConfig;
            if (((BuildConfig)buildConfig).buildTarget == BuildTarget.Android &&
                ((AndroidBuildConfig)buildConfig)?.hasCustomKeystore == true)
            {
                buildConfigData.keystorePass = buildKeystores[0].keystorePass;
                buildConfigData.keyaliasPass = buildKeystores[0].keyaliasPass;
            }

            buildConfigData.developmentBuildState = developmentBuildState;
            buildConfigData.personalAddressableName = personalAddressableState ? exportTaker : "";
            buildConfigData.buildNumber = buildNumber;
            buildConfigData.version = version;
            buildConfigData.exportTaker = exportTaker;
            BuildPipelineEditor.Instance.Build(buildConfigData);
            Close();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        SDKVersion();
    }

    private void SetMultiBuild()
    {
        IncreaseBuildNumber(BuildTarget.Android);
        if (buildTypesConfig.multiBuildTypeConfig.Count == 0)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("MultiBuildType In BuildTypesConfig ScriptableObject Cant Be Empty",
                "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        EditorGUILayout.Separator();
        for (int i = 0; i < Enum.GetValues(typeof(ProjectVersion)).Length; i++)
        {
            BuildWindowGUI.BeginBox();
            BuildWindowGUI.Title(Enum.GetValues(typeof(ProjectVersion)).GetValue(i).ToString() + " Password", "", TextAnchor.MiddleCenter);
            EditorGUILayout.BeginHorizontal();
            string keystoreTitle =
                EditorGUILayout.TextArea("Keystore Pass: ", EditorStyles.largeLabel, GetSizeOption());
            string keyaliasTitle =
                EditorGUILayout.TextArea("Keyalias Pass: ", EditorStyles.largeLabel, GetSizeOption());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            buildKeystores[i].projectVersion = (ProjectVersion)Enum.GetValues(typeof(ProjectVersion)).GetValue(i);
            buildKeystores[i].keystorePass =
                EditorGUILayout.PasswordField(buildKeystores[i].keystorePass, GetSizeOption());
            buildKeystores[i].keyaliasPass =
                EditorGUILayout.PasswordField(buildKeystores[i].keyaliasPass, GetSizeOption());
            EditorGUILayout.EndHorizontal();
            BuildWindowGUI.EndBox();
        }

        EditorGUILayout.Separator();
        BuildWindowGUI.Title("It Will Build All The Enable Following Configs",
            "", TextAnchor.MiddleCenter);
        EditorGUILayout.Separator();

        int mutiBuildCount = 0;
        for (int i = 0; i < buildTypesConfig.multiBuildTypeConfig.Count; i++)
        {
            BuildTypeConfig buildTypeConfig = buildTypesConfig.buildTypesConfig
                .Find(x => x.buildType == buildTypesConfig.multiBuildTypeConfig[i].buildType);
            if (buildTypeConfig == null)
                continue;

            Object configObject = buildTypeConfig.buildConfig;
            if (configObject == null)
                continue;

            mutiBuildCount++;
            BuildWindowGUI.BeginBox();
            EditorGUILayout.BeginHorizontal();
            multiBuildConfigsState[i].buildConfig = (BuildConfig)configObject;
            multiBuildConfigsState[i].state =
                EditorGUILayout.Toggle(multiBuildConfigsState[i].state,
                    GUILayout.Width(distanceBetweenMultiBuildStateAndConfig));
            configObject = EditorGUILayout.ObjectField((Object)configObject, typeof(Object), true);
            EditorGUILayout.EndHorizontal();
            BuildWindowGUI.EndBox();
        }

        if (mutiBuildCount == 0)
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Add Types Of MultiBuildType In BuildTypesConfig ScriptableObject",
                "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        if (!multiBuildConfigsState.Exists(x => x.state == true))
        {
            EditorGUILayout.Separator();
            BuildWindowGUI.Title("Enable At Least One Type Of Above Configs",
                "Note: Error", TextAnchor.MiddleCenter);
            EditorGUILayout.Separator();
            SDKVersion();
            return;
        }

        bool needsPasswords = BuildRules.NeedsKeystorePasswords(
            multiBuildConfigsState.FindAll(x => x.state && x.buildConfig != null).ConvertAll(x => x.buildConfig));
        foreach (var buildKeystore in buildKeystores)
        {
            if (needsPasswords && (string.IsNullOrEmpty(buildKeystore.keystorePass) ||
                                   string.IsNullOrEmpty(buildKeystore.keyaliasPass)))
            {
                EditorGUILayout.Space(space);
                BuildWindowGUI.Title("Enter Keystore Passwords To Build", "Note: Warning", TextAnchor.MiddleCenter);
                EditorGUILayout.Separator();
                SDKVersion();
                return;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Build", GUILayout.Height(buttonHeightSize), GUILayout.Width(buttonWidthSize)))
        {
            List<BuildConfigData> buildsConfigData = new List<BuildConfigData>();
            for (int i = 0; i < multiBuildConfigsState.Count; i++)
            {
                if (multiBuildConfigsState[i].state == false || multiBuildConfigsState[i].buildConfig == null)
                    continue;
                BuildConfigData buildConfigData = new BuildConfigData();
                buildConfigData.buildConfig = multiBuildConfigsState[i].buildConfig;
                buildConfigData.keystorePass = buildKeystores
                    .Find(x => x.projectVersion == multiBuildConfigsState[i].buildConfig.projectVersion)
                    .keystorePass;
                buildConfigData.keyaliasPass = buildKeystores
                    .Find(x => x.projectVersion == multiBuildConfigsState[i].buildConfig.projectVersion)
                    .keyaliasPass;
                buildConfigData.developmentBuildState = developmentBuildState;
                buildConfigData.personalAddressableName = personalAddressableState ? exportTaker : "";
                buildConfigData.buildNumber = buildNumber;
                buildConfigData.version = version;
                buildConfigData.exportTaker = exportTaker;
                buildsConfigData.Add(buildConfigData);
            }

            BuildPipelineEditor.Instance.Build(buildsConfigData);
            Close();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        SDKVersion();
    }

    private void CurrentBuildDescription(BuildConfig buildConfig)
    {
        EditorGUILayout.Space(space);
        EditorGUILayout.Separator();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string projectNameTitle =
            EditorGUILayout.TextArea("Project Name: ", EditorStyles.largeLabel, GetSizeOption());
        string projectNameValue =
            EditorGUILayout.TextArea(buildConfig.projectName, EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string buildTypeTitle =
            EditorGUILayout.TextArea("Build Type: ", EditorStyles.largeLabel, GetSizeOption());
        string buildTypeValue =
            EditorGUILayout.TextArea(buildConfig.buildType.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string buildTargetTitle =
            EditorGUILayout.TextArea("Build Target: ", EditorStyles.largeLabel, GetSizeOption());
        string buildTargetValue =
            EditorGUILayout.TextArea(buildConfig.buildTarget.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string exportTypeTitle =
            EditorGUILayout.TextArea("Export Type: ", EditorStyles.largeLabel, GetSizeOption());
        string exportTypeValue =
            EditorGUILayout.TextArea(buildConfig.exportType.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string packageNameTitle =
            EditorGUILayout.TextArea("Package Name: ", EditorStyles.largeLabel, GetSizeOption());
        string packageNameValue =
            EditorGUILayout.TextArea(buildConfig.packageName, EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string gameAnalyticsTitle =
            EditorGUILayout.TextArea("Company Name: ", EditorStyles.largeLabel, GetSizeOption());
        string gameAnalyticsValue =
            EditorGUILayout.TextArea(buildConfig.companyName, EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string isDevelopTitle =
            EditorGUILayout.TextArea("Is Develop Version: ", EditorStyles.largeLabel, GetSizeOption());
        string isDevelopValue =
            EditorGUILayout.TextArea(buildConfig.isDevelopVersion.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string marketTitle =
            EditorGUILayout.TextArea("Market State: ", EditorStyles.largeLabel, GetSizeOption());
        string marketValue =
            EditorGUILayout.TextArea(buildConfig.hasMarketFile.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();

        BuildWindowGUI.BeginBox();
        EditorGUILayout.BeginHorizontal();
        string addressableTitle =
            EditorGUILayout.TextArea("Addressable State: ", EditorStyles.largeLabel, GetSizeOption());
        string addressableValue =
            EditorGUILayout.TextArea(buildConfig.addressableUpdateBuildState.ToString(), EditorStyles.largeLabel,
                GetSizeOption());
        EditorGUILayout.EndHorizontal();
        BuildWindowGUI.EndBox();
    }

    private static void IncreaseBuildNumber(BuildTarget currentBuildTarget)
    {
        if (lastBuildTarget == currentBuildTarget)
            return;

        lastBuildTarget = currentBuildTarget;
        buildNumber = BuildNumberEditor.GetBuildNumber(currentBuildTarget) + 1;
    }

    private GUILayoutOption GetSizeOption()
    {
        return GUILayout.Width(210.0f);
    }

    private void SDKVersion()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        BuildWindowGUI.Title("", sdkVersion, TextAnchor.MiddleRight);
        EditorGUILayout.EndHorizontal();
    }
}

public class ProjectKeystoreConfig
{
    public ProjectVersion projectVersion;
    public string keystorePass;
    public string keyaliasPass;
}

public class MultiBuildConfigState
{
    public BuildConfig buildConfig;
    public bool state = true;
}

public class BuildConfigData
{
    public BuildConfig buildConfig;
    public string keystorePass;
    public string keyaliasPass;
    public bool developmentBuildState;
    public string personalAddressableName;
    public int buildNumber;
    public string version;
    public string exportTaker;
}