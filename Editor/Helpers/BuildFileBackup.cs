using System.Collections.Generic;
using System.IO;

public class BuildFileBackup
{
    private const string MetaExtension = ".meta";

    private readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
    private readonly Dictionary<string, string> folders = new Dictionary<string, string>();
    private readonly HashSet<string> missing = new HashSet<string>();

    public void Save(IEnumerable<string> paths)
    {
        Clear();
        foreach (string path in paths)
        {
            SavePath(path);
            if (path.Contains("Assets/"))
                SavePath(path + MetaExtension);
        }
    }

    public void Restore()
    {
        foreach (string path in missing)
            Delete(path);

        foreach (KeyValuePair<string, byte[]> file in files)
            File.WriteAllBytes(file.Key, file.Value);

        foreach (KeyValuePair<string, string> folder in folders)
        {
            Delete(folder.Key);
            Copy(folder.Value, folder.Key);
            Directory.Delete(folder.Value, true);
        }

        Clear();
    }

    private void SavePath(string path)
    {
        if (Directory.Exists(path))
        {
            string copy = UnityEditor.FileUtil.GetUniqueTempPathInProject();
            Copy(path, copy);
            folders[path] = copy;
        }
        else if (File.Exists(path))
        {
            files[path] = File.ReadAllBytes(path);
        }
        else
        {
            missing.Add(path);
        }
    }

    private void Clear()
    {
        files.Clear();
        folders.Clear();
        missing.Clear();
    }

    private static void Delete(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        else if (File.Exists(path))
            File.Delete(path);
    }

    private static void Copy(string from, string to)
    {
        Directory.CreateDirectory(to);
        foreach (string file in Directory.GetFiles(from))
            File.Copy(file, System.IO.Path.Combine(to, System.IO.Path.GetFileName(file)), true);
        foreach (string folder in Directory.GetDirectories(from))
            Copy(folder, System.IO.Path.Combine(to, System.IO.Path.GetFileName(folder)));
    }
}
