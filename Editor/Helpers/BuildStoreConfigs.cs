using UnityEditor;

public static class BuildStoreConfigs
{
    public const AndroidSdkVersions MinSdkVersion = AndroidSdkVersions.AndroidApiLevel28;
    public const AndroidSdkVersions TargetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
    public const ScriptingImplementation AndroidScripting = ScriptingImplementation.IL2CPP;
    public const AndroidArchitecture AndroidArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
    public const float MinimumIosVersion = 12f;
    public const BuildOptions IosCompression = BuildOptions.CompressWithLz4HC;
    public const BuildOptions IosDevelopCompression = BuildOptions.CompressWithLz4;

    private const string Develop = "DEVELOP";
    private const string InGame = "IN_GAME";
    private const string CafeBazaar = "CAFE_BAZAAR";
    private const string MyKet = "MY_KET";
    private const string GooglePlay = "GOOGLE_PLAY";
    private const string AppStore = "APP_STORE";
    private const string ZarinPal = "ZARIN_PAL";

    public static readonly BuildStoreConfig[] All =
    {
        new BuildStoreConfig("InGame", BuildTypes.InGame, ApkType.apk, false, new[] { InGame }),
        new BuildStoreConfig("CafeBazaarApk", BuildTypes.Apk_Bazaar, ApkType.apk, false, new[] { CafeBazaar }),
        new BuildStoreConfig("CafeBazaarAAB", BuildTypes.AAB_Bazaar, ApkType.aab, false, new[] { CafeBazaar }, true),
        new BuildStoreConfig("CafeBazaarDevelop", BuildTypes.Dev_Bazaar, ApkType.apk, true, new[] { CafeBazaar, Develop }),
        new BuildStoreConfig("MyKetApk", BuildTypes.Apk_MyKet, ApkType.apk, false, new[] { MyKet }),
        new BuildStoreConfig("MyKetDevelop", BuildTypes.Dev_MyKet, ApkType.apk, true, new[] { MyKet, Develop }),
        new BuildStoreConfig("GooglePlayApk", BuildTypes.Apk_GooglePlay, ApkType.apk, false, new[] { GooglePlay }),
        new BuildStoreConfig("GooglePlayAAB", BuildTypes.AAB_GooglePlay, ApkType.aab, false, new[] { GooglePlay }),
        new BuildStoreConfig("GooglePlayDevelop", BuildTypes.Dev_GooglePlay, ApkType.apk, true, new[] { GooglePlay, Develop }),
        new BuildStoreConfig("AppStore", BuildTypes.iOS_AppStore, ApkType.Xcode, false, new[] { AppStore }),
        new BuildStoreConfig("AppStoreDevelop", BuildTypes.Dev_AppStore, ApkType.Xcode, true, new[] { AppStore, Develop }),
        new BuildStoreConfig("ZarinPalApk", BuildTypes.Apk_ZarinPal, ApkType.apk, false, new[] { ZarinPal }),
        new BuildStoreConfig("ZarinPalDevelop", BuildTypes.Dev_ZarinPal, ApkType.apk, true, new[] { ZarinPal, Develop }),
    };

    public static readonly BuildTypes[] MultiBuild =
    {
        BuildTypes.Apk_Bazaar, BuildTypes.AAB_Bazaar, BuildTypes.Dev_MyKet, BuildTypes.Apk_MyKet,
        BuildTypes.Apk_GooglePlay, BuildTypes.AAB_GooglePlay
    };
}
