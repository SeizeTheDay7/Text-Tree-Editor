using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// JSON Manager for Editor
/// </summary>
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

    public static void SaveTreeToJson(Dictionary<string, NodeElement> nodeElementDict, ObjectField textTreeField)
    {
        if (textTreeField.value == null)
        {
            Debug.LogError("No text tree asset selected. Please select an asset to save.");
            return;
        }

        List<TextNodeData> textNodeDataList = nodeElementDict.Values.Select(v => v.textNodeData).ToList();
        string json = JsonUtility.ToJson(new Serialization<TextNodeData>(textNodeDataList), true);
        string path = AssetDatabase.GetAssetPath(textTreeField.value);

        File.WriteAllText(path, json);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Text tree saved to {path}");
    }

    public static List<TextNodeData> LoadJsonToList(TextAsset textAsset)
    {
        string path = AssetDatabase.GetAssetPath(textAsset);
        if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) { Debug.Log("Selected file is not a JSON file."); return null; }

        string json = File.ReadAllText(path);
        if (string.IsNullOrEmpty(json)) { return new List<TextNodeData>(); }

        var serialization = JsonUtility.FromJson<Serialization<TextNodeData>>(json);
        if (serialization == null || serialization.items == null) { Debug.LogError("Failed to deserialize JSON data."); return null; }

        var textNodeDataList = new List<TextNodeData>();
        foreach (var item in serialization.items)
        {
            if (item != null)
                textNodeDataList.Add(item);
            else
                Debug.LogWarning("Found null item in JSON data.");
        }
        Debug.Log($"Loaded {textNodeDataList.Count} items from JSON.");
        return textNodeDataList;
    }
}