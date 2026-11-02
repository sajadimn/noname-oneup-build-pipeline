using UnityEditor;
using UnityEngine;

public class iOSBuildPipelineEditor : ProjectBuildPipeline
{
    private static iOSBuildConfig iOSBuildConfig;

    public override BuildPlayerOptions SetBuildOptions(BuildConfigData _buildConfigData)
    {
        buildOptions = new BuildPlayerOptions();
        if (_buildConfigData == null)
            return buildOptions;

        iOSBuildConfig = _buildConfigData.buildConfig as iOSBuildConfig;
        if (iOSBuildConfig == null)
            return buildOptions;

        buildOptions.targetGroup = BuildTargetGroup.iOS;
        buildOptions.options = iOSBuildConfig.compressionMethod;

        base.SetBuildOptions(_buildConfigData);
        return buildOptions;
    }

    protected override void SetPlayerSettings(BuildConfigData _buildConfigData)
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, iOSBuildConfig.packageName);
        PlayerSettings.iOS.targetOSVersionString = iOSBuildConfig.targetMinimumIOSVersion.ToString();
        PlayerSettings.iOS.appleDeveloperTeamID = iOSBuildConfig.signingTeamId;
#if UNITY_IOS
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildConfig.runInXcodeType;
#endif
        base.SetPlayerSettings(_buildConfigData);
    }

    protected override void SetBuildConfigs(BuildConfigData _buildConfigData)
    {
        iOSBuildConfig = _buildConfigData.buildConfig as iOSBuildConfig;
        if (iOSBuildConfig == null)
            return;
        base.SetBuildConfigs(_buildConfigData);
    }

    protected override string GetPathName(BuildConfigData _buildConfigData)
    {
        string projectName = $"{_buildConfigData.buildConfig.projectName}_{_buildConfigData.buildConfig.buildType}";
        string locationPath = iOSBuildConfig.buildPath + "/" + projectName + "_V" + _buildConfigData.version +
                              "_B" + _buildConfigData.buildNumber;
        return locationPath;
    }

    public static string GetPathName()
    {
        string locationPath = iOSBuildConfig.buildPath + "/" + iOSBuildConfig.projectName + "_V" + Application.version +
                              "_B" + BuildNumberEditor.GetBuildNumber(iOSBuildConfig.buildTarget);
        return locationPath;
    }
}