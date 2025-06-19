using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


/// <summary>
/// Handling text tree for Editor 
/// </summary>
public static class TextTreeSOUtil
{
    public static void CreateNewTreeFile(ObjectField textTreeField)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New Text Tree",
            "TextTree_Name",
            "asset",
            "Please enter a file name to save the text tree to"
        );

        if (string.IsNullOrEmpty(path)) return;

        var textTreeSO = ScriptableObject.CreateInstance<TextTreeSO>();
        AssetDatabase.CreateAsset(textTreeSO, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        textTreeField.value = textTreeSO;
    }

    internal static void SaveTreeToSO(Dictionary<string, NodeElement> nodeElementDict, TextTreeSO textTreeSO)
    {
        textTreeSO.textNodeList = nodeElementDict.Values.Select(nodeElem => nodeElem.textNodeData).ToList();

        EditorUtility.SetDirty(textTreeSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Text tree saved to {AssetDatabase.GetAssetPath(textTreeSO)}");
    }
}