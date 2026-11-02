using UnityEngine;

public static class BuildJava
{
    public static string GetPath()
    {
        string java = Application.platform == RuntimePlatform.WindowsEditor ? "java.exe" : "java";
#if UNITY_ANDROID
        string jdk = UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath;
        if (!string.IsNullOrEmpty(jdk))
            return System.IO.Path.Combine(jdk, "bin", java);
#endif
        return java;
    }
}
