using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BeeCacheCleaner : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildTypesConfig config = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
        if (config != null && !config.cleanBeeCache)
            return;

        string projectPath = Directory.GetParent(Application.dataPath)?.FullName;
        if (projectPath != null)
        {
            string beePath = System.IO.Path.Combine(projectPath, "Library", "Bee");
            if (Directory.Exists(beePath))
            {
                Directory.Delete(beePath, true);
                Debug.Log($"{beePath} - Bee Cached Deleted");
            }
        }
    }
}