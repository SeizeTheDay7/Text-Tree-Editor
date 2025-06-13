using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;


/// <summary>
/// JSON Manager for Runtime
/// </summary>
public static class RuntimeJSONManager
{
    public static Dictionary<string, TextNodeData> LoadJsonToDict(TextAsset textAsset)
    {
        List<TextNodeData> textNodeDataList = LoadJsonToList(textAsset);
        Dictionary<string, TextNodeData> textNodeDataDict = new Dictionary<string, TextNodeData>();
        foreach (TextNodeData node in textNodeDataList)
        {
            if (node.key == null) { Debug.LogWarning("Node key is null"); continue; }
            textNodeDataDict[node.key] = node;
        }
        return textNodeDataDict;
    }

    private static List<TextNodeData> LoadJsonToList(TextAsset textAsset)
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