using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class BuildPipelineConfigGenerator
{
    static BuildPipelineConfigGenerator()
    {
        EditorApplication.delayCall += Generate;
    }

    public static void Generate()
    {
        CreateFolder("Assets", "Settings");
        CreateFolder("Assets/Settings", "BuildPipeline");
        Create<BuildTypesConfig>(BuildPipelinePaths.BuildTypesConfig);
    }

    public static T Create<T>(string path) where T : ScriptableObject
    {
        T config = AssetDatabase.LoadAssetAtPath<T>(path);
        if (config != null)
            return config;

        config = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        return config;
    }

    private static void CreateFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
            AssetDatabase.CreateFolder(parent, name);
    }
}
