using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;

public class AndroidSymbolShrinker
{
    private static string lastSymbolToShrinkLocation = nameof(lastSymbolToShrinkLocation);
    private static string symbolLocation = Application.dataPath;

    class ProcessResult
    {
        internal int exitCode { get; }
        internal string stdOut { get; }
        internal string stdErr { get; }

        internal bool failure => exitCode != 0;

        internal ProcessResult(int _exitCode, string _stdOut, string _stdErr)
        {
            exitCode = _exitCode;
            stdOut = _stdOut;
            stdErr = _stdErr;
        }

        public override string ToString()
        {
            return $"AndroidSymbolShrinker \nExitCode: {exitCode}\nStdOut: {stdOut}\nStdErr: {stdErr}";
        }
    }

    [MenuItem("OneUp/Build/Shrink Symbol File", false, 10)]
    public static void ShrinkSymbolFile()
    {
        symbolLocation = Application.dataPath.Replace("Assets", "Build");
        symbolLocation = EditorUtility.OpenFilePanel(
            "Open Android Symbol Package to shrink", symbolLocation, "*.zip");
        if (string.IsNullOrEmpty(symbolLocation))
            return;

        ShrinkSymbolFile(symbolLocation);
    }

    public static void ShrinkSymbolFile(string location)
    {
        if (string.IsNullOrEmpty(location))
            return;

        var targetDirectory = System.IO.Path.GetDirectoryName(location);
        var intermediatePath = System.IO.Path.Combine(targetDirectory, "TempShrink");
        var newZip = System.IO.Path.Combine(targetDirectory, System.IO.Path.GetFileNameWithoutExtension(location) + ".small.zip");
        EditorPrefs.SetString(lastSymbolToShrinkLocation, targetDirectory);

        var zipFileName = System.IO.Path.GetFullPath(System.IO.Path.Combine(EditorApplication.applicationContentsPath, "Tools", "7z"));
        if (Application.platform == RuntimePlatform.WindowsEditor)
            zipFileName += ".exe";

        if (!File.Exists(zipFileName))
            throw new Exception($"Failed to locate {zipFileName}");

        Cleanup(intermediatePath);
        Cleanup(newZip);
        var result = RunProcess(targetDirectory, zipFileName, $"x -o\"{intermediatePath}\" \"{location}\"");
        if (result.failure)
            throw new Exception(result.ToString());

        EditorUtility.DisplayProgressBar("Shrinking symbols", "Deleting/Renaming/Compressing symbol files", 0.5f);
        var files = Directory.GetFiles(intermediatePath, "*.*", SearchOption.AllDirectories);
        var symSo = ".sym.so";
        foreach (var file in files)
        {
            if (file.EndsWith(".dbg.so"))
                Cleanup(file);
            if (file.EndsWith(symSo))
            {
                var fileSO = file.Substring(0, file.Length - symSo.Length) + ".so";
                Log($"AndroidSymbolShrinker - ShrinkSymbolFile - Rename {file} --> {fileSO}");
                File.Move(file, fileSO);
            }
        }

        result = RunProcess(intermediatePath, zipFileName, $"a -tzip \"{newZip}\"");
        EditorUtility.ClearProgressBar();
        if (result.failure)
            throw new Exception(result.ToString());

        Cleanup(intermediatePath);
        Cleanup(location);
        File.Move(newZip, location);
        Log($"AndroidSymbolShrinker - ShrinkSymbolFile - New small symbol package: {newZip}");
    }

    private static void Log(string message)
    {
        UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, message);
    }

    private static ProcessResult RunProcess(string workingDirectory, string fileName, string args)
    {
        Log($"AndroidSymbolShrinker - Executing {fileName} {args} (Working Directory: {workingDirectory}");
        Process process = new Process();
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.WorkingDirectory = workingDirectory;
        process.StartInfo.CreateNoWindow = true;
        var output = new StringBuilder();
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.AppendLine(e.Data);
            }
        });

        var error = new StringBuilder();
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.AppendLine(e.Data);
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        Log($"AndroidSymbolShrinker - {fileName} exited with {process.ExitCode}");
        return new ProcessResult(process.ExitCode, output.ToString(), error.ToString());
    }

    private static void Cleanup(string path)
    {
        if (Directory.Exists(path))
        {
            Log($"AndroidSymbolShrinker - Cleanup - Delete Directory From: {path}");
            Directory.Delete(path, true);
        }

        if (File.Exists(path))
        {
            Log($"AndroidSymbolShrinker - Cleanup - Delete File From: {path}");
            File.Delete(path);
        }
    }
}