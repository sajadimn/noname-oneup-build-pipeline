using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;

public static class BuildRules
{
    public static bool NeedsKeystorePasswords(IEnumerable<BuildConfig> configs)
    {
        foreach (BuildConfig config in configs)
        {
            if (config is AndroidBuildConfig { hasCustomKeystore: true })
                return true;
        }

        return false;
    }

    public static bool ShouldContinue(BuildResult result)
    {
        return result == BuildResult.Succeeded;
    }

    public static int ParseBuildNumber(string text)
    {
        return int.TryParse(text, out int number) ? number : 0;
    }

    public static List<string> FindMissing(IEnumerable<string> paths, Func<string, bool> exists)
    {
        return paths.Where(path => !exists(path)).ToList();
    }

    public static string ToProjectPath(string selected, string projectRoot)
    {
        string root = projectRoot.TrimEnd('/') + "/";
        return selected.StartsWith(root) ? selected.Substring(root.Length) : selected;
    }

    public static List<string> FindEmptySettings(BuildConfig config)
    {
        List<string> empty = new List<string>();
        if (string.IsNullOrEmpty(config.projectName))
            empty.Add(nameof(config.projectName));
        if (string.IsNullOrEmpty(config.productName))
            empty.Add(nameof(config.productName));
        if (string.IsNullOrEmpty(config.packageName))
            empty.Add(nameof(config.packageName));
        return empty;
    }

    public static string GetBundleSignerJar(string configured, string packaged)
    {
        return string.IsNullOrEmpty(configured) || IsPackageCachePath(configured) ? packaged : configured;
    }

    public static bool IsPackageCachePath(string path)
    {
        return !string.IsNullOrEmpty(path) && path.Replace('\\', '/').Contains("Library/PackageCache/");
    }

    public static string ResolvePackagePath(string path, Func<string, string> findPackageFolder)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        const string prefix = "Packages/";
        if (path.StartsWith(prefix))
        {
            string rest = path.Substring(prefix.Length);
            int slash = rest.IndexOf('/');
            string name = slash < 0 ? rest : rest.Substring(0, slash);
            string folder = findPackageFolder(name);
            if (!string.IsNullOrEmpty(folder))
                return System.IO.Path.GetFullPath(slash < 0 ? folder : System.IO.Path.Combine(folder, rest.Substring(slash + 1)));
        }

        return System.IO.Path.GetFullPath(path);
    }

    public static void AddRequired(List<string> paths, string path, string fieldName)
    {
        paths.Add(string.IsNullOrEmpty(path) ? $"{fieldName} is empty" : path);
    }

    public static bool IsGooglePlayGamesMissing(bool hasGooglePlayGames, bool isInstalled)
    {
        return hasGooglePlayGames && !isInstalled;
    }
}
