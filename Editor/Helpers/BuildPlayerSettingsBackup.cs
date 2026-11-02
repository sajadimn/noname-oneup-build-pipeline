using UnityEditor;
using UnityEditor.Build;

public class BuildPlayerSettingsBackup
{
    private string companyName;
    private string productName;
    private string androidIdentifier;
    private string iosIdentifier;
    private ScriptingImplementation androidBackend;
    private AndroidArchitecture androidArchitectures;
    private AndroidSdkVersions minSdkVersion;
    private AndroidSdkVersions targetSdkVersion;
    private bool useCustomKeystore;
    private bool saved;

    public void Save()
    {
        companyName = PlayerSettings.companyName;
        productName = PlayerSettings.productName;
        androidIdentifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
        iosIdentifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS);
        androidBackend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android);
        androidArchitectures = PlayerSettings.Android.targetArchitectures;
        minSdkVersion = PlayerSettings.Android.minSdkVersion;
        targetSdkVersion = PlayerSettings.Android.targetSdkVersion;
        useCustomKeystore = PlayerSettings.Android.useCustomKeystore;
        saved = true;
    }

    public void Restore()
    {
        if (!saved)
            return;

        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
        RestoreIdentifier(NamedBuildTarget.Android, androidIdentifier);
        RestoreIdentifier(NamedBuildTarget.iOS, iosIdentifier);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, androidBackend);
        PlayerSettings.Android.targetArchitectures = androidArchitectures;
        PlayerSettings.Android.minSdkVersion = minSdkVersion;
        PlayerSettings.Android.targetSdkVersion = targetSdkVersion;
        PlayerSettings.Android.useCustomKeystore = useCustomKeystore;
        AssetDatabase.SaveAssets();
        saved = false;
    }

    private static void RestoreIdentifier(NamedBuildTarget target, string identifier)
    {
        if (PlayerSettings.GetApplicationIdentifier(target) != identifier)
            PlayerSettings.SetApplicationIdentifier(target, identifier);
    }
}
