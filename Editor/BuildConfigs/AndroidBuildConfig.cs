using UnityEditor;
using UnityEngine;

public class AndroidBuildConfig : BuildConfig
{
    [Header("Android")] public AndroidSdkVersions minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
    public AndroidSdkVersions targetSdkVersion = AndroidSdkVersions.AndroidApiLevel28;
    public ScriptingImplementation scriptingImplementation = ScriptingImplementation.IL2CPP;
    public AndroidArchitecture targetArchitectures = AndroidArchitecture.ARMv7;
    public bool createSymbolsZip = false;

    [Header("Android Paths")] public bool hasRes = false;

    [BuildPath(true, nameof(hasRes))] public string resPath = "";

    public bool hasManifest = false;

    [BuildPath(false, nameof(hasManifest))]
    public string manifestPath = "";

    public bool hasFirebase = false;

    [BuildPath(false, nameof(hasFirebase))]
    public string firebaseJsonPath = "";

    [BuildPath(true, nameof(hasFirebase))] public string firebaseManifestPath = "";

    [Header("KeyStore")] public bool hasCustomKeystore = false;

    [BuildPath(false, nameof(hasCustomKeystore))]
    public string keyStorePath = "";

    [BuildShowIf(nameof(hasCustomKeystore))]
    public string keyaliasName = "";

    [Header("Bundle Signer")] public bool bundleSignerState = false;

    [BuildPath(false, nameof(bundleSignerState))]
    public string bundleSignerJarFile;

    [Header("Google Play Games")] public bool hasGooglePlayGames = false;

    [BuildPath(true, nameof(hasGooglePlayGames))]
    public string googlePlayGamesManifest = "";

    [BuildShowIf(nameof(hasGooglePlayGames), 3)]
    public string clientId = "";

    [BuildShowIf(nameof(hasGooglePlayGames))]
    public string constantDirectory = "Assets";

    [BuildShowIf(nameof(hasGooglePlayGames))]
    public string className = "GPGSIds";

    [BuildShowIf(nameof(hasGooglePlayGames), 8)]
    public string configData = "";
}
