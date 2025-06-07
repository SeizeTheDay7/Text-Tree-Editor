using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;


internal static class JSONManager
{
    public static void CreateNewTreeFile(ObjectField textTreeField)
    {
        string path = EditorUtility.SaveFilePanel(
            "Select path for new text tree",
            Application.dataPath,
            "TextTree_Name",
            "json"
        );

        if (string.IsNullOrEmpty(path)) { Debug.Log("Creation canceled"); return; }

        File.WriteAllText(path, "");
        AssetDatabase.Refresh();
        textTreeField.value = AssetDatabase.LoadAssetAtPath<TextAsset>(path.Replace(Application.dataPath, "Assets"));

        Debug.Log("New text tree file created");
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

    public static Dictionary<string, TextNodeData> LoadJsonToDict(TextAsset textAsset)
    {
        string path = AssetDatabase.GetAssetPath(textAsset);
        if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) { Debug.Log("Selected file is not a JSON file."); return null; }

        string json = File.ReadAllText(path);
        if (string.IsNullOrEmpty(json)) { return new Dictionary<string, TextNodeData>(); }

        var serialization = JsonUtility.FromJson<Serialization<TextNodeData>>(json);
        if (serialization == null || serialization.items == null) { Debug.LogError("Failed to deserialize JSON data."); return null; }

        var textTreeDict = new Dictionary<string, TextNodeData>();
        foreach (var item in serialization.items)
        {
            if (item == null)
                Debug.LogError("Some item in JSON is null");
            else
                textTreeDict[item.key] = item;
        }
        Debug.Log($"Loaded {textTreeDict.Count} items from JSON.");
        return textTreeDict;
    }
}