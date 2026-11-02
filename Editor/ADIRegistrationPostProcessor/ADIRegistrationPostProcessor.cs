#if UNITY_ANDROID && UNITY_EDITOR
using System.IO;
using UnityEditor.Android;
using UnityEngine;

public class ADIRegistrationPostProcessor : IPostGenerateGradleAndroidProject
{
    private const string FileName = "adi-registration.properties";
    public int callbackOrder => 100;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string sourcePath = System.IO.Path.Combine(Application.dataPath, FileName);

        if (!File.Exists(sourcePath))
        {
            Debug.LogError($"[ADI] File not found:\n{sourcePath}");
            return;
        }

        string launcherPath = System.IO.Path.Combine(path, "launcher");

        if (!Directory.Exists(launcherPath))
        {
            launcherPath = path;
        }

        string assetsPath = System.IO.Path.Combine(launcherPath, "src", "main", "assets");
        Directory.CreateDirectory(assetsPath);
        string destinationPath = System.IO.Path.Combine(assetsPath, FileName);
        File.Copy(sourcePath, destinationPath, true);
        Debug.Log($"[ADI] File copied successfully:\n{destinationPath}");
    }
}

#endif