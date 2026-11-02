using UnityEngine;

public class BuildShowIfAttribute : PropertyAttribute
{
    public string BoolFieldName { get; }
    public int Lines { get; }

    public BuildShowIfAttribute(string boolFieldName, int lines = 1)
    {
        BoolFieldName = boolFieldName;
        Lines = lines;
    }
}
