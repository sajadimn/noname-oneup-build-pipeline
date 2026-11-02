using UnityEditor;

public static class BuildFieldVisibility
{
    public static bool IsVisible(SerializedProperty property, string boolFieldName)
    {
        if (string.IsNullOrEmpty(boolFieldName))
            return true;

        SerializedProperty boolField = property.serializedObject.FindProperty(boolFieldName);
        return boolField == null || boolField.boolValue;
    }
}
