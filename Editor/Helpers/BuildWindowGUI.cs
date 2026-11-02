using UnityEditor;
using UnityEngine;

public static class BuildWindowGUI
{
    public static void BeginBox()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    }

    public static void EndBox()
    {
        EditorGUILayout.EndVertical();
    }

    public static void Title(string title, string subtitle, TextAnchor alignment)
    {
        if (!string.IsNullOrEmpty(title))
            EditorGUILayout.LabelField(title, new GUIStyle(EditorStyles.boldLabel) { alignment = alignment });
        if (!string.IsNullOrEmpty(subtitle))
            EditorGUILayout.LabelField(subtitle, new GUIStyle(EditorStyles.miniLabel) { alignment = alignment });
    }
}
