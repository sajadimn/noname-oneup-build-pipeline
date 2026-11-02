using UnityEngine;

public class BuildPathAttribute : PropertyAttribute
{
    public bool IsFolder { get; }
    public string BoolFieldName { get; }

    public BuildPathAttribute(bool isFolder, string boolFieldName = null)
    {
        IsFolder = isFolder;
        BoolFieldName = boolFieldName;
    }
}
