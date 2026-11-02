using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildTypesConfig : ScriptableObject
{
    public List<BuildTypeConfig> buildTypesConfig = new List<BuildTypeConfig>();
    public List<MultiBuildTypeConfig> multiBuildTypeConfig = new List<MultiBuildTypeConfig>();

    [Header("Options")] public bool cleanBeeCache = true;
    public bool restoreFilesAfterBuild = true;
    public List<string> restorePaths = new List<string>();
    public string googlePlayGamesVersion = "2.3.0";
}

[Serializable]
public class BuildTypeConfig
{
    public BuildTypes buildType;
    public BuildConfig buildConfig;
}

[Serializable]
public class MultiBuildTypeConfig
{
    public BuildTypes buildType;
}
