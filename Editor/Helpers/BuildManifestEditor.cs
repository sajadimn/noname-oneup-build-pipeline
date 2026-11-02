using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class BuildManifestEditor
{
    private const string GooglePlayGamesPackage = "com.google.play.games";
    private const string GoogleScope = "com.google";
    private const string OpenUpmUrl = "https://package.openupm.com";

    public static string AddGooglePlayGames(string manifestJson, string version)
    {
        JObject manifest = JObject.Parse(manifestJson);
        if (manifest["dependencies"] is not JObject dependencies)
        {
            dependencies = new JObject();
            manifest["dependencies"] = dependencies;
        }

        dependencies[GooglePlayGamesPackage] = version;
        AddGoogleScope(manifest);
        return manifest.ToString(Formatting.Indented);
    }

    private static void AddGoogleScope(JObject manifest)
    {
        if (manifest["scopedRegistries"] is not JArray registries)
        {
            registries = new JArray();
            manifest["scopedRegistries"] = registries;
        }

        if (registries.OfType<JObject>().Any(x => x["scopes"].Any(y => (string)y == GoogleScope)))
            return;

        JObject registry = registries.OfType<JObject>().FirstOrDefault(x => (string)x["url"] == OpenUpmUrl);
        if (registry == null)
        {
            registry = new JObject {["name"] = "OpenUPM", ["url"] = OpenUpmUrl, ["scopes"] = new JArray()};
            registries.Add(registry);
        }

        ((JArray)registry["scopes"]).Add(GoogleScope);
    }
}
