using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildConfig : ScriptableObject
{
    [Header("Build Configs")] public ProjectVersion projectVersion;
    public BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;
    public BuildTarget buildTarget = BuildTarget.Android;
    public BuildTypes buildType;
    public ApkType exportType = ApkType.apk;
    public string projectName = "";
    public string companyName = "";
    public string productName = "";
    public string packageName = "";
    public bool isDevelopVersion = false;
    public bool addressableUpdateBuildState = false;

    [BuildShowIf(nameof(addressableUpdateBuildState))]
    public AddressableProfiles addressableProfile = AddressableProfiles.Default;

    public string[] extraScriptingDefines = new string[0];
    public string[] scenes = new string[0];

    [Header("Path Configs")] public bool hasMarketFile = false;

    [BuildPath(true, nameof(hasMarketFile))]
    public string marketFilesPath = "";

    public bool hasGameAnalytics = false;

    [BuildPath(true, nameof(hasGameAnalytics))]
    public string gameAnalyticsPath = "";

    public List<FilePathAddress> replaceExtraFile = new List<FilePathAddress>();
    public List<DirectoryPathAddress> replaceExtraDirectory = new List<DirectoryPathAddress>();
}

[Serializable]
public class FilePathAddress
{
    [BuildPath(false)] public string replaceFileFrom;
    [BuildPath(false)] public string replaceFileTo;
}

[Serializable]
public class DirectoryPathAddress
{
    [BuildPath(true)] public string replaceDirectoryFrom;
    [BuildPath(true)] public string replaceDirectoryTo;
}

public enum ApkType
{
    apk = 0,
    aab = 1,
    Xcode = 2,
}

public enum ProjectVersion
{
    Default = 0,
}

public enum AddressableProfiles
{
    Default = 0,
}

public enum BuildTypes
{
    InGame = 0,
    Apk_Bazaar = 1,
    AAB_Bazaar = 2,
    Dev_Bazaar = 3,
    Apk_MyKet = 4,
    Dev_MyKet = 5,
    Apk_GooglePlay = 6,
    AAB_GooglePlay = 7,
    Dev_GooglePlay = 8,
    iOS_AppStore = 9,
    Dev_AppStore = 10,
    Apk_ZarinPal = 11,
    Dev_ZarinPal = 12,
}
