using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildNumberEditor : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }
    private static BuildTarget buildTarget;

    public void OnPreprocessBuild(BuildReport report)
    {
    }

    public static void SetBuildVersion(string _version)
    {
        PlayerSettings.bundleVersion = _version;
    }

    public static void SetBuildNumber(BuildTarget _buildTarget, int _buildNumber)
    {
        buildTarget = _buildTarget;
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.macOS.buildNumber = _buildNumber.ToString();
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = _buildNumber.ToString();
                break;
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode = _buildNumber;
                break;
            case BuildTarget.PS4:
                PlayerSettings.PS4.appVersion = _buildNumber.ToString();
                break;
            case BuildTarget.XboxOne:
                PlayerSettings.XboxOne.Version = _buildNumber.ToString();
                break;
            case BuildTarget.WSAPlayer:
                PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major,
                    PlayerSettings.WSA.packageVersion.Minor, _buildNumber);
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.tvOS:
                PlayerSettings.tvOS.buildNumber = _buildNumber.ToString();
                break;
        }
    }

    private static void SetBuildNumber(BuildTarget _buildTarget)
    {
        buildTarget = _buildTarget;
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
                break;
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode++;
                break;
            case BuildTarget.PS4:
                PlayerSettings.PS4.appVersion = IncrementBuildNumber(PlayerSettings.PS4.appVersion);
                break;
            case BuildTarget.XboxOne:
                PlayerSettings.XboxOne.Version = IncrementBuildNumber(PlayerSettings.XboxOne.Version);
                break;
            case BuildTarget.WSAPlayer:
                PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major,
                    PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.tvOS:
                PlayerSettings.tvOS.buildNumber = IncrementBuildNumber(PlayerSettings.tvOS.buildNumber);
                break;
        }
    }

    public static int GetBuildNumber(BuildTarget _buildTarget)
    {
        string buildNumber = "";
        switch (_buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
                buildNumber = PlayerSettings.macOS.buildNumber;
                break;
            case BuildTarget.iOS:
                buildNumber = PlayerSettings.iOS.buildNumber;
                break;
            case BuildTarget.Android:
                buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
            case BuildTarget.PS4:
                buildNumber = PlayerSettings.PS4.appVersion;
                break;
            case BuildTarget.XboxOne:
                buildNumber = PlayerSettings.XboxOne.Version;
                break;
            case BuildTarget.WSAPlayer:
                buildNumber = PlayerSettings.WSA.packageVersion.Build.ToString();
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.tvOS:
                buildNumber = PlayerSettings.tvOS.buildNumber;
                break;
        }

        return BuildRules.ParseBuildNumber(buildNumber);
    }

    public static int GetIncrementBuildNumber(BuildTarget _buildTarget)
    {
        return IncrementBuildNumber(GetBuildNumber(_buildTarget));
    }

    private static int IncrementBuildNumber(int buildNumber)
    {
        return (buildNumber + 1);
    }

    private static string IncrementBuildNumber(string buildNumber)
    {
        int.TryParse(buildNumber, out int outputBuildNumber);

        return (outputBuildNumber + 1).ToString();
    }
}