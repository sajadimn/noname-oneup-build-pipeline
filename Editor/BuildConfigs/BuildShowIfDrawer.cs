using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BuildShowIfAttribute))]
public class BuildShowIfDrawer : PropertyDrawer
{
    private BuildShowIfAttribute ShowIf => (BuildShowIfAttribute)attribute;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!BuildFieldVisibility.IsVisible(property, ShowIf.BoolFieldName))
            return -EditorGUIUtility.standardVerticalSpacing;

        return ShowIf.Lines > 1
            ? EditorGUIUtility.singleLineHeight * (ShowIf.Lines + 1)
            : EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!BuildFieldVisibility.IsVisible(property, ShowIf.BoolFieldName))
            return;

        if (ShowIf.Lines <= 1)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect textRect = new Rect(position.x, labelRect.yMax, position.width, position.height - labelRect.height);
        EditorGUI.LabelField(labelRect, label);
        property.stringValue = EditorGUI.TextArea(textRect, property.stringValue);
    }
}
