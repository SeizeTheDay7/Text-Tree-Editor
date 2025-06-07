using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;


internal static class JSONManager
{
    public static void CreateNewField(ObjectField textTreeField)
    {
        string path = EditorUtility.SaveFilePanel(
            "Select path for new text tree",
            Application.dataPath,
            "TextTree_Name",
            "json"
        );

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Creation canceled");
            return;
        }

        File.WriteAllText(path, "");
        AssetDatabase.Refresh();
        textTreeField.value = AssetDatabase.LoadAssetAtPath<TextAsset>(path.Replace(Application.dataPath, "Assets"));
    }

    public static void SaveTreeToJson(Dictionary<string, TextNodeData> textTreeDict, ObjectField textTreeField)
    {
        if (textTreeField.value == null)
        {
            Debug.LogError("No text tree asset selected. Please select an asset to save.");
            return;
        }

        string json = JsonUtility.ToJson(new Serialization<TextNodeData>(textTreeDict.Values), true);
        string path = AssetDatabase.GetAssetPath(textTreeField.value);

        File.WriteAllText(path, json);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Text tree saved to {path}");
    }
}