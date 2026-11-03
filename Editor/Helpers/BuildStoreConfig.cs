public class BuildStoreConfig
{
    public readonly string name;
    public readonly BuildTypes buildType;
    public readonly ApkType exportType;
    public readonly bool isDevelopVersion;
    public readonly string[] extraScriptingDefines;
    public readonly bool bundleSignerState;

    public bool IsIos => exportType == ApkType.Xcode;

    public BuildStoreConfig(string name, BuildTypes buildType, ApkType exportType, bool isDevelopVersion,
        string[] extraScriptingDefines, bool bundleSignerState = false)
    {
        this.name = name;
        this.buildType = buildType;
        this.exportType = exportType;
        this.isDevelopVersion = isDevelopVersion;
        this.extraScriptingDefines = extraScriptingDefines;
        this.bundleSignerState = bundleSignerState;
    }
}
