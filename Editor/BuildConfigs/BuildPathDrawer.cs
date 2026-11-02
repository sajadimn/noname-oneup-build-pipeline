using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BuildPathAttribute))]
public class BuildPathDrawer : PropertyDrawer
{
    private const float ButtonWidth = 70f;
    private const float Spacing = 2f;

    private BuildPathAttribute Path => (BuildPathAttribute)attribute;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return BuildFieldVisibility.IsVisible(property, Path.BoolFieldName)
            ? EditorGUIUtility.singleLineHeight
            : -EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!BuildFieldVisibility.IsVisible(property, Path.BoolFieldName))
            return;

        Rect fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, position.height);
        Rect buttonRect = new Rect(fieldRect.xMax + Spacing, position.y, ButtonWidth, position.height);
        property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);
        if (GUI.Button(buttonRect, "Browse"))
            Browse(property);
    }

    private void Browse(SerializedProperty property)
    {
        string selected = Path.IsFolder
            ? EditorUtility.OpenFolderPanel(property.displayName, property.stringValue, "")
            : EditorUtility.OpenFilePanel(property.displayName, property.stringValue, "");
        if (!string.IsNullOrEmpty(selected))
            property.stringValue = BuildRules.ToProjectPath(selected, System.IO.Directory.GetCurrentDirectory());
    }
}
