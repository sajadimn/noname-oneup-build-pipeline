using UnityEditor;
using UnityEngine;

public class iOSBuildConfig : BuildConfig
{
    [Header("iOS")] public float targetMinimumIOSVersion = 11.0f;
    public BuildOptions compressionMethod = BuildOptions.CompressWithLz4;
    public string signingTeamId;
#if UNITY_IOS
    public iOSBuildType runInXcodeType = iOSBuildType.Debug;
#endif

    [Header("iOS Paths")] [BuildPath(true)] public string buildPath = "";
    public bool hasFirebase = false;

    [BuildPath(false, nameof(hasFirebase))]
    public string firebaseJsonPath = "";
}
